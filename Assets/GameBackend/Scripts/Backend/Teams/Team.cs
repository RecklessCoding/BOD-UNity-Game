using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Team
{
    public enum Type
    {
        RED,
        BLUE
    };

    public bool humanPlayer = false;

    public string teamName;

    public Type teamType;

    public int teamScore;

    public Spawn spawnPoint;

    public GameObject teamMembersObjct;

    public GameObject[] TeamMembers
    {
        get
        {
            return teamMembersObjct.GetComponent<TeamMembers>().teamMembers;
        }
    }

    public int TeamSize
    {
        get
        {
            return teamMembersObjct.GetComponent<TeamMembers>().TeamSize;
        }
    }

    public List<Agent> agents = new List<Agent>();

    public List<LifeStats> lifeStats = new List<LifeStats>();
    public List<LifeStats> AgentsLifeStats
    {
        get
        {
            return lifeStats;
        }
    }

    //public List<ICharacter> teamMembers = new List<ICharacter>();
}
