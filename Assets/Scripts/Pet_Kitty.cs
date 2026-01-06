using UnityEngine;

public class Pet_Kitty : PetBase
{
    protected override void Awake()
    {
        base.Awake();

        _attackDamage = 25.0f;
        _moveSpeed = 3.0f;

        Agent.speed = _moveSpeed;
    }
}
