using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//a class to store all the player information about each enemy
[System.Serializable]
public class SavedEnemyStats
{
    public string key;
    public bool encountered;

    public int defeatedCount;
    public int deathCount;
    public int defeatedRecord;
    public int shiftRecord;

    public SavedEnemyStats(string key)
    {
        encountered = false;

        defeatedCount = 0;
        deathCount = 0;
        defeatedRecord = 0;
        shiftRecord = 0;

        this.key = key;
    }
}

//a class to store all the base stats for an enemyType
public abstract class EnemyStats : MonoBehaviour
{
    //eventually store the enemyPrefab in here so I can procedurally generate enemyPools, fine for now
    [SerializeField] public GameObject enemyPrefab;
    [SerializeField] public float maxInterval, minInterval, maxSpeed, minSpeed;
    [SerializeField] public int maxHealth, healthInterval, startingEnemies, enemyScaleAmt, killScore, scoreIncrease, countScaleTime, statsScaleTime;
    [SerializeField] public string type;
    [SerializeField] public Sprite sprite;
    public bool encountered = false;

    //Almanac stuff
    public string title, subtext, description, behavior, weakness;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual string GetTitle()
    {
        return title;
    }

    public virtual string GetSubtext()
    {
        return subtext;
    }

    public virtual string GetDescription()
    {
        return description;
    }

    public virtual string GetBehavior()
    {
        return behavior;
    }

    public virtual string GetWeakness()
    {
        return weakness;
    }
}
