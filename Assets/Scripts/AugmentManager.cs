using System.Collections.Generic;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    public static AugmentManager Instance;

    [SerializeField] private List<AugmentData> _allAugments = new List<AugmentData>();

    // 증강별 현재 레벨 (Key: AugmentData, Value: 현재 레벨)
    private readonly Dictionary<AugmentData, int> _augmentLevels = new Dictionary<AugmentData, int>();

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
    }

    /// <summary>
    /// 현재 선택 가능한 증강 목록 반환
    /// </summary>
    public List<AugmentData> GetAvailableAugments()
    {
        List<AugmentData> result = new List<AugmentData>();

        foreach (var augment in _allAugments)
        {
            // 현재 레벨 확인
            int currentLevel = GetAugmentLevel(augment);

            // 최대 레벨 도달 시 제외
            if (currentLevel >= augment.maxLevel)
                continue;

            // 조건 체크
            if (!CheckCondition(augment))
                continue;

            result.Add(augment);
        }

        return result;
    }

    /// <summary>
    /// 카테고리별 선택 가능한 증강 목록 반환
    /// </summary>
    public List<AugmentData> GetAvailableAugmentsByCategory(AugmentCategory category)
    {
        List<AugmentData> all = GetAvailableAugments();
        List<AugmentData> filtered = new List<AugmentData>();

        foreach (var augment in all)
        {
            if (augment.category == category)
            {
                filtered.Add(augment);
            }
        }

        return filtered;
    }

    /// <summary>
    /// 증강 적용 (레벨업 포함)
    /// </summary>
    public void ApplyAugment(AugmentData augment)
    {
        if (augment == null)
        {
            Debug.LogWarning("AugmentManager : augment가 null입니다.");
            return;
        }

        // 현재 레벨 가져오기
        int currentLevel = GetAugmentLevel(augment);

        // 최대 레벨 체크
        if (currentLevel >= augment.maxLevel)
        {
            Debug.LogWarning($"최대 레벨에 도달한 증강입니다: {augment.augmentName}");
            return;
        }

        // 레벨업
        _augmentLevels[augment] = currentLevel + 1;
        int newLevel = _augmentLevels[augment];

        // 증강 효과 적용 (레벨 정보 전달)
        foreach (var effect in augment.effects)
        {
            if (effect == null) continue;
            effect.Apply(newLevel); // 레벨 정보 전달
        }

        Debug.Log($"증강 적용: {augment.augmentName} Lv.{newLevel}");
    }

    /// <summary>
    /// 특정 증강의 현재 레벨 반환
    /// </summary>
    public int GetAugmentLevel(AugmentData augment)
    {
        if (augment == null) return 0;
        return _augmentLevels.TryGetValue(augment, out int level) ? level : 0;
    }

    /// <summary>
    /// 증강 선택 조건 체크
    /// </summary>
    private bool CheckCondition(AugmentData augment)
    {
        if (augment.requiredAugment != null)
        {
            // 선행 증강 체크
            int requiredLevel = GetAugmentLevel(augment.requiredAugment);
            if (requiredLevel < augment.requiredLevel)
                return false;
        }

        return true;
    }

    /// <summary>
    /// 선택한 증강 목록 반환 (UI 표시용)
    /// </summary>
    public Dictionary<AugmentData, int> GetSelectedAugments()
    {
        return new Dictionary<AugmentData, int>(_augmentLevels);
    }

    /// <summary>
    /// 게임 재시작 시 초기화
    /// </summary>
    public void ResetAugments()
    {
        _augmentLevels.Clear();
        Debug.Log("증강 시스템 초기화 완료");
    }
}