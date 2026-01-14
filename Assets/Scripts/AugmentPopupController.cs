using System.Collections.Generic;
using UnityEngine;

public class AugmentPopupController : MonoBehaviour
{
    public static AugmentPopupController Instance;

    [SerializeField] private GameObject _popupRoot; // 팝업 전체 Root 오브젝트
    [SerializeField] private Transform _cardParent; // 카드가 생성될 부모 Transform
    [SerializeField] private AugmentCardUI _cardPrefab; // 증강 카드 프리팹

    // 현재 생성된 카드 목록
    private readonly List<AugmentCardUI> _spawnedCards = new List<AugmentCardUI>();

    private void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _popupRoot.SetActive(false);
    }

    /// <summary>
    /// 레벨에 따라 증강 팝업 열기
    /// </summary>
    public void OpenPopup(int playerLevel)
    {
        ClearCards();

        List<AugmentData> selectedAugments = new List<AugmentData>();

        // 레벨별 증강 선택 로직
        if (playerLevel == 2)
        {
            // 2렙: 무기 선택 1개 + 능력치 증강 2개
            selectedAugments = GetLevel2Augments();
        }
        else
        {
            // 4, 7, 10, 15, 20... : 랜덤 3개
            selectedAugments = GetRandomAugments(3);
        }

        // 카드 생성 (현재 레벨 정보 전달)
        foreach (var augment in selectedAugments)
        {
            AugmentCardUI card = Instantiate(_cardPrefab, _cardParent);
            int currentLevel = AugmentManager.Instance.GetAugmentLevel(augment);
            card.Init(augment, currentLevel);
            _spawnedCards.Add(card);
        }

        _popupRoot.SetActive(true);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 2렙 전용: 무기 선택 1개 + 능력치 증강 2개
    /// </summary>
    private List<AugmentData> GetLevel2Augments()
    {
        List<AugmentData> result = new List<AugmentData>();

        // 1. 무기 선택 1개
        List<AugmentData> weaponAugments = AugmentManager.Instance.GetAvailableAugmentsByCategory(AugmentCategory.WeaponSelect);
        if (weaponAugments.Count > 0)
        {
            int randomIndex = Random.Range(0, weaponAugments.Count);
            result.Add(weaponAugments[randomIndex]);
        }

        // 2. 능력치 증강 2개
        List<AugmentData> statAugments = AugmentManager.Instance.GetAvailableAugmentsByCategory(AugmentCategory.StatUpgrade);
        List<AugmentData> selected = PickRandomAugments(statAugments, 2);
        result.AddRange(selected);

        return result;
    }

    /// <summary>
    /// 일반 레벨: 랜덤 3개
    /// </summary>
    private List<AugmentData> GetRandomAugments(int count)
    {
        List<AugmentData> candidates = AugmentManager.Instance.GetAvailableAugments();
        return PickRandomAugments(candidates, count);
    }

    public void ClosePopup()
    {
        _popupRoot.SetActive(false);
        ClearCards();
        Time.timeScale = 1.0f;
    }

    public void ClearCards()
    {
        for (int i = 0; i < _spawnedCards.Count; i++)
        {
            if (_spawnedCards[i] != null)
            {
                Destroy(_spawnedCards[i].gameObject);
            }
        }

        _spawnedCards.Clear();
    }

    private List<AugmentData> PickRandomAugments(List<AugmentData> sources, int count)
    {
        List<AugmentData> result = new List<AugmentData>();

        if (sources == null || sources.Count == 0)
            return result;

        // 임시 리스트 생성 (Fisher-Yates 셔플)
        List<AugmentData> temp = new List<AugmentData>(sources);

        for (int i = temp.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (temp[i], temp[rand]) = (temp[rand], temp[i]);
        }

        // 원하는 개수만큼 선택
        int pickCount = Mathf.Min(count, temp.Count);
        for (int i = 0; i < pickCount; i++)
        {
            result.Add(temp[i]);
        }

        return result;
    }
}