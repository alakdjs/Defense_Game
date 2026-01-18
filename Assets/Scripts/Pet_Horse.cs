using UnityEngine;

public class Pet_Horse : PetBase
{
    protected override void Awake()
    {
        base.Awake();

        _attackDamage = 80.0f;
        _maxHp = 200.0f;
        _defense = 1.4f;
        _moveSpeed = 3.0f;
        Agent.speed = _moveSpeed;
    }
}
