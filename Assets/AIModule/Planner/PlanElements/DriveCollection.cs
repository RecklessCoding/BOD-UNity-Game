using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DriveCollection: PlanElement
{
    private List<Sense> senses;
    internal List<Sense> Senses
    {
        get
        {
            return senses;
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

    public DriveCollection(string name, List<Sense> senses, 
        List<DriveElement> driveElements) : base(name)
    {
        if (senses != null)
            this.senses = senses;
        else
            this.senses = new List<Sense>();

        if (driveElements != null)
            this.driveElements = driveElements;
        else
            this.driveElements = new List<DriveElement>();
    }
}