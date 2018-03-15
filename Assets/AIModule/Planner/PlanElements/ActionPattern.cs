using System.Collections.Generic;

public class ActionPattern : PlanElement
{
    private List<Action> actions;
    internal List<Action> Actions
    {
        get
        {
            return actions;
        }
    }

    public ActionPattern(string name, List<Action> actions) : base(name)
    {
        this.actions = actions;
    }
}