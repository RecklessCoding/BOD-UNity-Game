using System.Collections.Generic;
using UnityEngine;

/**
 * Represents an agent; contains its current state (i.e.: health, weapon, eyesight) and long term memory. 
 * */

public class Agent : MonoBehaviour, IAgent
{
    public void SetName(string name)
    {
        this.name = name;
    }
    public string GetName()
    {
        return name;
    }

    private Team.Type teamType;
    public void SetTeam(Team.Type teamType)
    {
        this.teamType = teamType;
    }
    public Team.Type GetTeam()
    {
        return teamType;
    }

    public Vector3 enemySpawnLocation;
    public Vector3 spawnLocation;

    private IShootable currentWeapon;
    public IShootable CurrentWeapon
    {
        get {
            return currentWeapon;
        }
    }

    private int health = 75;
    private const int MAX_HEALTH = 100;
    public int GetHealth()
    {
        return health;
    }
    public int GetMaxHealth()
    {
        return MAX_HEALTH;
    }

    public float speed;
    public float rotationSpeed;

    public AnimationController animationController;
    public AnimationController GetAnimationController()
    {
        return animationController;
    }

    private Rigidbody rigidbody;

    public Transform flagHolder;
    public Transform GetFlagHolder()
    {
        return flagHolder;
    }

    public Transform flagTransform;
    public bool HasFlag()
    {
        return flagTransform != null;
    }

    public Vector3 GetLocation()
    {
        return transform.position;
    }

    public int viewRadius = 60;
    public float viewDistance = 25;
    public int obstaclesLayer = 9;
    private const float MIN_FLAG_RANGE = 2f;

    public LayerMask viewLayerMask;

    private List<IAgent> allAgentsInSight = new List<IAgent>();
    internal List<IAgent> AllAgentsInSight
    {
        get
        {
            return allAgentsInSight;
        }
    }
    private List<IAgent> friendlyAgentsInSight = new List<IAgent>();
    internal List<IAgent> FriendlyAgentsInSight
    {
        get
        {
            return friendlyAgentsInSight;
        }
    }
    private List<IAgent> enemyAgentsInSight = new List<IAgent>();
    internal List<IAgent> EnemyAgentsInSight
    {
        get
        {
            return enemyAgentsInSight;
        }
    }

    private List<IGrabable> allFlagsInSight = new List<IGrabable>();
    private IGrabable enemyFlagInSight;
    internal List<IGrabable> AllFlagsInSight
    {
        get
        {
            return allFlagsInSight;
        }
    }
    internal IGrabable EnemyFlagInSight
    {
        get
        {
            return enemyFlagInSight;
        }
    }
    private IGrabable friendlyFlagInSight;
    internal IGrabable FriendlyFlagInSight
    {
        get
        {
            return friendlyFlagInSight;
        }
    }

    private bool isDamaged = false;
    internal bool IsDamaged
    {
        get
        {
            return isDamaged;
        }
    }

    public Rigidbody Rigidbody
    {
        get
        {
            return rigidbody;
        }

        set
        {
            rigidbody = value;
        }
    }

    private float ignoreDamageTimer = 0.5f;
    private float resetDamageTimer = 0;

    private bool isDead = false;
    public bool IsDead()
    {
        return isDead;
    }

    internal Vector3 locationLastIncomingFire;

    private LifeStats lifeStats;
    public LifeStats LifeStats
    {
        get
        {
            return lifeStats;
        }

        set
        {
            lifeStats = value;
        }
    }

    void Awake()
    {
        health = MAX_HEALTH;
        currentWeapon = GetComponentInChildren<IShootable>();
        Rigidbody = GetComponent<Rigidbody>();
        damagedCallback += DamageCallback;
    }

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        currentWeapon.SetOwner(this);

