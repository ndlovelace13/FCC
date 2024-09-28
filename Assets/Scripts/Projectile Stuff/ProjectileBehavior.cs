using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

//not used
public class Augment
{
    string type;
    int power;

    public Augment(string type)
    {
        this.type = type;
    }

    public string GetAugType()
    {
        return type;
    }

    public int GetAugPower()
    {
        return power;
    }

    public void SetAugPower(int newPower)
    {
        power = newPower;
    }

}


public class ProjectileBehavior : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float range;
    [SerializeField] public int damage;
    //[SerializeField] int augment1;
    //[SerializeField] int augment2;
    //[SerializeField] int augment3;
    int tier = 1;
    //string[] augs = new string[3];
    Vector3 startingPos = Vector3.zero;
    public Dictionary<string, int> actualAugs = new Dictionary<string, int>();
    public bool single = false;
    public bool enemyProj = false;
    


    bool arrived = false;

    public bool miniDandy = false;

    //Sprites
    GameObject spriteTrans;

    //Particles
    List<GameObject> particles;

    //repellent stuff
    public bool repellent = false;
    [SerializeField] Sprite repellentProj;

    // Start is called before the first frame update
    void Start()
    {
        //projPool = GameObject.FindWithTag("projectilePool");
        //spriteTrans = transform.GetChild(0).gameObject;
        //Debug.Log(spriteTrans.name);
    }

    public void SetProps(float r, int d, Dictionary<string, int> augsPass, Vector2 rotation, bool singleFire, bool enemyProj)
    {
        if (particles == null)
            getParticles();
        //else
            //ResetAugs();
        range = r;
        damage = d;
        //augs = new string[3];
        //augs[0] = aug1;
        //augs[1] = aug2;
        //augs[2] = aug3;
        actualAugs = augsPass;
        single = singleFire;
        this.enemyProj = enemyProj;
        foreach (var aug in actualAugs)
            Debug.Log("Augment " + aug.Key + " " + aug.Value + " made it all the way to the finish line");
        //this.tier = tier;
        //Debug.Log("Augments at start: " + augs[0] + " " + augs[1] + " " + augs[2]);
        Augmentation();
        spriteTrans = transform.GetChild(0).gameObject;
        //TO DO - TRANSITION THIS TO CHECK IN THE Sprite Apply thing
        SpriteApply();
        if (!enemyProj)
            ProjBegin();
        spriteTrans.transform.rotation = Quaternion.LookRotation(Vector3.forward, rotation);
    }

    public void RepellentSpawn(float r, Vector2 rotation)
    {
        repellent = true;
        if (particles == null)
            getParticles();
        range = r;
        spriteTrans = transform.GetChild(0).gameObject;
        SpriteApply();
        spriteTrans.transform.rotation = Quaternion.LookRotation(Vector3.forward, rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf && startingPos == Vector3.zero)
        {
            startingPos = transform.position;
            Debug.Log("starting pos:" + startingPos);
        }
        if (Mathf.Abs(Vector3.Distance(transform.position, startingPos)) > range && !arrived)
        {

            arrived = true;
            if (!enemyProj)
                ProjArrival();
        }
        /*if (gameObject.activeSelf)
        {
            transform.up = GetComponent<Rigidbody>().velocity;
        }*/
    }

    private void ProjBegin()
    {
        /*for (int i = 0; i < augs.Length; i++)
        {
            if (augs[i] != null && augs[i] != "")
            {
                Debug.Log("calling for " + augs[i]);
                GameControl.PlayerData.flowerStatsDict[augs[i]].OnProjTravel(gameObject);
            }
        }*/
        foreach (var aug in actualAugs)
        {
            GameControl.PlayerData.flowerStatsDict[aug.Key].OnProjTravel(gameObject, aug.Value);
        }
    }

    public int GetTier()
    {
        return tier;
    }

    private void ProjArrival()
    {
        //Debug.Log("Current Augs: " + augs[0] + " " + augs[1] + " " + augs[2]);
        /*for (int i = 0; i < augs.Length; i++)
        {
            if (augs[i] != null && augs[i] != "")
            {
                Debug.Log("calling for " + augs[i]);
                GameControl.PlayerData.flowerStatsDict[augs[i]].OnProjArrival(gameObject);
            }
        }*/
        foreach (var aug in actualAugs)
        {
            GameControl.PlayerData.flowerStatsDict[aug.Key].OnProjArrival(gameObject, aug.Value);
        }
        ObjectDeactivate();
    }

    //This is where to make the augment objects
    private void Augmentation()
    {
        /*foreach (var aug in augs)
        {
            if (aug != null && aug != "")
            {
                if (actualAugs.ContainsKey(aug))
                    actualAugs[aug]++;
                else
                    actualAugs.Add(aug, 1);
            }
        }*/
        //change this eventually to take in the augments with power level instead of just types
        setParticles();
    }

    /*public string[] getAugments()
    {
        //int[] allAugs = { augment1, augment2, augment3 };
        return augs;
    }*/

    public Dictionary<string, int> getActualAugs()
    {
        return actualAugs;
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

    public void setParticles()
    {
        /*for (int i = 0; i < augs.Length; i++)
        {
            //Debug.Log("current aug: " + augs[i]);
            if (augs[i] != "" && augs[i] != null)
            {
                FlowerStats currentStats = GameControl.PlayerData.flowerStatsDict[augs[i]];
                particles[i].GetComponent<Animator>().SetInteger("augment", currentStats.aug);
            }
            else
            {
                particles[i].GetComponent<Animator>().SetInteger("augment", 0);
            }
        }*/
        if (!enemyProj)
        {
            for (int i = 0; i < actualAugs.Count; i++)
            {
                FlowerStats currentStats = GameControl.PlayerData.flowerStatsDict[actualAugs.ElementAt(i).Key];
                particles[i].GetComponent<Animator>().SetInteger("augment", currentStats.aug);
            }
        }
        else
        {
            int counter = 0;
            foreach (var aug in actualAugs)
            {
                particles[counter].GetComponent<Animator>().SetInteger("augment", aug.Value);
                counter++;
            }
        }
        
    }

    public void ResetParticles()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].GetComponent<Animator>().SetInteger("augment", 0);
        }
    }

    private void SpriteApply()
    {
        //apply sprites instead here
        string type = string.Empty;
        if (actualAugs.Count > 0)
            type = actualAugs.First().Key;
        Sprite currentSprite;
        //apply if the proj is from an enemy
        if (enemyProj)
        {
            EnemyStats stats = GameControl.PlayerData.enemyStatsDict[type];
            currentSprite = stats.projSprite;
        }
        //apply if the proj is from a flower
        else if (!repellent)
        {
            if (miniDandy)
            {
                DandyStats dandy = (DandyStats)GameControl.PlayerData.flowerStatsDict["dandy"];
                if (type != "dandy")
                    currentSprite = dandy.miniProjSprite;
                else
                    currentSprite = dandy.projSprite;
            }
            else
            {
                FlowerStats stats = GameControl.PlayerData.flowerStatsDict[type];
                currentSprite = stats.projSprite;
            }

            miniDandy = false;
        }
        else //TODO - replace this with the repellent sprite eventually
            currentSprite = repellentProj;
        spriteTrans.GetComponent<SpriteRenderer>().sprite = currentSprite;
    }

    public void ObjectDeactivate()
    {
        ResetAugs();
        startingPos = Vector3.zero;
        gameObject.SetActive(false);
        Debug.Log("reached");
    }

    private void ResetAugs()
    {
        //augs = new string[3];
        arrived = false;
        enemyProj = false;
        repellent = false;
        ResetParticles();
        transform.localScale = Vector3.one;
    }


}

    
