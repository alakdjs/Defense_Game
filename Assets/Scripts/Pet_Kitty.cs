using UnityEngine;

public class Pet_Kitty : PetBase
{
    protected override void Awake()
    {
        base.Awake();

        _attackDamage = 50.0f;
        _maxHp = 200.0f;
        _defense = 1.1f;
        _moveSpeed = 3.0f;
        Agent.speed = _moveSpeed;
    }
}
