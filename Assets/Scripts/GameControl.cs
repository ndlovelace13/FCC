using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameControl PlayerData;

    //all variable declarations
    
    //game state stuff
    public bool firstRun = true;
    public bool tutorialActive = false;
    public bool speaking = false;
    public bool dialogueComplete = false;

    public bool descriptionVisited = false;
    public bool descriptionfromMenu = false;

    public int tutorialState = 0;
    public bool tutorialSkip = false;
    public bool discoveryDisplay = true;

    public bool firstCatalog = true;
    public bool purchaseMade = false;

    //tutorial conditions
    public int inputsTested = 0;
    public bool flowerHarvested = false;
    public bool singleTossed = false;
    public bool crownConstructionStarted = false;
    public bool crownComplete = false;
    public bool crownThrown = false;
    public bool redCrown = false;
    public bool fireReset = false;

    //flower probabilities
    public float uncommon = 0.05f;
    public float undiscovered = 0.05f;

    [SerializeField] public List<string> commonPool;
    [SerializeField] public List<string> discoveredUncommon;
    [SerializeField] public List<string> undiscoveredUncommon;

    //flower stuff
    [SerializeField] public Sprite[] flowerSprites;
    [SerializeField] public GameObject[] flowers;
    public FlowerStats[] flowerStats;
    public Dictionary<string, FlowerStats> flowerStatsDict;

    //player stats
    public int highScore = 0;
    public int highMin = 0;
    public int highSec = 0;

    public float balance = 0f;

    public int min = 0;
    public int sec = 0;
    public int score = 0;

    //affinity sash
    public int sashSlots = 3;
    public List<string> sashTypes;
    public GameObject sash;
    public GameObject[] currentAffinities;
    public Dictionary<string, int> affinityAmounts;
    public Sprite[] affinityTiers;
    public int[] affinityThresholds = { 10, 25, 40 };

    //upgradable stats
    public float playerSpeed = 5f;
    public float craftingSlow = 0.5f;
    //implement with precision throw mechanic
    public float throwDist;

    //upgrades
    [SerializeField] GameObject upgradeObj;
    public Dictionary<string, float> upgradeDict;
    [SerializeField] Sprite[] icons;

    public List<Upgrade> upgrades;

    //enemy related variables
    public float maxInterval = 0.35f;
    public float minInterval = 0.2f;
    public float maxSpeed = 2f;
    public float currentMax;
    public float minSpeed = 1f;
    public float currentMin;

    private void Awake()
    {
        if (PlayerData == null)
        {
            PlayerData = this;
            DontDestroyOnLoad(gameObject);
        }
        UpgradeInit();
        UpgradeApply();
        SetFlowers();
    }

    private void SetFlowers()
    {
        flowerStatsDict = new Dictionary<string, FlowerStats>();
        flowerStats = new FlowerStats[flowers.Length];
        for (int i = 0; i < flowers.Length; i++)
        {
            flowerStats[i] = flowers[i].GetComponent<FlowerStats>();
            flowerStatsDict.Add(flowerStats[i].type, flowerStats[i]);
            flowerStatsDict[flowerStats[i].type].UpdateAffinity(0);
        }

        //TO DO: get sprites from within flowerStats instead of a separate array
    }

    private void UpgradeInit()
    {
        upgradeDict = new Dictionary<string, float>()
        {
            //check if there is a way to directly influence the value of these variables
            {"uncommon", 0.05f},
            {"playerSpeed", 5f},
            {"craftingSlow", 0.5f}
        };
        upgrades = new List<Upgrade>();

        //uncommon rarity
        GameObject newUpgrade = Instantiate(upgradeObj);
        newUpgrade.transform.SetParent(transform);
        newUpgrade.GetComponent<Upgrade>().SetValues("uncommon", 1f, 1.5f, 15, 0.025f, "Better Seeds", "Increased Chance of Encountering Uncommon Flowers", "%", icons[0]);
        upgrades.Add(newUpgrade.GetComponent<Upgrade>());

        //playerSpeed
        newUpgrade = Instantiate(upgradeObj);
        newUpgrade.transform.SetParent(transform);
        newUpgrade.GetComponent<Upgrade>().SetValues("playerSpeed", 3f, 1.75f, 10, 0.5f, "Faster Shoes", "Incrases Player Movement Speed", " m/s", icons[1]);
        upgrades.Add(newUpgrade.GetComponent<Upgrade>());

        //crownSlow
        newUpgrade = Instantiate(upgradeObj);
        newUpgrade.transform.SetParent(transform);
        newUpgrade.GetComponent<Upgrade>().SetValues("craftingSlow", 5f, 2f, 5, -0.05f, "Skilled Hands", "Decreases Slow Effect when Crafting a crown", "% speed", icons[2]);
        upgrades.Add(newUpgrade.GetComponent<Upgrade>());
    }

    public void ResetRun()
    {
        min = 0;
        sec = 0;
        score = 0;
        currentMax = maxSpeed;
        currentMin = minSpeed;
    }

    public void UpgradeApply()
    {
        uncommon = upgradeDict["uncommon"];
        playerSpeed = upgradeDict["playerSpeed"];
        craftingSlow = upgradeDict["craftingSlow"];
    }

    public void FlowerDiscovery(string type)
    {
        //make an announcement to the crown announcement
        if (discoveryDisplay)
        {
            GameObject announce = GameObject.FindWithTag("mainAnnounce");
            announce.GetComponent<ScoreNotification>().newFeed("New Flower Discovered!");
        }
        //need to change this once rare flowers are added
        undiscoveredUncommon.Remove(type);
        discoveredUncommon.Add(type);
        //this is where to initialize entry in the almanac/mastery shit
    }

    public Sprite SpriteAssign(string type)
    {
        Sprite returnedSprite = flowerSprites[0];
        switch (type)
        {
            case "pink":
                returnedSprite = flowerSprites[0];
                break;
            case "white":
                returnedSprite = flowerSprites[1];
                break;
            case "orange":
                returnedSprite = flowerSprites[2];
                break;
            case "red":
                returnedSprite = flowerSprites[3];
                break;
            case "blue":
                returnedSprite = flowerSprites[4];
                break;
            case "green":
                returnedSprite = flowerSprites[5];
                break;
            case "yellow":
                returnedSprite = flowerSprites[6];
                break;
            case "default": Debug.Log("unhandled exception"); break;
        }
        return returnedSprite;
    }

    public void SashInit()
    {
        sashTypes = new List<string>();
        affinityAmounts = new Dictionary<string, int>();
        for (int i = 0; i < sashSlots; i++)
        {
            //placeholder needed here
            sashTypes.Add("null");
            affinityAmounts.Add("slot" + i, 0);
        }
        sash = GameObject.FindWithTag("sash");
        //don't do this yet
        currentAffinities = new GameObject[sashSlots];
    }

    public void affinityIncrease(string type)
    {
        affinityAmounts[type] += 1;
        Debug.Log("current " + type + " affinity: " + affinityAmounts[type]);
        //check whether move to the next affinity
        GameObject currentAffinity = currentAffinities[sashTypes.IndexOf(type)];
        if (affinityAmounts[type] >= affinityThresholds[currentAffinity.GetComponent<SashSlot>().currentTier])
        {
            currentAffinity.GetComponent<SashSlot>().tierUp();
        }
    }
}
