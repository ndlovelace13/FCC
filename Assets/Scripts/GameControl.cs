using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public int tutorialState = 0;

    //tutorial conditions
    public int inputsTested = 0;
    public bool flowerHarvested = false;
    public bool singleTossed = false;
    public bool crownConstructionStarted = false;
    public bool crownComplete = false;
    public bool crownThrown = false;

    //flower probabilities
    public float uncommon = 0.05f;

    //player stats
    public int highScore = 0;
    public int highMin = 0;
    public int highSec = 0;

    public float balance = 0f;

    public int min = 0;
    public int sec = 0;
    public int score = 0;

    //upgradable stats
    public float playerSpeed = 5f;
    public float craftingSlow = 0.5f;
    //implement with precision throw mechanic
    public float throwDist;

    //upgrades
    [SerializeField] GameObject upgradeObj;
    public Dictionary<string, float> upgradeDict;

    public List<Upgrade> upgrades;

    //enemy related variables
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
        newUpgrade.GetComponent<Upgrade>().SetValues("uncommon", 1f, 1.5f, 15, 0.025f, "Better Seeds", "Increased Chance of Encountering Uncommon Flowers");
        upgrades.Add(newUpgrade.GetComponent<Upgrade>());

        //playerSpeed
        newUpgrade = Instantiate(upgradeObj);
        newUpgrade.transform.SetParent(transform);
        newUpgrade.GetComponent<Upgrade>().SetValues("playerSpeed", 3f, 1.75f, 10, 0.5f, "Faster Shoes", "Incrases Player Movement Speed");
        upgrades.Add(newUpgrade.GetComponent<Upgrade>());

        //crownSlow
        newUpgrade = Instantiate(upgradeObj);
        newUpgrade.transform.SetParent(transform);
        newUpgrade.GetComponent<Upgrade>().SetValues("craftingSlow", 5f, 2f, 5, -0.05f, "Skilled Hands", "Decreases Slow Effect when Crafting a crown");
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
}
