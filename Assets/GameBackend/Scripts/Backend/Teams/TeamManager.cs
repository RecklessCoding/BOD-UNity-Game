using System.Linq;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    private static TeamManager instance;

    public GameObject submissions;

    public GameObject playerTemplate;

    public static TeamManager Instance
    {
        get
        {
            if (instance == null)
                instance = new TeamManager();

            return instance;
        }
    }
    public int teamSize = 5;

    [SerializeField]
    private Team redTeam;

    [SerializeField]
    private Team blueTeam;

    private bool redScored;
    private bool blueScored;

    public Team GetVictoriousTeam()
    {
        if (redTeam.teamScore > blueTeam.teamScore)
            return redTeam;
        else if (blueTeam.teamScore > redTeam.teamScore)
            return blueTeam;
        else
            return null;
    }

    public void AdjustScore()
    {
        if (TeamEliminated() != null)
        {
            if (TeamEliminated() == redTeam && !blueScored)
            {
                redTeam.teamScore++;
            }
            if (TeamEliminated() == blueTeam && !redScored)
            {
                blueTeam.teamScore++;
            }
        }
    }


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        instance = this;

        redTeam.teamMembersObjct = submissions.GetComponent<TeamSubmissions>().teamA;
        blueTeam.teamMembersObjct = submissions.GetComponent<TeamSubmissions>().teamB;

        StartOver();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Reset();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartOver();
        }

        if (RoundManager.Instance.IsRoundOver)
        {
            return;
        }

        CheckDeaths();
        CheckFlags();
    }

    public void Reset()
    {
        ResetTeams();
        RoundManager.Instance.ResetTime();
    }

    private void CheckFlags()
    {
        // Flag B in Team A's spawn, Team A Wins                                                                                                                    
        if (redTeam.spawnPoint.InSpawn(blueTeam.spawnPoint.flag.GetLocation()))
        {
            blueTeam.spawnPoint.flag.ResetFlag();
            RoundManager.Instance.Scored(redTeam, true);
            redScored = true;
        }

        // Flag A in Team B's spawn, Team B Wins
        if (blueTeam.spawnPoint.InSpawn(redTeam.spawnPoint.flag.GetLocation()))
        {
            redTeam.spawnPoint.flag.ResetFlag();
            RoundManager.Instance.Scored(blueTeam, true);
            blueScored = true;
        }
    }

    /**
    *   <summary>Checks if everyone is dead.</summary>
    **/
    private void CheckDeaths()
    {
        int deadCount = redTeam.agents.Count(x => x.IsDead());
        if (deadCount >= redTeam.agents.Count)
        {
         //   RoundManager.Instance.RoundOver(blueTeam, false);
        }

        deadCount = blueTeam.agents.Count(x => x.IsDead());
        if (deadCount >= blueTeam.agents.Count)
        {
            //  RoundManager.Instance.RoundOver(redTeam, false);
        }
    }

    private Team TeamEliminated()
    {
        int deadCount = redTeam.agents.Count(x => x.IsDead());
        if (deadCount >= redTeam.agents.Count)
        {
            return redTeam;
        }

        deadCount = blueTeam.agents.Count(x => x.IsDead());
        if (deadCount >= blueTeam.agents.Count)
        {
            return blueTeam;
        }
        return null;
    }


    internal void StartOver()
    {
        redTeam.teamScore = 0;
        blueTeam.teamScore = 0;
        InitiateAgentsMemory(redTeam);
        InitiateAgentsMemory(blueTeam);

        RoundManager.Instance.ResetRound();
        Reset();
    }

    private void ResetTeams()
    {
        // Hacking a last minute solution.
        redScored = false;
        blueScored = false;

        for (int i = 0; i < redTeam.agents.Count; i++)
        {
            ResetTeam(i, redTeam);
        }

        redTeam.agents.Clear();

        for (int i = 0; i < blueTeam.agents.Count; i++)
        {
            ResetTeam(i, blueTeam);
        }

        blueTeam.agents.Clear();

        int redTeamSize = redTeam.TeamSize;
        int blueTeamSize = blueTeam.TeamSize;

        for (int i = 0; i < redTeamSize; i++)
        {
            SpawnCharacters(i, redTeam);

        }
        for (int i = 0; i < blueTeamSize; i++)
        {
            SpawnCharacters(i, blueTeam);
        }

        UIManager.instance.CreateHealthbars();

        redTeam.spawnPoint.flag.ResetFlag();
        blueTeam.spawnPoint.flag.ResetFlag();
    }

    private void ResetTeam(int i, Team team)
    {
        GameObject.Destroy(team.agents[i].gameObject);
    }

    private void InitiateAgentsMemory(Team team)
    {
        for (int i = 0; i < 5 ; i++)
        {
            team.AgentsLifeStats.Add(new LifeStats());
        }
    }


    private void SpawnCharacters(int i, Team team)
    {   
        GameObject bot = GameObject.Instantiate(playerTemplate, team.spawnPoint.GetPosition(), Quaternion.identity);
        bot.GetComponent<Planner>().ReadPlan(team.TeamMembers[i].GetComponent<AIModule>().planFile);
        bot.GetComponent<Planner>().ChangeBehaviourLibrary(team.TeamMembers[i].GetComponent<AIModule>().behaviourLibrary);
        bot.GetComponent<Planner>().ChangeNav(team.TeamMembers[i].GetComponent<AIModule>().navmeshController);

        Agent newAgent = bot.GetComponent<Agent>();
        newAgent.SetTeam(team.teamType);

        if (team.AgentsLifeStats.Count > 0)
        {
            newAgent.LifeStats = team.AgentsLifeStats.ElementAt(i);
        }

        team.agents.Add(newAgent);
    }

    public Team GetRedTeam()
    {
        return redTeam;
    }

    public Team GetBlueTeam()
    {
        return blueTeam;
    }

    /**
    *   <summary>Alert team of a flag update.</summary>
    **/
    public void SendFlagMessage(Flag flag)
    {
        for (int i = 0; i < redTeam.agents.Count; i++)
        {
            if (redTeam.agents[i].flagStatusChangedCallback != null)
                redTeam.agents[i].flagStatusChangedCallback.Invoke(flag);
        }
        for (int i = 0; i < blueTeam.agents.Count; i++)
        {
            if (blueTeam.agents[i].flagStatusChangedCallback != null)
                blueTeam.agents[i].flagStatusChangedCallback.Invoke(flag);
        }
    }

    public void SendDied(Agent soldier)
    {
        // Send msg to soldiers
        for (int i = 0; i < redTeam.agents.Count; i++)
        {
            if (redTeam.agents[i].agentDiedCallBack != null)
                redTeam.agents[i].agentDiedCallBack.Invoke(soldier);
        }
        for (int i = 0; i < blueTeam.agents.Count; i++)
        {
            if (blueTeam.agents[i].agentDiedCallBack != null)
                blueTeam.agents[i].agentDiedCallBack.Invoke(soldier);
        }
    }
}