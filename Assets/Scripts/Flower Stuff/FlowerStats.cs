using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class will store the data about each individual flower use stats for the almanac
[System.Serializable]
public class SavedFlowerStats
{
    public string key;

    public bool discovered;
    public bool encountered;

    public int singleCount;
    public int crownCount;
    public int harvestCount;

    public int highestTier;
    public int highestPower;
    public int highestHarvest;
    public int highestShift;

    //crown completion stuff - link to completion tracker
    public int possibleCrowns;
    public int discoveredCrowns;

    //title discovery - check during naming convention
    public bool primary;
    public bool inside;
    public bool outside;
    public bool four;
    public bool five;

    public SavedFlowerStats(string key)
    {
        discovered = false;
        encountered = false;

        singleCount = 0;
        crownCount = 0;
        harvestCount = 0;

        highestTier = 0;
        highestPower = 0;
        highestHarvest = 0;
        highestShift = 0;

        possibleCrowns = 0;
        discoveredCrowns = 0;

        primary = false;
        inside = false;
        outside = false;
        four = false;
        five = false;

        this.key = key;
    }
}
public abstract class FlowerStats : MonoBehaviour
{
    [SerializeField] public int rarity;
    [SerializeField] public string type;
    public int id;

    [SerializeField] protected int basePoints;
    [SerializeField] protected int damage;
    [SerializeField] protected int range;
    [SerializeField] protected int projCount;
    [SerializeField] protected int projRange;

    public Sprite headSprite;
    public Sprite projSprite;
    public Sprite stemSprite;
    public GameObject pool;

    [SerializeField]
    public int[] pointsTiers = new int[4];
    [SerializeField]
    public int[] damageTiers = new int[4];
    [SerializeField]
    public int[] rangeTiers = new int[4];
    [SerializeField]
    public int[] projTiers = new int[4];

    [SerializeField] public int aug;

    //Title stuff
    [SerializeField] public string title;
    [SerializeField] public string primaryText, insideText, outsideText, fourText, fiveText;
    [SerializeField] public string description = "THIS IS A PLACEHOLDER DESCRIPTION FOR ALL THE FLOWERS. REPLACE IT SOMETIME YOU FUCKING IDIOT";
    [SerializeField] public string effects;
    [SerializeField] public Color textColor;

