using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoppyStats : FlowerStats
{
    public float poppyDebuff = 0.9f;

    // Start is called before the first frame update
    void Start()
    {
        description = "placeholder description";
        effects = "Placeholder effects";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //permanently decrease the enemy's speed
    public override void OnEnemyCollision(GameObject enemy, int tier)
    {
        EnemyBehavior enemyBe = enemy.GetComponent<EnemyBehavior>();


        enemyBe.poppyCount++;
        enemyBe.SpeedDown(poppyDebuff);
        Debug.Log("Enemy speed down");
    }
}
