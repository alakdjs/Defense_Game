using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 하나의 증강을 정의하는 데이터
/// - 여러 AugmentEffect를 가질 수 있다
/// </summary>
[CreateAssetMenu(menuName = "Augment/AugmentData")]
public class AugmentData : ScriptableObject
{
    [Header("Basic Info")]
    public string augmentName;
    public Sprite icon;

    [Header("Category")]
    [Tooltip("증강 카테고리 (무기선택 / 능력치강화)")]
    public AugmentCategory category = AugmentCategory.StatUpgrade;

    [Header("Max Level")]
    [Tooltip("최대 레벨 (뱀서 스타일: 보통 3~5)")]
    public int maxLevel = 5;

    [Header("Description")]
    [Tooltip("레벨별 설명 (배열 크기 = maxLevel)")]
    [TextArea(2, 4)]
    public string[] descriptions;

    [Header("Effects")]
    [Tooltip("적용할 효과 목록")]
    public List<AugmentEffect> effects;

    [Header("Conditions")]
    [Tooltip("선행 증강 (필요 시)")]
    public AugmentData requiredAugment;

    [Tooltip("선행 증강 필요 레벨")]
    public int requiredLevel = 1;

    /// <summary>
    /// 레벨에 맞는 설명 반환
    /// </summary>
    public string GetDescription(int level)
    {
        if (descriptions == null || descriptions.Length == 0)
            return "설명 없음";

        // 레벨 범위 체크
        int index = Mathf.Clamp(level, 0, descriptions.Length - 1);
        return descriptions[index];
    }

    /// <summary>
    /// 유효성 검사 (에디터용)
    /// </summary>
    private void OnValidate()
    {
        // 최대 레벨은 최소 1 이상
        if (maxLevel < 1)
            maxLevel = 1;

        // 설명 배열 크기를 maxLevel에 맞춤
        if (descriptions == null || descriptions.Length != maxLevel)
        {
            System.Array.Resize(ref descriptions, maxLevel);
        }

        // 선행 증강 체크
        if (requiredAugment != null && requiredLevel < 1)
        {
            Debug.LogWarning($"{augmentName}: requiredAugment가 설정되었는데 requiredLevel이 0입니다.");
        }
    }
}

/// <summary>
/// 증강 카테고리
/// </summary>
public enum AugmentCategory
{
    WeaponSelect,   // 무기 선택
    StatUpgrade,    // 능력치 강화
}