using System;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private MonsterSpawner _spawner;
    [SerializeField] private PlayerLevel _playerLevel;
    [SerializeField] private MonsterBase _bossInstance = null;

    [Header("Wave List")]
    [SerializeField] private WaveData[] _waves;

    [Header("Difficulty Tuning")]
    [Tooltip("레벨이 오르면 감소/MaxAlive 증가")]
    [SerializeField] private float _spawnIntervalDecreasePerLevel = 0.01f;

    [Tooltip("레벨이 오르면 MaxAlive가 증가하는 양")]
    [SerializeField] private int _maxAliveIncreasePerLevel = 2;

    [Tooltip("스폰 간격 최소치")]
    [SerializeField] private float _minSpawnInterval = 0.15f;

    [Header("Runtime Debug")]
    [SerializeField] private int _currentWaveIndex = 0;
    [SerializeField] private float _waveTime = 0.0f;
    [SerializeField] private int _aliveCount = 0;
    [SerializeField] private bool _bossSpawned = false;

    private float _spawnTimer = 0.0f;
    private bool _isRunning = false;

    // ================================== UI ==================================
    public string CurrentWaveName
    {
        get
        {
            if (_waves == null || _waves.Length == 0)
                return string.Empty;

            WaveData wave = _waves[_currentWaveIndex];
            return (wave != null) ? wave.waveName : string.Empty;
        }
    }

    public float CurrentWaveDuration
    {
        get
        {
            if (_waves == null || _waves.Length == 0)
                return 0.0f;

            WaveData wave = _waves[_currentWaveIndex];
            return (wave != null) ? Mathf.Max(0.0f, wave.waveDuration) : 0.0f;
        }
    }

    public float CurrentWaveTime => _waveTime;

    public float CurrentWaveProgress01
    {
        get
        {
            float d = CurrentWaveDuration;
            if (d <= 0.0001f)
                return 0.0f;

            return Mathf.Clamp01(_waveTime / d);
        }
    }

    public float CurrentWaveRemainingTime
    {
        get
        {
            float d = CurrentWaveDuration;
            return Mathf.Max(0.0f, d - _waveTime);
        }
    }

    public event Action OnWaveStarted;

    // ====================================================================

    private void OnEnable()
    {
        MonsterBase.OnAnyMonsterDied += HandleAnyMonsterDied;
    }

    private void OnDisable()
    {
        MonsterBase.OnAnyMonsterDied -= HandleAnyMonsterDied;
    }

    private void Start()
    {
        if (_spawner == null)
        {
            Debug.LogError("[WaveManager] MonsterSpawner가 연결되지 않았습니다.");
            enabled = false;
            return;
        }

        if (_playerLevel == null)
        {
            Debug.LogError("[WaveManager] PlayerLevel이 연결되지 않았습니다.");
            enabled = false;
            return;
        }

        if (_waves == null || _waves.Length == 0)
        {
            Debug.LogWarning("[WaveManager] WaveData 비어있음");
            return;
        }

        StartWave(0);
    }

    private void Update()
    {
        if (_isRunning == false)
            return;

        WaveData wave = _waves[_currentWaveIndex];
        if (wave == null)
            return;

        _waveTime += Time.deltaTime;

        // 일반 웨이브 시간 진행 중 스폰
        if (wave.isBossWave == false)
        {
            TickSpawn(wave);

            // 시간 종료 -> 다음 웨이브
            if (_waveTime >= wave.waveDuration)
            {
                GoNextWave();
            }
        }
        else
        {
            // 보스 웨이브: waveDuration 동안은 잡몹 스폰하다가,
            // 시간이 끝나면 보스를 소환하고 전멸까지 대기
            if (_bossSpawned == false)
            {
                TickSpawn(wave);

                if (_waveTime >= wave.waveDuration)
                {
                    SpawnBoss(wave);
                }
            }
            else
            {
                // 보스 소환 이후: 전멸 체크
                if (IsBossWaveCleared(wave))
                {
                    GoNextWave();
                }
            }
        }
    }

    private void StartWave(int index)
    {
        _currentWaveIndex = Mathf.Clamp(index, 0, _waves.Length - 1);
        _waveTime = 0.0f;
        _spawnTimer = 0.0f;
        _bossSpawned = false;
        _isRunning = true;

        WaveData wave = _waves[_currentWaveIndex];
        Debug.Log($"[WaveManager] Wave Start: {wave.waveName} (Index: {_currentWaveIndex})");

        // UI 갱신
        OnWaveStarted?.Invoke();
    }

    private void GoNextWave()
    {
        _isRunning = false;

        int next = _currentWaveIndex + 1;
        if (next >= _waves.Length)
        {
            Debug.Log("[WaveManager] All waves cleared!");
            return;
        }

        StartWave(next);
    }

    private void TickSpawn(WaveData wave)
    {
        int level = (_playerLevel != null) ? _playerLevel.Level : 1;

        // 난이도 반영: 레벨이 오를수록 스폰 간격 감소
        float spawnInterval = wave.baseSpawnInterval - (level - 1) * _spawnIntervalDecreasePerLevel;
        spawnInterval = Mathf.Max(_minSpawnInterval, spawnInterval);

        // 난이도 반영: 레벨이 오를수록 MaxAlive 증가
        int maxAlive = wave.baseMaxAlive + (level - 1) * _maxAliveIncreasePerLevel;
        maxAlive = Mathf.Max(1, maxAlive);

        // 동시 활성 제한
        if (_aliveCount >= maxAlive)
            return;

        _spawnTimer += Time.deltaTime;
        if (_spawnTimer < spawnInterval)
            return;

        _spawnTimer = 0.0f;

        // 현재 시간에 해당하는 세그먼트에서 몬스터 선택
        GameObject prefab = PickPrefabForTime(wave, _waveTime);
        if (prefab == null)
            return;

        MonsterBase spawned = _spawner.SpawnMonster(prefab);
        if (spawned != null)
        {
            _aliveCount++;
        }
    }

    private void SpawnBoss(WaveData wave)
    {
        _bossSpawned = true;

        if (wave.bossPrefab == null)
        {
            Debug.LogWarning("[WaveManager] BossWave인데 bossPrefab이 null -> 다음 웨이브로 넘어감");
            GoNextWave();
            return;
        }

        // 보스 소환
        MonsterBase boss = _spawner.SpawnMonster(wave.bossPrefab);
        if (boss != null)
        {
            _bossInstance = boss;
            _aliveCount++;
        }

    }

    private bool IsBossWaveCleared(WaveData wave)
    {
        // includeAddsInBossClear == true : 보스 포함 전체 전멸(_aliveCount==0)일 때 클리어
        // includeAddsInBossClear == false: 전체 전멸
        if (wave.includeAddsInBossClear)
        {
            // 보스 + 잡몹 전멸
            return _aliveCount <= 0;
        }

        return _bossInstance == null;
    }

    private GameObject PickPrefabForTime(WaveData wave, float time)
    {
        if (wave.segments == null || wave.segments.Count == 0)
            return null;

        WaveData.WaveSegment seg = null;

        // 현재 시간에 해당하는 세그먼트 찾기
        for (int i = 0; i < wave.segments.Count; i++)
        {
            var s = wave.segments[i];
            if (time >= s.startTime && time < s.endTime)
            {
                seg = s;
                break;
            }
        }

        if (seg == null || seg.pool == null || seg.pool.Count == 0)
            return null;

        // 가중치 랜덤
        float total = 0.0f;
        for (int i = 0; i < seg.pool.Count; i++)
        {
            total += Mathf.Max(0.0f, seg.pool[i].weight);
        }

        if (total <= 0.0f)
            return null;

        float r = UnityEngine.Random.Range(0.0f, total);
        float acc = 0.0f;

        for (int i = 0; i < seg.pool.Count; i++)
        {
            acc += Mathf.Max(0.0f, seg.pool[i].weight);
            if (r <= acc)
                return seg.pool[i].prefab;
        }

        return seg.pool[seg.pool.Count - 1].prefab;
    }

    private void HandleAnyMonsterDied(MonsterBase monster)
    {
        // AliveCount 감소
        _aliveCount = Mathf.Max(0, _aliveCount - 1);
    }
}
