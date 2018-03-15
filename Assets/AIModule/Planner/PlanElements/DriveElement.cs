using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DriveElement : PlanElement
{
    private List<Condition> conditions;
    internal List<Condition> Conditions
    {
        get
        {
            return conditions;
        }
    }

    private float checkTime = 0;

    private float nextCheck = 0;
    internal float NextCheck
    {
        get
        {
            return nextCheck;
        }
    }
    internal void UpdateNextCheck()
    {
        nextCheck = Time.time + checkTime;
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

    public DriveElement(string name, List<Condition> condition, PlanElement triggerableElement, float checkTime) : base(name)
    {
        if (condition != null)
            this.conditions = condition;
        else
            this.conditions = new List<Condition>();

        this.triggerableElement = triggerableElement;

        this.checkTime = checkTime;
        this.nextCheck = this.checkTime;
    }
}