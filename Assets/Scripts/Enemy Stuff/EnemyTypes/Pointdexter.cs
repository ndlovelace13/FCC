using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pointdexter : EnemyBehavior
{
    public bool isTargeting = true;
    public bool isRetreating = false;
    public bool isCharging = false;
    public bool isPointing = false;

    float currentTime = 0f;

    GameObject ally;
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
            //SPEED UP MAY NEED REWORK
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
                if (target == crown)
                    StartCoroutine(Surprised(surpriseTime));
                target = player;
            }
            if (!isFrozen && !surprised)
                moveSpeed = backupSpeed;
            //movement
            if (!isBlinded)
            {
                
                //move away from the target
                if (isRetreating)
                {
                    Vector2 direction = new Vector2(target.position.x - shadow.position.x, target.position.y - shadow.position.y);
                    direction.Normalize();
                    gameObject.GetComponent<Rigidbody2D>().velocity = -direction * moveSpeed;
                }
                //move towards the ally
                else if ((isTargeting || isCharging) && ally != null)
                {
                    Vector2 direction = new Vector2(ally.transform.position.x - shadow.position.x, ally.transform.position.y - shadow.position.y);
                    direction.Normalize();
                    gameObject.GetComponent<Rigidbody2D>().velocity = direction * moveSpeed;
                }
            }
            else
                Debug.Log("Enemy isn't currently tracking");
            if (health <= 0)
            {
                Deactivate();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public override IEnumerator StateUpdate()
    {
        PointdexterStats stats = (PointdexterStats)myStats;
        float allyModifier = 1f;

        while (gameObject.activeSelf)
        {
            //target acquisition
            if (isTargeting)
            {
                Debug.Log("currently Targeting");
                float currentAllyDist = 0;
                if (ally)
                    currentAllyDist = Vector2.Distance(ally.GetComponent<EnemyBehavior>().shadow.position, shadow.position);
                //get an ally if one does not exist or was killed
                if (ally == null || !ally.activeSelf || currentAllyDist > 10)
                {
                    allyModifier = AllyAcquire(allyModifier, true);
                }
                    

                //check for retreat
                if (Vector2.Distance(player.transform.position, shadow.position) < 3)
                {
                    isTargeting = false;
                    isRetreating = true;
                    currentTime = 0f;
                    SpeedDown(allyModifier);
                    SpeedDown(1.15f);
                }
                else if (currentAllyDist < 1)
                {
                    isTargeting = false;
                    isCharging = true;
                    //return to base speed
                    SpeedDown(allyModifier);
                    //match ally speed
                    allyModifier = backupSpeed / ally.GetComponent<EnemyBehavior>().moveSpeed;
                    SpeedUp(allyModifier);
                    Debug.Log("current speed: " + backupSpeed + " allySpeed: " + ally.GetComponent<EnemyBehavior>().moveSpeed + " modifier: " + allyModifier);
                    currentTime = 0f;
                }
            }
            //start pointing anim while in range of enemies
            if (isCharging)
            {
                Debug.Log("currently Charging");
                //transition to the pointing
                if (currentTime >= stats.pointCharge)
                {
                    isPointing = true;
                    isCharging = false;
                    //do the pointing shit here
                    StartCoroutine(PointBuff(stats.buffRange));
                    currentTime = 0f;
                    //return to base
                    SpeedDown(allyModifier);
                    //slow down during point
                    SpeedDown(0.75f);
                }
                
            }
            //pointing (buffing)
            if (isPointing)
            {
                Debug.Log("currently Pointing");
                if (currentTime >= stats.pointCooldown)
                {
                    SpeedUp(0.75f);
                    if (Vector2.Distance(player.transform.position, shadow.position) < 3)
                    {
                        isTargeting = false;
                        isPointing = false;
                        isRetreating = true;
                        SpeedDown(1.15f);
                    }
                    else
                    {
                        isPointing = false;
                        isTargeting = true;
                        //new ally and new speed
                        allyModifier = AllyAcquire(allyModifier, false);
                        //set the movement to 1.25x
                        //SpeedUp(1.25f);
                    }
                    currentTime = 0f;
                }
            }
            if (isRetreating)
            {
                Debug.Log("currently Retreating");
                if (currentTime >= stats.retreatTime)
                {
                    isTargeting = true;
                    isRetreating = false;
                    currentTime = 0f;
                    SpeedUp(1.15f);
                    //new ally and new speed
                    allyModifier = AllyAcquire(allyModifier, false);
                    //set the movement to 1.25x
                    //SpeedUp(1.25f);
                }
            }

            //add to the timer
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }


    //get a random ally anytime the targeting state is active
    private float AllyAcquire(float allyModifier, bool speedDown)
    {
        if (speedDown)
            SpeedDown(allyModifier);
        Debug.Log("new ally acquired");
        //retrieve all enemies
        GameObject[] enemies = AllEnemies();
        //choose a random enemy from the pile - Could change to take debuffs into effect here
        int choice = Random.Range(0, enemies.Length);
        ally = enemies[choice];

        allyModifier = backupSpeed / (ally.GetComponent<EnemyBehavior>().moveSpeed * 1.5f);
        SpeedUp(allyModifier);
        return allyModifier;
    }

    IEnumerator PointBuff(float range)
    {
        GameObject[] enemies = AllEnemies();
        List<EnemyBehavior> buffedEnemies = new List<EnemyBehavior>();
        foreach (GameObject enemy in enemies)
        {
            Debug.Log("Distance to enemy: " + Vector2.Distance(enemy.GetComponent<EnemyBehavior>().shadow.position, shadow.position));
            if (Vector2.Distance(enemy.GetComponent<EnemyBehavior>().shadow.position, shadow.position) < range)
            {
                buffedEnemies.Add(enemy.GetComponent<EnemyBehavior>());
            }
        }

        Debug.Log(buffedEnemies.Count + " enemies buffed");
        //actually call a buff coroutine here
        foreach (var enemy in buffedEnemies)
        {
            enemy.PointBuff();
        }
        yield return null;
    }

    private GameObject[] AllEnemies()
    {
        //retrieve all active enemies
        EnemyBehavior[] enemyBehaviors = UnityEngine.Object.FindObjectsOfType<EnemyBehavior>();
        GameObject[] enemies = new GameObject[enemyBehaviors.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = enemyBehaviors[i].gameObject;
        }
        enemies = enemies.Where(child => child.name != gameObject.name).ToArray();
        return enemies;
    }

    public override IEnumerator StateReset()
    {
        isCharging = false;
        isTargeting = true;
        isRetreating = false;
        isPointing = false;
        currentTime = 0f;
        /*Vector2 direction = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y);
        direction.Normalize();
        gameObject.GetComponent<Rigidbody2D>().velocity = direction * backupSpeed;
        GetComponent<Animator>().SetInteger("SprinterState", 0);*/
        return base.StateReset();
    }
}
