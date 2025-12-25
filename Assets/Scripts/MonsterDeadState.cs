using UnityEngine;

public class MonsterDeadState : IState
{
    private Monster_PartyMonster _monster;

    public MonsterDeadState(Monster_PartyMonster monster)
    {
        _monster = monster;
    }

    public void Enter()
    {
        _monster.Agent.isStopped = true;

        if (_monster.Animator != null)
        {
            _monster.Animator.SetTrigger("Die");
        }
        else
        {
            // 애니메이션 없으면 즉시 제거
            _monster.OnDieAnimationEnd();
        }
    }
    
    public void Execute()
    {

    }

    public void Exit()
    {

    }
}
