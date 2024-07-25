using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsPage : Page
{
    //Totals
    [SerializeField] TMP_Text shiftsComplete;
    [SerializeField] TMP_Text totalIncome;
    [SerializeField] TMP_Text totalCrowns;
    [SerializeField] TMP_Text totalFlowers;
    [SerializeField] TMP_Text totalSkinwalkers;
    [SerializeField] TMP_Text totalEssence;
    [SerializeField] TMP_Text totalSpent;
    [SerializeField] TMP_Text totalUpgrades;
    [SerializeField] TMP_Text totalDrives;

    //Records
    [SerializeField] TMP_Text highMoney;
    [SerializeField] TMP_Text highTime;
    [SerializeField] TMP_Text highCrowns;
    [SerializeField] TMP_Text highDiscoveries;
    [SerializeField] TMP_Text highKills;
    [SerializeField] TMP_Text highSeeds;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void FillPage()
    {
        //fill the totals
        shiftsComplete.text = GameControl.SaveData.shiftCounter + " Shifts Completed";
        totalIncome.text = string.Format("{0:C}", GameControl.SaveData.totalIncome) + " Earned"; ;
        totalCrowns.text = GameControl.SaveData.totalCrowns + " Crowns Crafted";
        totalFlowers.text = GameControl.SaveData.totalFlowers + " Flowers Harvested";
        totalSkinwalkers.text = GameControl.SaveData.totalKills + " Skinwalkers Eliminated";
        totalEssence.text = GameControl.SaveData.totalSeeds + " Essence Seeds Collected";
        totalSpent.text = string.Format("{0:C}", GameControl.SaveData.totalIncome) + " Spent";
        totalUpgrades.text = GameControl.SaveData.totalUpgrades + " Upgrades Purchased";
        totalDrives.text = GameControl.SaveData.totalDrives + " Research Drives Completed";

        //fill the records
        highMoney.text = string.Format("{0:C}", GameControl.SaveData.highMoney);
        highTime.text = string.Format("{0:00}:{1:00}", GameControl.SaveData.highMin, GameControl.SaveData.highSec);
        highCrowns.text = GameControl.SaveData.highCrowns.ToString();
        highDiscoveries.text = GameControl.SaveData.highDiscoveries.ToString();
        highKills.text = GameControl.SaveData.highEnemies.ToString();
        highSeeds.text = GameControl.SaveData.highSeeds.ToString();
    }
}
