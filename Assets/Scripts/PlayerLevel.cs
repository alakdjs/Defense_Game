using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [SerializeField] private int _level = 1;
    [SerializeField] private int _maxLevel = 100;

    [SerializeField] private float _currentExp = 0.0f;
    [SerializeField] private float _maxExp = 10.0f;
    [SerializeField] private float _expIncrease = 2.0f;

    [SerializeField] private PlayerExpUI _expUI;

    public int Level => _level;
    public float CurrentExp => _currentExp;
    public float MaxExp => _maxExp;

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
        _currentExp = 0.0f;
        _maxExp += _expIncrease;
        Debug.Log($"현재레벨: {_level}");
    }

    private void UpdateUI()
    {
        _expUI.SetExp(_currentExp, _maxExp, _level);
    }
}
