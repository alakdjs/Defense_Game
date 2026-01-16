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
    public AugmentCategory category;

    [Header("Effects")]
    [Tooltip("적용할 효과 목록")]
    public List<AugmentEffect> effects;

    [Header("Conditions")]
    [Tooltip("이 증강이 등장/선택되기 위한 선행 증강 목록")]
    public List<AugmentData> requiredAugment;

    [Header("Description")]
    [TextArea]
    public string description;

    [Header("Rules")]
    [Tooltip("true면 1회만 선택 가능 (선택 후 다시 등장하지 않음)")]
    public bool oneTimeOnly = false;

    [Header("Stack Limit")]
    [Tooltip("이 증강의 최대 선택(스택) 개수 (0이면 무제한)")]
    public int maxStack = 0;

    /// <summary>
    /// 스택형 증강, 선행 조건을 만족하면 선택 가능
    /// </summary>
    public bool CanSelect(AugmentManager manager)
    {
        if (manager == null)
            return false;

        // WeaponSelect 같은 카테고리가 잠겨있으면 등장/선택 불가
        if (manager.IsCategoryLocked(category))
            return false;

        // 1회성 증강: 이미 1스택 이상이면 더 이상 선택 불가
        if (oneTimeOnly && manager.GetAugmentStack(this) > 0)
            return false;

        // 최대 증강 스택 제한 (0이면 무제한)
        if (maxStack > 0 && manager.GetAugmentStack(this) >= maxStack)
            return false;

        // 선행 조건이 없으면 항상 가능
        if (requiredAugment == null || requiredAugment.Count == 0)
            return true;

        // 선행 증강을 최소 1스택 이상 보유해야 함
        for (int i = 0; i < requiredAugment.Count; i++)
        {
            AugmentData pre = requiredAugment[i];
            if (pre == null)
                continue;

            if (manager.GetAugmentStack(pre) <= 0)
                return false;
        }

        return true;
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