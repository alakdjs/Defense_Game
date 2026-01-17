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

    // 펫 소환 증강: 현재 살아있는 활성 펫 수 카운트
    private readonly Dictionary<AugmentData, int> _activePetCount = new Dictionary<AugmentData, int>();

    // 소환된 펫이 어떤 증강에서 나왔는지 매핑(죽으면 증강 카운트 감소)
    private readonly Dictionary<PetBase, AugmentData> _petOwnerAugment = new Dictionary<PetBase, AugmentData>();

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
    /// 펫 소환 증강의 현재 활성 펫 수 조회 (없으면 0)
    /// </summary>
    public int GetActivePetCount(AugmentData data)
    {
        if (data == null)
            return 0;

        if (_activePetCount.TryGetValue(data, out int c))
            return c;

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

        bool usedSpawnPet = false;

        // 효과 적용 (Effect는 스택 개념을 모르고, 1회 적용만 담당)
        if (data.effects != null)
        {
            for (int i = 0; i < data.effects.Count; i++)
            {
                AugmentEffect e = data.effects[i];
                if (e == null) continue;

                // SummonPet은 Manager에서 직접 처리(소환된 Pet 참조/카운트 관리 필요)
                if (e.effectType == EffectType.SpawnPet)
                {
                    usedSpawnPet = true;

                    if (_tower == null)
                    {
                        Debug.LogWarning("[AugmentManager] TowerMain이 없어서 SpawnPet을 처리할 수 없음.");
                        continue;
                    }

                    GameObject prefab = e.PickSpawnPetPrefab();
                    if (prefab == null)
                    {
                        Debug.LogWarning($"[AugmentManager] SpawnPet 프리팹 선택 실패: {data.augmentName}");
                        continue;
                    }

                    // TowerMain 스폰포인트 1개에서 계속 소환
                    PetBase pet = _tower.SpawnPet(prefab);

                    if (pet == null)
                    {
                        Debug.LogWarning($"[AugmentManager] SpawnPet 실패: {prefab.name}에 PetBase가 없습니다.");
                        continue;
                    }

                    // 활성 카운트 증가
                    if (!_activePetCount.ContainsKey(data))
                        _activePetCount[data] = 0;

                    _activePetCount[data]++;

                    // 매핑 등록 + 죽음 이벤트 구독
                    _petOwnerAugment[pet] = data;
                    pet.OnDisposed += HandleSpawndPetDisposed;

                    Debug.Log($"[AugmentManager] SpawnPet 적용: {data.augmentName} (활성: {_activePetCount[data]}/{data.maxStack})");
                }
                else
                {
                    e.Apply(_player, _tower);
                }
            }
        }

        // 일반 증강만 누적 스택 증가
        if (!usedSpawnPet)
        {
            if (!_augmentStacks.ContainsKey(data))
                _augmentStacks[data] = 0;

            _augmentStacks[data]++;

            Debug.Log($"[증강] {data.augmentName} 선택! (스택: {_augmentStacks[data]})");
        }
        else
        {
            Debug.Log($"[증강] {data.augmentName} 선택! (펫 소환 증강)");
        }
    }

    /// <summary>
    /// 펫이 죽거나 파괴되면 호출되어, 해당 증강의 활성 펫 수를 감소
    /// </summary>
    private void HandleSpawndPetDisposed(PetBase pet)
    {
        if (pet == null)
            return;

        pet.OnDisposed -= HandleSpawndPetDisposed;

        if (_petOwnerAugment.TryGetValue(pet, out AugmentData owner))
        {
            _petOwnerAugment.Remove(pet);

            if (_activePetCount.ContainsKey(owner))
            {
                _activePetCount[owner]--;

                if (_activePetCount[owner] < 0)
                    _activePetCount[owner] = 0;

                Debug.Log($"[AugmentManager.HandleSpawndPetDisposed] 펫 사망/파괴로 활성 카운트 감소: {owner.augmentName} (활성: {_activePetCount[owner]}/{owner.maxStack})");
            }
        }
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

        _activePetCount.Clear();
        _petOwnerAugment.Clear();

        Debug.Log("증강 시스템 초기화 완료");
    }
}
