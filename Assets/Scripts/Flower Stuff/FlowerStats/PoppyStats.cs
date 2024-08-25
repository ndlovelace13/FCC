using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoppyStats : FlowerStats
{
    public float poppyDebuff = 0.9f;

    // Start is called before the first frame update
    void Start()
    {
        description = "Despite its undeniable beauty, the poppy contains a powerful narcotic that is very effective in slowing the skinwalker threat. Unrelated to its potency in the field, its seeds are delicious - just don't eat too many before a drug test";
        effects = "Sedation - Afflicted enemies are permanently slowed - this effect can only be removed by a buffing enemy";
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


    public override List<SpecialStats> GetSpecialValues(int power)
    {
        List<SpecialStats> returnedStats = new List<SpecialStats>();

        //slowing effect
        SpecialStats poisonDamages = new SpecialStats("Sedation Effect", poppyDebuff, "Base Speed");
        returnedStats.Add(poisonDamages);

        return returnedStats;
    }
}
