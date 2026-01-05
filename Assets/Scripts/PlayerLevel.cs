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

        if (_currentExp >= _maxExp)
        {
            LevelUp();
        }

        UpdateUI();

    }

    private void LevelUp()
    {
        _level++;

        // 경험치 관련
        _currentExp = 0.0f;
        _maxExp += _expIncrease;

        // 스탯 증가
        _player.AddMaxHp(_hpIncreasePerLevel);
        _player.AddAttack(_attackIncreasePerLevel);
        _player.AddDefense(_defenseIncreasePerLevel);

        // 체력 회복
        _playerHp.Heal(_hpIncreasePerLevel);
    }

    private void UpdateUI()
    {
        if (_expUI == null)
            return;

        _expUI.SetExp(_currentExp, _maxExp, _level);
    }
}
