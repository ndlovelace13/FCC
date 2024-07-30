using System.Collections;
using System.Collections.Generic;
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

    //power scaling
    float cooldownDecrease = 0.075f;
    float timeIncrease = 1f;
    float cloudTimeIncrease = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        description = "The Toxic Flower is universally known to be quite effective against skinwalkers, though it is referred to by different names in some regions - notably the \"League\" and \"Rainbow\" flowers. The origins of these alternative names is unclear.";
        effects = "Poison - Afflicted enemies will be slowed and take poison damage over time\nCloud Burst - Projectiles leave behind dissapating toxic clouds as they travel";
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
        StartCoroutine(PoisonHandler(enemy, t));
        
    }

    IEnumerator PoisonHandler(GameObject enemy, int power)
    {
        float thisPoisonTime = poisonTime + timeIncrease * (power - 1);
        float thisPoisonCooldown = poisonCooldown - cooldownDecrease * (power - 1);
        StartCoroutine(SlowApply(poisonSlow, thisPoisonTime, 3, enemy));
        float poisonTimer = 0f;
        while (poisonTimer < thisPoisonTime)
        {
            yield return new WaitForSeconds(thisPoisonCooldown);
            if (!enemy.activeSelf || !enemy.GetComponent<EnemyBehavior>().isPoisoned)
                break;
            enemy.GetComponent<EnemyBehavior>().DealDamage(poisonDamage, Color.green);
            poisonTimer += poisonCooldown;
        }
        //Debug.Log("Done");
        enemy.GetComponent<EnemyBehavior>().isPoisoned = false;
        enemy.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public override void OnProjTravel(GameObject proj, int power)
    {
        poisonPool = GameObject.FindGameObjectWithTag("poisonPool");
        StartCoroutine(PoisonSpawn(proj, power));
    }

    IEnumerator PoisonSpawn(GameObject proj, int power)
    {
        bool first = true;
        float thisCloudTime = cloudTime + cloudTimeIncrease * (power - 1);
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
            //int tier = proj.GetComponent<ProjectileBehavior>().GetTier();
            //string[] augs = proj.GetComponent<ProjectileBehavior>().getAugments();
            Dictionary<string, int> actualAugs = proj.GetComponent<ProjectileBehavior>().getActualAugs();
            newCloud.GetComponent<AoeBehavior>().Activate(actualAugs, thisCloudTime, "green");
            yield return new WaitForSeconds(cloudCooldown);
        }
    }

    public override List<SpecialStats> GetSpecialValues(int power)
    {
        List<SpecialStats> returnedStats = new List<SpecialStats>();

        //poison damage
        SpecialStats poisonDamages = new SpecialStats("Poison Damage", poisonDamage, "");
        returnedStats.Add(poisonDamages);

        //poison tickRate
        SpecialStats poisonTick = new SpecialStats("Poison Applied Every", poisonCooldown - cooldownDecrease * (power - 1), "seconds");
        returnedStats.Add(poisonTick);

        //active time
        SpecialStats poisonTimer = new SpecialStats("Poison Length", poisonTime + timeIncrease * (power - 1), "seconds");
        returnedStats.Add(poisonTimer);

        //slow amount
        SpecialStats slowAm = new SpecialStats("Slow Amount", poisonSlow, "Base Speed");
        returnedStats.Add(slowAm);

        //cloud time
        SpecialStats cloudTimes = new SpecialStats("Cloud Time", cloudTime + cloudTimeIncrease * (power - 1), "seconds");
        returnedStats.Add(cloudTimes);

        //cloud spawn rate
        SpecialStats cloudRelease = new SpecialStats("Cloud Released Every", cloudCooldown, "seconds");
        returnedStats.Add(cloudRelease);

        return returnedStats;
    }
}
