using UnityEngine;

public class MonsterAttackState : IState
{
    private MonsterBase _monster;

    public MonsterAttackState(MonsterBase monster)
    {
        _monster = monster;
    }

    public void Enter()
    {
        _monster.Agent.isStopped = true;
        // _monster.Animator.SetTrigger("Attack");
        _monster.PerformAttack();
    }

    public void Execute()
    {
        // 애니메이션 이벤트에서 상태 전환
    }

    public void Exit()
    {
        /*
        _monster.Agent.isStopped = true;

        if (_monster.Animator != null)
        {
            _monster.Animator.SetTrigger("Attack");
        }
        */
    }
}
