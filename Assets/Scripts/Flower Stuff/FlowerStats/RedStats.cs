using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedStats : FlowerStats
{
    //aoe stats
    [SerializeField] GameObject flamesPool;
    float flameTime = 2f;

    //burn effect stats
    int burnDamage = 2;
    float burnCooldown = 0.5f;
    float burnTime = 3;

    //power scaling
    int damageIncrease = 1;
    float timeIncrease = 1.5f;
    float flameTimeIncrease = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnProjTravel(GameObject proj, int power)
    {
        base.OnProjTravel(proj, power);
        if (GameControl.PlayerData.tutorialActive)
            GameControl.PlayerData.redCrown = true;
    }

    public override void OnProjArrival(GameObject proj, int power)
    {
        Debug.Log("ITS WORKING");
        if (flamesPool == null)
        {
            flamesPool = GameObject.FindGameObjectWithTag("flamePool");
        }
        GameObject newFlame = flamesPool.GetComponent<ObjectPool>().GetPooledObject();
        newFlame.SetActive(true);
        newFlame.transform.position = proj.transform.position;
        int tier = proj.GetComponent<ProjectileBehavior>().GetTier();
        float thisFlameTime = flameTime + flameTimeIncrease * (power - 1);
        newFlame.GetComponent<AoeBehavior>().Activate(proj.GetComponent<ProjectileBehavior>().getActualAugs(), thisFlameTime, "red");
        //proj.GetComponent<ProjectileBehavior>().ObjectDeactivate();
    }

    public override void OnEnemyCollision(GameObject enemy, int t)
    {
        enemy.GetComponent<EnemyBehavior>().isBurning = true;
        enemy.GetComponent<SpriteRenderer>().color = Color.red;
        StartCoroutine(BurnHandler(enemy, t));
    }

    IEnumerator BurnHandler(GameObject enemy, int power)
    {
        float burnTimer = 0f;
        //TODO - fuck up this system, foul ass particle system needs to go
        GameObject part = enemy.GetComponent<EnemyBehavior>().nextParticle();
        enemy.GetComponent<EnemyBehavior>().setParticle(part, 1);
        float thisBurnTime = burnTime + timeIncrease * (power - 1);
        int thisBurnDmg = burnDamage + damageIncrease * (power - 1);
        while (burnTimer < thisBurnTime)
        {
            //Debug.Log("ouch " + health);
            yield return new WaitForSeconds(burnCooldown);
            if (enemy.activeSelf == false)
                break;
            enemy.GetComponent<EnemyBehavior>().DealDamage(thisBurnDmg, Color.red);
            burnTimer += burnCooldown;
        }
        Debug.Log("Done");
        enemy.GetComponent<EnemyBehavior>().isBurning = false;
        enemy.GetComponent<EnemyBehavior>().setParticle(part, 0);
        enemy.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
