using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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
    float moveSpeed;
    float minSpeed;
    float maxSpeed;
    float backupSpeed;
    bool backupUsed = false;
    float speedCooldown = 10f;
    float speedIncrement = 0.925f;
    bool speedUp;
    bool surprised = false;
    float surpriseTime = 1f;

    [SerializeField] GameObject notif;

    //1 FIRE
    bool isBurning = false;
    int burnDamage = 2;
    float burnCooldown = 0.5f;
    float burnTime = 3;

    //2 ICE
    bool isFrozen = false;
    bool isSlowed = false;
    float freezeTime = 1f;
    float slowTime = 2f;
    float slowAmount = 0.5f;

    //3 POISON
    bool isPoisoned = false;
    int poisonDamage = 1;
    float poisonCooldown = 0.3f;
    float poisonTime = 4f;
    float poisonSlow = 0.7f;

    //4 ELECTRIC
    public bool isElectrified = false;
    int numTargets = 2;
    float stunAmount = 0.2f;
    float stunTime = 1f;
    int electricDamage = 5;
    bool electricPassed = false;
    [SerializeField] GameObject lightning;
    GameObject lightningPool;

    //Particles
    List<GameObject> particles;

    //Seed Stuff
    float seedProb = 0.4f;
    GameObject seedPool;

    // Start is called before the first frame update
    void Start()
    {
        destructScore = 10;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = player;
        //moveSpeed = maxSpeed;
        scoreNotif = GameObject.FindGameObjectWithTag("scoreAnnounce").GetComponent<TMP_Text>();
        lightningPool = GameObject.FindGameObjectWithTag("lightningPool");
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
        }
    }

    IEnumerator Surprised(float surpriseTime)
    {
        if (!surprised)
        {
            Debug.Log("This mf surprised");
            //GetComponent<SpriteRenderer>().color = Color.blue;
            //backupSpeed = moveSpeed;
            surprised = true;
            moveSpeed = 0f;
            yield return new WaitForSeconds(surpriseTime);
            //moveSpeed = backupSpeed;
            surprised = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("collision happening");
        if (other.gameObject.tag == "projectile")
        {
            DealDamage(other.gameObject.GetComponent<ProjectileBehavior>().damage, Color.white);
            int[] augments = other.gameObject.GetComponent<ProjectileBehavior>().getAugments();
            AugmentApplication(augments);
            other.gameObject.GetComponent<ProjectileBehavior>().ObjectDeactivate();
        }
        else if (other.gameObject.tag == "aoe")
        {
            int[] augments = other.gameObject.GetComponent<AoeBehavior>().getAugments();
            AugmentApplication(augments);
        }
    }

    private void AugmentApplication(int[] augs)
    {
        foreach (int aug in augs)
        {
            switch(aug)
            {
                case 1: StartCoroutine(FireApply()); break; //apply burn
                case 2: StartCoroutine(FreezeApply()); break; //apply freeze
                case 3: StartCoroutine(PoisonApply()); break; //apply poison
                case 4: if (!electricPassed) { StartCoroutine(ElectricApply(numTargets, augs)); } break; //apply electric
            }
        }
    }

    IEnumerator FireApply()
    {
        Debug.Log("This mf burning");
        isBurning = true;
        GetComponent<SpriteRenderer>().color = Color.red;
        StartCoroutine(BurnHandler());
        yield return null;
    }

    IEnumerator BurnHandler()
    {
        float burnTimer = 0f;
        GameObject part = nextParticle();
        setParticle(part, 1);
        while (burnTimer < burnTime)
        {
            Debug.Log("ouch " + health);
            yield return new WaitForSeconds(burnCooldown);
            DealDamage(burnDamage, Color.red);
            burnTimer += burnCooldown;
        }
        Debug.Log("Done");
        isBurning = false;
        setParticle(part, 0);
        GetComponent<SpriteRenderer>().color = Color.white;
    }
    
    IEnumerator FreezeApply()
    {
        if (!isFrozen)
        {
            Debug.Log("This mf frozen");
            GetComponent<SpriteRenderer>().color = Color.blue;
            //backupSpeed = moveSpeed;
            isFrozen = true;
            moveSpeed = 0f;
            if (!isBurning)
            {
                yield return new WaitForSeconds(freezeTime);
            }
            //moveSpeed = backupSpeed;
            isFrozen = false;
        }
        StartCoroutine(SlowApply(slowAmount, slowTime, 2));
        GetComponent<SpriteRenderer>().color = Color.cyan;
    }

    IEnumerator SlowApply(float slowEffect, float slowTime, int particle)
    {
        GameObject part = nextParticle();
        setParticle(part, particle);
        isSlowed = true;
        /*if (isFrozen)
        {
            backupSpeed = backupSpeed * slowEffect;
            backupUsed = true;
        }
        else
            moveSpeed = moveSpeed * slowEffect;*/
        SpeedDown(slowEffect);
        yield return new WaitForSeconds(slowTime);
        //this might be fucked;
        /*if (isFrozen || backupUsed)
        {
            Debug.Log("BACKUP");
            backupSpeed = backupSpeed / slowEffect;
        }
        else
            moveSpeed = moveSpeed / slowEffect;*/
        SpeedUp(slowEffect);
        isSlowed = false;
        GetComponent<SpriteRenderer>().color = Color.white;
        setParticle(part, 0);
        if (particle == 4)
            isElectrified = false;
        /*if (backupUsed)
        {
            moveSpeed = backupSpeed;
            backupUsed = false;
        }*/
    }

    IEnumerator PoisonApply()
    {
        Debug.Log("This mf poisoned");
        isPoisoned = true;
        GetComponent<SpriteRenderer>().color = Color.green;
        StartCoroutine(PoisonHandler());
        StartCoroutine(SlowApply(poisonSlow, poisonTime, 3));
        yield return null;
    }

    IEnumerator PoisonHandler()
    {
        float poisonTimer = 0f;
        while (poisonTimer < poisonTime)
        {
            Debug.Log("ouch " + health);
            yield return new WaitForSeconds(poisonCooldown);
            DealDamage(poisonDamage, Color.green);
            poisonTimer += poisonCooldown;
        }
        Debug.Log("Done");
        isPoisoned = false;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    IEnumerator ElectricApply(int remainingTargets, int[] augs)
    {
        Debug.Log("This mf electrified");
        isElectrified = true;
        GetComponent<SpriteRenderer>().color = Color.yellow;
        StartCoroutine(SlowApply(stunAmount, stunTime, 4));
        if (remainingTargets > 0)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
            GameObject closestEnemy = ClosestEnemy(enemies);
            if (closestEnemy != null)
            {
                StartCoroutine(LightningEffect(gameObject, closestEnemy));
                closestEnemy.GetComponent<EnemyBehavior>().ElectricPass(remainingTargets - 1, augs);
            }
            DealDamage(electricDamage, Color.yellow);
        }
        //isElectrified = false;
        yield return null;
    }

    IEnumerator LightningEffect(GameObject origin, GameObject target)
    {
        Vector2 length = target.transform.position - origin.transform.position;
        float angleRadians = Mathf.Atan2(length.y, -length.x);
        if (angleRadians < 0)
            angleRadians += 2 * Mathf.PI;
        //angleRadians += Mathf.PI / 2;
        Vector2 direction = new Vector2(Mathf.Sin(angleRadians), Mathf.Cos(angleRadians));
        GameObject newLightning = lightningPool.GetComponent<ObjectPool>().GetPooledObject();
        newLightning.transform.position = Vector3.Lerp(origin.transform.position, target.transform.position, 0.5f);
        newLightning.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        //GameObject newLightning = Instantiate(lightning, Vector3.Lerp(origin.transform.position, target.transform.position, 0.5f), Quaternion.LookRotation(Vector3.forward, direction));
        newLightning.transform.localScale = new Vector2(length.magnitude, newLightning.transform.localScale.y);
        newLightning.GetComponent<LightningBehavior>().Activate();
        yield return null;
        //newLightning.SetActive(false);
    }

    public void ElectricPass(int remainingTargets, int[] augs)
    {
        electricPassed = true;
        StartCoroutine(ElectricApply(remainingTargets, augs));
        AugmentApplication(augs);
        electricPassed = false;
    }

    private GameObject ClosestEnemy(GameObject[] enemies)
    {
        Debug.Log("WHATATT " + enemies.Length);
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
        //assign a random speed
        maxSpeed = GameControl.PlayerData.currentMax;
        minSpeed = GameControl.PlayerData.currentMin;
        backupSpeed = Random.Range(minSpeed, maxSpeed);
        GetComponent<Animator>().speed = backupSpeed * 0.5f;
        //begin the gradual speed up routine
        StartCoroutine(GradualSpeedUp());
    }

    private void Deactivate()
    {
        scoreNotif.GetComponent<ScoreNotification>().newFeed("Enemy Defeated | +" + destructScore);
        //int currentScore = PlayerPrefs.GetInt("totalScore");
        //PlayerPrefs.SetInt("totalScore", currentScore + destructScore);
        GameControl.PlayerData.score += destructScore;
        isBurning = false;
        isFrozen = false;
        isSlowed = false;
        isPoisoned = false;
        isElectrified = false;
        isActive = false;
        if (Random.Range(0f, 1f) < seedProb)
        {
            GameObject newSeed = seedPool.GetComponent<ObjectPool>().GetPooledObject();
            newSeed.SetActive(true);
            newSeed.transform.localPosition = transform.localPosition;
        }
        gameObject.SetActive(false);
    }

    private void SpeedUp(float mod)
    {
        backupSpeed = backupSpeed / mod;
    }

    private void SpeedDown(float mod)
    {
        backupSpeed = backupSpeed * mod;
    }

    IEnumerator GradualSpeedUp()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(speedCooldown);
            if (backupSpeed < GameControl.PlayerData.playerSpeed + 1)
            {
                SpeedUp(speedIncrement);
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

    private GameObject nextParticle()
    {
        foreach (var particle in particles)
        {
            if (particle.GetComponent<Animator>().GetInteger("augment") == 0)
            { return particle; }
        }
        return null;
    }
}
