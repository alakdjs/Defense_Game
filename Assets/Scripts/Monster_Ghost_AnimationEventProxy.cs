using UnityEngine;

/// <summary>
/// Animator(=Little_Ghost2_v1)에 붙여서 AnimationEvent를 받는 용도.
/// 실제 로직은 부모의 MonsterBase로 전달.
/// </summary>
public class Monster_Ghost_AnimationEventProxy : MonoBehaviour
{
    [SerializeField] private MonsterBase _monster;

    private void Awake()
    {
        if (_monster == null)
        {
            _monster = GetComponentInParent<MonsterBase>();
        }

        if (_monster == null)
        {
            Debug.LogWarning($"[MonsterAnimationEventProxy] MonsterBase not found in parents. ({name})");
        }
    }

    // attack 클립의 AnimationEvent에서 호출
    public void OnAttackHit()
    {
        if (_monster == null)
            return;

        _monster.AnimEvent_AttackHit();
    }

    // attack 클립의 AnimationEvent에서 호출
    public void OnAttackAnimationEnd()
    {
        if (_monster == null)
            return;

        _monster.AnimEvent_AttackEnd();
    }
}
