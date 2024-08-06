using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeStats : FlowerStats
{
    [SerializeField] GameObject coffeePool;
    float beanTime = 5f;

    //speed effect stats
    float speedTime = 3f;
    float speedEffect = 1.1f;

    //power scaling
    float timeIncrease = 1f;
    float speedIncrease = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        effects = "Caffeination - The Coffee Flower's projectiles grant the player with a speed boost upon collision\nCoffee Beans - Projectiles will drop an speed-boosting item upon reaching the end of their trajectory";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnProjArrival(GameObject proj, int power)
    {
        Debug.Log("ITS WORKING");
        if (coffeePool == null)
        {
            coffeePool = GameObject.FindGameObjectWithTag("coffeePool");
        }
        GameObject newBean = coffeePool.GetComponent<ObjectPool>().GetPooledObject();
        newBean.SetActive(true);
        newBean.transform.position = proj.transform.position;
        float thisBeanTime = beanTime;
        newBean.GetComponent<AoeBehavior>().Activate(proj.GetComponent<ProjectileBehavior>().getActualAugs(), thisBeanTime, "");
        //proj.GetComponent<ProjectileBehavior>().ObjectDeactivate();
    }

    public override List<SpecialStats> GetSpecialValues(int power)
    {
        List<SpecialStats> returnedStats = new List<SpecialStats>();

        //bean speed boost
        SpecialStats speedTimer = new SpecialStats("Speed Boost Length", speedTime + timeIncrease * (power - 1), "Seconds");
        returnedStats.Add(speedTimer);

        //speed effect
        SpecialStats speedBoost = new SpecialStats("Speed Boost Effect", speedEffect + speedIncrease * (power - 1), "Base Speed");
        returnedStats.Add(speedBoost);

        //bean active time
        SpecialStats beanTimer = new SpecialStats("Coffee Bean Length", beanTime, "Seconds");
        returnedStats.Add(beanTimer);

        return returnedStats;
    }

    public override void OnPlayerCollision(GameObject playerShadow, int power)
    {
        Debug.Log("doing the thing");
        base.OnPlayerCollision(playerShadow, power);
        StartCoroutine(SpeedBoost(playerShadow, power));
    }

    IEnumerator SpeedBoost(GameObject playerShadow, int power)
    {
        Debug.Log("doing the actual thing");
        float thisSpeedLength = speedTime + timeIncrease * (power - 1);
        float thisSpeedEffect = speedEffect + speedIncrease * (power - 1);
        PlayerMovement player = playerShadow.GetComponent<PlayerMovement>();
        Animator particle = playerShadow.GetComponent<PlayerStatus>().playerParticle;
        playerShadow.GetComponent<PlayerStatus>().speedBuffs++;

        //apply the speed boost
        player.speed *= thisSpeedEffect;
        particle.SetInteger("augment", 6);

        //wait for the timer to run
        float currentTime = 0f;
        while (currentTime < thisSpeedLength)
        {
            if (GameControl.PlayerData.gameOver)
            {
                particle.SetInteger("augment", 0);
                yield break;
            }
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            Debug.Log("Player Speed: " + player.speed);
        }

        //remove the speed boost
        if (playerShadow.GetComponent<PlayerStatus>().speedBuffs > 0)
            playerShadow.GetComponent<PlayerStatus>().speedBuffs--;
        player.speed /= thisSpeedEffect;
        if (playerShadow.GetComponent<PlayerStatus>().speedBuffs == 0)
        {
            particle.SetInteger("augment", 0);
        }
        
        

        yield return null;
    }
}
