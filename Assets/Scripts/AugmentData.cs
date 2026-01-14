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

    /// <summary>
    /// 스택형 증강, 선행 조건을 만족하면 선택 가능
    /// </summary>
    public bool CanSelect(AugmentManager manager)
    {
        if (manager == null)
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