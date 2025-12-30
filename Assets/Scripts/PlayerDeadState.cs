using UnityEngine;

public class PlayerDeadState : IState
{
    private PlayerController _player;

    public PlayerDeadState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.Animator.SetBool("IsMoving", false);
        _player.Animator.SetBool("IsDead", true);

        // 이동 중이었다면 즉시 멈춤
        _player.Rigidbody.linearVelocity = Vector3.zero;
    }

    public void Execute()
    {
        // Dead에서는 아무것도 하지 않음.
        // 모든 입력 무시.
    }

    public void Exit()
    {
        // 보통 Dead에서 나가는 일은 없지만,
        // 필요하면 여기에 복귀용 코드 작성 가능.
    }
}
