using UnityEngine;

/// <summary>
/// 모든 증강 효과를 하나의 클래스에서 처리
/// </summary>
[CreateAssetMenu(menuName = "Augment/AugmentEffect")]
public class AugmentEffect : ScriptableObject
{
    [Header("Effect Info")]
    [Tooltip("효과 설명 (디버그용)")]
    public string effectName;

    [Header("Effect Type")]
    public EffectType effectType;

    [Header("Target")]
    [Tooltip("스탯 효과일 때 사용")]
    public StatType statType;

    [Tooltip("무기 장착 효과일 때 사용")]
    public WeaponType weaponTypeToEquip;

    [Header("Values per Level")]
    [Tooltip("레벨별 효과 값 (퍼센트, %)")]
    public float[] valuesPerLevel;

    /// <summary>
    /// 효과 적용
    /// </summary>
    public void Apply(int level)
    {
        if (level < 1 || level > valuesPerLevel.Length)
        {
            Debug.LogWarning($"AugmentEffect: 잘못된 레벨 {level} (valuesPerLevel 길이: {valuesPerLevel.Length})");
            return;
        }

        float value = valuesPerLevel[level - 1];

        // PlayerController 찾기
        PlayerController player = FindPlayerController();
        if (player == null)
        {
            Debug.LogError("PlayerController를 찾을 수 없습니다!");
            return;
        }

        switch (effectType)
        {
            case EffectType.StatBoost:
                ApplyStatBoost(player, value);
                break;

            case EffectType.WeaponDamage:
                ApplyWeaponDamage(player, value);
                break;

            case EffectType.WeaponRange:
                ApplyWeaponRange(player, value);
                break;

            case EffectType.Heal:
                ApplyHeal(player, value);
                break;

            case EffectType.EquipWeapon:
                EquipWeapon(player);
                break;

            default:
                Debug.LogWarning($"구현되지 않은 EffectType: {effectType}");
                break;
        }

        Debug.Log($"[증강 효과 적용] {effectName} | Type: {effectType} | Value: {value}% | Level: {level}");
    }

    #region Effect Implementations

    /// <summary>
    /// 플레이어 스탯 증가
    /// </summary>
    private void ApplyStatBoost(PlayerController player, float percentIncrease)
    {
        player.AddStatMultiplier(statType, percentIncrease);
    }

    /// <summary>
    /// 현재 장착 무기 데미지 증가
    /// </summary>
    private void ApplyWeaponDamage(PlayerController player, float percentIncrease)
    {
        player.UpgradeCurrentWeaponDamage(percentIncrease);
    }

    /// <summary>
    /// 현재 장착 무기 사거리 증가
    /// </summary>
    private void ApplyWeaponRange(PlayerController player, float percentIncrease)
    {
        player.IncreaseCurrentWeaponRange(percentIncrease);
    }

    /// <summary>
    /// 즉시 체력 회복
    /// </summary>
    private void ApplyHeal(PlayerController player, float amount)
    {
        player.Heal(amount);
    }

    /// <summary>
    /// 무기 장착
    /// </summary>
    private void EquipWeapon(PlayerController player)
    {
        WeaponData weaponToEquip = WeaponDatabase._Instance.GetRandomWeapon(weaponTypeToEquip);

        if (weaponToEquip != null)
        {
            player.EquipWeapon(weaponToEquip);
            Debug.Log($"[증강] {weaponTypeToEquip} 타입 무기 장착: {weaponToEquip.name}");
        }
        else
        {
            Debug.LogError($"[증강] {weaponTypeToEquip} 타입의 무기를 찾을 수 없습니다!");
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// PlayerController 찾기
    /// </summary>
    private PlayerController FindPlayerController()
    {
        // 태그로 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            return playerObj.GetComponent<PlayerController>();
        }

        // 태그가 없으면 직접 찾기
        return FindAnyObjectByType<PlayerController>();
    }

    #endregion

    #region Validation

    /// <summary>
    /// 인스펙터 유효성 검사
    /// </summary>
    private void OnValidate()
    {
        // 효과 이름이 비어있으면 타입으로 자동 설정
        if (string.IsNullOrEmpty(effectName))
        {
            effectName = effectType.ToString();
        }

        // 값 배열이 비어있으면 경고
        if (valuesPerLevel == null || valuesPerLevel.Length == 0)
        {
            Debug.LogWarning($"{name}: valuesPerLevel이 비어있습니다.");
        }
    }

    #endregion
}

/// <summary>
/// 효과 타입 정의 (간소화)
/// </summary>
public enum EffectType
{
    StatBoost,          // 플레이어 스탯 증가
    WeaponDamage,       // 현재 장착 무기 데미지 증가
    WeaponRange,        // 현재 장착 무기 사거리 증가
    Heal,               // 즉시 체력 회복
    EquipWeapon,        // 무기 장착
}

/// <summary>
/// 스탯 타입 정의
/// </summary>
public enum StatType
{
    MaxHealth,          // 최대 체력
    MoveSpeed,          // 이동 속도
    AttackDamage,       // 공격력
    AttackSpeed,        // 공격 속도 (쿨다운 감소)
    Armor,              // 방어력
    PickupRange,        // 아이템 획득 범위 (현재는 몬스터 인식 범위로 사용)
}