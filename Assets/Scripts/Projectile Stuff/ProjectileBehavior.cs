using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public enum augment
{
    NULL,
    FIRE,
    ICE,
    POISON,
    ELECTRIC
}


public class ProjectileBehavior : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float range;
    [SerializeField] public int damage;
    [SerializeField] int augment1;
    [SerializeField] int augment2;
    [SerializeField] int augment3;
    Vector3 startingPos = Vector3.zero;

    bool fire;
    [SerializeField] GameObject flamesPool;
    float flameTime = 2f;

    bool ice;

    bool poison;
    [SerializeField] GameObject poisonPool;
    float poisonCooldown = 0.5f;
    float poisonTime = 1f;

    bool electric;

    //Sprites
    GameObject spriteTrans;

    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite fireSprite;
    [SerializeField] Sprite iceSprite;
    [SerializeField] Sprite poisonSprite;
    [SerializeField] Sprite electricSprite;

    //Particles
    List<GameObject> particles;

    // Start is called before the first frame update
    void Start()
    {
        //spriteTrans = transform.GetChild(0).gameObject;
        //Debug.Log(spriteTrans.name);
    }

    public void SetProps(float r, int d, int aug1, int aug2, int aug3, Vector2 rotation)
    {
        if (particles == null)
            getParticles();
        else
            ResetAugs();
        range = r;
        damage = d;
        augment1 = aug1;
        augment2 = aug2;
        augment3 = aug3;
        Debug.Log(aug1 + " " + aug2  + " " + aug3);
        Augmentation();
        spriteTrans = transform.GetChild(0).gameObject;
        SpriteApply(aug1);
        if (poison)
        {
            StartCoroutine(PoisonSpawn());
        }
        spriteTrans.transform.rotation = Quaternion.LookRotation(Vector3.forward, rotation);
        //GetComponent<Rigidbody2D>().velocity =  * speed;
        //Debug.Log("velocity be like : " + spriteTrans.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf && startingPos == Vector3.zero)
        {
            startingPos = transform.position;
            Debug.Log("starting pos:" + startingPos);
        }
        if (Mathf.Abs(Vector3.Distance(transform.position, startingPos)) > range)
        {
            if (fire)
                FlamesSpawn();
            ObjectDeactivate();
        }
        /*if (gameObject.activeSelf)
        {
            transform.up = GetComponent<Rigidbody>().velocity;
        }*/
    }

    private void Augmentation()
    {
        //apply augmentation effects that don't concern enemy collision here
        int[] augs = getAugments();
        setParticles(augs);
        foreach (int aug in augs)
        {
            switch (aug)
            {
                case 1: fire = true; flamesPool = GameObject.FindGameObjectWithTag("flamePool"); break;
                case 2: ice = true; break;
                case 3: poison = true; poisonPool = GameObject.FindGameObjectWithTag("poisonPool"); break;
                case 4: electric = true; break;
                default: break;
            }
        }
    }

    public int[] getAugments()
    {
        int[] augs = { augment1, augment2, augment3 };
        return augs;
    }

    private void getParticles()
    {
        //retrieving all particles
        particles = new List<GameObject>();
        Transform temp = transform.GetChild(1);
        int childCount = temp.childCount;
        Debug.Log("child count" + childCount);
        for (int i = 0; i < childCount; i++)
        {
            particles.Add(temp.GetChild(i).gameObject);
        }
    }

    public void setParticles(int[] augs)
    {
        for (int i = 0; i < augs.Length; i++)
        {
            //Debug.Log("current aug: " + augs[i]);
            particles[i].GetComponent<Animator>().SetInteger("augment", augs[i]);
        }
    }

    private void SpriteApply(int type)
    {
        //apply sprites instead here
        Sprite currentSprite;
        switch (type)
        {
            case 1: currentSprite = fireSprite; break;
            case 2: currentSprite = iceSprite; break;
            case 3: currentSprite = poisonSprite; break;
            case 4: currentSprite = electricSprite; break;
            default: currentSprite = defaultSprite; break;
        }
        spriteTrans.GetComponent<SpriteRenderer>().sprite = currentSprite;
    }

    public void FlamesSpawn()
    {
        GameObject newFlame = flamesPool.GetComponent<ObjectPool>().GetPooledObject();
        newFlame.SetActive(true);
        newFlame.transform.position = transform.position;
        newFlame.GetComponent<AoeBehavior>().Activate(getAugments(), flameTime, 1);
    }

    IEnumerator PoisonSpawn()
    {
        while (poison)
        {
            yield return new WaitForSeconds(poisonCooldown);
            GameObject newCloud = poisonPool.GetComponent<ObjectPool>().GetPooledObject();
            newCloud.SetActive(true);
            newCloud.transform.position = transform.position;
            newCloud.GetComponent<AoeBehavior>().Activate(getAugments(), poisonTime, 3);
        }
    }

    public void ObjectDeactivate()
    {
        ResetAugs();
        startingPos = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void ResetAugs()
    {
        augment1 = 0;
        augment2 = 0;
        augment3 = 0;
        fire = false;
        ice = false;
        poison = false;
        electric = false;
        setParticles(getAugments());
    }    
}

    
