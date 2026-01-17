using System;
using System.Collections.Generic;
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

    [Header("Values")]
    [Tooltip("스택 1회 선택 시 적용될 효과 값")]
    public float[] valuesPerLevel;

    [Serializable]
    public class WeightedPetPrefab
    {
        [Tooltip("소환할 펫 프리팹")]
        public GameObject prefab;

        [Tooltip("가중치(비율)")]
        public int weight = 1;
    }

    [Header("Pet")]
    [Tooltip("소환할 펫 프리팹 + 가중치 목록")]
    public List<WeightedPetPrefab> weightedPetPrefabs = new List<WeightedPetPrefab>();

    [Tooltip("펫 랜덤 소환 여부(true면 weightedPetPrefabs에서 가중 랜덤 1마리)")]
    public bool spawnRandomPet = true;

    [Tooltip("랜덤이 아니라면, 이 프리팹을 소환")]
    public GameObject specificPetPrefab;

    /// <summary>
    /// 효과 적용
    /// - 스택형 운영: 선택될 때마다 동일하게 1회 적용
    /// </summary>
    public void Apply(PlayerController player, TowerMain tower)
    {
        // EquipWeapon, SpawnPet은 value가 필요 없으므로 valuesPerLevel 검사 제외
        if (effectType != EffectType.EquipWeapon && effectType != EffectType.SpawnPet)
        {
            if (valuesPerLevel == null || valuesPerLevel.Length == 0)
            {
                Debug.LogWarning($"{name}: valuesPerLevel이 비어있습니다.");
                return;
            }
        }

        float value = 0.0f;
        if (valuesPerLevel != null && valuesPerLevel.Length > 0)
        {
            value = valuesPerLevel[0];
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

            case EffectType.AuraSphere:
                ApplyAddAuraSphere(player, value);
                break;

            case EffectType.TowerMaxHp:
                {
                    if (tower != null)
                    {
                        tower.AddMaxHp(value);
                    }
                    break;
                }

            case EffectType.TowerDefense:
                {
                    if (tower != null)
                    {
                        tower.AddDefense(value);
                    }
                    break;
                }

            case EffectType.SpawnPet:
                {
                    // 펫 소환은 AugmentManager에서 처리(소환된 Pet 참조/카운트 관리 필요)
                    break;
                }

            default:
                Debug.LogWarning($"구현되지 않은 EffectType: {effectType}");
                break;
        }

        Debug.Log($"[증강 효과 적용] {effectName} | Type: {effectType} | Value(Add): {value}");
    }

    public GameObject PickSpawnPetPrefab()
    {
        if (effectType != EffectType.SpawnPet)
            return null;

        if (!spawnRandomPet)
        {
            return specificPetPrefab;
        }

        if (weightedPetPrefabs == null || weightedPetPrefabs.Count == 0)
        {
            Debug.LogWarning($"[AugmentEffect.SpawnPet] weightedPetPrefabs가 비어있습니다: {name}");
            return null;
        }

        int totalWeight = 0;

        for (int i = 0; i < weightedPetPrefabs.Count; i++)
        {
            WeightedPetPrefab e = weightedPetPrefabs[i];
            if (e == null || e.prefab == null || e.weight <= 0)
                continue;

            totalWeight += e.weight;
        }

        if (totalWeight <= 0)
        {
            Debug.LogWarning($"[AugmentEffect.SpawnPet] totalWeight가 0 입니다: {name}");
            return null;
        }

        int roll = UnityEngine.Random.Range(0, totalWeight); // [0, totalWeight)
        int acc = 0;

        for (int i = 0; i < weightedPetPrefabs.Count; i++)
        {
            WeightedPetPrefab e = weightedPetPrefabs[i];
            if (e == null || e.prefab == null || e.weight <= 0)
                continue;

            acc += e.weight;
            if (roll < acc)
            {
                return e.prefab;
            }
        }

        return null;
    }

    #region Effect Implementations

    /// <summary>
    /// 플레이어 스탯 증가
    /// </summary>
    private void ApplyStatBoost(PlayerController player, float addValue)
    {
        player.AddStatAdditive(statType, addValue);
    }

    /// <summary>
    /// 현재 장착 무기 데미지 증가
    /// </summary>
    private void ApplyWeaponDamage(PlayerController player, float addValue)
    {
        player.UpgradeCurrentWeaponDamage(addValue);
    }

    /// <summary>
    /// 현재 장착 무기 사거리 증가
    /// </summary>
    private void ApplyWeaponRange(PlayerController player, float addValue)
    {
        player.IncreaseCurrentWeaponRange(addValue);
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

    /// <summary>
    /// 플레이어 몸에서 360도 대칭 추가탄 개수 증가
    /// </summary>
    private void ApplyAddAuraSphere(PlayerController player, float addValue)
    {
        // valuesPerLevel이 float이라서 int로 변환
        int addCount = Mathf.RoundToInt(addValue);

        if (addCount <= 0)
        {
            Debug.LogWarning($"[증강] ExtraRadialBullets addValue가 0 이하입니다. ({addValue})");
            return;
        }

        player.AddAuraSphere(addCount);
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

        // EquipWeapon, SpawnPet은 value가 필요 없으므로 valuesPerLevel 검사 제외
        if (effectType != EffectType.EquipWeapon && effectType != EffectType.SpawnPet)
        {
            // 값 배열이 비어있으면 경고
            if (valuesPerLevel == null || valuesPerLevel.Length == 0)
            {
                Debug.LogWarning($"{name}: valuesPerLevel이 비어있습니다.(OnValidate)");
            }
        }
    }

    #endregion
}

/// <summary>
/// 효과 타입 정의
/// </summary>
public enum EffectType
{
    StatBoost,          // 플레이어 스탯 증가
    WeaponDamage,       // 현재 장착 무기 데미지 증가
    WeaponRange,        // 현재 장착 무기 사거리 증가
    Heal,               // 즉시 체력 회복
    EquipWeapon,        // 무기 장착

    AuraSphere,         // 파동탄(플레이어 몸에서 360도 대칭 추가탄 발사)

    // 타워 증강
    TowerMaxHp,         // 타워의 최대 체력 증가
    TowerDefense,       // 타워의 방어력 증가

    SpawnPet            // 펫 소환
}

/// <summary>
/// 스탯 타입 정의
/// </summary>
public enum StatType
{
    MaxHealth,          // 최대 체력
    MoveSpeed,          // 이동 속도
    AttackDamage,       // 공격력
    AttackSpeed,        // 공격 속도 (쿨타임 감소)
    Defense            // 방어력
}
