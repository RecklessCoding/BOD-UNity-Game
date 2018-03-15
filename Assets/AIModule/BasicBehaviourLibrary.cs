using System.Collections.Generic;
using UnityEngine;

public class BasicBehaviourLibrary : BehaviourLibraryLinker
{
    /*
    NAVIGATOR
    */
    private bool returningToSpawn = false;

    public bool DoesPathExist()
    {
        return NavAgent.pathGenerated.Count > 0;
    }

    public void SetPathToEnemyBase()
    {
        NavAgent.TargetCell = GridManager.instance.FindClosestCell(EnemySpawnLocation);
    }

    public void SetPathToRandom()
    {
        Vector2 pos;
        pos.x = (float)Random.Range(-GridManager.instance.gridSize.x * 0.5f, GridManager.instance.gridSize.x * 0.5f);
        pos.y = (float)Random.Range(-GridManager.instance.gridSize.y * 0.5f, GridManager.instance.gridSize.y * 0.5f);

        NavAgent.TargetCell = GridManager.instance.FindClosestCell(pos);
    }

    public void LookAtNextNavPoint()
    {
        LookAt(NavAgent.pathGenerated[0]);
    }

    public void MoveToNextNode()
    {
        if (NavAgent.pathGenerated.Count > 0)
        {
            MoveTowards(NavAgent.pathGenerated[0]);
        }
    }


    public void ReturnToBase()
    {
        if (!returningToSpawn)
        {
            returningToSpawn = true;

            NavAgent.TargetCell = GridManager.instance.FindClosestCell(SpawnLocation);
        }
    }

    public void LookAtDamage()
    {
        LookAt(LocationLastIncomingFire);
    }



    /*
    COMBAT
    */
    public bool UnderAttack()
    {
        return IsDamaged;
    }

    public bool AllTargetsAreDead()
    {
        return !EnemiesSpotted();
    }

    public void ShootEnemiesInSight()
    {
        if (!EnemiesSpotted())
            return;

        IAgent targetSoldier = EnemyAgentsInSight[0];
        // Any enemy soldiers in sight?
        if (targetSoldier != null)
        {
            // Look at them, if we are, shoot.
            if (LookAt(targetSoldier.GetLocation()))
            {
                // pew pew
                Shoot();
            }
        }
    }

    public bool EnemiesSpotted()
    {
        return EnemyAgentsInSight.Count > 0;
    }

    public void Shoot()
    {
        if (IsDead)
            return;
        CurrentWeapon.Shoot();
    }

    /*
    Flag
    */
    public bool HoldsFlag()
    {
        return HasFlag;
    }

    public bool EnemyTeamFlagInSight()
    {
        return EnemyFlagInSight != null;
    }

    public void GrabEnemyTeamFlag()
    {
        if (EnemyTeamFlagInSight())
        {
            GrabFlag(EnemyFlagInSight);
        }
    }
}