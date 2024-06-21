using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour
{
    int health;
    int destructScore;
    TMP_Text scoreNotif;
    [SerializeField] int maxHealth;

    public bool isActive;

    Transform player;
    Transform crown;
    Transform target;

    //SPEED SHIT
    public float moveSpeed;
    float minSpeed;
    float maxSpeed;
    float backupSpeed;
    bool backupUsed = false;
    float speedCooldown = 10f;
    float speedIncrement = 0.9f;
    bool speedUp;
    bool surprised = false;
    float surpriseTime = 1f;

    [SerializeField] GameObject notif;

    public string[] augments;

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

    public bool wasKilled = false;

    SpriteRenderer[] allSprites;

    //Particles
    List<GameObject> particles;

    //Seed Stuff
    float seedProb = 0.4f;
    GameObject seedPool;

    // Start is called before the first frame update
    void Start()
    {
        //destructScore = 10;
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<CapsuleCollider2D>().transform;
        target = player;
        //moveSpeed = maxSpeed;
        scoreNotif = GameObject.FindGameObjectWithTag("scoreAnnounce").GetComponent<TMP_Text>();
        //lightningPool = GameObject.FindGameObjectWithTag("lightningPool");
        seedPool = GameObject.FindGameObjectWithTag("seedPool");
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            //SPEED UP MAY NEED REWORK
            GameObject[] crowns = GameObject.FindGameObjectsWithTag("finalCrown");
            foreach (GameObject obj in crowns)
            {
                if (obj.GetComponent<CrownAttack>().crownActive == true)
                    crown = obj.transform;
            }
            if (crown != null)
            {
                TargetSwap();
            }
            else
            {
                if (target == crown)
                    StartCoroutine(Surprised(surpriseTime));
                target = player.transform;
            }
            if (!isFrozen && !surprised)
                moveSpeed = backupSpeed;
            //movement
            Vector2 direction = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y);
            direction.Normalize();
            gameObject.GetComponent<Rigidbody2D>().velocity = direction * moveSpeed;
            if (health <= 0)
            {
                Deactivate();
            }
            
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

    IEnumerator Surprised(float surpriseTime)
    {
        if (!surprised)
        {
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
        }
    }

    public void CollisionCheck(Collider2D other)
    {
        augments = new string[3];
        //Debug.Log("collision happening");
        if (other.gameObject.tag == "projectile")
        {
            GameObject otherParent = other.gameObject.transform.parent.gameObject;
            DealDamage(otherParent.GetComponent<ProjectileBehavior>().damage, Color.white);
            augments = otherParent.GetComponent<ProjectileBehavior>().getAugments();
            AugmentApplication(augments);
            otherParent.GetComponent<ProjectileBehavior>().ObjectDeactivate();
        }
        else if (other.gameObject.tag == "aoe")
        {
            GameObject otherParent = other.gameObject.transform.parent.gameObject;
            augments =  otherParent.GetComponent<AoeBehavior>().getAugments();
            AugmentApplication(augments);
        }
    }

    public void AugmentApplication(string[] augs)
    {
        foreach (string aug in augs)
        {
            if (aug != null && aug != "")
            {
                GameControl.PlayerData.flowerStatsDict[aug].OnEnemyCollision(gameObject);
            }
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
        health -= damage;
        if (health < maxHealth / 2 && GetComponentInChildren<HatBehavior>() != null)
            GetComponentInChildren<HatBehavior>().HatFall();
        GameObject newNotif = Instantiate(notif, transform.position + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)), Quaternion.identity);
        newNotif.GetComponent<DamageNotif>().Creation(damage.ToString(), color);
    }

    private void TargetSwap()
    {
        float playerDist = Vector3.Distance(player.transform.position, gameObject.transform.position);
        float crownDist = Vector3.Distance(crown.transform.position, gameObject.transform.position);
        if (playerDist > crownDist)
        {
            target = crown.transform;
        }
    }

    public void Activate()
    {
        //initialize variables
        health = GameControl.PlayerData.currentHealth;
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
        maxSpeed = GameControl.PlayerData.currentMax;
        minSpeed = GameControl.PlayerData.currentMin;
        backupSpeed = Random.Range(minSpeed, maxSpeed);
        GetComponent<Animator>().speed = backupSpeed * 0.5f;
        //begin the gradual speed up routine
        StartCoroutine(GradualSpeedUp());
        StartCoroutine(KillReset());
        allSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Deactivate()
    {
        scoreNotif.GetComponent<ScoreNotification>().newFeed("Enemy Defeated | +" + GameControl.PlayerData.killScore);
        //int currentScore = PlayerPrefs.GetInt("totalScore");
        //PlayerPrefs.SetInt("totalScore", currentScore + destructScore);
        GameControl.PlayerData.score += GameControl.PlayerData.killScore;
        isBurning = false;
        isFrozen = false;
        isSlowed = false;
        isPoisoned = false;
        isElectrified = false;
        isActive = false;
        surprised = false;
        wasKilled = true;
        if (Random.Range(0f, 1f) < GameControl.PlayerData.seedChance)
        {
            GameObject newSeed = seedPool.GetComponent<ObjectPool>().GetPooledObject();
            newSeed.SetActive(true);
            newSeed.transform.localPosition = transform.localPosition;
        }
        GameControl.PlayerData.activeEnemies--;
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

    private void getParticles()
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
}
