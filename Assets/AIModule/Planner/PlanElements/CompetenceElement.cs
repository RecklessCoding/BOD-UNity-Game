using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CompetenceElement
{
    private string name = "";
    internal string Name
    {
        get
        {
            return name;
        }
    }

    private List<Condition> conditions;
    internal List<Condition> Conditions
    {
        get
        {
            return conditions;
        }
    }

    private PlanElement triggerableElement;
    internal PlanElement TriggerableElement
    {
        get
        {
            return triggerableElement;
        }
        set
        {
            triggerableElement = value;
        }
    }


    public CompetenceElement(string name, List<Condition> conditions, PlanElement triggerableElement)
    {
        this.name = name;

        if (conditions != null)
            this.conditions = conditions;
        else
            this.conditions = new List<Condition>();

        this.triggerableElement = triggerableElement;
    }
}