using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class GreenStats : FlowerStats
{
    //aoe stats
    [SerializeField] GameObject poisonPool;
    float cloudCooldown = 0.5f;
    float cloudTime = 1f;

    //enemy stats
    int poisonDamage = 1;
    float poisonCooldown = 0.3f;
    float poisonTime = 4f;
    float poisonSlow = 0.7f;

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
        enemy.GetComponent<EnemyBehavior>().isPoisoned = true;
        enemy.GetComponent<SpriteRenderer>().color = Color.green;
        StartCoroutine(PoisonHandler(enemy));
        StartCoroutine(SlowApply(poisonSlow, poisonTime, 3, enemy));
    }

    IEnumerator PoisonHandler(GameObject enemy)
    {
        float poisonTimer = 0f;
        while (poisonTimer < poisonTime)
        {
            yield return new WaitForSeconds(poisonCooldown);
            if (!enemy.activeSelf)
                break;
            enemy.GetComponent<EnemyBehavior>().DealDamage(poisonDamage, Color.green);
            poisonTimer += poisonCooldown;
        }
        //Debug.Log("Done");
        enemy.GetComponent<EnemyBehavior>().isPoisoned = false;
        enemy.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public override void OnProjTravel(GameObject proj)
    {
        poisonPool = GameObject.FindGameObjectWithTag("poisonPool");
        StartCoroutine(PoisonSpawn(proj));
    }

    IEnumerator PoisonSpawn(GameObject proj)
    {
        bool first = true;
        while (proj.activeSelf)
        {
            if (first)
            {
                yield return new WaitForSeconds(cloudCooldown);
                first = false;
            }
            GameObject newCloud = poisonPool.GetComponent<ObjectPool>().GetPooledObject();
            newCloud.SetActive(true);
            newCloud.transform.position = proj.transform.position;
            int tier = proj.GetComponent<ProjectileBehavior>().GetTier();
            string[] augs = proj.GetComponent<ProjectileBehavior>().getAugments();
            newCloud.GetComponent<AoeBehavior>().Activate(augs, cloudTime, "green", tier);
            yield return new WaitForSeconds(cloudCooldown);
        }
    }
}
