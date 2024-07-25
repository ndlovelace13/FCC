using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCompletionPage : Page
{
    [SerializeField] TMP_Text flowerTimes;
    [SerializeField] Image flowerImage;

    [SerializeField] TMP_Text crownName;
    [SerializeField] TMP_Text crownTimes;

    [SerializeField] TMP_Text flowerCompletion;
    [SerializeField] TMP_Text skinwalkerCompletion;
    [SerializeField] TMP_Text crownCompletion;
    [SerializeField] TMP_Text upgradeCompletion;
    [SerializeField] TMP_Text researchCompletion;

    [SerializeField] TMP_Text overallCompletion;

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
        flowerImage.sprite = GameControl.PlayerData.flowerStatsDict[GameControl.SaveData.mostUsedFlower].headSprite;
        flowerTimes.text = GameControl.SaveData.flowerTimes + " Times";

        crownName.text = GameControl.SaveData.mostUsedCrown;
        crownTimes.text = GameControl.SaveData.crownTimes + " Times";

        flowerCompletion.text = "<align=left>Flowers - (" +
            GameControl.PlayerData.allDiscovered.Count + "/" + GameControl.PlayerData.flowerStatsDict.Count + ")<line-height=0>\r\n<align=right>" +
            ((float)GameControl.PlayerData.allDiscovered.Count / GameControl.PlayerData.flowerStatsDict.Count).ToString("0.00%") + "<line-height=1em>";

        int skinwalkerEncountered = 0;
        foreach (var enemy in GameControl.PlayerData.savedEnemyDict)
        {
            if (enemy.Value.encountered)
                skinwalkerEncountered++;
        }

        skinwalkerCompletion.text = "<align=left>Skinwalkers - (" +
            skinwalkerEncountered + "/" + GameControl.PlayerData.savedEnemyDict.Count + ")<line-height=0>\r\n<align=right>" +
            ((float)skinwalkerEncountered / GameControl.PlayerData.savedEnemyDict.Count).ToString("0.00%") + "<line-height=1em>";

        crownCompletion.text = "<align=left>Crowns - (" +
            GameControl.CrownCompletion.totalDiscovered + "/" + GameControl.CrownCompletion.allCrowns.Count + ")<line-height=0>\r\n<align=right>" +
            ((float)GameControl.CrownCompletion.totalDiscovered / GameControl.CrownCompletion.allCrowns.Count).ToString("0.00%") + "<line-height=1em>";

        upgradeCompletion.text = "<align=left>Upgrades - (" +
            GameControl.SaveData.totalUpgrades + "/" + GameControl.SaveData.upgradeAmount + ")<line-height=0>\r\n<align=right>" +
            ((float)GameControl.SaveData.totalUpgrades / GameControl.SaveData.upgradeAmount).ToString("0.00%") + "<line-height=1em>";

        researchCompletion.text = "<align=left>Research - (" +
            GameControl.SaveData.totalDrives + "/" + GameControl.SaveData.researchAmount + ")<line-height=0>\r\n<align=right>" +
            ((float)GameControl.SaveData.totalDrives / GameControl.SaveData.researchAmount).ToString("0.00%") + "<line-height=1em>";

        int possible = GameControl.PlayerData.flowerStatsDict.Count + GameControl.PlayerData.savedEnemyDict.Count + GameControl.CrownCompletion.allCrowns.Count + GameControl.SaveData.upgradeAmount + GameControl.SaveData.researchAmount;
        int earned = GameControl.PlayerData.allDiscovered.Count + skinwalkerEncountered + GameControl.CrownCompletion.totalDiscovered + GameControl.SaveData.totalUpgrades + GameControl.SaveData.totalDrives;

        overallCompletion.text = ((float)earned / possible).ToString("0.00%");
    }
}
