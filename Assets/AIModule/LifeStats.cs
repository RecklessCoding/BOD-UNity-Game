using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeStats {

    private bool diedLastRound = false;
    internal bool DiedLastRound
    {
        get
        {
            return diedLastRound;
        }

        set
        {
            diedLastRound = value;
        }
    }

    public int timesDied = 0;
    internal int TimesDied
    {
        get
        {
            return timesDied;
        }
    }
    internal void IncreaseTimesDied()
    {
        timesDied++;
    }

    private int timesScore = 0;
    internal int TimesScored
    {
        get
        {
            return timesScore;
        }
    }


    internal void IncreaseTimesScored()
    {
        timesScore++;
    }

    private Vector3 locationOfLastDeath;
}
