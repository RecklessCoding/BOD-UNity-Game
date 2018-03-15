using System.Collections.Generic;

public class PlanElement
{
    private string name = "";
    internal string Name
    {
        get
        {
            return name;
        }
    }

    public PlanElement(string name)
    {
        this.name = name;
    }
}