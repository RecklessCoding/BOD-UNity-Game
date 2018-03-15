using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAgent
{
    void SetName(string name);
    string GetName();

    AnimationController GetAnimationController();

    Team.Type GetTeam();

    Vector3 GetLocation();

    int GetHealth();
    int GetMaxHealth();

    int DamageHandler(int damage, Vector3 damagerLocation);

    Transform GetFlagHolder();

    void DropFlag();
    void GrabFlag(IGrabable flag);
    bool HasFlag();
    Transform GetFlag();

    bool IsDead();
}
