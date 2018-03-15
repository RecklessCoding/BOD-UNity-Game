using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DriveCollection: PlanElement
{
    private List<Condition> conditions;
    internal List<Condition> Conditions
    {
        get
        {
            return conditions;
        }
    }

    private List<DriveElement> driveElements;
    internal List<DriveElement> DriveElements
    {
        get
        {
            return driveElements;
        }
    }

    public DriveCollection(string name, List<Condition> condition, 
        List<DriveElement> driveElements) : base(name)
    {
        if (condition != null)
            this.conditions = condition;
        else
            this.conditions = new List<Condition>();

        if (driveElements != null)
            this.driveElements = driveElements;
        else
            this.driveElements = new List<DriveElement>();
    }
}