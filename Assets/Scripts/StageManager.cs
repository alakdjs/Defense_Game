using System;
using UnityEditor;
using UnityEngine;

public enum StageType
{
    Day1 = 1,
    Sunset = 2,
    Night = 3,
    Dawn = 4,
    SnowDay = 5
}

public class StageManager : MonoBehaviour
{
    public event Action<StageType> OnStageChanged;

    [SerializeField] private int _wavesPerStage = 10; // 스테이지당 웨이브 수
    private StageType _currentStage = StageType.Day1;

    public StageType GetStageByWaveIndex(int waveIndex)
    {
        // waveIndex 0부터
        int stageIndex = (waveIndex / _wavesPerStage) + 1;
        stageIndex = Mathf.Clamp(stageIndex, 1, 5);

        return (StageType)stageIndex;
    }

    public void UpdateStageByWaveIndex(int waveIndex)
    {
        StageType next = GetStageByWaveIndex(waveIndex);

        if (next == _currentStage)
            return;

        _currentStage = next;
        Debug.Log($"[StageManager] Stage Changed: {_currentStage}");

        OnStageChanged?.Invoke(_currentStage);
    }
}
