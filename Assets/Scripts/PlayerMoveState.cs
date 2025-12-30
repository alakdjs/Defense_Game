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
        _player.Animator.SetBool("IsMoving", true);
    }

    public void Execute()
    {
        // 만약 타겟이 없으면 Idle로 복귀
        if (!_player.HasTarget)
        {
            _player.StateMachine.ChangeState(_player.IdleState);
            return;
        }

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

            // 도착했으면 Idle 상태로 전환
            _player.StateMachine.ChangeState(_player.IdleState);
            return;
        }

        // -------- 이동 처리 --------
        Vector3 moveVelocity = new Vector3(
            direction.x * _player.MoveSpeed,
            _player.Rigidbody.linearVelocity.y,
            direction.z * _player.MoveSpeed
        );

        _player.Rigidbody.linearVelocity = moveVelocity;

        /*
        // -------- 회전 처리 --------
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
            _player.transform.rotation =
                Quaternion.Slerp(_player.transform.rotation, targetRot, Time.deltaTime * 10f);
        }
        */
    }

    public void Exit()
    {
        // 이동을 멈출 때 속도 제거
        _player.Rigidbody.linearVelocity = Vector3.zero;
        _player.Animator.SetBool("IsMoving", false);
    }
}
