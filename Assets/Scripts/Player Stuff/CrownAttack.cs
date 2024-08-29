using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrownAttack : MonoBehaviour
{
    GameObject projPool;
    GameObject crown;
    Transform lastCrown;
    GameObject player;

    [SerializeField] float speed = 5f;
    float range;
    int damage;
    //string augment1;
    //string augment2;
    //string augment3;
    Dictionary<string, int> actualAugs;
    int projCount;
    int tier;

    string projType;
    public bool crownActive = false;
    public bool crownArmed = false;

    public bool attacking = false;

    //single fire bool
    bool singleFire = false;
    // Start is called before the first frame update
    void Start()
    {
        projPool = GameObject.FindGameObjectWithTag("projectilePool");
        player = GameObject.FindGameObjectWithTag("Player");
        //player = player.transform.GetChild(2).gameObject;
        crown = gameObject;
        Debug.Log(projPool.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CollisionCheck(Collider2D collision)
    {
        if (collision.gameObject.tag == "enemy" && crownArmed)
        {
            Debug.Log("triggered from collision");
            CrownAttacking();
        }
    }

    public void SetProjStats(float r, int d, string type, Dictionary<string, int> actualAugs, int numProjs, int tier)
    {
        range = r;
        damage = d;
        projType = type;
        //augment1 = aug1;
        //augment2 = aug2;
        //augment3 = aug3;
        this.actualAugs = actualAugs;
        projCount = numProjs;
        this.tier = tier;
    }

    public void CrownActive()
    {
        crownActive = true;
        SpriteRenderer[] children = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer child in children)
        {
            if (child.gameObject.tag == "shadow")
                child.enabled = true;
        }
    }

    public void CrownArmed()
    {
        crownArmed = true;
        GetComponent<SpriteRenderer>().sortingOrder = -1;
        SpriteRenderer[] flowers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer flower in flowers)
        {
            if (flower.gameObject.tag != "finalCrown" && flower.gameObject.tag != "shadow")
                flower.sortingOrder = 0;
        }
        StartCoroutine(Detonate());
    }

    IEnumerator Detonate()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("triggered from detonation");
        CrownAttacking();
    }

    public void CrownAttacking()
    {
        if (!attacking)
        {
            attacking = true;
            StopAllCoroutines();
            //Debug.Log("why");
            float angle;
            Debug.Log(projType);
            lastCrown = crown.transform;
            angle = 0f;
            float difference = Mathf.PI * 2 / projCount;
            for (int i = 0; i < projCount; i++)
            {
                StartCoroutine(ProjSpawn(angle));
                angle += difference;
            }
            Invoke("CrownDestroy", 0);
        }
        else
            Debug.Log("dupe attack cancelled");
        
        /*switch (projType)
        {
            case "white":
                angle = 0f;
                for (int i = 0; i < 4; i++)
                {
                    StartCoroutine(ProjSpawn(angle));
                    angle += Mathf.PI / 2f;
                }
                break;
            case "pink":
                angle = Mathf. PI / 4f;
                for (int i = 0; i < 4; i++)
                {
                    StartCoroutine(ProjSpawn(angle));
                    angle += Mathf.PI / 2f;
                }
                break;
            case "orange":
                angle = Mathf.PI / 3f;
                for (int i = 0; i < 4; i++)
                {
                    StartCoroutine(ProjSpawn(angle));
                    if (i % 2 == 0)
                        angle += Mathf.PI / 3f;
                    else
                        angle += Mathf.PI * 2 / 3f;
                }
                break;
            case "blue":
                angle = Mathf.PI / 3f;
                for (int i = 0; i < 6; i++)
                {
                    StartCoroutine(ProjSpawn(angle));
                    angle += Mathf.PI / 3f;
                }
                break;
            case "yellow":
                angle = -Mathf.PI / 6f;
                for (int i = 0; i < 6; i++)
                {
                    StartCoroutine(ProjSpawn(angle));
                    if (i == 2)
                        angle += 2 * Mathf.PI / 3f;
                    else
                        angle += Mathf.PI / 6f;
                }
                break;
            case "green":
                angle = Mathf.PI / 3f;
                for (int i = 0; i < 6; i++)
                {
                    StartCoroutine(ProjSpawn(angle));
                    if (i == 2)
                        angle += 2 * Mathf.PI / 3f;
                    else
                        angle += Mathf.PI / 6f;
                }
                break;
            case "red":
                angle = Mathf.PI / 6f;
                for (int i = 0; i < 6; i++)
                {
                    StartCoroutine(ProjSpawn(angle));
                    angle += Mathf.PI / 3f;
                }
                break;

            default:
                proj = projPool.GetComponent<ObjectPool>().GetPooledObject();
                //do the attacking stuff here
                proj.transform.position = crown.transform.position;
                proj.SetActive(true);
                break;

        }*/

        
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
        if (singleFire)
            proj.transform.position = player.transform.position;
        else
            proj.transform.position = lastCrown.position;

        //rotating towards direction of movement
        proj.SetActive(true);
        proj.GetComponent<ProjectileBehavior>().SetProps(range, damage, actualAugs, projDir, singleFire, false);
        proj.GetComponentInChildren<Rigidbody2D>().velocity = projDir * speed;
        singleFire = false;
        yield return null;
    }

    public void SingleFire(GameObject flower)
    {
        //get the position based on the mouse direction and translate to an angle
        Vector2 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = endPos - new Vector2(player.transform.position.x, player.transform.position.y);
        //direction.Normalize();
        float angleRadians = Mathf.Atan2(-direction.y, direction.x);
        if (angleRadians < 0)
            angleRadians += 2 * Mathf.PI;
        angleRadians += Mathf.PI / 2;
        Debug.Log("radians: " + angleRadians);
        //set the params based on the flower being thrown]
        FlowerBehavior behavior = flower.GetComponent<FlowerBehavior>();

        //increment single flower throw for associated type
        GameControl.PlayerData.savedFlowerDict[behavior.type].singleCount++;

        string type;
        //Wild Check
        if (behavior.type == "wild")
        {
            WildStats wild = (WildStats)GameControl.PlayerData.flowerStatsDict["wild"];
            type = wild.WildTypeRandomize();
        }
        else
            type = behavior.type;

        range = GameControl.PlayerData.flowerStatsDict[type].GetProjRange(behavior.tier);
        damage = GameControl.PlayerData.flowerStatsDict[type].GetDamage(behavior.tier);
        //augment1 = type;
        //Debug.Log("Type: " + type + " Augment: " + augment1);
        //augment2 = "";
        //augment3 = "";
        tier = behavior.tier - 1;
        singleFire = true;
        actualAugs = new Dictionary<string, int>();
        actualAugs.Add(type, behavior.tier);
        StartCoroutine(ProjSpawn(angleRadians));
        //get that shit outta here
        Destroy(flower);
    }

    void CrownDestroy()
    {
        if (GameControl.PlayerData.tutorialState == 6)
            GameControl.PlayerData.crownThrown = true;
        if (GameControl.PlayerData.tutorialState == 7 && !actualAugs.ContainsKey("red"))
            GameControl.PlayerData.fireReset = true;
        Destroy(crown);
        attacking = false;
    }
}
