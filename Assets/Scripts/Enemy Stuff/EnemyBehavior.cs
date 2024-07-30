using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public abstract class EnemyBehavior : MonoBehaviour
{
    [SerializeField] public Transform shadow;

    public string type;
    protected int health;
    TMP_Text scoreNotif;
    [SerializeField] int maxHealth;

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
    [SerializeField] protected float backupSpeed;
    float speedCooldown = 10f;
    float speedIncrement = 0.9f;


    public bool surprised = false;
    protected float surpriseTime = 1f;
    public Vector3 preSurpriseVel;

    [SerializeField] GameObject notif;

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

    public bool wasKilled = false;

    SpriteRenderer[] allSprites;

    //Particles
    List<GameObject> particles;

    //Seed Stuff
    GameObject seedPool;

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
        /*if (isActive)
        {
            
            
            
        }*/
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
        yield return null;
    }

    IEnumerator SortingAdjust()
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

    public void CollisionCheck(Collider2D other)
    {
        //Debug.Log("collision happening");
        if (other.gameObject.tag == "projectile")
        {
            GameObject otherParent = other.gameObject.transform.parent.gameObject;
            DealDamage(otherParent.GetComponent<ProjectileBehavior>().damage, Color.white);
            actualAugs = otherParent.GetComponent<ProjectileBehavior>().getActualAugs();
            AugmentApplication(actualAugs);
            otherParent.GetComponent<ProjectileBehavior>().ObjectDeactivate();
        }
        else if (other.gameObject.tag == "aoe")
        {
            GameObject otherParent = other.gameObject.transform.parent.gameObject;
            actualAugs = otherParent.GetComponent<AoeBehavior>().getActualAugs();
            AugmentApplication(actualAugs);
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

    public void DealDamage(int damage, Color color)
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

    public void Activate()
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
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<CapsuleCollider2D>().transform;
        target = player;
        //Debug.Log(player.position);
        StartCoroutine(SortingAdjust());
        StartCoroutine(StandardBehavior());
        StartCoroutine(StateUpdate());
    }

    protected void Deactivate()
    {
        StartCoroutine(StateReset());
        AkSoundEngine.PostEvent("EnemyKilled", gameObject);
        scoreNotif.GetComponent<ScoreNotification>().newFeed("Enemy Defeated | ", mySpawner.killScore);
        GameControl.PlayerData.enemyScore += mySpawner.killScore;
        GameControl.PlayerData.shiftEnemies++;

        //increment the total defeatedCount & shift defeated
        GameControl.PlayerData.savedEnemyDict[type].defeatedCount++;
        GameControl.PlayerData.enemyKills[type]++;

        //int currentScore = PlayerPrefs.GetInt("totalScore");
        //PlayerPrefs.SetInt("totalScore", currentScore + destructScore);
        GameControl.PlayerData.score += mySpawner.killScore;
        if (Random.Range(0f, 1f) < GameControl.PlayerData.seedChance)
        {
            GameObject newSeed = seedPool.GetComponent<ObjectPool>().GetPooledObject();
            newSeed.SetActive(true);
            newSeed.transform.localPosition = transform.localPosition;
        }
        mySpawner.activeEnemies--;
        gameObject.SetActive(false);
    }

    IEnumerator KillReset()
    {
        yield return new WaitForSeconds(5);
        wasKilled = false;
    }

    public void SpeedUp(float mod)
    {
        backupSpeed = backupSpeed / mod;
    }

    public void SpeedDown(float mod)
    {
        backupSpeed = backupSpeed * mod;
    }

    IEnumerator GradualSpeedUp()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(speedCooldown);
            if (backupSpeed < GameControl.PlayerData.playerSpeed)
            {
                if (backupSpeed < GameControl.PlayerData.playerSpeed - 2f)
                    SpeedUp(speedIncrement);
                else
                    SpeedUp(speedIncrement + 0.05f);
                GetComponent<Animator>().speed = backupSpeed * 0.5f;
            }
        }
    }

    protected void getParticles()
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
