using UnityEngine;

public class PlayerMoveState : IState
{
    private PlayerController _player;

    public PlayerMoveState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        // 이동 애니메이션 실행
        _player.Animator.SetBool("IsMovingPlayer", true);
    }

    public void Execute()
    {
        // 키보드 입력 우선 처리
        if (_player.KeyboardInput != Vector3.zero)
        {
            HandleKeyboardMovement();
            return;
        }

        // 만약 타겟이 없으면 Idle로 복귀
        if (!_player.HasTarget)
        {
            _player.StateMachine.ChangeState(_player.IdleState);
            return;
        }

        HandleMouseMovement();
    }

    public void Exit()
    {
        // 이동을 멈출 때 속도 제거
        _player.Rigidbody.linearVelocity = Vector3.zero;
        _player.Animator.SetBool("IsMovingPlayer", false);
    }

    // 키보드 이동 처리
    private void HandleKeyboardMovement()
    {
        Vector3 moveVelocity = new Vector3(
            _player.KeyboardInput.x * _player.Speed,
            _player.Rigidbody.linearVelocity.y,
            _player.KeyboardInput.z * _player.Speed
        );
        _player.Rigidbody.linearVelocity = moveVelocity;
    }

    // 마우스 클릭 이동 처리
    private void HandleMouseMovement()
    {
        Vector3 current = _player.transform.position;
        Vector3 target = _player.TargetPosition;

        // Y값 고정 (지면 높이 차이가 있을 수 있으므로)
        target.y = current.y;
        Vector3 direction = (target - current).normalized;

        // 도착 체크
        float distance = Vector3.Distance(current, target);
        if (distance < 0.2f)
        {
            _player.Rigidbody.linearVelocity =
                new Vector3(0, _player.Rigidbody.linearVelocity.y, 0);
            _player.ClearTarget();
            _player.StateMachine.ChangeState(_player.IdleState); // 도착했으면 Idle 상태로 전환
            return;
        }

        // 이동 처리
        Vector3 moveVelocity = new Vector3(
            direction.x * _player.Speed,
            _player.Rigidbody.linearVelocity.y,
            direction.z * _player.Speed
        );
        _player.Rigidbody.linearVelocity = moveVelocity;
    }
}
