using UnityEngine;

public class Pet_Chicken : PetBase
{
    protected override void Awake()
    {
        base.Awake();

        _attackDamage = 20.0f;
        _moveSpeed = 3.0f;

        Agent.speed = _moveSpeed;
    }
}
