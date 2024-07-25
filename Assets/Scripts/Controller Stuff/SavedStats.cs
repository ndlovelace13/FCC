using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//DO NOT USE
[System.Serializable]
public class SavedStats
{
    //totals
    public int shiftsComplete;
    public float totalIncome;
    public int totalCrowns;
    public int totalFlowers;
    public int totalKills;
    public int totalSeeds;
    public float totalSpent;
    public int totalUpgrades;
    public int totalDrives;

    //shiftRecords - pull from the saveData

    //favorites
    public string flowerType;
    public int flowerTimes;

    public string crownName;
    public string crownTimes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
