using UnityEngine;
using UnityEngine.AI;


public class Monster_Snowman : MonsterBase
{
    [SerializeField] private Transform _mouthPoint;
    [SerializeField] private GameObject _snowballPrefab;
    [SerializeField] private float _attackCoolTime = 2.0f;

    [SerializeField] private float _aimHeightOffset = 1.3f; // 플레아어 머리 쪽 보정값

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

        Vector3 aimPos = _target.position + Vector3.up * _aimHeightOffset; 
        Vector3 dir = (aimPos - _mouthPoint.position).normalized;

        GameObject snowball = Instantiate(_snowballPrefab, _mouthPoint.position, Quaternion.LookRotation(dir));

        Monster_Snowman_Snowball projectile = snowball.GetComponent<Monster_Snowman_Snowball>();

        if (projectile != null)
        {
            //Debug.Log("Snowball Init 호출됨");
            projectile.Init(AttackRange);
        }
        else
        {
            Debug.LogError("Snowball 스크립트 없음!");
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
            scale.y -= Time.deltaTime * 0.5f;
            scale.y = Mathf.Max(0.0f, scale.y);
            transform.localScale = scale;

            // Die() 애니메이션 효과 대체용 -> y 스케일을 줄여서 바닥 밑으로 없어지는 연출
            if (scale.y <= 0.01f)
            {
                OnDieAnimationEnd();
            }
        }
    }

}
