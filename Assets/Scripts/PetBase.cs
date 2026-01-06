using UnityEngine;
using UnityEngine.AI;

public class PetBase : MonoBehaviour
{
    [Header("Base Stat")]
    [SerializeField] protected float _maxHp = 100.0f;
    [SerializeField] protected float _attackDamage = 5.0f; // 공격력
    [SerializeField] protected float _attackRange = 2.0f;  // 공격 범위
    [SerializeField] protected float _moveSpeed = 3.5f;    // 이동 속도
    [SerializeField] protected float _detectRange = 6.0f;  // 몬스터 탐지 범위

    protected float _currentHp;
    protected bool _isDead = false;

    [SerializeField] protected bool _canAct = true;

    [SerializeField] protected Animator _animator;
    [SerializeField] protected Transform _tower;
    protected Transform _targetMonster;

    protected NavMeshAgent _agent;

    // FSM

}
