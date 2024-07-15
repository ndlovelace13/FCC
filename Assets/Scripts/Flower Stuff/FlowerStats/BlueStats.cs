using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueStats : FlowerStats
{
    //freeze stuff
    float freezeTime = 1.5f;
    float slowTime = 2f;
    float slowAmount = 0.5f;

    //power scaling
    float timeIncrease = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnEnemyCollision(GameObject enemy, int t)
    {
        base.OnEnemyCollision(enemy, t);
        StartCoroutine(FreezeApply(enemy, t));
    }

    IEnumerator FreezeApply(GameObject enemy, int power)
    {
        float thisFreezeTime = freezeTime + timeIncrease * (power - 1);
        float thisSlowTime = slowTime + timeIncrease * (power - 1);
        if (!enemy.GetComponent<EnemyBehavior>().isFrozen)
        {
            enemy.GetComponent<Animator>().SetBool("Surprise", true);
            Debug.Log("This mf frozen");
            
            //backupSpeed = moveSpeed;
            
            if (!enemy.GetComponent<EnemyBehavior>().isBurning)
            {
                enemy.GetComponent<SpriteRenderer>().color = Color.blue;
                enemy.GetComponent<EnemyBehavior>().isFrozen = true;
                enemy.GetComponent<EnemyBehavior>().moveSpeed = 0f;
                float currentTime = 0f;
                while (currentTime < thisFreezeTime)
                {
                    //Debug.Log("still frozen " + thisFreezeTime);
                    if (!enemy.activeSelf)
                    {
                        enemy.GetComponent<EnemyBehavior>().isFrozen = false;
                        enemy.GetComponent<Animator>().SetBool("Surprise", false);
                        yield break;
                    }
                    currentTime += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                //yield return new WaitForSeconds(freezeTime);
            }
            //moveSpeed = backupSpeed;
            enemy.GetComponent<EnemyBehavior>().isFrozen = false;
            enemy.GetComponent<Animator>().SetBool("Surprise", false);

        }
        if (enemy.activeSelf)
        {
            StartCoroutine(SlowApply(slowAmount, thisSlowTime, 2, enemy));
            enemy.GetComponent<SpriteRenderer>().color = Color.cyan;
        }
    }
}
