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
    /// 해당 레벨에서 증강 팝업을 열어야 하는지(레벨업 시)
    /// </summary>
    public bool ShouldOpenPopupAtLevel(int playerLevel)
    {
        return true;
    }

    /// <summary>
    /// 레벨에 따라 증강 팝업 열기
    /// 호출하는 쪽(레벨업 시스템)에서 ShouldOpenPopupAtLevel을 확인하고 호출
    /// </summary>
    public void OpenPopup(int playerLevel)
    {
        // 지정 레벨이 아니면 열지 않음
        if (!ShouldOpenPopupAtLevel(playerLevel))
            return;

        ClearCards();

        // 모든 증강(무기 포함)을 전부 동일하게 랜덤 3개
        List<AugmentData> selectedAugments = GetRandomAugments(3);

        // 카드 생성 (현재 스택 정보 전달)
        for (int i = 0; i < selectedAugments.Count; i++)
        {
            AugmentData augment = selectedAugments[i];
            if (augment == null)
                continue;

            AugmentCardUI card = Instantiate(_cardPrefab, _cardParent);

            // 레벨 개념 제거: 현재 스택 전달
            int currentStack = AugmentManager.Instance.GetAugmentStack(augment);
            card.Init(augment, currentStack);

            _spawnedCards.Add(card);
        }

        _popupRoot.SetActive(true);
        Time.timeScale = 0f; // 일시정지
    }

    /// <summary>
    /// 일반 레벨: 랜덤 N개
    /// </summary>
    private List<AugmentData> GetRandomAugments(int count)
    {
        // 선행 조건 만족한 증강만 반환
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
