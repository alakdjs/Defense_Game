using System.Collections.Generic;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    public static AugmentManager Instance;

    [SerializeField] private List<AugmentData> _allAugments = new List<AugmentData>();

    // 증강 스택(중복 선택 횟수)
    private readonly Dictionary<AugmentData, int> _augmentStacks = new Dictionary<AugmentData, int>();

    private readonly HashSet<AugmentCategory> _lockedCategories = new HashSet<AugmentCategory>();

    // 타겟 목록
    [SerializeField] private PlayerController _player;
    [SerializeField] private TowerMain _tower;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (_player == null)
        {
            Debug.LogWarning("[AugmentManager] PlayerController가 할당되지 않았습니다. Inspector에서 연결해주세요.");
        }

        if (_tower == null)
        {
            Debug.LogWarning("[AugmentManager] TowerMain이 할당되지 않았습니다. Inspector에서 연결해주세요.");
        }
    }

    /// <summary>
    /// 증강 스택 조회 (없으면 0)
    /// </summary>
    public int GetAugmentStack(AugmentData data)
    {
        if (data == null)
            return 0;

        if (_augmentStacks.TryGetValue(data, out int s))
            return s;

        return 0;
    }

    /// <summary>
    /// 선택 가능한 증강 목록 반환 (선행 조건 반영)
    /// </summary>
    public List<AugmentData> GetAvailableAugments()
    {
        List<AugmentData> result = new List<AugmentData>();

        if (_allAugments == null)
            return result;

        for (int i = 0; i < _allAugments.Count; i++)
        {
            AugmentData a = _allAugments[i];
            if (a == null)
                continue;

            // 선행 조건을 만족하는 경우만 후보에 포함
            if (a.CanSelect(this))
                result.Add(a);
        }

        return result;
    }

    // 증강 카테고리 잠금(총, 칼 무기 선택 시 더 이상 안뜨게 하기 위함)
    public bool IsCategoryLocked(AugmentCategory category)
    {
        return _lockedCategories.Contains(category);
    }

    public void LockCategory(AugmentCategory category)
    {
        _lockedCategories.Add(category);
    }

    /// <summary>
    /// 증강 적용(스택 증가 + 효과 적용)
    /// </summary>
    public void ApplyAugment(AugmentData data)
    {
        if (data == null)
            return;

        if (!data.CanSelect(this))
        {
            Debug.LogWarning($"[증강] 선행 조건 미충족: {data.augmentName}");
            return;
        }

        // WeaponSelect 계열은 하나라도 선택하면 카테고리 자체를 잠금 (Rifle/Sword 모두 후보에서 제거)
        if (data.category == AugmentCategory.WeaponSelect)
        {
            LockCategory(AugmentCategory.WeaponSelect);
        }

        if (!_augmentStacks.ContainsKey(data))
            _augmentStacks[data] = 0;

        _augmentStacks[data]++;

        if (_player == null)
        {
            Debug.LogWarning("[AugmentManager] PlayerController 참조가 없습니다. 증강이 정상 적용되지 않을 수 있습니다.");
        }

        if (_tower == null)
        {
            Debug.LogWarning("[AugmentManager] TowerMain 참조가 없습니다. 타워 증강이 적용되지 않습니다.");
        }

        // 효과 적용 (Effect는 스택 개념을 모르고, 1회 적용만 담당)
        // (Find를 Effect에서 하지 않고, Manager가 참조를 전달)
        if (data.effects != null)
        {
            for (int i = 0; i < data.effects.Count; i++)
            {
                AugmentEffect e = data.effects[i];
                if (e == null) continue;

                e.Apply(_player, _tower);
            }
        }

        Debug.Log($"[증강] {data.augmentName} 선택! (스택: {_augmentStacks[data]})");
    }

    /// <summary>
    /// 선택한 증강 목록 반환 (UI 표시용)
    /// </summary>
    public Dictionary<AugmentData, int> GetSelectedAugments()
    {
        // 현재 스택 데이터를 복사해서 반환 (외부에서 수정 방지)
        return new Dictionary<AugmentData, int>(_augmentStacks);
    }

    /// <summary>
    /// 카테고리별 선택 가능한 증강 목록 반환 (선행 조건 반영)
    /// </summary>
    public List<AugmentData> GetAvailableAugmentsByCategory(AugmentCategory category)
    {
        List<AugmentData> result = new List<AugmentData>();

        if (_allAugments == null)
            return result;

        for (int i = 0; i < _allAugments.Count; i++)
        {
            AugmentData a = _allAugments[i];
            if (a == null)
                continue;

            if (a.category != category)
                continue;

            if (a.CanSelect(this))
                result.Add(a);
        }

        return result;
    }


    /// <summary>
    /// 게임 재시작 시 초기화
    /// </summary>
    public void ResetAugments()
    {
        _augmentStacks.Clear();
        _lockedCategories.Clear();
        Debug.Log("증강 시스템 초기화 완료");
    }
}
