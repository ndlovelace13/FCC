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
        Stalk, //0
        Approach, //1
        Punch, //2
        Charge, //3
        Hook, //4
        Insult, //5
    }

    //state Stuff
    BossState currentState;
    BossState prevState;
    float passedTime = 0f;
    float stateTime = 0f;
    bool stateCancel = false;
    bool directionLock = false;

    //spawn Stuff
    bool spawning = true;
    public float spawnTime = 20f;
    BullyStats stats;

    //insult Stuff
    ObjectPool projPool;

    //bulb handler
    [SerializeField] GameObject bulb;

    //hook Stuff
    [SerializeField] GameObject fistPrefab;

    //health Stuff
    [SerializeField] GameObject healthbarPrefab;
    int healthScale;
    GameObject healthBar;

    //poppy stuff
    [SerializeField] GameObject poppyHead;

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
                passedTime += Time.deltaTime * speedMod;
                //implement anim speed here based on slow effects
                GetComponent<Animator>().speed = speedMod;
            }
            else
                GetComponent<Animator>().speed = 0f;

            //TODO - implement function to update the anim based on currentState, scale anim speed based off of currentSlow effect

            if (!directionLock)
                StartCoroutine(DirectionHandle());
            //movement
            if (!isBlinded)
            {

            }
            else
                Debug.Log("Enemy isn't currently tracking");
            if (health <= 0)
            {
                Deactivate();
                yield break;
            }
            //wait for end of frame regardless of the situation
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DirectionHandle()
    {
        Vector2 direction = GetComponentInChildren<Rigidbody2D>().velocity;
        if (direction.x < 0f)
            transform.localScale = new Vector3(-1, 1);
        else
            transform.localScale = Vector3.one;
        yield return null;
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
            Debug.Log("particle found");
            particle.GetComponent<Animator>().SetInteger("augment", 0);
        }

        //assign base speed
        backupSpeed = mySpawner.currentMax;
        moveSpeed = backupSpeed;
        //GetComponent<Animator>().speed = backupSpeed * 0.5f;

        allSprites = GetComponentsInChildren<SpriteRenderer>();

        //get the projPool
        projPool = GameObject.FindWithTag("projectilePool").GetComponent<ObjectPool>();

        //set the enemy's initial target to be the player
        player = player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerStatus>().transform;
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
        bulb.GetComponent<SizeLerp>().Execute(false);
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
        GetComponent<Animator>().SetInteger("state", (int)currentState);
        yield return null;

    }
    
    //Stalk Coroutine - default state between
    IEnumerator Stalk()
    {
        currentState = BossState.Stalk;
        //directionLock = true;
        //calculate the downtime
        passedTime = 0f;
        stateTime = Random.Range(4f, 7f);
        //choose a direction
        int right = Random.Range(0, 2);

        /*//lock the direction function
        directionLock = true;
        
        //set the direction

        if (right == 1)
        {
            transform.localScale = new Vector3(1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1);
        }*/

        Vector3 dir;
        if (right == 1)
            dir = Vector3.forward;
        else
            dir = Vector3.back;

        while (passedTime < stateTime)
        {
            Vector2 radiusAdj;
            //stay within a certain distance of player while stalking
            Vector2 direction = target.position - transform.position;
            //rotate around player depending on direction
            //transform.RotateAround(target.position, dir, Time.deltaTime * moveSpeed * 2);
            Vector2 newForward = Vector3.Cross(direction, Vector3.back).normalized * moveSpeed / 2f;
            //transform.rotation = Quaternion.identity;

            if (direction.magnitude < 5.5)
            {
                radiusAdj = -direction.normalized * 4f;
                GetComponent<Animator>().SetBool("adjusting", true);
                GetComponent<Animator>().speed = Mathf.Abs(GetComponent<Animator>().speed);
            }
            else if (direction.magnitude > 8.5)
            {
                radiusAdj = direction.normalized * 4f;
                GetComponent<Animator>().SetBool("adjusting", true);
                GetComponent<Animator>().speed = Mathf.Abs(GetComponent<Animator>().speed);
            }
            else
            {
                radiusAdj = Vector2.zero;
                GetComponent<Animator>().SetBool("adjusting", false);
                GetComponent<Animator>().speed = Mathf.Abs(GetComponent<Animator>().speed);
            }

            GetComponentInChildren<Rigidbody2D>().velocity = newForward + radiusAdj;
                

            yield return new WaitForEndOfFrame();
        }

        //once stalk is complete, choose an attack abd callStateUpdate to execute
        Debug.Log("stalking complete");
        GetComponentInChildren<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Animator>().speed = Mathf.Abs(GetComponent<Animator>().speed);
        GetComponent<Animator>().SetBool("adjusting", false);

        List<BossState> nextStates = new List<BossState>();
        for (int i = 0; i < System.Enum.GetValues(typeof(BossState)).Length; i++)
        {
            BossState state = (BossState)i;
            if (state != currentState && state != prevState && state != BossState.Punch)
                nextStates.Add((BossState)i);
        }
        currentState = nextStates[Random.Range(0, nextStates.Count)];
        //directionLock = false;
        //currentState = BossState.Hook;
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

            //start the punch anim early if time allows for it
            if (passedTime > stateTime / 3 * 2)
                GetComponent<Animator>().SetInteger("state", 2);
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
        stateTime = 2f;
        //punch at the target - stop if canceled
        SpeedUp(0.75f);
        while (passedTime < stateTime / 2)
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
            Debug.Log("Now Cooling Down " + GetComponentInChildren<Rigidbody2D>().velocity);
            GetComponentInChildren<Rigidbody2D>().velocity = Vector2.Lerp(currentVel, Vector2.zero, (passedTime - currentTime) / (stateTime - currentTime));
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
        stateTime = 4f;
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
        while (passedTime < 3f && !stateCancel)
        {
            GetComponentInChildren<Rigidbody2D>().velocity = -direction.normalized * moveSpeed;
            if (isFrozen || isElectrified) { stateCancel = true; break; }
            yield return new WaitForEndOfFrame();
        }
        Vector2 currentVel = GetComponentInChildren<Rigidbody2D>().velocity;
        float currentTime = passedTime;
        GetComponent<Animator>().SetTrigger("finish");
        //slow to a halt unless cancelled
        while (passedTime < stateTime && !stateCancel)
        {
            Debug.Log("Now Cooling Down " + GetComponentInChildren<Rigidbody2D>().velocity);
            GetComponentInChildren<Rigidbody2D>().velocity = Vector2.Lerp(currentVel, Vector2.zero, (passedTime - currentTime) / (stateTime - currentTime));
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
        //reset time vals
        passedTime = 0f;
        stateTime = 6f;
        //hand in ground anim
        while (passedTime < 1f)
        {
            if (isFrozen || isElectrified) { stateCancel = true; break; }
            yield return new WaitForEndOfFrame();
        }
        GameObject fist = new GameObject();
        if (!stateCancel)
        {
            int sideChoice = Random.Range(0, 2);
            Vector3 sideOffset;
            if (sideChoice == 0)
                sideOffset = new Vector3(5f, 0f);
            else
                sideOffset = new Vector3(-5f, 0f);
            Vector3 spawnLoc = target.transform.position + sideOffset;
            Debug.Log("Spawn Location: " + spawnLoc + " | target Location: " + target.transform.position + " | player location: " + player.transform.position);
            //spawn the fist
            fist = Instantiate(fistPrefab, spawnLoc, Quaternion.identity);
            fist.transform.SetParent(transform);
        }
        //chill, repeat the same anim while the fist is active
        while (fist.activeSelf && !stateCancel)
        {
            yield return new WaitForEndOfFrame();
        }
        GetComponent<Animator>().SetTrigger("finish");
        Destroy(fist);
        passedTime = 0f;
        while (passedTime < 0.5f)
        {
            yield return new WaitForEndOfFrame();
        }
        
        currentState = BossState.Stalk;
        StartCoroutine(StateUpdate());
        yield break;
    }

    //Insult Coroutine
    IEnumerator Insult()
    {
        Debug.Log("Insult Called");
        //reset time vals
        passedTime = 0f;
        stateTime = 1.5f;
        while (passedTime < stateTime)
        {
            if (isFrozen || isElectrified) { stateCancel = true; break; }
            yield return new WaitForEndOfFrame();
        }    
        if (!stateCancel)
        {
            //proj spawn here
            Vector2 direction = player.transform.position - transform.position;
            //radian conversion (idk what this does tbh)
            float angleRadians = Mathf.Atan2(-direction.y, direction.x);
            if (angleRadians < 0)
                angleRadians += 2 * Mathf.PI;
            angleRadians += Mathf.PI / 2;
            StartCoroutine(ProjSpawn(angleRadians));
        }
        passedTime = 0f;
        while (passedTime < 0.5f)
        {
            yield return new WaitForEndOfFrame();
        }

        currentState = BossState.Stalk;
        StartCoroutine(StateUpdate());
        yield break;
    }

    IEnumerator ProjSpawn(float angle)
    {
        Vector2 projDir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        projDir.Normalize();
        GameObject proj = projPool.GetComponent<ObjectPool>().GetPooledObject();
        if (proj == null)
            Debug.Log("get fucked");
        //Debug.Log(proj.transform.position);
        //Debug.Log(crown.transform.position);
        //proj.transform.rotation = new Quaternion(angle);
        proj.transform.position = transform.position;
        proj.transform.localScale = Vector3.one * 2;
        Dictionary<string, int> projAugs = new Dictionary<string, int>();
        projAugs.Add("bully", 7);
        //rotating towards direction of movement
        proj.SetActive(true);
        proj.GetComponent<ProjectileBehavior>().SetProps(20f, 0, projAugs, projDir, false, true);
        proj.GetComponentInChildren<Rigidbody2D>().velocity = projDir * moveSpeed * 2;
        yield return null;
    }

    IEnumerator BossSpawn()
    {
        //disable the sprite renderer and animator
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Animator>().enabled = false;

        float timer = 0f;
        Debug.Log("Boss Spawn Initiated");
        healthBar = Instantiate(healthbarPrefab);
        //healthBar.GetComponent<BossHealthBar>().AssignBoss(gameObject);

        while (timer < spawnTime)
        {
            transform.localScale = Vector3.Lerp(Vector3.one * 0.1f, Vector3.one, timer / spawnTime);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        //boss actually spawned here - activate the sprite and state machine
        GameControl.PlayerData.bossActive = true;
        GameControl.PlayerData.bossSpawning = false;
        spawning = false;
        invulnerable = false;

        //do the bulb anim
        bulb.GetComponent<Animator>().SetTrigger("BullySpawn");

        //enable the sprite renderer and animator
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Animator>().enabled = true;

        yield return new WaitForSeconds(1f);
        bulb.SetActive(false);

        if (GameControl.PlayerData.savedEnemyDict[type].encountered == false)
            GameControl.PlayerData.savedEnemyDict[type].encountered = true;

        Debug.Log("Boss Spawning Complete");


        //Now start all the regular routines
        stats = (BullyStats)myStats;
        StartCoroutine(SortingAdjust());
        StartCoroutine(StandardBehavior());
        StartCoroutine(StateUpdate());
        
    }

    //Death Behavior
    protected override void Deactivate()
    {
        AkSoundEngine.PostEvent("EnemyKilled", gameObject);
        scoreNotif.GetComponent<ScoreNotification>().newFeed("Great Enemy Defeated | ", mySpawner.killScore);
        GameControl.PlayerData.enemyScore += mySpawner.killScore;
        GameControl.PlayerData.shiftEnemies++;
        

        //increment the total defeatedCount & shift defeated
        GameControl.PlayerData.savedEnemyDict[type].defeatedCount++;
        GameControl.PlayerData.enemyKills[type]++;


        GameControl.PlayerData.score += mySpawner.killScore;

        GameControl.PlayerData.gameWin = true;
        GameControl.SaveData.bullyDefeated = true;

        //seed stuff from 3 - 5 seeds depending on seedChahnce stat
        int seedCount = 3;
        for (int i = 0; i < 2; i++)
        {
            if (Random.Range(0f, 1f) < GameControl.PlayerData.seedChance)
            {
                seedCount++;
            }
        }

        for (int i = 0; i < seedCount; i++)
        {
            GameObject newSeed = seedPool.GetComponent<ObjectPool>().GetPooledObject();
            newSeed.SetActive(true);
            newSeed.transform.localPosition = transform.localPosition;
            //lerp the seed to a random drop zone in the immediate vicinity of the boss
            newSeed.GetComponent<EssenceBehavior>().LootDrop();
        }

        //spawn a poppy here and unlock it for the player
        StartCoroutine(PoppySpawn());

        //start health bar deactivate
        healthBar.GetComponentInChildren<BossHealthBar>().BossKilled();
        
        mySpawner.activeEnemies--;
    }

    IEnumerator PoppySpawn()
    {
        GameObject newFlower = GameObject.FindWithTag("flowerPool").GetComponent<ObjectPool>().GetPooledObject();
        if (newFlower != null)
        {
            newFlower.SetActive(true);
            newFlower.transform.position = transform.position;

            GameObject head = Instantiate(poppyHead);
            head.transform.SetParent(newFlower.transform);
            head.transform.position = newFlower.transform.position;
            head.GetComponent<SpriteRenderer>().enabled = false;
            head.SetActive(true);
            //execute the initial growth anim
            Animator stemAnim = newFlower.GetComponentsInChildren<Animator>().Last();
            stemAnim.Play("BasicGrow");
            //Debug.Log(stemAnim.gameObject.name);
            while (stemAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                Debug.Log(stemAnim.GetCurrentAnimatorStateInfo(0).normalizedTime);
                yield return new WaitForEndOfFrame();
            }
            Debug.Log("exited loop");
            
            //set the head sprite
            head.GetComponent<SpriteRenderer>().sprite = GameControl.PlayerData.flowerStatsDict["poppy"].GetHeadSprite(0);
            //reset the behavior object or pull values from last time
            head.GetComponent<FlowerBehavior>().picked = false;
            head.GetComponent<FlowerBehavior>().growing = false;

            //set the stem and head pos
            GameControl.PlayerData.flowerStatsDict["poppy"].SetStem(head, stemAnim.gameObject);

            head.GetComponent<SpriteRenderer>().enabled = true;
            Debug.Log("Poppy head placed");

            //pass the torch to the player object - REMOVE THIS WHEN THE END OF THE GAME SHIFTS
            GameObject.FindWithTag("Player").GetComponentInChildren<PlayerMovement>().GameWin(newFlower.transform.position, head.GetComponent<FlowerBehavior>().type);

            gameObject.SetActive(false);
        }
    }
}
