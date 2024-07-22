using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPage : Page
{
    // Start is called before the first frame update
    public string type;

    //description stuff
    [SerializeField] Image skinwalkerArt;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text behaviorText;
    [SerializeField] TMP_Text weaknessText;

    //Base stats
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text speedText;
    [SerializeField] TMP_Text valueText;

    //player stats
    [SerializeField] TMP_Text totalKills;
    [SerializeField] TMP_Text killRecord;
    [SerializeField] TMP_Text timesKilled;

    void Start()
    {
        
    }

    public void SetType(string newType)
    {
        type = newType;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void FillPage()
    {
        EnemyStats currentEnemy = GameControl.PlayerData.enemyStatsDict[type];

        //set the sprite
        skinwalkerArt.sprite = currentEnemy.sprite;


        if (GameControl.PlayerData.savedEnemyDict[type].encountered)
        {
            //currentEnemy = GameControl.PlayerData.enemyStatsDict[type];
            SavedEnemyStats savedInfo = GameControl.PlayerData.savedEnemyDict[type];

            skinwalkerArt.color = new Color(skinwalkerArt.color.r, skinwalkerArt.color.g, skinwalkerArt.color.b, 1f / 255);
            title.text = currentEnemy.GetTitle();
            subheader.text = currentEnemy.GetSubtext();
            descriptionText.text = currentEnemy.GetDescription();
            behaviorText.text = currentEnemy.GetBehavior();
            weaknessText.text = currentEnemy.GetWeakness();

            //stats
            healthText.text = currentEnemy.maxHealth.ToString();
            speedText.text = currentEnemy.minSpeed + "-" + currentEnemy.maxSpeed;
            valueText.text = string.Format("{0:C}", currentEnemy.killScore / 100f);

            //player stats
            totalKills.text = savedInfo.defeatedCount.ToString();
            killRecord.text = "Shift #" + savedInfo.shiftRecord + " - " + savedInfo.defeatedRecord;
            timesKilled.text = savedInfo.deathCount.ToString();
        }
        else
        {
            skinwalkerArt.color = new Color(skinwalkerArt.color.r, skinwalkerArt.color.g, skinwalkerArt.color.b, 1f);
            title.text = "???";
            subheader.text = "Undiscovered Variant";
            descriptionText.text = "You have not yet encountered this skinwalker variant";
            behaviorText.text = "Unknown Behavior";
            weaknessText.text = "Unknown Weaknesses";

            //stats
            healthText.text = "?";
            speedText.text = "?-?";
            valueText.text = "?";

            //player stats
            totalKills.text = "N/A";
            killRecord.text = "N/A";
            timesKilled.text = "N/A";
        }
    }
}
