using System;
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

    public bool tutorialComplete = false;

    public bool firstCatalog = true;
    public bool firstResearch = true;
    public bool firstAlmanac = true;
    public bool sashActivated = false;
    public bool sashActive = false;
    public bool firstSeed = false;
    public bool bullyDefeated = false;

    public bool contractSigned = false;

    public bool crownCounterSet = false;

    //Tool Unlocks & Dialogue Stuff
    public bool catalogUnlocked = false;
    public bool researchUnlocked = false;
    public bool completionUnlocked = false;
    public bool almanacUnlocked = false;
    public Queue<string[]> dialogueQueue = new Queue<string[]>();

    //Persistent Total Counters
    public int shiftCounter = 0;
    public int completeShifts = 0;
    public float totalIncome = 0;
    public int totalCrowns = 0;
    public int totalFlowers = 0;
    public int totalKills = 0;
    public int totalSeeds = 0;
    public float totalSpent = 0;
    public int totalUpgrades = 0;
    public int totalDrives = 0;

    //Completion Stats
    public int upgradeAmount = 0;
    public int researchAmount = 0;

    //Persistent Record Counters
    public float highMoney = 0;
    public int highMin = 0;
    public int highSec = 0;
    public int highEnemies = 0;
    public int highSeeds = 0;
    public int highCrowns = 0;
    public int highDiscoveries = 0;

    //Favorites
    public string mostUsedFlower = "white";
    public int flowerTimes;

    public string mostUsedCrown;
    public int crownTimes;


    //item unlocks
    public int sashSlots = 3;
    public int repellentCount = 1;

    //Balances
    public int essenceCount = 0;
    public float balance = 0f;
    public int newDiscoveries = 0;

    //Upgrades, Research, Unlocks
    public List<UpgradeEssentials> upgrades;
    public List<ResearchData> researchData;
    public List<string> discoveredUncommon;
    public List<string> discoveredRare;

    //Almanac Stats
    public List<SavedFlowerStats> flowerSaveData;
    public List<SavedEnemyStats> enemySaveData;

    //Completion Data
    public List<CrownData> discoveredCrowns;

    //ShiftReports
    public List<ShiftReport> shiftReports;
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

    public bool nodeShiftCancel = false;

    public bool shiftJustEnded = false;
    public bool continuePressed = false;
    public bool balanceUpdated = false;
    public bool gameVarsSet = false;

    public bool loading = false;
    public bool gameOver = false;
    public bool gameWin = false;
    public bool gamePaused = false;
    public bool repellentMode = false;

    public bool menusReady = false;
    public bool menuActive = false;

    public bool unlockDone = false;
    public bool unlockNotifActive = false;

    public bool quitCooldown = false;

    //boss fight stuff
    public bool bossSpawning = false;
    public bool bossActive = false;

    public GameObject tutorialHandler;

    public int currentReportIndex = 0;

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
    public float rare = 0.1f;
    //public float undiscovered = 0.05f;

    [SerializeField] public List<string> allDiscovered;
    [SerializeField] public List<string> commonPool;
    //[SerializeField] public List<string> discoveredUncommon;
    [SerializeField] public List<string> undiscoveredUncommon;
    [SerializeField] public List<string> undiscoveredRare;

    //flower stuff
    //[SerializeField] public Sprite[] flowerSprites;
    [SerializeField] public GameObject[] flowers;
    public FlowerStats[] flowerStats;
    public Dictionary<string, FlowerStats> flowerStatsDict;
    public Dictionary<string, SavedFlowerStats> savedFlowerDict;
    public string[] flowerTypes;
    public GameObject flowerPool;
    public Dictionary<string, ObjectPool> flowerPoolDict;

    //used for keeping track of most used flowers - TODO - could overhaul the sash system to use this metric instead
    public Dictionary<string, int> flowerUse;
    //used for keeping track of enemy eliminations
    public Dictionary<string, int> enemyKills;

    //player stats
    /*public int highScore = 0;
    public int highMin = 0;
    public int highSec = 0;*/

    //public float balance = 0f;

    public int min = 0;
    public int sec = 0;
    public int score = 0;
    public int shiftSeeds = 0;
    public int shiftEnemies = 0;
    public int shiftCrowns = 0;
    public int shiftDiscoveries = 0;
    public int remainingRepellent = 0;

    //specific score totals
    public int discoveryScore = 0;
    public int constructionScore = 0;
    public int enemyScore = 0;
    public int otherScore = 0;


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
    public float repellentLength = 2f;
    public float repellentEffect = 3f;
    

    //upgrades
    [SerializeField] GameObject upgradeObj;
    public Dictionary<string, float> upgradeDict;
    [SerializeField] Sprite[] icons;

    public List<Upgrade> upgrades;
    public List<Page> catalogPages;
    [SerializeField] GameObject catalogPage;

    //research
    [SerializeField] GameObject ResearchPrefab;
    public List<Research> researchItems;
    [SerializeField] GameObject UnlockPrefab;

    [SerializeField] public GameObject BlackoutPrefab;

    //almanac variables
    public List<Page> almanacPages;
    [SerializeField] GameObject stickerPage;
    [SerializeField] public GameObject flowerInfoPage;
    [SerializeField] public GameObject flowerStatPage;
    [SerializeField] public GameObject enemyPage;
    [SerializeField] GameObject helperPage;
    [SerializeField] GameObject controlPage;
    [SerializeField] GameObject playerStatsPage;
    [SerializeField] GameObject playerCompPage;

    //enemy related variables
    [SerializeField] List<EnemyStats> enemyTypes = new List<EnemyStats>();
    List<GameObject> enemySpawners = new List<GameObject>();
    [SerializeField] GameObject enemySpawnPrefab;
    public Dictionary<string, EnemyStats> enemyStatsDict;
    public Dictionary<string, SavedEnemyStats> savedEnemyDict;
    public int newEnemyTime = 60;

    //money stuff
    public ObjectPool moneyPool;
    public ObjectPool moneySpawner;

    //cursor vs. crosshair
    public bool crosshairActive = false;

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
        SaveHandler.DeleteSaveFile();
        SaveData = new SaveData();
        if (!gameVarsSet)
        {
            SetFlowers();
            EnemyDict();
            UpgradeInit();
            UpgradeApply();
            ResearchInit();
            CrownCompletion.PermutationEst();
            AlmanacInit();
            gameVarsSet = true;
        }
        //init shiftReports list if one doesn't exist
        SaveData.shiftReports = new List<ShiftReport>();
        continuePressed = true;
    }

    public void LoadGame()
    {
        SaveHandler.LoadGame();
        if (!gameVarsSet)
        {
            //Restore flower unlocks
            SetFlowers();
            EnemyDict();
            //Restore the Upgrades & Research
            UpgradeInit();
            UpgradeApply();
            ResearchInit();
            CrownCompletion.PermutationEst();
            AlmanacInit();
        }
        continuePressed = true;
        gameVarsSet = true;
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
            newPool.GetComponent<ObjectPool>().Establish(flowers[i], 75);
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
            //DiscoveredPooling();
        }

        if (SaveData.discoveredRare == null)
        {
            Debug.Log("Rareflower data not found");
            SaveData.discoveredRare = new List<string>();
        }
        else
        {
            Debug.Log("rare flower save data restored");
            for (int i = 0; i < SaveData.discoveredRare.Count; i++)
            {
                undiscoveredRare.Remove(SaveData.discoveredRare[i]);
            }
        }

        //check for flowerSaveData list, init if it doesn't exist
        if (SaveData.flowerSaveData == null)
        {
            //initialize the flowerSaveData list
            SaveData.flowerSaveData = new List<SavedFlowerStats>();
            foreach (var flower in flowerStatsDict)
            {
                SavedFlowerStats newData = new SavedFlowerStats(flower.Key);
                SaveData.flowerSaveData.Add(newData);
            }
        }
        else
        {
            int savedCount = SaveData.flowerSaveData.Count;
            int iterator = 0;
            foreach (var flower in flowerStatsDict)
            {
                if (iterator >= savedCount)
                {
                    SavedFlowerStats newData = new SavedFlowerStats(flower.Key);
                    SaveData.flowerSaveData.Add(newData);
                }
                iterator++;
            }
        }
        

        //init the SavedFlowerDict once data is either created or restored
        savedFlowerDict = new Dictionary<string, SavedFlowerStats>();
        foreach (var flower in SaveData.flowerSaveData)
        {
            savedFlowerDict.Add(flower.key, flower);
        }

        Debug.Log(savedFlowerDict.Count + " flower saveData in the dict");

        //init the flowerUse stuff so that it doesn't tweak out if continue is pressed
        flowerUse = new Dictionary<string, int>();
        foreach (var flower in allDiscovered)
        {
            flowerUse.Add(flower, 0);
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
            {"crownMult", 1f},
            {"repellentLength", 2f },
            {"repellentEffect", 3f }
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
        newUpgrade.GetComponent<Upgrade>().SetValues("playerSpeed", 4f, 1.75f, 10, 0.25f, "Faster Shoes", "Increases Player Movement Speed", " m/s", icons[1]);
        upgrades.Add(newUpgrade.GetComponent<Upgrade>());

        //crownSlow
        newUpgrade = Instantiate(upgradeObj);
        newUpgrade.transform.SetParent(transform);
        newUpgrade.GetComponent<Upgrade>().SetValues("craftingSlow", 5f, 2f, 5, -0.05f, "Skilled Hands", "Decreases Slow Effect when Crafting a crown", "% speed", icons[2]);
        upgrades.Add(newUpgrade.GetComponent<Upgrade>());

        //seedChance
        newUpgrade = Instantiate(upgradeObj);
        newUpgrade.transform.SetParent(transform);
        newUpgrade.GetComponent<Upgrade>().SetValues("seedChance", 2f, 1.75f, 10, 0.05f, "Seedier Replicants", "Increased Chance of Essence Seed Drop on Kill", "%", icons[3]);
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

        //repellent timeframe
        newUpgrade = Instantiate(upgradeObj);
        newUpgrade.transform.SetParent(transform);
        newUpgrade.GetComponent<Upgrade>().SetValues("repellentLength", 12f, 1.5f, 5, 0.5f, "Improved Aerosols", "Increases the active time of replicant repellent", "secs", icons[5]);
        upgrades.Add(newUpgrade.GetComponent<Upgrade>());

        //repellent effect
        newUpgrade = Instantiate(upgradeObj);
        newUpgrade.transform.SetParent(transform);
        newUpgrade.GetComponent<Upgrade>().SetValues("repellentEffect", 15f, 1.5f, 5, 0.25f, "Repellent Revision", "Increases the effective time of replicant repellent", "secs", icons[5]);
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
            /*for (int i = 0; i < upgrades.Count; i++)
            {
                upgrades[i].SetValues(SaveData.upgrades[i]);
                //set the value in the dict
                upgradeDict[upgrades[i].upgradeKey] = upgrades[i].currentValue;
            }*/

            int savedCount = SaveData.upgrades.Count;
            int iterator = 0;
            foreach (var upgrade in upgrades)
            {
                if (iterator >= savedCount)
                {
                    UpgradeEssentials newUp = new UpgradeEssentials();
                    newUp.SetStats(upgrade);
                    SaveData.upgrades.Add(newUp);
                }
                else
                {
                    upgrades[iterator].SetValues(SaveData.upgrades[iterator]);
                    //set the value in the dict
                    upgradeDict[upgrades[iterator].upgradeKey] = upgrades[iterator].currentValue;
                }
                iterator++;
            }
        }

        //Catalog Pages initialized
        catalogPages = new List<Page>();
        GameObject catalogContainer = new GameObject("CatalogContainer");
        catalogContainer.transform.SetParent(transform);

        for (int i = 0; i < upgrades.Count; i++)
        {
            //instantiate a new page object every three upgrades
            if (i % 3 == 0)
            {
                //Debug.Log("Adding " + i);
                GameObject newPage = Instantiate(catalogPage);
                newPage.transform.SetParent(catalogContainer.transform);
                newPage.GetComponent<CatalogPage>().SetStartIndex(i);
                catalogPages.Add(newPage.GetComponent<Page>());
            }
        }
        Debug.Log("There are " + catalogPages.Count + " pages in the catalog");
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

    private void AlmanacInit()
    {
        //make an almanac object to contain all the pages persistently
        GameObject almanacContainer = new GameObject("AlmanacContainer");
        almanacContainer.transform.SetParent(transform);

        //initialize sticker pages for flowers and enemies
        GameObject flowerStickers = Instantiate(stickerPage);
        flowerStickers.GetComponent<StickerPage>().StickerAssign(flowerStatsDict);
        flowerStickers.transform.SetParent(almanacContainer.transform);

        GameObject enemyStickers = Instantiate(stickerPage);
        enemyStickers.GetComponent<StickerPage>().StickerAssign(enemyStatsDict);
        enemyStickers.transform.SetParent(almanacContainer.transform);

        almanacPages = new List<Page> { flowerStickers.GetComponent<Page>(), enemyStickers.GetComponent<Page>() };

        //init an info and stat page for each flower
        List<Page> flowerPages = flowerStickers.GetComponent<StickerPage>().PageInit(almanacPages.Count);
        almanacPages.AddRange(flowerPages);
        
        //init an info page for each enemy
        List<Page> enemyPages = enemyStickers.GetComponent<StickerPage>().PageInit(almanacPages.Count);
        almanacPages.AddRange(enemyPages);

        //create the other info pages
        //TODO - floral sash info page?
        //TODO - mastery page?
        GameObject helperObj = Instantiate(helperPage);
        Page crownPage = helperObj.GetComponent<HelperPage>();
        helperObj.transform.SetParent(almanacContainer.transform);

        GameObject controlObj = Instantiate(controlPage);
        Page controlPagee = controlObj.GetComponent<HelperPage>();
        controlObj.transform.SetParent(almanacContainer.transform);

        GameObject playerInfoObj = Instantiate(playerStatsPage);
        Page playerInfo = playerInfoObj.GetComponent<PlayerStatsPage>();
        playerInfoObj.transform.SetParent(almanacContainer.transform);

        //Player Completion Page here
        GameObject playerCompObj = Instantiate(playerCompPage);
        Page playerCompletion = playerCompObj.GetComponent<PlayerCompletionPage>();
        playerCompObj.transform.SetParent(almanacContainer.transform);

        almanacPages.Add(crownPage);
        almanacPages.Add(controlPagee);
        almanacPages.Add(playerInfo);
        almanacPages.Add(playerCompletion);

        Debug.Log("There are currently " + almanacPages.Count + " pages in the almanac");
    }

    //Enemy Initialization
    private void EnemyDict()
    {
        enemyStatsDict = new Dictionary<string, EnemyStats>();
        foreach (var enemy in enemyTypes)
        {
            enemyStatsDict.Add(enemy.type, enemy);
        }
        
        //create save data for each enemy if it doesn't exist, then add to dict for easy access
        if (SaveData.enemySaveData == null)
        {
            SaveData.enemySaveData = new List<SavedEnemyStats>();
            foreach (var enemy in enemyStatsDict)
            {
                SavedEnemyStats newData = new SavedEnemyStats(enemy.Key);
                SaveData.enemySaveData.Add(newData);
            }
        }
        else
        {
            int savedCount = SaveData.enemySaveData.Count;
            int iterator = 0;
            foreach (var enemy in enemyStatsDict)
            {
                if (iterator >= savedCount)
                {
                    SavedEnemyStats newData = new SavedEnemyStats(enemy.Key);
                    SaveData.enemySaveData.Add(newData);
                }
                iterator++;
            }
        }

        savedEnemyDict = new Dictionary<string, SavedEnemyStats>();
        foreach (var enemy in SaveData.enemySaveData)
            savedEnemyDict.Add(enemy.key, enemy);
        Debug.Log(savedEnemyDict.Count + " enemy saveData in the dict");
    }

    //Enemy Reset for ResetRun
    private void EnemyInit()
    {
        enemySpawners.Clear();
        //enemyStatsDict = new Dictionary<string, EnemyStats>();
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
        gameWin = false;
        loading = true;
        //reset the scores
        min = 0;
        sec = 0;
        score = 0;
        shiftSeeds = 0;
        shiftEnemies = 0;
        shiftCrowns = 0;
        shiftDiscoveries = 0;
        discoveryScore = 0;
        constructionScore = 0;
        enemyScore = 0;
        otherScore = 0;
        unlockDone = false;

        Debug.Log("resetting repellents to: " + SaveData.repellentCount);
        remainingRepellent = SaveData.repellentCount;

        crosshairActive = true;

        bossActive = false;
        bossSpawning = false;

        

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
        moneyPool.Pooling();
        moneySpawner.Pooling();

        //Reset the flower use counters
        flowerUse = new Dictionary<string, int>();
        foreach (var flower in allDiscovered)
        {
            flowerUse.Add(flower, 0);
        }

        //Reset the enemy kill counters
        enemyKills = new Dictionary<string, int>();
        foreach (var enemy in enemyTypes)
        {
            enemyKills.Add(enemy.type, 0);
        }
        
        GameObject.FindWithTag("flowerPool").GetComponent<FlowerCalc>().PreroundCalc();
        foreach(var flowerStat in flowerStats)
        {
            flowerStatsDict[flowerStat.type].UpdateAffinity(0);
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
        allDiscovered = allDiscovered.Union(SaveData.discoveredRare).ToList();
        foreach (var flower in allDiscovered)
        {
            flowerPoolDict[flower].Pooling();
            if (!savedFlowerDict[flower].discovered)
                savedFlowerDict[flower].discovered = true;
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
        Debug.Log("Flower Discovery called for " + type);
        if (SaveData.discoveredUncommon.Contains(type) || SaveData.discoveredRare.Contains(type))
            return;
        else
        {
            Debug.Log("Flower discovery went through");
            if (undiscoveredUncommon.Contains(type))
            {
                undiscoveredUncommon.Remove(type);
                SaveData.discoveredUncommon.Add(type);
                allDiscovered = allDiscovered.Union(SaveData.discoveredUncommon).ToList();
            }
            else
            {
                undiscoveredRare.Remove(type);
                SaveData.discoveredRare.Add(type);
                allDiscovered = allDiscovered.Union(SaveData.discoveredRare).ToList();
            }
            
            foreach (var flower in allDiscovered)
                Debug.Log(flower);
            SaveHandler.SaveGame();
        }
        
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
        Debug.Log("Sash initialized??? START HERE");
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
            sash.GetComponent<SashBehavior>().enabled = true;
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

    public void GameWinBehavior(string type)
    {
        StartCoroutine(GameWinExe(type));
    }

    IEnumerator GameWinExe(string type)
    {
        //check for flower discovery and pop-up with discovery notif if not
        if (!allDiscovered.Contains(type))
        {
            FlowerDiscovery(type);
            GameObject unlock = Instantiate(UnlockPrefab);
            unlock.GetComponent<UnlockNotif>().BeginNotif(SpriteAssign(type), "New Flower Discovered!");
        }
        else
        {
            unlockNotifActive = false;
        }

        while (unlockNotifActive)
        {
            yield return new WaitForEndOfFrame();
        }

        //then fade to black
        GameObject blackoutObj = Instantiate(BlackoutPrefab);
        blackoutObj.GetComponent<BlackoutBehavior>().BeginBlackout("You Eliminated the Replicant Threat", "...For Now...", "Homebase", 3f);
        unlockDone = false;

        yield return null;
    }
}
