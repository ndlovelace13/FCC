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

            //boss summon check
            if (GameControl.PlayerData.bossSpawning && summoning == false)
            {
                StartCoroutine(BossSummoning());
                summoning = true;
            }
            if (sacrifice)
            {
                StartCoroutine(Sacrifice());
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public override IEnumerator StateUpdate()
    {
        PointdexterStats stats = (PointdexterStats)myStats;
        float allyModifier = 1f;
        float currentAllyDist = 0f;

        while (gameObject.activeSelf && !sacrifice)
        {
            //target acquisition
            if (isTargeting)
            {
                //Set Anim State
                GetComponent<Animator>().SetInteger("state", 0);
                GetComponent<Animator>().speed = backupSpeed * 0.5f;

                Debug.Log("currently Targeting");
                //get an ally if one does not exist or was killed
                if (ally == null || !ally.activeSelf || currentAllyDist > 15)
                {
                    allyModifier = AllyAcquire(allyModifier, true);
                }
                if (ally)
                    currentAllyDist = Vector2.Distance(ally.GetComponent<EnemyBehavior>().shadow.position, shadow.position);


                //check for retreat
                if (Vector2.Distance(player.transform.position, shadow.position) < 3)
                {
                    isTargeting = false;
                    isRetreating = true;
                    currentTime = 0f;
                    SpeedDown(allyModifier);
                    SpeedDown(1.15f);
                }
                else if (ally && currentAllyDist < 2f)
                {
                    isTargeting = false;
                    isCharging = true;
                    GetComponent<Animator>().SetInteger("state", 1);
                    GetComponent<Animator>().speed = 1;
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
                //Set Anim State
                GetComponent<Animator>().SetInteger("state", 1);
                GetComponent<Animator>().speed = 1;

                Debug.Log(currentTime);
                //transition to the pointing
                if (currentTime >= stats.pointCharge)
                {
                    GetComponent<Animator>().SetInteger("state", 2);
                    GetComponent<Animator>().speed = 1;
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
                //Set Anim State
                GetComponent<Animator>().SetInteger("state", 2);
                GetComponent<Animator>().speed = 1;
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
                //Set Anim State
                GetComponent<Animator>().SetInteger("state", 3);
                GetComponent<Animator>().speed = backupSpeed * 0.5f;
                //Debug.Log("currently Retreating");
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
            if (!isFrozen && !surprised)
                currentTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

    }


    //get a random ally anytime the targeting state is active
    private float AllyAcquire(float allyModifier, bool speedDown)
    {
        if (speedDown && allyModifier != 0)
        {
            Debug.Log("previous ally modifier: " + allyModifier);
            SpeedDown(allyModifier);
        }
            
        Debug.Log("new ally acquired");
        //retrieve all enemies
        GameObject[] enemies = AllEnemies();
        //choose a random enemy from the pile - Could change to take debuffs into effect here
        int choice = Random.Range(0, enemies.Length);
        ally = enemies[choice];

        allyModifier = backupSpeed / (ally.GetComponent<EnemyBehavior>().backupSpeed * 1.5f);
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
        GetComponent<Animator>().SetInteger("state", 0);
        return base.StateReset();
    }
}
