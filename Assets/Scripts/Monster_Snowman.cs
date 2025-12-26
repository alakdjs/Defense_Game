using UnityEngine;
using UnityEngine.AI;


public class Monster_Snowman : MonsterBase
{
    [SerializeField] private Transform _mouthPoint;
    [SerializeField] private GameObject _snowballPrefab;
    [SerializeField] private float _attackCoolTime = 2.0f;

    private float _lastAttackTime;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void PerformAttack()
    {
        if (Time.time < _lastAttackTime + _attackCoolTime)
        {
            StateMachine.ChangeState(ChaseState);
            return;
        }

        _lastAttackTime = Time.time;

        FireSnowball();
        StateMachine.ChangeState(ChaseState);
    }

    private void FireSnowball()
    {
        if (_snowballPrefab == null || _mouthPoint == null || _target == null)
            return;

        Vector3 dir = (_target.position - _mouthPoint.position).normalized;

        GameObject snowball = Instantiate(_snowballPrefab, _mouthPoint.position, Quaternion.LookRotation(dir));

        Monster_Snowman_Snowball projectile = snowball.GetComponent<Monster_Snowman_Snowball>();

        if (projectile != null)
        {
            projectile.Init(AttackRange);
        }
    }

    public override void Die()
    {
        // Melt 처리, FSM은 Die()처리로 인식
    }

    public override void OnDieAnimationEnd()
    {
        base.OnDieAnimationEnd();
    }

    private void LateUpdate()
    {
        // 죽었을 때만 Melt 효과
        if (_isDead)
        {
            Vector3 scale = transform.localScale;
            scale.y -= Time.deltaTime * 2.0f;
            scale.y = Mathf.Max(0.0f, scale.y);
            transform.localScale = scale;

            if (scale.y <= 0.01f)
            {
                CleanUpHpBar();
                Destroy(gameObject);
            }
        }
    }

}
