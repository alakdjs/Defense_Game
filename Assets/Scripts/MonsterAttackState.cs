using UnityEngine;

public class MonsterAttackState : IState
{
    private MonsterController _monster;
    private bool _hasAttacked = false;

    public MonsterAttackState(MonsterController monster)
    {
        _monster = monster;
    }

    public void Enter()
    {
        _monster.Agent.isStopped = true;
        _monster.Animator.SetTrigger("Attack");
        _hasAttacked = false;
    }

    public void Execute()
    {
        // 애니메이션이 끝났을 때
        if (!_hasAttacked)
        {
            _hasAttacked = true;
            return;
        }

        // 다시 추적
        _monster.StateMachine.ChangeState(_monster.ChaseState);
    }

    public void Exit()
    {

    }
}
