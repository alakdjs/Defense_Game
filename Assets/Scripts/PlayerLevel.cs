using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [SerializeField] private int _level = 1;
    [SerializeField] private int _maxLevel = 100;

    [SerializeField] private float _currentExp = 0.0f;
    [SerializeField] private float _maxExp = 10.0f;
    [SerializeField] private float _expIncrease = 2.0f;

    [SerializeField] private float _hpIncreasePerLevel = 10.0f;
    [SerializeField] private float _attackIncreasePerLevel = 0.1f;
    [SerializeField] private float _defenseIncreasePerLevel = 0.1f;

    [SerializeField] private PlayerExpUI _expUI;

    private PlayerController _player;
    private PlayerHp _playerHp;

    public int Level => _level;
    public float CurrentExp => _currentExp;
    public float MaxExp => _maxExp;

    private void Awake()
    {
        _player = GetComponent<PlayerController>();
        _playerHp = GetComponent<PlayerHp>();
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddExp(float amount)
    {
        if (_level >= _maxLevel)
            return;

        _currentExp += amount;

        // 경험치가 많이 들어와서 여러 번 레벨업 가능한 경우
        while (_level < _maxLevel && _currentExp >= _maxExp)
        {
            // 남은 경험치 유지 (초과분은 다음 레벨 경험치로 넘김)
            _currentExp -= _maxExp;

            bool openedPopup = LevelUp();

            // 증강 UI가 열렸으면(=Time.timeScale 0), 추가 레벨업 처리는 다음 프레임으로 넘김
            if (openedPopup)
                break;
        }

        UpdateUI();

    }

    private bool LevelUp()
    {
        _level++;

        // 경험치 관련
        _maxExp += _expIncrease;

        // 스탯 증가
        _player.AddMaxHp(_hpIncreasePerLevel);
        _player.AddAttack(_attackIncreasePerLevel);
        _player.AddDefense(_defenseIncreasePerLevel);

        // 체력 회복
        _playerHp.Heal(_hpIncreasePerLevel);

        // 특정 레벨에서 증강 카드 UI 등장 + 게임 일시정지(Time.timeScale = 0f)는 팝업 내부에서 처리
        if (AugmentPopupController.Instance != null && AugmentPopupController.Instance.ShouldOpenPopupAtLevel(_level))
        {
            AugmentPopupController.Instance.OpenPopup(_level);
            return true;
        }

        return false;
    }

    private void UpdateUI()
    {
        if (_expUI == null)
            return;

        _expUI.SetExp(_currentExp, _maxExp, _level);
    }
}
