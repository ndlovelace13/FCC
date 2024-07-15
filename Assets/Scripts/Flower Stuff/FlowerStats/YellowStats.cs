using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowStats : FlowerStats
{
    int numTargets = 2;
    float stunAmount = 0.2f;
    float stunTime = 1f;
    int electricDamage = 5;

    //power scaling
    int targetIncrease = 1;
    int damageIncrease = 3;

    GameObject lightningPool;
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
        //ElectricApply the numTargets based on power level t
        if (lightningPool == null)
            lightningPool = GameObject.FindGameObjectWithTag("lightningPool");
        if (!enemy.GetComponent<EnemyBehavior>().electricPassed)
        {
            Dictionary<string, int> actualAugs = new Dictionary<string, int>(enemy.GetComponent<EnemyBehavior>().actualAugs);
            int thisTargets = numTargets + targetIncrease * (t - 1);
            actualAugs.Remove("yellow");
            StartCoroutine(ElectricApply(thisTargets, actualAugs, enemy, t));
        }
    }

    IEnumerator ElectricApply(int remainingTargets, Dictionary<string, int> actualAugs, GameObject enemy, int tier)
    {
        //TODO - remove electricity from the augments at some point before passing
        Debug.Log("This mf electrified");
        enemy.GetComponent<EnemyBehavior>().isElectrified = true;
        enemy.GetComponent<SpriteRenderer>().color = Color.yellow;
        StartCoroutine(SlowApply(stunAmount, stunTime, 4, enemy));
        if (remainingTargets > 0)
        {
            EnemyBehavior[] enemyBehaviors = UnityEngine.Object.FindObjectsOfType<EnemyBehavior>();
            GameObject[] enemies = new GameObject[enemyBehaviors.Length];
            for ( int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = enemyBehaviors[i].gameObject;
            }
            GameObject closestEnemy = enemy.GetComponent<EnemyBehavior>().ClosestEnemy(enemies);
            if (closestEnemy != null)
            {
                StartCoroutine(LightningEffect(enemy, closestEnemy));
                ElectricPass(remainingTargets - 1, actualAugs, closestEnemy, tier);
            }
            int thisDamage = electricDamage + damageIncrease * (tier - 1);
            enemy.GetComponent<EnemyBehavior>().DealDamage(thisDamage, Color.yellow);
        }
        //isElectrified = false;
        yield return null;
    }

    IEnumerator LightningEffect(GameObject origin, GameObject target)
    {
        Vector2 length = target.transform.position - origin.transform.position;
        float angleRadians = Mathf.Atan2(length.y, -length.x);
        if (angleRadians < 0)
            angleRadians += 2 * Mathf.PI;
        //angleRadians += Mathf.PI / 2;
        Vector2 direction = new Vector2(Mathf.Sin(angleRadians), Mathf.Cos(angleRadians));
        GameObject newLightning = lightningPool.GetComponent<ObjectPool>().GetPooledObject();
        newLightning.transform.position = Vector3.Lerp(origin.transform.position, target.transform.position, 0.5f);
        newLightning.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        //GameObject newLightning = Instantiate(lightning, Vector3.Lerp(origin.transform.position, target.transform.position, 0.5f), Quaternion.LookRotation(Vector3.forward, direction));
        newLightning.transform.localScale = new Vector2(length.magnitude, newLightning.transform.localScale.y);
        newLightning.GetComponent<LightningBehavior>().Activate();
        yield return null;
        //newLightning.SetActive(false);
    }

    public void ElectricPass(int remainingTargets, Dictionary<string, int> actualAugs, GameObject nextEnemy, int tier)
    {
        nextEnemy.GetComponent<EnemyBehavior>().electricPassed = true;
        StartCoroutine(ElectricApply(remainingTargets, actualAugs, nextEnemy, tier));
        nextEnemy.GetComponent<EnemyBehavior>().AugmentApplication(actualAugs);
        nextEnemy.GetComponent<EnemyBehavior>().electricPassed = false;
    }
}
