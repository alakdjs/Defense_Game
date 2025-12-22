using UnityEngine;

public class MonsterAttackState : IState
{
    private MonsterController _monster;

    public MonsterAttackState(MonsterController monster)
    {
        _monster = monster;
    }

    public void Enter()
    {
        _monster.Agent.isStopped = true;
        _monster.Animator.SetTrigger("Attack");
    }

    public void Execute()
    {
        // 애니메이션 이벤트에서 상태 전환
    }

    public void Exit()
    {

    }
}
