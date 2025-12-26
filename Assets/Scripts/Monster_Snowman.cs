using UnityEngine;
using UnityEngine.AI;


public class Monster_Snowman : MonsterBase
{
    [SerializeField] private float _detectRange = 8.0f;
    [SerializeField] private float _attackCooldown = 2.0f;

    [SerializeField] private Transform _mouthPoint;
    [SerializeField] private GameObject _snowballPrefab;
    [SerializeField] private float _snowballSpeed = 10.0f;


    
}
