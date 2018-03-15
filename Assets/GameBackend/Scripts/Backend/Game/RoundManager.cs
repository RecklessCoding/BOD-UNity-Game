using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    private static RoundManager instance;
    public static RoundManager Instance
    {
        get
        {
            if (instance != null)
                return instance;
            else
                instance = new RoundManager();
            return instance;
        }
    }

    public int maxTime;
    public int currentTime;

    public int maxNumberOfRounds;
    private int currentRoundNumber = 1;

    private bool isTimeOver = false;

    private float timeToTriggerRoundOver;

    private bool isRoundOver = false;
    internal bool IsRoundOver
    {
        get
        {
            return isRoundOver;
        }
    }

    private bool didScored = false;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UIManager.instance.SetRound(currentRoundNumber);

        InvokeRepeating("ReduceTime", 1, 1);
    }

    void ReduceTime()
    {
        if (currentTime > 0)
            currentTime--;
    }

    // Update is called once per frame
    void Update()
    {
        CheckTimers();
        if (isRoundOver)
        {
            return;
        }
    }

    private void CheckTimers()
    {
        if (currentTime <= 0 && !isTimeOver)
        {
            isTimeOver = true;
            if (maxNumberOfRounds > currentRoundNumber)
            {
                RoundOver();
            }
            else
            {
                GameOver();
            }
        }
        if (Time.time >= timeToTriggerRoundOver && isRoundOver)
        {
            RoundOverComplete();
        }
    }

    public void ResetTime()
    {
        currentTime = Mathf.FloorToInt(maxTime);
        isTimeOver = false;
    }

    public void RoundOver()
    {
        currentRoundNumber++;
        isRoundOver = true;
        timeToTriggerRoundOver = Time.time + 3;
        UIManager.instance.SetRound(currentRoundNumber);
        UIManager.instance.RoundOver(TeamManager.Instance.GetVictoriousTeam());

        TeamManager.Instance.AdjustScore();
        TeamManager.Instance.Reset();
    }

    public void Scored(Team team, bool fromFlag)
    {
        UIManager.instance.TeamScored(team);

        if (team != null)
        {
            if (fromFlag)
            {
                team.teamScore++;
            }
        }

        isRoundOver = true;
        timeToTriggerRoundOver = Time.time + 2;
    }

    public void RoundOverComplete()
    {
        isRoundOver = false;
        UIManager.instance.OverlayOff();
    }

    public void GameOver()
    {   
        UIManager.instance.GameOver(TeamManager.Instance.GetVictoriousTeam());
    }

    internal void ResetRound()
    {
        currentRoundNumber = 0;
        UIManager.instance.SetRound(currentRoundNumber);
    }
}