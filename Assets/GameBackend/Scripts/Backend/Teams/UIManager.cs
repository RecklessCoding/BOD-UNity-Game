using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    public Text overlay;
    public GameObject overlayGameObject;

    public Text teamAText;
    public Text teamBText;

    public Transform healthLayoutHolder;
    public GameObject healthLayout;

    public Text timeSpeedText;

    public Text timerText;
    public Text roundText;

    public float curTime = 1;

    public GameObject restartButton;

    public void IncreaseTime()
    {
        if (curTime < 4)
        {
            curTime += 0.5f;
            curTime = Mathf.Clamp(curTime, 0, 20);
            Time.timeScale = curTime;
            timeSpeedText.text = curTime + "x";
        }
    }

    public void DecreaseTime()
    {
        if (curTime > 0)
        {
            curTime -= 0.5f;
            curTime = Mathf.Clamp(curTime, 0, 20);
            Time.timeScale = curTime;
            timeSpeedText.text = curTime + "x";
        }
    }

    public void SetRound(int round)
    {
        roundText.text = "Round:" + round;
    }

    void Awake()
    {
        instance = this;
        Time.timeScale = 1;
    }

    private void Start()
    {
        timeSpeedText.text = curTime + "x";
    }

    void Update()
    {
        timerText.text = GetTimeString(RoundManager.Instance.currentTime);

        Team teamA = TeamManager.Instance.GetRedTeam();
        Team teamB = TeamManager.Instance.GetBlueTeam();
        teamAText.text = teamA.teamName + "\n" + "Score: " + teamA.teamScore;
        teamBText.text = teamB.teamName + "\n" + "Score: " + teamB.teamScore;
    }

    public string GetTimeString(int time)
    {
        int minutes = time / 60;
        int seconds = time - (minutes * 60);

        string timeString = minutes.ToString() + ":" + ((seconds < 10) ? "0" : "") + seconds;

        return timeString;
    }

    public void RoundOver(Team winner)
    {
        overlayGameObject.SetActive(true);
        if (winner == null)
        {
            overlay.text = "<color=white>Draw!</color>";
        }
        else
        {
            overlay.text = "<color=" + ((winner.teamType == Team.Type.RED) ? "red" : "blue") + ">" + winner.teamName + " is ahead on the score!</color>";
        }
    }

    public void TeamScored(Team teamWhoScored)
    {
        overlayGameObject.SetActive(true);
        overlay.text = "<color=" + ((teamWhoScored.teamType == Team.Type.RED) ? "red" : "blue") + ">" + teamWhoScored.teamName + " has scored!</color>";
    }

    public void OverlayOff()
    {
        overlayGameObject.SetActive(false);
    }

    public void GameOver(Team winner)
    {
        Time.timeScale = 0;
        restartButton.SetActive(true);
        overlayGameObject.SetActive(true);
        if (winner == null)
        {
            overlay.text = "<color=white>Draw!</color>";
        }
        else
        {
            overlay.text = "<color=" + ((winner.teamType == Team.Type.RED) ? "red" : "blue") + ">" + winner.teamName + " has won the game!</color>";
        }
    }

    public void Restart()
    {
        Time.timeScale = curTime;
        overlayGameObject.SetActive(false);
        restartButton.SetActive(false);
 
        TeamManager.Instance.StartOver();
    }

    public void CreateHealthbars()
    {
        // Clear healthbars
        for (int i = 0; i < healthLayoutHolder.childCount; i++)
        {
            GameObject.Destroy(healthLayoutHolder.GetChild(i).gameObject);
        }

        for (int i = 0; i < TeamManager.Instance.GetRedTeam().agents.Count; i++)
        {
            CreateHealthbar(TeamManager.Instance.GetRedTeam().agents[i], Color.red);
        }

        for (int i = 0; i < TeamManager.Instance.GetBlueTeam().agents.Count; i++)
        {
            CreateHealthbar(TeamManager.Instance.GetBlueTeam().agents[i], Color.blue);
        }
    }

    void CreateHealthbar(IAgent character, Color color)
    {
        GameObject newHealthbar = GameObject.Instantiate(healthLayout, healthLayoutHolder);
        newHealthbar.transform.SetParent(healthLayoutHolder);
        newHealthbar.transform.position = Vector3.zero;
        newHealthbar.GetComponent<HealthLayout>().ChangeColor(color);

        newHealthbar.GetComponent<HealthLayout>().SetUp(character);
    }
}