    // Start is called before the first frame update
    void Start()
    {
        UpdateAffinity(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual Sprite GetHeadSprite(int tier)
    {
        return headSprite;
    }

    public virtual void SetStem(GameObject head, GameObject stem)
    {
        head.transform.localPosition = new Vector3(-0.061f, 0.44f);
        stem.GetComponent<SpriteRenderer>().sprite = stemSprite;
    }

    public virtual int GetProjRange(int tier)
    {
        return projRange;
    }
    
    public void SetProjRange(int projRange)
    {
        this.projRange = projRange;
    }

    public virtual int GetRange(int tier)
    {
        return range;
    }

    public void SetRange(int range)
    {
        this.range = range;
    }

    public virtual int GetDamage(int tier)
    {
        return damage;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public virtual int GetPoints(int tier)
    {
        return basePoints;
    }

    public void SetPoints(int points)
    {
        this.basePoints = points;
    }

    public virtual int GetProjCount(int tier)
    {
        return projCount;
    }

    public void SetProjCount(int projCount)
    {
        this.projCount = projCount;
    }

    public string GetColorHex()
    {
        return ColorUtility.ToHtmlStringRGB(textColor);
    }

    public virtual string Colorizer(string input)
    {
        string colorText = GetColorHex();
        string returnString = "<color=#" + colorText + ">";
        returnString += input + "</color>";
        return returnString;
    }

    public string GetTitle()
    {
        return Colorizer(title);
    }

    public string GetPrimaryText()
    {
        GameControl.PlayerData.savedFlowerDict[type].primary = true;
        return Colorizer(primaryText);
    }

    public string GetInsideText()
    {
        GameControl.PlayerData.savedFlowerDict[type].inside = true;
        return Colorizer(insideText);
    }

    public string GetOutsideText()
    {
        GameControl.PlayerData.savedFlowerDict[type].outside = true;
        return Colorizer(outsideText);
    }

    public string GetFourText()
    {
        GameControl.PlayerData.savedFlowerDict[type].four = true;
        return Colorizer(fourText);
    }

    public string GetFiveText()
    {
        GameControl.PlayerData.savedFlowerDict[type].five = true;
        return Colorizer(fiveText);
    }


    public void UpdateAffinity(int tier)
    {
        basePoints = pointsTiers[tier];
        damage = damageTiers[tier];
        range = rangeTiers[tier];
        projCount = projTiers[tier];

        //check for new highestTier here
        if (GameControl.PlayerData.savedFlowerDict != null)
        {
            if (GameControl.PlayerData.savedFlowerDict[type].highestTier < tier)
                GameControl.PlayerData.savedFlowerDict[type].highestTier = tier;
        }
    }

    public virtual List<SpecialStats> GetSpecialValues(int power)
    {
        List<SpecialStats> defaultResult = new List<SpecialStats>();
        return defaultResult;
    }

    public virtual void OnProjArrival(GameObject proj, int power)
    {
        Debug.Log("on arrival called for " + type + " | Level: " + power);
        //proj.GetComponent<ProjectileBehavior>().ObjectDeactivate();
    }

    public virtual void OnEnemyCollision(GameObject enemy, int tier)
    {
        Debug.Log("enemy collision called for " + type + " | Level: " + tier);
    }

    public virtual void OnPlayerCollision(GameObject playerShadow, int power)
    {
        Debug.Log("player collision called for " + type + "| Level: " + power);
    }

    public virtual void OnProjTravel(GameObject proj, int power)
    {

    }

    public virtual void OnHitboxEnter(GameObject flower)
    {
        Debug.Log("Hitbox enter called for " + type);
    }

    public void SlowHandler(float slowEffect, float slowTime, int particle, GameObject enemy)
    {
        //call on self and any bossParts 
        if (enemy.CompareTag("boss"))
        {
            EnemyBehavior[] bossParts = UnityEngine.Object.FindObjectsOfType<EnemyBehavior>();
            GameObject[] enemies = new GameObject[bossParts.Length];
            for (int i = 0; i < enemies.Length; i++)
            {
                if (bossParts[i].transform.root == enemy.transform)
                    enemies[i] = bossParts[i].gameObject;
            }
            Debug.Log("Count:" + enemies.Length);
            foreach (var part in enemies)
            {
                Debug.Log("Calling on " + part.name);
                StartCoroutine(SlowApply(slowEffect, slowTime, particle, part));
            }
                
        }
        else if (enemy.CompareTag("bossPart"))
        {
            Debug.Log("this is already slowed");
        }
        //call normally
        else
        {
            StartCoroutine(SlowApply(slowEffect, slowTime, particle, enemy));
        }
    }

    protected IEnumerator SlowApply(float slowEffect, float slowTime, int particle, GameObject enemy)
    {
        Debug.Log("slow beginning");
        GameObject part = enemy.GetComponent<EnemyBehavior>().nextParticle();
        enemy.GetComponent<EnemyBehavior>().setParticle(part, particle);
        enemy.GetComponent<EnemyBehavior>().isSlowed = true;
        /*if (isFrozen)
        {
            backupSpeed = backupSpeed * slowEffect;
            backupUsed = true;
        }
        else
            moveSpeed = moveSpeed * slowEffect;*/
        enemy.GetComponent<EnemyBehavior>().SpeedDown(slowEffect);
        float currentTime = 0f;
        while (currentTime < slowTime)
        {
            if (!enemy.activeSelf || !enemy.GetComponent<EnemyBehavior>().isSlowed)
            {
                enemy.GetComponent<EnemyBehavior>().SpeedUp(slowEffect);
                enemy.GetComponent<EnemyBehavior>().isSlowed = false;
                enemy.GetComponent<SpriteRenderer>().color = Color.white;
                enemy.GetComponent<EnemyBehavior>().setParticle(part, 0);
                if (particle == 4)
                    enemy.GetComponent<EnemyBehavior>().isElectrified = false;
                yield break;
            }
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //yield return new WaitForSeconds(slowTime);
        //this might be fucked;
        /*if (isFrozen || backupUsed)
        {
            Debug.Log("BACKUP");
            backupSpeed = backupSpeed / slowEffect;
        }
        else
            moveSpeed = moveSpeed / slowEffect;*/
        enemy.GetComponent<EnemyBehavior>().SpeedUp(slowEffect);
        enemy.GetComponent<EnemyBehavior>().isSlowed = false;
        enemy.GetComponent<SpriteRenderer>().color = Color.white;
        enemy.GetComponent<EnemyBehavior>().setParticle(part, 0);
        if (particle == 4)
            enemy.GetComponent<EnemyBehavior>().isElectrified = false;
        /*if (backupUsed)
        {
            moveSpeed = backupSpeed;
            backupUsed = false;
        }*/
    }
}
