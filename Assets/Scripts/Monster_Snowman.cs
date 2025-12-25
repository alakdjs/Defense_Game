using UnityEngine;
using UnityEngine.AI;


public class Monster_Snowman : MonsterBase
{
    [SerializeField] private float _detectRange = 8.0f;
    [SerializeField] private float _attackCooldown = 2.0f;

    [SerializeField] private Transform _mouthPoint;
    [SerializeField] private GameObject _snowballPrefab;
    [SerializeField] private float _snowballSpeed = 10.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
