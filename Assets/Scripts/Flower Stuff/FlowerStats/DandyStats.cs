using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DandyStats : FlowerStats
{
    public Sprite miniProjSprite;

    int splitCount = 3;
    float splitAngle = Mathf.PI / 6f;
    public bool miniDandy = false;
    GameObject ogProjCopy;
    ObjectPool projPool;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnProjArrival(GameObject proj)
    {
        base.OnProjArrival(proj);
        //find the projectile pool
        projPool = GameObject.FindWithTag("projectilePool").GetComponent<ObjectPool>();

        Vector2 vel = proj.GetComponent<Rigidbody2D>().velocity;
        float angleRadians = Mathf.Atan2(vel.x, vel.y);
        float startingAngle = angleRadians - splitAngle;
        //ogProjCopy = proj;
        for (int i = 0; i < splitCount; i++)
        {
            StartCoroutine(ProjSpawn(startingAngle, proj));
            startingAngle += splitAngle;
        }
    }

    //TO DO: ONLY SPAWNS MULTIPLES OFF THE FIRST ONE WHEN DOUBLE DANDY
    IEnumerator ProjSpawn(float angle, GameObject original)
    {
        Vector2 projDir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        projDir.Normalize();
        GameObject proj = projPool.GetComponent<ObjectPool>().GetPooledObject();
        if (proj == null)
            Debug.Log("get fucked");
        proj.transform.position = original.transform.position;

        //rotating towards direction of movement
        proj.SetActive(true);
        if (original.GetComponentInChildren<SpriteRenderer>().sprite == projSprite)
        {
            proj.GetComponent<ProjectileBehavior>().miniDandy = true;
        }
        else
        {
            proj.transform.localScale = Vector3.one / splitCount;
        }
        string[] augs = original.GetComponent<ProjectileBehavior>().getAugments();
        for (int i = 0; i < augs.Length; i++)
        {
            if (augs[i] == "dandy")
            {
                augs[i] = "";
                break;
            }
        }
        float speed = original.GetComponent<Rigidbody2D>().velocity.magnitude;
        proj.GetComponent<ProjectileBehavior>().SetProps(range / 2f, damage / splitCount, augs[0], augs[1], augs[2], projDir);
        proj.GetComponent<Rigidbody2D>().velocity = projDir * speed;
        yield return null;
    }
}
