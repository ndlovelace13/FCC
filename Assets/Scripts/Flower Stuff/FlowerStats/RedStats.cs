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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnProjTravel(GameObject proj)
    {
        base.OnProjTravel(proj);
        if (GameControl.PlayerData.tutorialActive)
            GameControl.PlayerData.redCrown = true;
    }

    public override void OnProjArrival(GameObject proj)
    {
        Debug.Log("ITS WORKING");
        if (flamesPool == null)
        {
            flamesPool = GameObject.FindGameObjectWithTag("flamePool");
        }
        GameObject newFlame = flamesPool.GetComponent<ObjectPool>().GetPooledObject();
        newFlame.SetActive(true);
        newFlame.transform.position = proj.transform.position;
        newFlame.GetComponent<AoeBehavior>().Activate(proj.GetComponent<ProjectileBehavior>().getAugments(), flameTime, "red");
        //proj.GetComponent<ProjectileBehavior>().ObjectDeactivate();
    }

    public override void OnEnemyCollision(GameObject enemy)
    {
        enemy.GetComponent<EnemyBehavior>().isBurning = true;
        enemy.GetComponent<SpriteRenderer>().color = Color.red;
        StartCoroutine(BurnHandler(enemy));
    }

    IEnumerator BurnHandler(GameObject enemy)
    {
        float burnTimer = 0f;
        GameObject part = enemy.GetComponent<EnemyBehavior>().nextParticle();
        enemy.GetComponent<EnemyBehavior>().setParticle(part, 1);
        while (burnTimer < burnTime)
        {
            //Debug.Log("ouch " + health);
            yield return new WaitForSeconds(burnCooldown);
            if (enemy.activeSelf == false)
                break;
            enemy.GetComponent<EnemyBehavior>().DealDamage(burnDamage, Color.red);
            burnTimer += burnCooldown;
        }
        Debug.Log("Done");
        enemy.GetComponent<EnemyBehavior>().isBurning = false;
        enemy.GetComponent<EnemyBehavior>().setParticle(part, 0);
        enemy.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
