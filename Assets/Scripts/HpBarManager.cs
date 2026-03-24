using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// HpBar 풀링 매니저
/// - Screen Space Overlay Canvas 기준
/// - 몬스터 수가 많을 때 성능 최적화
/// Managers에 없고, Canvas의 HpBarBanager에 스크립트 붙임
/// </summary>
public class HpBarManager : MonoBehaviour
{
    [SerializeField] private HpBar _hpBarPrefab;
    [SerializeField] private int _initialPoolSize = 60;
    [SerializeField] private Transform _hpBarRoot;

    private Queue<HpBar> _pool = new Queue<HpBar>();

    public static HpBarManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (_hpBarRoot == null)
        {
            Debug.LogError("HpBarRoot가 연결되지 않았습니다. HpBarManager_Log");
            return;
        }

        // 초기 풀 생성
        for (int i = 0; i < _initialPoolSize; i++)
        {
            CreateNewHpBar();
        }
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    private void HandleGameStateChanged(GameState oldState, GameState newState)
    {
        if (_hpBarRoot == null)
            return;

        bool hideHpBar =
            newState == GameState.Paused ||
            newState == GameState.AugmentSelect ||
            newState == GameState.Result;

        _hpBarRoot.gameObject.SetActive(!hideHpBar);
    }

    private HpBar CreateNewHpBar()
    {
        HpBar bar = Instantiate(_hpBarPrefab, _hpBarRoot);
        bar.gameObject.SetActive(false);
        _pool.Enqueue(bar);
        return bar;
    }

    // HpBar 하나 가져오기
    public HpBar GetHpBar(Transform target, float maxHp, Vector3 offset, bool monsterHp, bool useAutoHide)
    {
        if (_pool.Count == 0)
        {
            CreateNewHpBar();
        }

        HpBar bar = _pool.Dequeue();
        bar.gameObject.SetActive(false);

        bar.Init(target, maxHp, monsterHp, useAutoHide);
        bar.SetWorldOffset(offset);

        return bar;
    }

    // HpBar 반환
    public void ReturnHpbar(HpBar bar)
    {
        if (bar == null)
            return;

        bar.gameObject.SetActive(false);
        _pool.Enqueue(bar);
    }
}
