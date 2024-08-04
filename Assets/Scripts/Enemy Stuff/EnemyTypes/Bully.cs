using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Bully : EnemyBehavior
{
    enum BossState
    {
        Stalk,
        Approach,
        Punch,
        Charge,
        Hook,
        Insult
    }

    BossState currentState;
    BossState prevState;

    bool spawning = true;
    public float spawnTime = 30f;

    [SerializeField] GameObject healthbarPrefab;
    int healthScale;
    GameObject healthBar;

    // Start is called before the first frame update
    /*void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    //the basic behavior of the enemy
    public override IEnumerator StandardBehavior()
    {
        
        while (gameObject.activeSelf)
        {

            GameObject[] crowns = GameObject.FindGameObjectsWithTag("finalCrown");
            foreach (GameObject obj in crowns)
            {
                if (obj.GetComponent<CrownAttack>().crownActive == true)
                    crown = obj.transform;
            }
            if (crown != null)
            {
                //Debug.Log("target swap called");
                TargetSwap();
            }
            else
            {
                target = player;
            }
            if (!isFrozen)
                moveSpeed = backupSpeed;
            //movement
            if (!isBlinded)
            {

            }
            else
                Debug.Log("Enemy isn't currently tracking");
            if (health <= 0)
            {
                Deactivate();
            }
            //wait for end of frame regardless of the situation
            yield return new WaitForEndOfFrame();
        }
    }

    //right when the boss spawns do all this shit
    public override void Activate()
    {
        //Play boss spawn sound here
        AkSoundEngine.PostEvent("EnemySpawn", gameObject);
        //initialize variables
        StartCoroutine(HealthEst());
        isActive = true;
        GetComponent<SpriteRenderer>().color = Color.white;
        if (particles == null)
        {
            getParticles();
        }
        foreach (var particle in particles)
        {
            particle.GetComponent<Animator>().SetInteger("augment", 0);
        }

        //assign base speed
        backupSpeed = mySpawner.currentMax;
        moveSpeed = backupSpeed;
        GetComponent<Animator>().speed = backupSpeed * 0.5f;

        allSprites = GetComponentsInChildren<SpriteRenderer>();

        //set the enemy's initial target to be the player
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<CapsuleCollider2D>().transform;
        target = player;
        //Debug.Log(player.position);
        

        //start spawning stuff
        GameControl.PlayerData.bossSpawning = true;
        StartCoroutine(BossSpawn());
    }

    //init health, figure out the health scaling here
    IEnumerator HealthEst()
    {
        maxHealth = mySpawner.currentHealth;
        health = maxHealth / 2;

        var currentEnemies = FindObjectsByType<EnemyBehavior>(FindObjectsSortMode.None);
        int enemyCount = currentEnemies.Length - 1;

        healthScale = Mathf.CeilToInt((float)health / enemyCount);

        Debug.Log(healthScale + " scale amount | enemies: " + currentEnemies.Length);
        yield return null;
    }

    public void HealthSacrifice()
    {
        health += healthScale;
        if (health > maxHealth)
            health = maxHealth;
        //size should grow here
    }

    public override IEnumerator StateUpdate()
    {
        BullyStats stats = (BullyStats)myStats;

        while (gameObject.activeSelf)
        {
            if (!summoning)
            {
                //put state machine here
            }
            yield return new WaitForEndOfFrame();
        }

    }

    IEnumerator BossSpawn()
    {
        float timer = 0f;
        Debug.Log("Boss Spawn Initiated");
        healthBar = Instantiate(healthbarPrefab);
        //healthBar.GetComponent<BossHealthBar>().AssignBoss(gameObject);

        while (timer < spawnTime)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        //boss actually spawned here - activate the sprite and state machine
        GameControl.PlayerData.bossActive = true;
        GameControl.PlayerData.bossSpawning = false;
        spawning = false;
        Debug.Log("Boss Spawning Complete");

        //Now start all the regular routines
        StartCoroutine(SortingAdjust());
        StartCoroutine(StandardBehavior());
        StartCoroutine(StateUpdate());
    }
}
