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

    public override void OnProjArrival(GameObject proj, int power)
    {
        base.OnProjArrival(proj, power);
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
        //string[] augs = original.GetComponent<ProjectileBehavior>().getAugments();
        int tier = original.GetComponent<ProjectileBehavior>().GetTier();
        Dictionary<string, int> actualAugs = new Dictionary<string, int>(original.GetComponent<ProjectileBehavior>().getActualAugs());
        if (actualAugs.ContainsKey("dandy"))
        {
            if (actualAugs["dandy"] == 1)
                actualAugs.Remove("dandy");
            else
                actualAugs["dandy"]--;
        }
        float speed = original.GetComponent<Rigidbody2D>().velocity.magnitude;
        proj.GetComponent<ProjectileBehavior>().SetProps(range / 2f, damage / splitCount, actualAugs, projDir);
        proj.GetComponent<Rigidbody2D>().velocity = projDir * speed;
        yield return null;
    }
}
