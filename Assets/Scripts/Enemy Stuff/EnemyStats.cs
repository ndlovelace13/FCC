using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//a class to store all the base stats for an enemyType
public abstract class EnemyStats : MonoBehaviour
{
    //eventually store the enemyPrefab in here so I can procedurally generate enemyPools, fine for now
    [SerializeField] public GameObject enemyPrefab;
    [SerializeField] public float maxInterval, minInterval, maxSpeed, minSpeed;
    [SerializeField] public int maxHealth, healthInterval, startingEnemies, enemyScaleAmt, killScore, scoreIncrease, countScaleTime, statsScaleTime;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
