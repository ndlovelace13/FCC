using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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
        Insult,
    }

    //state Stuff
    BossState currentState;
    BossState prevState;
    float passedTime = 0f;
    float stateTime = 0f;
    bool stateCancel = false;

    //spawn Stuff
    bool spawning = true;
    public float spawnTime = 20f;
    BullyStats stats;

    //health Stuff
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

    //the basic behavior of the enemy - begins when the boss is fully spawned
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
            {
                moveSpeed = backupSpeed;
                passedTime += Time.deltaTime;
            }
            
            //TODO - implement function to update the anim based on currentState, scale anim speed based off of currentSlow effect
                
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
        invulnerable = true;
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

    public void HealthSacrifice(float healthRatio)
    {
        int healthInc = Mathf.CeilToInt(healthScale * healthRatio);
        //Debug.Log("health increasing by: " + healthInc + " | health ratio: " + healthRatio);
        health += healthInc;
        if (health > maxHealth)
            health = maxHealth;
        //size should grow here
    }

    public override IEnumerator StateUpdate()
    {
        //reset state cancel
        stateCancel = false;
        //apply the new state
        switch (currentState)
        {
            case BossState.Stalk: StartCoroutine(Stalk()); break;
            case BossState.Approach: StartCoroutine(Approach()); break;
            case BossState.Punch: StartCoroutine(Punch()); break;
            case BossState.Charge: StartCoroutine(Charge()); break;
            case BossState.Hook: StartCoroutine(Hook()); break;
            case BossState.Insult: StartCoroutine(Insult()); break;
        }
        //set prevState if applicable
        if (currentState != BossState.Stalk && currentState != BossState.Punch)
            prevState = currentState;
        //probably not necessary - maybe tho
        //else if (currentState == BossState.Punch)
            //prevState = BossState.Approach;
        yield return null;

    }
    
    //Stalk Coroutine - default state between
    IEnumerator Stalk()
    {
        currentState = BossState.Stalk;
        //calculate the downtime
        passedTime = 0f;
        stateTime = Random.Range(4f, 7f);
        //choose a direction
        int right = Random.Range(0, 2);
        Vector3 dir;
        if (right == 1)
            dir = Vector3.forward;
        else
            dir = Vector3.back;

        while (passedTime < stateTime)
        {
            //rotate around player depending on direction
            transform.RotateAround(target.position, dir, Time.deltaTime * moveSpeed * 2);
            transform.rotation = Quaternion.identity;

            //stay within a certain distance of player while stalking
            Vector2 direction = transform.position - target.position;
            if (direction.magnitude < 5.5)
            {
                GetComponentInChildren<Rigidbody2D>().velocity = direction.normalized * 4f;
            }
            else if (direction.magnitude > 8.5)
                GetComponentInChildren<Rigidbody2D>().velocity = -direction.normalized * 4f;
            else
                GetComponentInChildren<Rigidbody2D>().velocity = Vector2.zero;

            yield return new WaitForEndOfFrame();
        }

        //once stalk is complete, choose an attack abd callStateUpdate to execute
        Debug.Log("stalking complete");
        GetComponentInChildren<Rigidbody2D>().velocity = Vector2.zero;

        List<BossState> nextStates = new List<BossState>();
        for (int i = 0; i < System.Enum.GetValues(typeof(BossState)).Length; i++)
        {
            BossState state = (BossState)i;
            if (state != currentState && state != prevState && state != BossState.Punch)
                nextStates.Add((BossState)i);
        }
        currentState = nextStates[Random.Range(0, nextStates.Count)];
        StartCoroutine(StateUpdate());
    }
    
    //Approach Coroutine
    IEnumerator Approach()
    {
        Debug.Log("Approach Called");
        //max approach time is 3 seconds - if not there in time, just punch anyways
        passedTime = 0f;
        stateTime = 3f;
        Vector2 direction = shadow.position - target.position;
        SpeedDown(0.75f);

        while (direction.magnitude > 3f && passedTime < stateTime)
        {
            GetComponentInChildren<Rigidbody2D>().velocity = -direction.normalized * moveSpeed;
            if (isFrozen || isElectrified) { stateCancel = true; break; }
            yield return new WaitForEndOfFrame();
            direction = shadow.position - target.position;
        }
        SpeedUp(0.75f);
        //if cancelled, return to stalk - otherwise proceed to punch
        if (stateCancel)
            currentState = BossState.Stalk;
        else
            currentState = BossState.Punch;
        StartCoroutine(StateUpdate());
        yield break;
    }

    //Punch Coroutine
    IEnumerator Punch()
    {
        Debug.Log("Punch Called");
        //get punch direction
        Vector2 direction = shadow.position - target.position;
        //reset time variables
        passedTime = 0f;
        stateTime = 3f;
        //punch at the target - stop if canceled
        SpeedUp(0.75f);
        while (passedTime < stateTime / 1)
        {
            GetComponentInChildren<Rigidbody2D>().velocity = -direction.normalized * moveSpeed;
            if (isFrozen || isElectrified) { stateCancel = true; break; }
            yield return new WaitForEndOfFrame();
        }
        Vector2 currentVel = GetComponentInChildren<Rigidbody2D>().velocity;
        //slow to a halt unless cancelled
        float currentTime = passedTime;
        while (passedTime < stateTime && !stateCancel)
        {
            Debug.Log("Now Cooling Down");
            GetComponentInChildren<Rigidbody2D>().velocity = Vector2.Lerp(currentVel, Vector2.zero, (passedTime - currentTime) / stateTime);
            if (isFrozen || isElectrified) { stateCancel = true; break; }
            yield return new WaitForEndOfFrame();
        }
        SpeedDown(0.75f);

        //reset the velocity and state
        GetComponentInChildren<Rigidbody2D>().velocity = Vector2.zero;
        currentState = BossState.Stalk;
        StartCoroutine(StateUpdate());
        yield break;
    }

    //Charge Coroutine
    IEnumerator Charge()
    {
        Debug.Log("Charge Called");
        //reset time vars
        passedTime = 0f;
        stateTime = 5f;
        //slow while charging up charge
        Vector2 direction = shadow.position - target.position;
        SpeedDown(0.25f);
        while (passedTime < 1f)
        {
            GetComponentInChildren<Rigidbody2D>().velocity = -direction.normalized * moveSpeed;
            if (isFrozen || isElectrified) { stateCancel = true; break; }
            yield return new WaitForEndOfFrame();
            direction = shadow.position - target.position;
        }
        //speed back to normal
        SpeedUp(0.25f);
        //speed up to charge speed & get final direction
        SpeedUp(2 / 3f);
        while (passedTime < 3.5f && !stateCancel)
        {
            GetComponentInChildren<Rigidbody2D>().velocity = -direction.normalized * moveSpeed;
            if (isFrozen || isElectrified) { stateCancel = true; break; }
            yield return new WaitForEndOfFrame();
        }
        Vector2 currentVel = GetComponentInChildren<Rigidbody2D>().velocity;
        //slow to a halt unless cancelled
        while (passedTime < stateTime && !stateCancel)
        {
            Debug.Log("Now Cooling Down");
            GetComponentInChildren<Rigidbody2D>().velocity = Vector2.Lerp(currentVel, Vector2.zero, passedTime / stateTime);
            if (isFrozen || isElectrified) { stateCancel = true; break; }
            yield return new WaitForEndOfFrame();
        }
        //prepare for the next state
        SpeedDown(2 / 3f);
        currentState = BossState.Stalk;
        StartCoroutine(StateUpdate());
        yield break;
    }

    //Hook Coroutine
    IEnumerator Hook()
    {
        Debug.Log("Hook Called");
        //TODO - Spawn a fist obj here on a random side of the player
        yield return new WaitForSeconds(2f);
        currentState = BossState.Stalk;
        StartCoroutine(StateUpdate());
        yield break;
    }

    //Insult Coroutine
    IEnumerator Insult()
    {
        Debug.Log("Insult Called");
        //TODO - Spawn a projectile here to fly to the player, make the debuff as well
        yield return new WaitForSeconds(2f);
        currentState = BossState.Stalk;
        StartCoroutine(StateUpdate());
        yield break;
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
        invulnerable = false;
        Debug.Log("Boss Spawning Complete");

        //Now start all the regular routines
        stats = (BullyStats)myStats;
        StartCoroutine(SortingAdjust());
        StartCoroutine(StandardBehavior());
        StartCoroutine(StateUpdate());
    }
}
