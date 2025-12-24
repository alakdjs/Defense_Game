using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// HpBar 풀링 매니저
/// - Screen Space Overlay Canvas 기준
/// - 몬스터 수가 많을 때 성능 최적화
/// </summary>
public class HpBarManager : MonoBehaviour
{
    [SerializeField] private HpBar _hpBarPrefab;
    [SerializeField] private int _initialPoolSize = 50;

    private Queue<HpBar> _pool = new Queue<HpBar>();
    private Canvas _canvas;

    public static HpBarManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
        {
            Debug.LogError("캔버스를 찾을 수 없음 HpBarManager_Log");
            return;
        }

        // 초기 풀 생성
        for (int i = 0; i < _initialPoolSize; i++)
        {
            CreateNewHpBar();
        }
    }

    private HpBar CreateNewHpBar()
    {
        HpBar bar = Instantiate(_hpBarPrefab, _canvas.transform);
        bar.gameObject.SetActive(false);
        _pool.Enqueue(bar);
        return bar;
    }

    // HpBar 하나 가져오기
    public HpBar GetHpBar(Transform target, float maxHp, Vector3 offset)
    {
        if (_pool.Count == 0)
        {
            CreateNewHpBar();
        }

        HpBar bar = _pool.Dequeue();
        bar.gameObject.SetActive(true);

        bar.Init(target, maxHp);
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
