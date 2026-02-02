using UnityEngine;
using UnityEngine.AI;

public class GiftBoxSpawner : MonoBehaviour
{
    [SerializeField] private WaveManager _waveManager;
    [SerializeField] private GameObject _giftBoxPrefab;

    [Header("Spawn Area (BoxCollider)")]
    [Tooltip("선물상자 스폰 범위로 사용할 BoxCollider")]
    [SerializeField] private BoxCollider _spawnArea;

    [Header("Spawn Settings")]
    [Tooltip("웨이브 1번 지날 때마다 스폰할 개수")]
    [SerializeField] private int _spawnCountPerWave = 2;

    [Tooltip("NavMesh 샘플 반경")]
    [SerializeField] private float _navMeshSampleRadius = 2.0f;

    [Tooltip("스폰 시도 횟수(실패 대비)")]
    [SerializeField] private int _maxAttemptsPerBox = 20;

    [Tooltip("이미 있는 선물상자와 겹침 방지 반경")]
    [SerializeField] private float _avoidOverlapRadius = 0.6f;

    [Tooltip("겹침 체크에 사용할 레이어")]
    [SerializeField] private LayerMask _giftBoxLayer;

    private void OnEnable()
    {
        if (_waveManager != null)
            _waveManager.OnWaveCompleted += HandleWaveCompleted;
    }

    private void OnDisable()
    {
        if (_waveManager != null)
            _waveManager.OnWaveCompleted -= HandleWaveCompleted;
    }

    private void HandleWaveCompleted(int completedWaveIndex)
    {
        // completedWaveIndex는 0-based
        if (_giftBoxPrefab == null)
        {
            Debug.LogWarning("[GiftBoxSpawner] GiftBox Prefab이 비어있습니다.");
            return;
        }

        if (_spawnArea == null)
        {
            Debug.LogWarning("[GiftBoxSpawner] SpawnArea(BoxCollider)가 비어있습니다.");
            return;
        }

        for (int i = 0; i < _spawnCountPerWave; i++)
        {
            TrySpawnOne();
        }
    }

    private void TrySpawnOne()
    {
        Bounds b = _spawnArea.bounds;

        for (int attempt = 0; attempt < _maxAttemptsPerBox; attempt++)
        {
            // BoxCollider bounds 내부 랜덤 점 (y는 bounds 중심)
            float x = Random.Range(b.min.x, b.max.x);
            float z = Random.Range(b.min.z, b.max.z);
            Vector3 rawPos = new Vector3(x, b.center.y, z);

            // NavMesh 위로 스냅
            if (NavMesh.SamplePosition(rawPos, out NavMeshHit hit, _navMeshSampleRadius, NavMesh.AllAreas) == false)
                continue;

            Vector3 spawnPos = hit.position;

            // 겹침 방지(선물상자 레이어로 체크)
            if (_giftBoxLayer.value != 0)
            {
                bool overlapped = Physics.CheckSphere(spawnPos, _avoidOverlapRadius, _giftBoxLayer);
                if (overlapped)
                    continue;
            }

            Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            Instantiate(_giftBoxPrefab, spawnPos, rot);
            return;
        }

        Debug.LogWarning("[GiftBoxSpawner] 선물상자 스폰 실패(시도 횟수 초과)");
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_spawnArea == null)
            return;

        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawCube(_spawnArea.bounds.center, _spawnArea.bounds.size);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_spawnArea.bounds.center, _spawnArea.bounds.size);
    }
#endif
}
