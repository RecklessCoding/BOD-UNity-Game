using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentBLTemplate : BasicBehaviourLibrary {

    public void MovetoFlag()
    {
        LookAt(EnemyFlagInSight.GetLocation());
        MoveTowards(EnemyFlagInSight.GetLocation());
    }
}