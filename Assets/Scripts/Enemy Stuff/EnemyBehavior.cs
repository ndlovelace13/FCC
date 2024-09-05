using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public abstract class EnemyBehavior : MonoBehaviour
{
    [SerializeField] public Transform shadow;
    [SerializeField] public Rigidbody2D rb2D;

    public string type;
    public int health;
    protected TMP_Text scoreNotif;
    [SerializeField] public int maxHealth;

    public bool isActive;

    protected Transform player;
    protected Transform crown;
    [SerializeField] protected Transform target;

    public EnemySpawn mySpawner;
    protected EnemyStats myStats;

    //SPEED SHIT
    public float moveSpeed;
    float minSpeed;
    float maxSpeed;
    [SerializeField] public float backupSpeed;
    float speedCooldown = 15f;
    float speedIncrement = 0.925f;
    public float speedMod = 1f;


    public bool surprised = false;
    protected float surpriseTime = 1f;
    public Vector3 preSurpriseVel;

    [SerializeField] protected GameObject notif;

    public string[] augments;
    public Dictionary<string, int> actualAugs;

    //1 FIRE
    public bool isBurning = false;

    //2 ICE
    public bool isFrozen = false;
    public bool isSlowed = false;
    
    //3 POISON
    public bool isPoisoned = false;

    //4 ELECTRIC
    public bool isElectrified = false;
    public bool electricPassed = false;

    //5 BLINDING
    public bool isBlinded = false;

    //POPPY Stuff
    public int poppyCount = 0;

    public bool wasKilled = false;


    //Boss Spawning Stuff
    public bool isBoss;
    protected bool sacrifice = false;
    protected bool summoning = false;
    protected bool invulnerable = false;

    protected SpriteRenderer[] allSprites;

    //Particles
    protected List<GameObject> particles;

    //Seed Stuff
    protected GameObject seedPool;

    // Start is called before the first frame update
    protected void Start()
    {
        //destructScore = 10;
        
        target = player;
        //moveSpeed = maxSpeed;
        scoreNotif = GameObject.FindGameObjectWithTag("scoreAnnounce").GetComponent<TMP_Text>();
        //lightningPool = GameObject.FindGameObjectWithTag("lightningPool");
        seedPool = GameObject.FindGameObjectWithTag("seedPool");
    }

    // Update is called once per frame
    protected void Update()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerStatus>().transform;
    }

    public virtual IEnumerator StandardBehavior()
    {
        Debug.Log("Standard Behavior is not defined");
        yield return null;
    }

    public virtual IEnumerator StateUpdate()
    {
        Debug.Log("No states to update");
        yield return null;
    }

    public virtual IEnumerator StateReset()
    {
        isBurning = false;
        isFrozen = false;
        isSlowed = false;
        isPoisoned = false;
        isBlinded = false;
        isElectrified = false;
        isActive = false;
        surprised = false;
        wasKilled = true;
        yield return null;
    }

    public IEnumerator Cleanse()
    {
        isBurning = false;
        isFrozen = false;
        isSlowed = false;
        isPoisoned = false;
        isBlinded = false;
        isElectrified = false;
        surprised = false;

        //reset enemy to base speed, removing each stack of poppy debuff
        PoppyStats poppy = (PoppyStats)GameControl.PlayerData.flowerStatsDict["poppy"];

        for (int i = 0; i < poppyCount; i++)
        {
            SpeedUp(poppy.poppyDebuff);
        }
        poppyCount = 0;

        yield return null;
    }

    protected IEnumerator SortingAdjust()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForEndOfFrame();
            //assign sortingorder to both itself and hat if exists
            allSprites = GetComponentsInChildren<SpriteRenderer>();
            //visual layer in reference to player
            if (player.localPosition.y < transform.localPosition.y)
            {
                foreach (SpriteRenderer sprite in allSprites)
                {
                    if (sprite.sortingLayerName != "Background")
                        sprite.sortingOrder = 0;
                }

                //GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
            else
            {
                foreach (SpriteRenderer sprite in allSprites)
                {
                    if (sprite.sortingLayerName != "Background")
                        sprite.sortingOrder = 2;
                }
            }
        }
    }

    protected IEnumerator Surprised(float surpriseTime)
    {
        if (!surprised)
        {
            preSurpriseVel = GetComponentInChildren<Rigidbody2D>().velocity;
            GetComponent<Animator>().SetBool("Surprise", true);
            Debug.Log("This mf surprised");
            //GetComponent<SpriteRenderer>().color = Color.blue;
            //backupSpeed = moveSpeed;
            surprised = true;
            moveSpeed = 0f;
            yield return new WaitForSeconds(surpriseTime);
            //moveSpeed = backupSpeed;
            surprised = false;
            GetComponent<Animator>().SetBool("Surprise", false);
            GetComponentInChildren<Rigidbody2D>().velocity = preSurpriseVel;
        }
    }

    public virtual void CollisionCheck(Collider2D other)
    {
        //Debug.Log("collision happening");
        if (!invulnerable)
        {
            if (other.gameObject.tag == "projectile")
            {
                GameObject otherParent = other.gameObject.transform.parent.gameObject;
                if (!otherParent.GetComponent<ProjectileBehavior>().enemyProj)
                {
                    DealDamage(otherParent.GetComponent<ProjectileBehavior>().damage, Color.white);
                    actualAugs = otherParent.GetComponent<ProjectileBehavior>().getActualAugs();
                    AugmentApplication(actualAugs);
                    otherParent.GetComponent<ProjectileBehavior>().ObjectDeactivate();
                }
            }
            else if (other.gameObject.tag == "aoe")
            {
                GameObject otherParent = other.gameObject.transform.parent.gameObject;
                actualAugs = otherParent.GetComponent<AoeBehavior>().getActualAugs();
                AugmentApplication(actualAugs);
            }
        }
    }

    public void AugmentApplication(Dictionary<string, int> actualAugs)
    {
        foreach (var aug in actualAugs)
        {
            GameControl.PlayerData.flowerStatsDict[aug.Key].OnEnemyCollision(gameObject, aug.Value);
        }
    }

    public GameObject ClosestEnemy(GameObject[] enemies)
    {
        //Debug.Log("WHATATT " + enemies.Length);
        GameObject closestEnemy = null;
        float closestDist = Mathf.Infinity;
        foreach (GameObject potentialTarget in enemies)
        {
            if (potentialTarget != gameObject)
            {
                Vector3 dirToTarget = potentialTarget.transform.position - transform.position;
                float dSqrToTarget = dirToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDist && !potentialTarget.GetComponent<EnemyBehavior>().isElectrified)
                {
                    closestDist = dSqrToTarget;
                    closestEnemy = potentialTarget;
                }
            }
        }
        return closestEnemy;
    }

    public virtual void DealDamage(int damage, Color color)
    {
        AkSoundEngine.PostEvent("EnemyHit", gameObject);
        health -= damage;
        if (health < maxHealth / 2 && GetComponentInChildren<HatBehavior>() != null)
            GetComponentInChildren<HatBehavior>().HatFall();
        GameObject newNotif = Instantiate(notif, transform.position + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)), Quaternion.identity);
        newNotif.GetComponent<DamageNotif>().Creation(damage.ToString(), color);
    }

    protected void TargetSwap()
    {
        float playerDist = Vector3.Distance(player.transform.position, gameObject.transform.position);
        float crownDist = Vector3.Distance(crown.transform.position, gameObject.transform.position);
        if (playerDist > crownDist && crownDist < 10f)
        {
            target = crown;
        }
    }
    public void SetObjects(EnemySpawn newSpawner)
    {
        mySpawner = newSpawner;
        myStats = newSpawner.thisEnemy;
    }

    public virtual void Activate()
    {
        AkSoundEngine.PostEvent("EnemySpawn", gameObject);
        //initialize variables
        health = mySpawner.currentHealth;
        maxHealth = health;
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
        GetComponent<Animator>().SetBool("Surprise", false);
        //assign a random speed
        maxSpeed = mySpawner.currentMax;
        minSpeed = mySpawner.currentMin;
        backupSpeed = Random.Range(minSpeed, maxSpeed);
        moveSpeed = backupSpeed;
        GetComponent<Animator>().speed = backupSpeed * 0.5f;
        //begin the gradual speed up routine
        StartCoroutine(GradualSpeedUp());
        StartCoroutine(KillReset());
        allSprites = GetComponentsInChildren<SpriteRenderer>();

        //set the enemy's initial target to be the player
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerStatus>().transform;
        target = player;
        //Debug.Log(player.position);
        StartCoroutine(SortingAdjust());
        StartCoroutine(StandardBehavior());
        StartCoroutine(StateUpdate());
    }

    protected virtual void Deactivate()
    {
        StartCoroutine(StateReset());
        AkSoundEngine.PostEvent("EnemyKilled", gameObject);
        scoreNotif.GetComponent<ScoreNotification>().newFeed("Enemy Defeated | ", mySpawner.killScore);

        //Spawn coins
        GameObject moneySpawner = GameControl.PlayerData.moneySpawner.GetPooledObject();
        moneySpawner.transform.position = transform.position;
        moneySpawner.SetActive(true);
        moneySpawner.GetComponent<CoinSpawn>().Payout(mySpawner.killScore, ScoreCategory.enemy);

        //GameControl.PlayerData.enemyScore += mySpawner.killScore;
        GameControl.PlayerData.shiftEnemies++;

        //increment the total defeatedCount & shift defeated
        GameControl.PlayerData.savedEnemyDict[type].defeatedCount++;
        GameControl.PlayerData.enemyKills[type]++;

        //int currentScore = PlayerPrefs.GetInt("totalScore");
        //PlayerPrefs.SetInt("totalScore", currentScore + destructScore);
        
        //GameControl.PlayerData.score += mySpawner.killScore;
        if (Random.Range(0f, 1f) < GameControl.PlayerData.seedChance)
        {
            GameObject newSeed = seedPool.GetComponent<ObjectPool>().GetPooledObject();
            newSeed.SetActive(true);
            newSeed.transform.localPosition = transform.localPosition;
        }
        mySpawner.activeEnemies--;
        gameObject.SetActive(false);
    }

    public IEnumerator BossSummoning()
    {
        Debug.Log("Boss Summoning Beginning");
        if (isBoss)
        {
            Debug.Log("bro what");
            yield break;
        }
        else
        {
            float waitTime = Random.Range(2f, 15f);
            Debug.Log("waiting for " + waitTime);
            yield return new WaitForSeconds(waitTime);
            sacrifice = true;
        }
    }

    public IEnumerator Sacrifice()
    {
        target = GameObject.FindWithTag("boss").GetComponent<EnemyBehavior>().shadow.transform;
        moveSpeed = backupSpeed;
        while (gameObject.activeSelf)
        {
            //check for death
            if (health < 0)
            {
                Deactivate();
                yield break;
            }
            //check whether the enemy is within sacrifice range of the boss
            float dist = Vector2.Distance(target.position, shadow.position);
            if (dist < 2)
            {
                StartCoroutine(Despawn(true));
                yield break;
            }
            //if the boss gets activated anyways, deactivate
            else if (GameControl.PlayerData.bossActive)
            {
                Debug.Log("Boss Active - killing self");
                StartCoroutine(Despawn(false));
                yield break;
            }
            //move towards the boss 
            if (!isFrozen && !surprised)
                moveSpeed = backupSpeed;
            Vector2 direction = new Vector2(target.position.x - shadow.position.x, target.position.y - shadow.position.y);
            direction.Normalize();
            gameObject.GetComponent<Rigidbody2D>().velocity = direction * moveSpeed;
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator Despawn(bool voluntary)
    {
        StartCoroutine(StateReset());
        Debug.Log("Now Killing Self");
        //increment the enemy stats if you go the scaling route - only if voluntary = true
        if (voluntary)
            GameObject.FindWithTag("boss").GetComponent<Bully>().HealthSacrifice((float)health / (float)maxHealth);
        Debug.Log(health + " / " + maxHealth);

        mySpawner.activeEnemies--;
        gameObject.SetActive(false);
        yield return null;
    }

    IEnumerator KillReset()
    {
        yield return new WaitForSeconds(5);
        wasKilled = false;
    }

    public void SpeedUp(float mod)
    {
        speedMod /= mod;
        backupSpeed = backupSpeed / mod;
    }

    public void SpeedDown(float mod)
    {
        speedMod *= mod;
        backupSpeed = backupSpeed * mod;
    }

    IEnumerator GradualSpeedUp()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(speedCooldown);
            if (backupSpeed < myStats.speedCap)
            {
                SpeedUp(speedIncrement);
            }
            GetComponent<Animator>().speed = backupSpeed * 0.5f;
        }
    }

    public void getParticles()
    {
        //retrieving all particles
        particles = new List<GameObject>();
        Transform temp = transform.GetChild(0);
        int childCount = temp.childCount;
        Debug.Log("child count" + childCount);
        for (int i = 0; i < childCount; i++)
        {
            particles.Add(temp.GetChild(i).gameObject);
        }
    }

    public void setParticle(GameObject particle, int type)
    {
        if (particle == null)
            Debug.Log("ERMM WHAT THE SIGMA");
        particle.GetComponent<Animator>().SetInteger("augment", type);
        /*for (int i = 0; i < augs.Length; i++)
        {
            //Debug.Log("current aug: " + augs[i]);
            if (augs[i] != particleIgnore)
                particles[i].GetComponent<Animator>().SetInteger("augment", augs[i]);
            else
                particles[i].GetComponent<Animator>().SetInteger("augment", 0);
        }*/
    }

    public GameObject nextParticle()
    {
        foreach (var particle in particles)
        {
            if (particle.GetComponent<Animator>().GetInteger("augment") == 0)
            { return particle; }
        }
        Debug.Log("no particles found?");
        return null;
    }

    //called by the pointdexter when in range
    public void PointBuff()
    {
        StartCoroutine(Buffed());
    }

    IEnumerator Buffed()
    {
        //boost speed and remove all status effects
        SpeedDown(1.25f);
        StartCoroutine(Cleanse());
        GameObject part = nextParticle();
        setParticle(part, 6);


        float timer = 0f;
        while (timer < 3f)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (!gameObject.activeSelf)
            {
                break;
            }
        }
        SpeedUp(1.25f);
        setParticle(part, 0);
    }
}
