using System.Collections.Generic;
using UnityEngine;

/***
 * As the name suggests this class links your behaviour library with the Agent class. It contains a number of accessors. 
 * Students should not touch this class, but instead, akin to the example implement it.
 * */
public class BehaviourLibraryLinker : MonoBehaviour
{
    /* Agent is kept private; this way there is limit of how much code you can/should change. */
    private Agent agent;

    private NavmeshController navAgent;
    public NavmeshController NavAgent
    {
        get
        {
            return navAgent;
        }

        set
        {
            navAgent = value;
        }
    }

    /* Short-term memory of the agent with all relevant objects within its sight. */
    protected List<IAgent> AllAgentsInSight { get { return agent.AllAgentsInSight; } }
    protected List<IAgent> FriendlyAgentsInSight { get { return agent.FriendlyAgentsInSight; } }
    protected List<IAgent> EnemyAgentsInSight { get { return agent.EnemyAgentsInSight; } }
    protected List<IGrabable> AllFlagsInSight { get { return agent.AllFlagsInSight; } }
    protected IGrabable EnemyFlagInSight { get { return agent.EnemyFlagInSight; } }
    protected IGrabable FriendlyFlagInSight { get { return agent.FriendlyFlagInSight; } }

    protected Vector3 LocationLastIncomingFire { get { return agent.locationLastIncomingFire; } }

    /* Long-term memory of the agent for all relevant objects within its sight. */
    protected Vector3 EnemySpawnLocation { get { return agent.enemySpawnLocation; } }
    protected Vector3 SpawnLocation { get { return agent.spawnLocation; } }

    /* Current state of the agent.*/
    protected bool IsDamaged { get { return agent.IsDamaged; } }
    protected bool HasFlag { get { return agent.HasFlag(); } }
    protected bool IsDead { get { return agent.IsDead(); } }

    /* Global state of the game, which the agent is aware of. */
    private bool friendlyFlagTaken = false;
    protected bool FriendlyFlagTaken
    {
        get
        {
            return friendlyFlagTaken;
        }
    }
    public bool enemyFlagTaken = false;
    protected bool EnemyFlagTaken
    {
        get
        {
            return enemyFlagTaken;
        }
    }

    protected IShootable CurrentWeapon
    {
        get
        {
            return agent.CurrentWeapon;
        }
    }

    private Agent lastAgentWhoDied;
    protected Agent LastAgentWhoDied
    {
        get
        {
            return lastAgentWhoDied;
        }
    }

    public void Awake()
    {
        agent = GetComponent<Agent>();
        NavAgent = GetComponent<NavmeshController>();
        agent.flagStatusChangedCallback += FlagStatusChangedCallback;
        agent.agentDiedCallBack += AgentDiedCallback;
    }

    // Use this for initialization
    public void Start()
    {
        agent = GetComponent<Agent>();
        NavAgent = GetComponent<NavmeshController>();
        friendlyFlagTaken = false;
        enemyFlagTaken = false;
    }

    internal bool CheckBoolSense(Sense Sense)
    {
        return (bool)this.GetType().GetMethod(Sense.Name).Invoke(this, null);
    }

    internal double CheckDoubleSense(Sense Sense)
    {
        return (double)this.GetType().GetMethod(Sense.Name).Invoke(this, null);
    }

    internal void ExecuteAction(Action action)
    {
        this.GetType().GetMethod(action.Name).Invoke(this, null);
    }

    protected void AgentDiedCallback(Agent agentWhoDied)
    {
        lastAgentWhoDied = agentWhoDied;
    }

    protected bool LookAt(Vector3 position)
    {
        bool lookingAt = false;

        if (agent.IsDead())
            return false;

        // calculate gun offset needed to aim gun at target postion

        Vector3 lastRotation = transform.eulerAngles;
        Transform gun = agent.CurrentWeapon.GetMuzzle();
        Vector3 gunOffset = gun.localPosition;
        Vector3 toTarget = position - transform.position;
        float distance = toTarget.magnitude;
        float offsetAngle = Mathf.Atan(-gunOffset.x / distance) * Mathf.Rad2Deg; // x offset to angle offset  

        Quaternion lookRotation = Quaternion.LookRotation(toTarget, transform.up) * Quaternion.AngleAxis(-offsetAngle, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, agent.rotationSpeed * Time.deltaTime);

        lookingAt = Quaternion.Angle(transform.rotation, lookRotation) < 0.1f;

        if (lookingAt)
        {
            Ray ray = new Ray(gun.position, gun.forward);
            lookingAt = !Physics.SphereCast(ray, 0.15f, distance, 1 << agent.obstaclesLayer);
        }

        return lookingAt;
    }

    protected void Move(Vector3 direction)
    {
        direction = Vector3.Normalize(direction);
        direction *= agent.speed;
        GetComponent<Rigidbody>().velocity = direction;
    }

    protected void MoveTowards(Vector3 position)
    {
        Move(position - transform.position);
    }

    protected void GrabFlag(IGrabable flag)
    {
        agent.GrabFlag(flag);
    }

    protected void FlagStatusChangedCallback(IGrabable flag)
    {
        if (flag.GetTeam() != agent.GetTeam())
        {
            enemyFlagTaken = !enemyFlagTaken;
        }
        else
        {
            friendlyFlagTaken = !friendlyFlagTaken;
        }
    }
}