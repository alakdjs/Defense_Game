using UnityEngine;

public class Pet_Chicken : PetBase
{
    protected override void Awake()
    {
        base.Awake();

        _attackDamage = 35.0f;
        _maxHp = 200.0f;
        _defense = 1.0f;
        _moveSpeed = 3.0f;
        Agent.speed = _moveSpeed;
    }
}
