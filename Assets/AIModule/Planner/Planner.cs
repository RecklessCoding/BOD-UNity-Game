using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planner : MonoBehaviour
{
    private List<DriveCollection> drives = new List<DriveCollection>();

    public int botNumber = 0;
    public static int botCount = 0;

    public BehaviourLibraryLinker behaviourLibrary;

    public TextAsset planFile;

    void Awake()
    {
    }

    void Start()
    {
        behaviourLibrary.Start();

        botCount++;
        botNumber = botCount;

       drives = new XMLPlanReader().ReadFile(planFile);      
    }

    public Vector3 directionTestVector3;

    void Update()
    {
        DrivesHandler();
    }

    public void ReadPlan(TextAsset planFile)
    {
        this.planFile = planFile;
        drives = new XMLPlanReader().ReadFile(planFile);
    }

    public void ChangeBehaviourLibrary(BehaviourLibraryLinker behaviourLibrary)
    {
        System.Type type = behaviourLibrary.GetType();
        Component copy = gameObject.AddComponent(type);
        Destroy(GetComponent(this.behaviourLibrary.GetType()));
        this.behaviourLibrary = (BehaviourLibraryLinker) gameObject.GetComponent(type);
    }

    public void ChangeNav(NavmeshController navmeshController)
    {
        gameObject.GetComponent<BehaviourLibraryLinker>().NavAgent = navmeshController;
    }

    public void MakeSelectedAgent()
    {
        ABOD3_Bridge.GetInstance().ChangeSelectedBot(botNumber);
    }

    private void DrivesHandler()
    {
        foreach (DriveCollection drive in drives)
        {
            if (drive.Conditions.Count != 0)
            {
                int numConditionsNeeded = 0;
                foreach (Condition goal in drive.Conditions)
                {
                    numConditionsNeeded = CheckCondition(numConditionsNeeded, goal);
                }
                if (numConditionsNeeded == drive.Conditions.Count)
                {
                    ABOD3_Bridge.GetInstance().AletForElement(botNumber, drive.Name, "D");
                    DriveElementsHandler(drive.DriveElements);
                }
            }
            else
            {
                ABOD3_Bridge.GetInstance().AletForElement(botNumber, drive.Name, "D");
                DriveElementsHandler(drive.DriveElements);
            }
        }
    }

    private void DriveElementsHandler(List<DriveElement> driveElements)
    {
        foreach (DriveElement driveElement in driveElements)
        {
            if (Time.time >= driveElement.NextCheck)
            {
                driveElement.UpdateNextCheck();
                int numConditionsNeeded = 0;
                foreach (Condition trigger in driveElement.Conditions)
                {
                    numConditionsNeeded = CheckCondition(numConditionsNeeded, trigger);
                }
                if (numConditionsNeeded == driveElement.Conditions.Count)
                {
                    ABOD3_Bridge.GetInstance().AletForElement(botNumber, driveElement.Name, "DE");
                    PlanElement elementToBeTriggered = driveElement.TriggerableElement;
                    if (elementToBeTriggered is Competence)
                    {
                        CompetenceHandler((Competence)elementToBeTriggered);
                    }
                    else if (elementToBeTriggered is ActionPattern)
                    {
                        ActionPatternHandler((ActionPattern)elementToBeTriggered);
                    }
                    else if (elementToBeTriggered is Action)
                    {
                        TriggerAction((Action)elementToBeTriggered);
                    }
                }
            }
        }
    }

    private int CheckCondition(int numTriggersTrue, Condition condition)
    {
        switch (condition.Comperator)
        {
            case "bool":
                if (ConditionIsBool(condition))
                    numTriggersTrue = numTriggersTrue + 1;
                break;
            case "=":
                if (ConditionIsEqual(condition))
                    numTriggersTrue = numTriggersTrue + 1;
                break;
            case "<":
                if (ConditionIsLessThan(condition))
                    numTriggersTrue = numTriggersTrue + 1;
                break;
            case "<=":
                if (ConditionIsLessThanAndEqual(condition))
                    numTriggersTrue = numTriggersTrue + 1;
                break;
            case ">":
                if (ConditionIsGreaterThan(condition))
                    numTriggersTrue = numTriggersTrue + 1;
                break;
            case ">=":
                if (ConditionrIsGreaterThanAndEqual(condition))
                    numTriggersTrue = numTriggersTrue + 1;
                break;
            default:
                break;
        }

        return numTriggersTrue;
    }
    private bool ConditionIsBool(Condition condition)
    {
        try
        {
            if (condition.Value == 1) // If we are checking for false
            {
                if (behaviourLibrary.CheckBoolCondition(condition))
                {
                    return true;
                }
            }
            else if (condition.Value == 0) // If we are checking for false
            {
                if (!(behaviourLibrary.CheckBoolCondition(condition)))
                {
                    return true;
                }
            }
            else
            {
                Debug.LogError("Condition: " + condition.Name + " expected output should be 0 or 1");
            }
        }
        catch (System.Exception error)
        {
            Debug.LogError("Checking condition: " + condition.Name + " produced error: " + error);
        }
        return false;
    }

    private bool ConditionIsEqual(Condition trigger)
    {
        return (behaviourLibrary.CheckDoubleCondition(trigger) == trigger.Value);
    }

    private bool ConditionIsLessThan(Condition trigger)
    {
        return (behaviourLibrary.CheckDoubleCondition(trigger) < trigger.Value);
    }

    private bool ConditionIsLessThanAndEqual(Condition trigger)
    {
        return (behaviourLibrary.CheckDoubleCondition(trigger) <= trigger.Value);
    }

    private bool ConditionIsGreaterThan(Condition trigger)
    {
        return (behaviourLibrary.CheckDoubleCondition(trigger) > trigger.Value);
    }
    private bool ConditionrIsGreaterThanAndEqual(Condition trigger)
    {
        return (behaviourLibrary.CheckDoubleCondition(trigger) >= trigger.Value);
    }

    private void CompetenceHandler(Competence competence)
    {
        Condition goal = competence.Goals[0];

        if (CheckCondition(0, goal) == 0)
        {
            ABOD3_Bridge.GetInstance().AletForElement(botNumber, competence.Name, "C");

            int numCEActivated = 0;
            foreach (CompetenceElement competenceElement in competence.Elements)
            {
                if (CompetenceElementsHandler(competenceElement))
                {
                    numCEActivated = numCEActivated + 1;
                }
            }
        }
    }
    private bool CompetenceElementsHandler(CompetenceElement competenceElement)
    {
        int numConditionsNeeded = 0;
        foreach (Condition condition in competenceElement.Conditions)
        {
            numConditionsNeeded = CheckCondition(numConditionsNeeded, condition);
        }
        if (numConditionsNeeded == competenceElement.Conditions.Count)
        {
            ABOD3_Bridge.GetInstance().AletForElement(botNumber, competenceElement.Name, "CE");

            PlanElement elementToBeTriggered = competenceElement.TriggerableElement;
            if (elementToBeTriggered is Competence)
            {
                CompetenceHandler((Competence)elementToBeTriggered);
            }
            else if (elementToBeTriggered is ActionPattern)
            {
                ActionPatternHandler((ActionPattern)elementToBeTriggered);
            }
            else
            {
                TriggerAction((Action)elementToBeTriggered);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ActionPatternHandler(ActionPattern actionPattern)
    {
        ABOD3_Bridge.GetInstance().AletForElement(botNumber, actionPattern.Name, "AP");
        StartCoroutine(ExecuteActionPattern(actionPattern, 0));
    }

    IEnumerator ExecuteActionPattern(ActionPattern actionPattern, int currentActionindex)
    {
        if (currentActionindex < actionPattern.Actions.Count)
        {
            TriggerAction(actionPattern.Actions[currentActionindex]);
            yield return new WaitForSeconds((float)
                actionPattern.Actions[currentActionindex].TimeToComplete);

            //  DrivesHandler();

            StartCoroutine(ExecuteActionPattern(actionPattern, currentActionindex + 1));
        }
        else
            yield return new WaitForSeconds(0);
    }

    private void TriggerAction(Action action)
    {
        ABOD3_Bridge.GetInstance().AletForElement(botNumber, action.Name, "A");
        try
        {
            behaviourLibrary.ExecuteAction(action);
        }
        catch (System.Exception error)
        {
            Debug.LogError("Actions: " + action.Name + " produced error: " + error);
        }
    }
}