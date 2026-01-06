using UnityEngine;

public class Pet_Tiger : PetBase
{
    protected override void Awake()
    {
        base.Awake();

        _attackDamage = 50.0f;
        _moveSpeed = 3.0f;

        Agent.speed = _moveSpeed;
    }
}
