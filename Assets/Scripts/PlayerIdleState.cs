using UnityEngine;

public class PlayerIdleState : IState
{
    private PlayerController _player;


    public PlayerIdleState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.Animator.SetBool("IsMoving", false);
        _player.Rigidbody.linearVelocity = Vector3.zero;
    }

    public void Execute()
    {
        _player.CheckDead();

        // 움직이기 시작하면 즉시 MoveState로 전환
        if (_player.HasTarget)
        {
            _player.Animator.SetBool("IsMoving", true);
            _player.StateMachine.ChangeState(_player.MoveState);
            return;
        }

    }

    public void Exit()
    {

    }
}
