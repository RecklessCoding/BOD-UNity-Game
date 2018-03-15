using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Competence : PlanElement
{
    private List<Condition> goals;
    internal List<Condition> Goals
    {
        get
        {
            return goals;
        }
    }

    private List<CompetenceElement> elements;
    internal List<CompetenceElement> Elements
    {
        get
        {
            return elements;
        }
    }

    public Competence(string name, List<Condition> goals, List<CompetenceElement> elements) : base(name)
    {
        if (goals != null)
            this.goals = goals;
        else
            this.goals = new List<Condition>();

        if (elements != null)
            this.elements = elements;
        else
            this.elements = new List<CompetenceElement>();
    }
}