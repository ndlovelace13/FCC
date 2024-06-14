using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueStats : FlowerStats
{

    float freezeTime = 3f;
    float slowTime = 2f;
    float slowAmount = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnEnemyCollision(GameObject enemy)
    {
        base.OnEnemyCollision(enemy);
        StartCoroutine(FreezeApply(enemy));
    }

    IEnumerator FreezeApply(GameObject enemy)
    {
        if (!enemy.GetComponent<EnemyBehavior>().isFrozen)
        {
            Debug.Log("This mf frozen");
            enemy.GetComponent<SpriteRenderer>().color = Color.blue;
            //backupSpeed = moveSpeed;
            enemy.GetComponent<EnemyBehavior>().isFrozen = true;
            enemy.GetComponent<EnemyBehavior>().moveSpeed = 0f;
            if (!enemy.GetComponent<EnemyBehavior>().isBurning)
            {
                yield return new WaitForSeconds(freezeTime);
            }
            //moveSpeed = backupSpeed;
            if (enemy.activeSelf)
                enemy.GetComponent<EnemyBehavior>().isFrozen = false;
        }
        if (enemy.activeSelf)
        {
            StartCoroutine(SlowApply(slowAmount, slowTime, 2, enemy));
            enemy.GetComponent<SpriteRenderer>().color = Color.cyan;
        }
    }
}
