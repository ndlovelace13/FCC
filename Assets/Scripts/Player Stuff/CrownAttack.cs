using System;
using System.Collections;
using System.Collections.Generic;
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
    int augment1;
    int augment2;
    int augment3;

    string projType;
    public bool crownActive = false;
    public bool crownArmed = false;

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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "enemy" && crownArmed)
        {
            CrownAttacking();
        }
    }

    public void SetProjStats(float r, int d, string type, int aug1, int aug2, int aug3)
    {
        range = r;
        damage = d;
        projType = type;
        augment1 = aug1;
        augment2 = aug2;
        augment3 = aug3;
    }

    public void CrownActive()
    {
        crownActive = true;
    }

    public void CrownArmed()
    {
        crownArmed = true;
        StartCoroutine(Detonate());
    }

    IEnumerator Detonate()
    {
        yield return new WaitForSeconds(1);
        CrownAttacking();
    }

    public void CrownAttacking()
    {
        //Debug.Log("why");
        GameObject proj;
        float angle;
        Debug.Log(projType);
        lastCrown = crown.transform;
        switch (projType)
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

        }
        Invoke("CrownDestroy", 0);
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
        proj.GetComponent<ProjectileBehavior>().SetProps(range, damage, augment1, augment2, augment3, projDir);
        proj.GetComponent<Rigidbody2D>().velocity = projDir * speed;
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
        //set the params based on the flower being thrown
        range = flower.GetComponent<FlowerStats>().range;
        damage = flower.GetComponent<FlowerStats>().damage;
        string type = flower.GetComponent<FlowerStats>().type;
        augment1 = player.GetComponent<CrownConstruction>().augmentCheck(type);
        Debug.Log("Type: " + type + " Augment: " + augment1);
        augment2 = 0;
        augment3 = 0;
        singleFire = true;
        StartCoroutine(ProjSpawn(angleRadians));
        //get that shit outta here
        Destroy(flower);
    }

    void CrownDestroy()
    {
        if (GameControl.PlayerData.tutorialState == 6)
            GameControl.PlayerData.crownThrown = true;
        if (GameControl.PlayerData.tutorialState == 7 && augment1 == 0)
            GameControl.PlayerData.fireReset = true;
        Destroy(crown);
    }
}
