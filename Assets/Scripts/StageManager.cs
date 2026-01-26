using System;
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

    // 첫 세팅(초기 스테이지)에서도 이벤트를 1회 발행하기 위한 플래그
    private bool _hasInitializedStage = false;

    public StageType CurrentStage => _currentStage;

    public StageType GetStageByWaveIndex(int waveIndex)
    {
        // waveIndex 0부터
        int stageIndex = (waveIndex / _wavesPerStage) + 1;
        stageIndex = Mathf.Clamp(stageIndex, 1, 5);

        return (StageType)stageIndex;
    }

    /// <summary>
    /// 웨이브 인덱스로 스테이지를 갱신하고, 변경 시 OnStageChanged 발행
    /// - 첫 호출(초기화) 때도 이벤트를 1회 발행하도록 처리
    /// - forceInvoke=true면 같은 스테이지여도 이벤트 강제 발행
    /// </summary>
    public void UpdateStageByWaveIndex(int waveIndex, bool forceInvoke = false)
    {
        StageType next = GetStageByWaveIndex(waveIndex);

        // 첫 초기화: Day1이라도 반드시 1회는 이벤트 발행
        if (_hasInitializedStage == false)
        {
            _hasInitializedStage = true;
            _currentStage = next;

            Debug.Log($"[StageManager] Stage Initialized: {_currentStage}");
            OnStageChanged?.Invoke(_currentStage);
            return;
        }

        // 동일 스테이지이고 강제 발행이 아니면 패스
        if (next == _currentStage && forceInvoke == false)
            return;

        _currentStage = next;
        Debug.Log($"[StageManager] Stage Changed: {_currentStage}");

        OnStageChanged?.Invoke(_currentStage);
    }

    /// <summary>
    /// 현재 스테이지를 무조건 1회 이벤트로 브로드캐스트
    /// - StageLightingController가 Start()에서 즉시 적용하기 위해 사용
    /// </summary>
    public void ForceBroadcastCurrentStage()
    {
        // 아직 초기화가 안 됐으면 Day1(현재값)을 초기화로 처리
        if (_hasInitializedStage == false)
        {
            _hasInitializedStage = true;
            Debug.Log($"[StageManager] Stage Force Initialized: {_currentStage}");
        }
        else
        {
            Debug.Log($"[StageManager] Stage Force Broadcast: {_currentStage}");
        }

        OnStageChanged?.Invoke(_currentStage);
    }

}
