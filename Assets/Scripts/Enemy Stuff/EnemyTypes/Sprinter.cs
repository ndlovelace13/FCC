using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprinter : EnemyBehavior
{
    bool isCharging = false;
    bool isSprinting = false;
    bool isResting = true;

    bool justReset = false;
    float resetCooldown = 1f;

    float currentTimer = 0f;

    //float walkSpeed;
    // Start is called before the first frame update
    /*void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

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
            if (!isBlinded && !isSprinting)
            {
                //Debug.Log(target.position);
                Vector2 direction = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y);
                direction.Normalize();
                rb2D.MovePosition(rb2D.position + direction * moveSpeed * Time.deltaTime);
            }
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
        surpriseTime = 0.5f;
        SprinterStats realStats = (SprinterStats)myStats;
        //walkSpeed = backupSpeed;
        while (gameObject.activeSelf && !sacrifice)
        {
            if ((isFrozen || isElectrified) && !justReset)
            {
                Debug.Log("RESET");
                if (isCharging)
                    SpeedUp(0.5f);
                //reset the cycle
                isResting = true;
                isSprinting = false;
                isCharging = false;
                currentTimer = 0f;
                justReset = true;
                GetComponent<Animator>().SetInteger("SprinterState", 0);
                StartCoroutine(StunReset());
            }
            if (isResting)
            {
                if (currentTimer >= realStats.sprintCooldown)
                {
                    currentTimer = 0f;
                    isCharging = true;
                    isResting = false;
                    //slow down and start to set the sprintDir
                    SpeedDown(0.5f);
                    Debug.Log("NOW CHARGING");
                    GetComponent<Animator>().SetInteger("SprinterState", 1);
                }
            }
            else if (isCharging)
            {
                if (currentTimer >= realStats.chargeTime)
                {
                    //set the sprintDirection
                    isCharging = false;
                    isSprinting = true;
                    currentTimer = 0f;

                    //begin the sprint
                    //SpeedUp(realStats.sprintSpeed);
                    Vector2 direction = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y);
                    direction.Normalize();
                    rb2D.MovePosition(rb2D.position + direction * moveSpeed * realStats.sprintSpeed * Time.deltaTime);
                    //gameObject.GetComponent<Rigidbody2D>().velocity = direction * backupSpeed * realStats.sprintSpeed;
                    Debug.Log("NOW SPRINTING");
                    GetComponent<Animator>().SetInteger("SprinterState", 2);
                }
            }
            else if (isSprinting)
            {
                if (currentTimer >= realStats.sprintLength)
                {
                    isSprinting = false;
                    isResting = true;
                    currentTimer = 0f;

                    //set the moveSpeed to the normal walkSpeed
                    SpeedUp(0.5f);
                    Debug.Log("NOW RESTING");
                    GetComponent<Animator>().SetInteger("SprinterState", 0);
                }

            }
            if (!surprised)
            {
                currentTimer += Time.deltaTime;
                StartCoroutine(DirectionHandle());
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DirectionHandle()
    {
        Vector2 direction = GetComponent<Rigidbody2D>().velocity;
        if (direction.x > 0f)
            transform.localScale = new Vector3(-1, 1);
        else
            transform.localScale = Vector3.one;
        yield return null;
    }

    IEnumerator StunReset()
    {
        yield return new WaitForSeconds(resetCooldown);
        justReset = false;
    }

    public override IEnumerator StateReset()
    {
        if (isCharging)
            SpeedUp(0.5f);
        isCharging = false;
        isSprinting = false;
        isResting = true;
        currentTimer = 0f;
        justReset = false;
        Vector2 direction = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y);
        direction.Normalize();
        rb2D.MovePosition(rb2D.position + direction * moveSpeed * Time.deltaTime);
        //gameObject.GetComponent<Rigidbody2D>().velocity = direction * backupSpeed;
        GetComponent<Animator>().SetInteger("SprinterState", 0);
        return base.StateReset();
    }
}
