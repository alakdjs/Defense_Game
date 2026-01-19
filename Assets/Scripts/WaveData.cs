using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave/Wave Data")]
public class WaveData : ScriptableObject
{
    [Header("Wave Info")]
    [Tooltip("웨이브 이름")]
    public string waveName;

    [Tooltip("이 웨이브의 총 진행 시간(초)")]
    public float waveDuration = 60.0f;

    [Header("Spawn Setting (Base)")]
    [Tooltip("기본 스폰 간격(초)")]
    public float baseSpawnInterval = 5.0f;

    [Tooltip("기본 최대 동시 활성 몬스터 수")]
    public int baseMaxAlive = 100;

    [Header("Timed Segments")]
    public List<WaveSegment> segments = new List<WaveSegment>();

    [Header("Boss Setting")]
    [Tooltip("보스 웨이브 여부. true면 waveDuration 끝에 보스를 소환하고 전멸까지 대기")]
    public bool isBossWave = false;

    [Tooltip("보스 프리팹")]
    public GameObject bossPrefab;

    [Tooltip("보스 소환 시 추가로 남아있는 잡몹도 전멸 조건에 포함할지")]
    public bool includeAddsInBossClear = true;

    [Serializable]
    public class WeightedPrefab
    {
        public GameObject prefab;
        [Range(0.0f, 100.0f)] public float weight = 1.0f;
    }
    
    [Serializable]
    public class WaveSegment
    {
        [Tooltip("세그먼트 시작 시간(초)")]
        public float startTime;

        [Tooltip("세그먼트 종료 시간(초)")]
        public float endTime;

        [Tooltip("이 구간에서 사용될 몬스터 풀(가중치 랜덤)")]
        public List<WeightedPrefab> pool = new List<WeightedPrefab>();
    }
}
