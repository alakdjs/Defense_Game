using UnityEngine;

public class Pet_Dear : PetBase
{
    protected override void Awake()
    {
        base.Awake();

        _attackDamage = 70.0f;
        _maxHp = 200.0f;
        _defense = 1.3f;
        _moveSpeed = 3.0f;
        Agent.speed = _moveSpeed;
    }
}