        if (teamType == Team.Type.RED)
        {
            enemySpawnLocation = TeamManager.Instance.GetBlueTeam().spawnPoint.transform.position;
            spawnLocation = TeamManager.Instance.GetRedTeam().spawnPoint.transform.position;
        } else if (teamType == Team.Type.BLUE)
        {
            enemySpawnLocation = TeamManager.Instance.GetRedTeam().spawnPoint.transform.position;
            spawnLocation = TeamManager.Instance.GetBlueTeam().spawnPoint.transform.position;
        }
    }

    void Update()
    {
        CheckDamageTimer();
        UpdateEyesight();
        animationController.UpdateHealth(health);
        Rigidbody.velocity = new Vector3(Mathf.Clamp(Rigidbody.velocity.x, -speed, speed), Rigidbody.velocity.y, Mathf.Clamp(Rigidbody.velocity.z, -speed, speed));

        if (IsDead())
        {
            Rigidbody.velocity = new Vector3(0, Rigidbody.velocity.y, 0);
        }
    }

    void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    private void CheckDamageTimer()
    {
        // Stop looking at the location we were damaged from after the timer
        if (Time.time >= resetDamageTimer)
        {
            isDamaged = false;
        }
    }

    protected void DamageCallback(Vector3 location)
    {
        resetDamageTimer = Time.time + ignoreDamageTimer;
        locationLastIncomingFire = location;
        isDamaged = true;
    }

    private void UpdateEyesight()
    {
        ClearLists();
        UpdateAgentsInSight();
        UpdateFlagsInSight();
    }

    private void UpdateFlagsInSight()
    {
        if (InSight(TeamManager.Instance.GetRedTeam().spawnPoint.flag.transform))
        {
            allFlagsInSight.Add(TeamManager.Instance.GetRedTeam().spawnPoint.flag);
            if (teamType == Team.Type.RED)
            {
                friendlyFlagInSight = TeamManager.Instance.GetRedTeam().spawnPoint.flag;
            }
            else
            {
                enemyFlagInSight = TeamManager.Instance.GetRedTeam().spawnPoint.flag;
            }
        }
        if (InSight(TeamManager.Instance.GetBlueTeam().spawnPoint.flag.transform))
        {
            allFlagsInSight.Add(TeamManager.Instance.GetBlueTeam().spawnPoint.flag);
            if (teamType == Team.Type.BLUE)
            {
                friendlyFlagInSight = TeamManager.Instance.GetBlueTeam().spawnPoint.flag;
            }
            else
            {
                enemyFlagInSight  = TeamManager.Instance.GetBlueTeam().spawnPoint.flag;
            }
        }
    }

    private void UpdateAgentsInSight()
    {
        if (teamType.Equals(Team.Type.RED))
        {
            friendlyAgentsInSight = GetSoldiersInSight(TeamManager.Instance.GetRedTeam().agents);
            enemyAgentsInSight.AddRange(GetSoldiersInSight(TeamManager.Instance.GetBlueTeam().agents));
        }
        else
        {
            enemyAgentsInSight = GetSoldiersInSight(TeamManager.Instance.GetRedTeam().agents);
            friendlyAgentsInSight = GetSoldiersInSight(TeamManager.Instance.GetBlueTeam().agents);
        }
        allAgentsInSight.AddRange(friendlyAgentsInSight);
        allAgentsInSight.AddRange(enemyAgentsInSight);
    }

    private void ClearLists()
    {
        enemyFlagInSight = null;
        friendlyFlagInSight = null;
        allFlagsInSight.Clear();
        allAgentsInSight.Clear();
        friendlyAgentsInSight.Clear();
        enemyAgentsInSight.Clear();
    }

    private List<IAgent> GetSoldiersInSight(List<Agent> agentsToCheckFor)
    {
        List<IAgent> agentsInSight = new List<IAgent>();
        for (int i = 0; i < agentsToCheckFor.Count; i++)
        {
            // is us?
            if (agentsToCheckFor[i] == this)
                continue;

            // raycast to them
            if (InSight(agentsToCheckFor[i].transform))
            {
                // hit so add them
                agentsInSight.Add(agentsToCheckFor[i]);
            }
        }
        return agentsInSight;
    }
    private bool InSight(Transform position)
    {
        RaycastHit hit = CastRaycast(-(transform.position - position.position), viewDistance, viewLayerMask, false);
        // raycast to them
        if (hit.transform != null)
        {
            // hit the target?
            if (hit.transform.gameObject.GetInstanceID() == position.gameObject.GetInstanceID())
            {
                return true;
            }
            // hit so true
        }
        return false;
    }

    /// <summary>
    /// Casts a ray in the direction given
    /// </summary>
    /// <param name="direction"></param>
    /// <returns>Null if out of our view cone</returns>
    private RaycastHit CastRaycast(Vector3 direction, float length, LayerMask layermask, bool debug = true)
    {
        direction.Normalize();

        length = Mathf.Clamp(length, -viewDistance, viewDistance);

        if (debug)
            Debug.DrawRay(transform.position + Vector3.up * 1.2f, direction * length, Color.black);

        float angleDif = (transform.localEulerAngles.y - GetRotation(direction)) % 360;
        if (angleDif < 0)
            angleDif = 360 + angleDif;

        RaycastHit raycastHit = new RaycastHit();

        // Check if its out of our sight
        if (angleDif > viewRadius && angleDif < (360 - viewRadius))
        {
            return raycastHit;
        }

        Physics.Raycast(transform.position + Vector3.up * 1.2f, direction, out raycastHit, viewDistance, layermask);

        return raycastHit;
    }

    void OnDrawGizmos()
    {
        Vector3 direction = (new Vector3(viewDistance * Mathf.Cos(-viewRadius * Mathf.PI / 180), 0, viewDistance * Mathf.Sin(-viewRadius * Mathf.PI / 180)));
        direction = Quaternion.AngleAxis(-90 + transform.localEulerAngles.y, Vector3.up) * direction;
        Debug.DrawRay(transform.position + Vector3.up * 0.2f, direction, (teamType == Team.Type.RED) ? Color.red : Color.blue);

        direction = (new Vector3(viewDistance * Mathf.Cos(viewRadius * Mathf.PI / 180), 0, viewDistance * Mathf.Sin(viewRadius * Mathf.PI / 180)));
        direction = Quaternion.AngleAxis(-90 + transform.localEulerAngles.y, Vector3.up) * direction;
        Debug.DrawRay(transform.position + Vector3.up * 0.2f, direction, (teamType == Team.Type.RED) ? Color.red : Color.blue);

        direction = (new Vector3(viewDistance * Mathf.Cos(0 * Mathf.PI / 180), 0, viewDistance * Mathf.Sin(0 * Mathf.PI / 180)));
        direction = Quaternion.AngleAxis(-90 + transform.localEulerAngles.y, Vector3.up) * direction;
        Debug.DrawRay(transform.position + Vector3.up * 0.2f, direction, (teamType == Team.Type.RED) ? Color.red : Color.blue);
    }

    public Transform GetFlag()
    {
        return flagTransform;
    }

    public int DamageHandler(int damage, Vector3 damagerLocation)
    {
        if (IsDead())
            return 0;

        health -= damage;
        animationController.Damage();

        if (damagedCallback != null)
            damagedCallback.Invoke(damagerLocation);

        if (health <= 0)
        {
            AgentDied();
        }

        return health;
    }

    private void AgentDied()
    {
        isDead = true;
        lifeStats.IncreaseTimesDied();
        lifeStats.DiedLastRound = true;

        TeamManager.Instance.SendDied(this);
        GetComponent<Collider>().enabled = false;

        Rigidbody.isKinematic = true;

        if (HasFlag())
        {
            DropFlag();
        }
    }

    public void DropFlag()
    {
        if (!HasFlag())
            return;

        flagTransform.SetParent(null);
        flagTransform.position = transform.position;
        flagTransform.position = new Vector3(flagTransform.position.x, 1, flagTransform.position.z);
        flagTransform.localEulerAngles = new Vector3(-90, 0, 0);

        flagTransform.GetComponent<Flag>().IsGrabbed = false;
        flagTransform.GetComponent<BoxCollider>().enabled = true;

        flagTransform = null;
    }

    public void GrabFlag(IGrabable flag)
    {
        if (flag.GetTeam() == teamType)
            return;
        if ((flag.GetTransform().position - transform.position).magnitude > MIN_FLAG_RANGE)
            return;
        if (flag.Grabbed())
            return;

        flag.GetTransform().SetParent(flagHolder);
        flag.GetTransform().localPosition = Vector3.zero;
        flag.GetTransform().localRotation = Quaternion.identity;

        flag.SetGrabbed(true, this);
        flag.GetTransform().GetComponent<BoxCollider>().enabled = false;

        flagTransform = flag.GetTransform();
    }

    public float GetRotation(Vector3 direction)
    {
        return ((((180 * Mathf.Atan2(direction.x, direction.z)) / Mathf.PI) + 0) % 360) - 0;
    }

    /* In case wondering what delegate: delegate is a type that safely encapsulates a method, 
     * similar to a function pointer in C and C++. 
     * Unlike C function pointers, delegates support OO, while remaining type safe and secure. 
     * */
    public delegate void DamagedCallback(Vector3 location);
    public DamagedCallback damagedCallback;

    public delegate void FlagStatusChangedCallback(Flag flag);
    public FlagStatusChangedCallback flagStatusChangedCallback;

    public delegate void AgentDiedCallback(Agent soldier);
    public AgentDiedCallback agentDiedCallBack;
}