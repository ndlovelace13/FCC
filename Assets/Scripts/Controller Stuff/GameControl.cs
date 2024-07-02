using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//THIS CLASS SAVES ALL PLAYER DATA, THIS IS WHAT IS WRITTEN TO FILE AND LOADED
[System.Serializable]
public class SaveData
{
    //Game States
    public bool firstRun = true;

    public bool firstCatalog = true;
    public bool firstResearch = true;
    public bool sashActivated = false;
    public bool sashActive = false;

    //Persistent Counters
    public int shiftCounter = 0;

    public int highScore = 0;
    public int highMin = 0;
    public int highSec = 0;

    public int sashSlots = 3;

    //Balances
    public int essenceCount = 0;
    public float balance = 0f;

    //Upgrades, Research, Unlocks
    public List<UpgradeEssentials> upgrades;
    public List<ResearchData> researchData;
    public List<string> discoveredUncommon;

    //Completion Data
    public List<CrownData> discoveredCrowns;
}

public class GameControl : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameControl PlayerData;

    //THE IMPORTANT PERSISTENT SAVE DATA
    public static SaveHandler SaveHandler;
    public static SaveData SaveData;

    //Completion Handler
    public static CrownCompletionism CrownCompletion;

    //all variable declarations

    //testing crawd
    public bool testing = false;
    
    //game state stuff
    //public bool firstRun = true;
    public bool tutorialActive = false;
    public bool speaking = false;
    public bool dialogueComplete = false;

    public bool descriptionVisited = false;
    public bool descriptionfromMenu = false;

    public int tutorialState = 0;
    public bool tutorialSkip = false;
    public bool discoveryDisplay = true;

    //public bool firstCatalog = true;
    public bool purchaseMade = false;

    //public bool firstResearch = true;
    public bool donationMade = false;

    public bool balanceUpdated = false;

    public bool loading = false;
    public bool gameOver = false;
    public bool gamePaused = false;

    public GameObject tutorialHandler;

    //public int shiftCounter = 0;

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
    //public float undiscovered = 0.05f;

    [SerializeField] public List<string> allDiscovered;
    [SerializeField] public List<string> commonPool;
    //[SerializeField] public List<string> discoveredUncommon;
    [SerializeField] public List<string> undiscoveredUncommon;

    //flower stuff
    //[SerializeField] public Sprite[] flowerSprites;
    [SerializeField] public GameObject[] flowers;
    public FlowerStats[] flowerStats;
    public Dictionary<string, FlowerStats> flowerStatsDict;
    public string[] flowerTypes;
    public GameObject flowerPool;
    public Dictionary<string, ObjectPool> flowerPoolDict;

    //player stats
    /*public int highScore = 0;
    public int highMin = 0;
    public int highSec = 0;*/

    //public float balance = 0f;

    public int min = 0;
    public int sec = 0;
    public int score = 0;

    //affinity sash
    /*public bool sashActivated = false;
    public bool sashActive = false;
    public int sashSlots = 3;*/
    public List<string> sashTypes;
    [SerializeField] GameObject sashPrefab;
    public GameObject sash;
    public GameObject[] currentAffinities;
    public Dictionary<string, int> affinityAmounts;
    public Sprite[] affinityTiers;
    public int[] affinityThresholds = { 10, 25, 40 };
    public int[] tierScores = { 50, 100, 200 };

    //upgradable stats
    public float playerSpeed = 5f;
    public float craftingSlow = 0.5f;
    public float pickupDist = 0.5f;
    public float seedChance = 0.25f;
    public float crownMult = 1f;
    //implement with precision throw mechanic
    //public float throwDist;

    //upgrades
    [SerializeField] GameObject upgradeObj;
    public Dictionary<string, float> upgradeDict;
    [SerializeField] Sprite[] icons;

    public List<Upgrade> upgrades;

    //essence progression
    //public int essenceCount = 0;

    //research
    [SerializeField] GameObject ResearchPrefab;
    public List<Research> researchItems;
    [SerializeField] GameObject UnlockPrefab;

    //enemy related variables
    [SerializeField] List<EnemyStats> enemyTypes = new List<EnemyStats>();
    List<GameObject> enemySpawners = new List<GameObject>();
    [SerializeField] GameObject enemySpawnPrefab;
    public int newEnemyTime = 60;

    private void Awake()
    {
        if (PlayerData == null)
        {
            PlayerData = this;
            SaveHandler = GetComponent<SaveHandler>();
            CrownCompletion = GetComponent<CrownCompletionism>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NewGame()
    {
        SaveData = new SaveData();
        SetFlowers();
        UpgradeInit();
        UpgradeApply();
        ResearchInit();
        CrownCompletion.PermutationEst();
    }

    public void LoadGame()
    {
        SaveHandler.LoadGame();
        //Restore flower unlocks
        SetFlowers();
        //Restore the Upgrades & Research
        UpgradeInit();
        UpgradeApply();
        ResearchInit();
        CrownCompletion.PermutationEst();
        //Restore the Discovery Stuff

    }

    private void SetFlowers()
    {
        allDiscovered = commonPool;
        //establish all flower arrays and lists
        flowerStatsDict = new Dictionary<string, FlowerStats>();
        flowerPoolDict = new Dictionary<string, ObjectPool>();
        flowerStats = new FlowerStats[flowers.Length];
        flowerTypes = new string[flowers.Length];
        for (int i = 0; i < flowers.Length; i++)
        {
            //add flowerStats to the array
            GameObject newFlower = Instantiate(flowers[i]);
            newFlower.transform.SetParent(transform);
            newFlower.GetComponent<SpriteRenderer>().enabled = false;
            flowerStats[i] = newFlower.GetComponent<FlowerStats>();
            //establish an associated flowerPool and store in the flowerStats - create once up front instead of each gameplay loop
            GameObject newPool = Instantiate(flowerPool);
            newPool.transform.SetParent(transform);
            flowers[i].GetComponent<SpriteRenderer>().enabled = true;
            newPool.GetComponent<ObjectPool>().Establish(flowers[i], 50);
            flowerPoolDict.Add(flowerStats[i].type, newPool.GetComponent<ObjectPool>());
            //store in dictionary for easy access based on type
            flowerStatsDict.Add(flowerStats[i].type, flowerStats[i]);
            //reset the affinity levels
            flowerStatsDict[flowerStats[i].type].UpdateAffinity(0);
        }
        if (SaveData.discoveredUncommon == null)
        {
            Debug.Log("flower save data not found");
            SaveData.discoveredUncommon = new List<string>();
        }
        else
        {
            Debug.Log("flower save data restored");
            for (int i = 0; i < SaveData.discoveredUncommon.Count; i++)
            {
                undiscoveredUncommon.Remove(SaveData.discoveredUncommon[i]);
            }
            DiscoveredPooling();
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
            {"craftingSlow", 0.5f},
            {"seedChance", 0.25f},
            {"pickupDist", 1f},
            {"crownMult", 1f}
        };
        upgrades = new List<Upgrade>();

        //uncommon rarity
        GameObject newUpgrade = Instantiate(upgradeObj);
        newUpgrade.transform.SetParent(transform);
        newUpgrade.GetComponent<Upgrade>().SetValues("uncommon", 3f, 1.5f, 15, 0.025f, "Enhanced Pollination", "Increased Chance of Encountering Uncommon Flowers", "%", icons[0]);
        upgrades.Add(newUpgrade.GetComponent<Upgrade>());

        //playerSpeed
        newUpgrade = Instantiate(upgradeObj);
        newUpgrade.transform.SetParent(transform);
        newUpgrade.GetComponent<Upgrade>().SetValues("playerSpeed", 3f, 1.75f, 10, 0.5f, "Faster Shoes", "Increases Player Movement Speed", " m/s", icons[1]);
        upgrades.Add(newUpgrade.GetComponent<Upgrade>());

        //crownSlow
        newUpgrade = Instantiate(upgradeObj);
        newUpgrade.transform.SetParent(transform);
        newUpgrade.GetComponent<Upgrade>().SetValues("craftingSlow", 5f, 2f, 5, -0.05f, "Skilled Hands", "Decreases Slow Effect when Crafting a crown", "% speed", icons[2]);
        upgrades.Add(newUpgrade.GetComponent<Upgrade>());

        //seedChance
        newUpgrade = Instantiate(upgradeObj);
        newUpgrade.transform.SetParent(transform);
        newUpgrade.GetComponent<Upgrade>().SetValues("seedChance", 2f, 1.75f, 10, 0.05f, "Seedier Skinwalkers", "Increased Chance of Essence Seed Drop on Kill", "%", icons[3]);
        upgrades.Add(newUpgrade.GetComponent<Upgrade>());

        //pickupDist
        newUpgrade = Instantiate(upgradeObj);
        newUpgrade.transform.SetParent(transform);
        newUpgrade.GetComponent<Upgrade>().SetValues("pickupDist", 4f, 1.5f, 10, 0.5f, "Essence Magnet", "Increases Pickup Distance on Essence Seeds", "m", icons[4]);
        upgrades.Add(newUpgrade.GetComponent<Upgrade>());

        //crownMult
        newUpgrade = Instantiate(upgradeObj);
        newUpgrade.transform.SetParent(transform);
        newUpgrade.GetComponent<Upgrade>().SetValues("crownMult", 10f, 1.75f, 6, 0.25f, "Crown Pay Raise", "Increases the score multiplier on crown creation", "x", icons[5]);
        upgrades.Add(newUpgrade.GetComponent<Upgrade>());
        
        //if there is no previous data, initialize the saveData from the objects
        if (SaveData.upgrades == null)
        {
            Debug.Log("upgrade data not found");
            SaveData.upgrades = new List<UpgradeEssentials>();
            foreach (Upgrade upgrade in upgrades)
            {
                UpgradeEssentials newUp = new UpgradeEssentials();
                newUp.SetStats(upgrade);
                SaveData.upgrades.Add(newUp);
            }
        }
        //otherwise, initialize the upgrade data from the saveData
        else
        {
            Debug.Log("upgrade data FOUND");
            for (int i = 0; i < upgrades.Count; i++)
            {
                upgrades[i].SetValues(SaveData.upgrades[i]);
                //set the value in the dict
                upgradeDict[upgrades[i].upgradeKey] = upgrades[i].currentValue;
            }
        }
    }

    /*private void UpgradeRestore()
    {
        upgradeDict = new Dictionary<string, float>()
        {
            //check if there is a way to directly influence the value of these variables
            {"uncommon", 0.05f},
            {"playerSpeed", 5f},
            {"craftingSlow", 0.5f},
            {"seedChance", 0.25f},
            {"pickupDist", 1f},
            {"crownMult", 1f}
        };
        foreach (var upgrade in SaveData.upgrades)
        {
            GameObject newUpgrade = Instantiate(upgradeObj);
            newUpgrade.transform.SetParent(transform);
            newUpgrade.GetComponent<Upgrade>().SetValues(upgrade);
            //update the dictionary from the stored upgrade obj
            upgradeDict[upgrade.upgradeKey] = upgrade.currentValue;
        }
    }*/

    private void ResearchInit()
    {
        researchItems = new List<Research>();
        GameObject newResearch = Instantiate(ResearchPrefab);
        newResearch.transform.parent = transform;
        //Add the elements to the research list in the controller
        researchItems.Add(newResearch.GetComponent<UncommonSeedResearch>());
        researchItems.Add(newResearch.GetComponent<SashResearch>());

        //update the values according to the save data
        if (SaveData.researchData == null)
        {
            SaveData.researchData = new List<ResearchData>();
            foreach (Research research in researchItems)
            {
                ResearchData newRes = new ResearchData();
                newRes.SetData(research);
                SaveData.researchData.Add(newRes);
            }
        }
        else
        {
            for (int i = 0; i < researchItems.Count; i++)
            {
                researchItems[i].RestoreData(SaveData.researchData[i]);
            }
        }
        
        
    }

    private void EnemyInit()
    {
        enemySpawners.Clear();
        foreach (var enemy in enemyTypes)
        {
            GameObject newSpawner = Instantiate(enemySpawnPrefab);
            newSpawner.GetComponent<EnemySpawn>().thisEnemy = enemy;
            //start the pooling from the prefab in the enemyStats obj
            newSpawner.GetComponent<ObjectPool>().objectToPool = enemy.enemyPrefab;
            newSpawner.GetComponent<ObjectPool>().Pooling();
            enemySpawners.Add(newSpawner);
            newSpawner.SetActive(false);
        }
    }

    public void ResetRun()
    {
        gameOver = false;
        loading = true;
        min = 0;
        sec = 0;
        score = 0;
        SaveData.shiftCounter++;
        //enemy reset
        /*currentMax = maxSpeed;
        currentMin = minSpeed;
        currentHealth = maxHealth;
        currentMaxEnemies = startingEnemies;*/
        EnemyInit();

        balanceUpdated = false;
        sash = GameObject.FindWithTag("sash");
        tutorialHandler = GameObject.FindWithTag("tutorialHandler");
        if (!SaveData.sashActive)
        {
            sash.SetActive(false);
        }
        DiscoveredPooling();
        GameObject.FindWithTag("flowerPool").GetComponent<FlowerCalc>().PreroundCalc();
        foreach(var flowerStat in flowerStats)
        {
            flowerStatsDict[flowerStat.type].UpdateAffinity(0);
        }
        foreach (var flowerStat in flowerStats)
        {
        }
        //NewUnlocks();
        GameObject.FindWithTag("Player").GetComponentInChildren<PlayerMovement>(true).IntroMove();
    }

    public void FinishIntro()
    {
        //Other pre-round stuff here
        loading = false;
        GameObject.FindWithTag("timer").GetComponent<Timer>().TimerStart();
        //start the basic enemy spawning
        StartCoroutine(EnemySpawning());
        GameObject announce = GameObject.FindWithTag("mainAnnounce");
        announce.GetComponent<ScoreNotification>().newFeed("Start Crafting!");
    }

    public void NewUnlocks()
    {
        if (SaveData.sashActive)
        {
            //delivery anim??? - find a different way to activate
            sash.SetActive(true); //fuck with the transform so that it isn't off screen
            SashInit();
        }
        if (SaveData.sashActivated)
        {
            tutorialHandler.GetComponent<InRoundTutorial>().SashIntro();
            SaveData.sashActivated = false;
        }
        else
        {
            FinishIntro();
        }
    }

    public void DiscoveredPooling()
    {
        //TO DO - combine into one list of all discovered
        //allDiscovered = commonPool;
        allDiscovered = allDiscovered.Union(SaveData.discoveredUncommon).ToList();
        foreach(var flower in allDiscovered)
        {
            flowerPoolDict[flower].Pooling();
        }
    }

    public void UpgradeApply()
    {
        uncommon = upgradeDict["uncommon"];
        playerSpeed = upgradeDict["playerSpeed"];
        craftingSlow = upgradeDict["craftingSlow"];
        seedChance = upgradeDict["seedChance"];
        pickupDist = upgradeDict["pickupDist"];
        crownMult = upgradeDict["crownMult"];
    }

    public void FlowerDiscovery(string type)
    {
        //make an announcement to the crown announcement
        /*if (discoveryDisplay)
        {
            GameObject announce = GameObject.FindWithTag("mainAnnounce");
            announce.GetComponent<ScoreNotification>().newFeed("New Flower Discovered!");
        }*/
        //need to change this once rare flowers are added
        undiscoveredUncommon.Remove(type);
        SaveData.discoveredUncommon.Add(type);
        allDiscovered = allDiscovered.Union(SaveData.discoveredUncommon).ToList();
        SaveHandler.SaveGame();
        //this is where to initialize entry in the almanac/mastery shit
    }

    //TO DO - switch this to take a flower object instead?
    public Sprite SpriteAssign(string type)
    {
        Sprite returnedSprite;
        returnedSprite = flowerStatsDict[type].headSprite;
        /*switch (type)
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
            case "dandy":
                returnedSprite = flowerSprites[7];
                break;
            case "default": Debug.Log("unhandled exception"); break;
        }*/
        return returnedSprite;
    }

    public void SashInit()
    {
        if (SaveData.sashActive)
        {
            sashTypes = new List<string>();
            affinityAmounts = new Dictionary<string, int>();
            for (int i = 0; i < SaveData.sashSlots; i++)
            {
                //placeholder needed here
                sashTypes.Add("null");
                affinityAmounts.Add("slot" + i, 0);
            }
            //don't do this yet
            currentAffinities = new GameObject[SaveData.sashSlots];
            sash.GetComponent<SashBehavior>().slotInit();
        }
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

    IEnumerator EnemySpawning()
    {
        int enemyTypeCounter = 0;
        float time = 0f;
        do
        {
            //break if there aren't anymore enemy types to introduce
            if (enemyTypeCounter == enemySpawners.Count)
                yield break;
            enemySpawners[enemyTypeCounter].SetActive(true);
            enemySpawners[enemyTypeCounter].GetComponent<EnemySpawn>().enemyBegin();
            while (time < newEnemyTime)
            {
                //break if the shift is complete
                if (gameOver)
                    yield break;
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;
            }
            time = 0f;
            enemyTypeCounter++;
        } while (!gameOver);

        
    }
}
