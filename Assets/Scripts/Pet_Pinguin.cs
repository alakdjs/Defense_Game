using UnityEngine;

public class Pet_Pinguin : PetBase
{
    protected override void Awake()
    {
        base.Awake();

        _attackDamage = 60.0f;
        _maxHp = 200.0f;
        _defense = 1.2f;
        _moveSpeed = 3.0f;
        Agent.speed = _moveSpeed;
    }
}
