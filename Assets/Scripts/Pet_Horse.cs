using UnityEngine;

public class Pet_Horse : PetBase
{
    protected override void Awake()
    {
        base.Awake();

        _attackDamage = 35.0f;
        _moveSpeed = 3.0f;

        Agent.speed = _moveSpeed;
    }
}
