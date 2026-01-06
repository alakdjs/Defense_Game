using UnityEngine;

public class Pet_Pinguin : PetBase
{
    protected override void Awake()
    {
        base.Awake();

        _attackDamage = 30.0f;
        _moveSpeed = 3.0f;

        Agent.speed = _moveSpeed;
    }
}
