using UnityEngine;

public interface IShootable
{

    Bullet Shoot();

    Bullet GetBullet();

    IAgent GetOwner();

    void SetOwner(IAgent newOwner);

    Transform GetMuzzle();

}
