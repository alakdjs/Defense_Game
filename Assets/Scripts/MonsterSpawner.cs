using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _monsterPrefabs;
    [SerializeField] private float _spawnCooltime = 5.0f; // 스폰 쿨타임
    [SerializeField] private int _maxSpawnCount = 500; // 최대 스폰 개수
    
    [SerializeField] private Transform _monsterRoot;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _tower;
    [SerializeField] private float _minSpawnDistance = 20.0f; // 최소 스폰 범위
    [SerializeField] private float _spawnRadius = 40.0f; // 스폰 범위
    [SerializeField] private int _maxTryCount = 20; // 무한루프 방지

    private float _spawnTimer = 0.0f;
    private int _currentSpawnCount = 0;

    private void Update()
    {
        if (_currentSpawnCount >= _maxSpawnCount)
            return;

        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= _spawnCooltime)
        {
            _spawnTimer = 0.0f;
            SpawnMonster();
        }
    }

    private void SpawnMonster()
    {
        if (_monsterPrefabs == null || _monsterPrefabs.Length == 0) 
            return;

        if (_player == null)
            return;

        Vector3 spawnPos;
        bool found = TryGetValidSpawnPosition(out spawnPos);

        if (!found)
            return;

        int index = Random.Range(0, _monsterPrefabs.Length);
        GameObject prefab = _monsterPrefabs[index];

        if (prefab == null)
            return;

        GameObject monsterObj = Instantiate(prefab, spawnPos, Quaternion.identity, _monsterRoot);
        _currentSpawnCount++;
    }

    private bool TryGetValidSpawnPosition(out Vector3 result)
    {
        for (int i = 0; i < _maxTryCount; i++)
        {
            // 타워 기준 원형 랜덤 위치
            Vector2 randomCircle = Random.insideUnitCircle.normalized *
                                   Random.Range(_minSpawnDistance, _spawnRadius);

            Vector3 randomPos = new Vector3(
                _tower.position.x + randomCircle.x,
                0f,
                _tower.position.z + randomCircle.y
            );

            // NavMesh 위인지 검사
            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                // y == 0 인 NavMesh만 허용
                if (Mathf.Abs(hit.position.y) > 0.01f)
                    continue;

                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }
}
