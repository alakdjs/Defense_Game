using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _monsterPrefabs;
    [SerializeField] private float _spawnCooltime = 2.0f;
    [SerializeField] private int _maxSpawnCount = 500;

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

        int index = Random.Range(0, _monsterPrefabs.Length);
        GameObject prefab = _monsterPrefabs[index];

        if (prefab == null)
            return;

        Vector3 spawnPos = transform.position;
        GameObject monsterObj = Instantiate(prefab, spawnPos, Quaternion.identity);
        _currentSpawnCount++;
    }
}
