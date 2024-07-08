using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

//use this class to store all data about the results of the run
//add to a list in the saveData so the player can go back and look at run history if they want
[System.Serializable]
public class ShiftReport
{
    public int shiftNum;

    //MONEY STUFF
    public float scorePay;
    public List<float> scoreBreakdown;

    //construction - 0, enemies - 1, discovery - 2, other - 3
    public float timeBonus;

    public float totalProfit;
    public float withdrawals = 0f;
    public float finalBalance;

    //STATS
    public int timeMin;
    public int timeSec;

    public int enemies;
    public int seedsEarned;

    //FLOWER STATS
    public string mostUsed;
    public int mostUsedCount;

    //HIGH SCORE BOOLS
    public bool highTime = false;
    public bool highMoney = false;
    public bool highEnemies = false;
    public bool highSeeds = false;

    public ShiftReport(int shift, float scorePay, List<float> scores, float timeBonus, int seeds)
    {
        //set the values in the shiftReport
        shiftNum = shift;
        this.scorePay = scorePay;
        //scoreBreakdown = scores;
        scoreBreakdown = scores;

        this.timeBonus = timeBonus;
        totalProfit = this.scorePay + this.timeBonus;

        finalBalance = GameControl.SaveData.balance;

        //stats set
        seedsEarned = seeds;
        timeMin = GameControl.PlayerData.min;
        timeSec = GameControl.PlayerData.sec;
        enemies = GameControl.PlayerData.shiftEnemies;

        //most used - TODO Implement different view once the sash is unlocked
        foreach (var flower in GameControl.PlayerData.flowerUse)
        {
            Debug.Log("Type: " + flower.Key + " Times Played: " + flower.Value);
        }
        mostUsedCount = GameControl.PlayerData.flowerUse.Values.Max();
        mostUsed = GameControl.PlayerData.flowerUse.FirstOrDefault(x => x.Value == mostUsedCount).Key;


        //high score checking

        //money check
        if (GameControl.SaveData.highMoney < totalProfit)
        {
            GameControl.SaveData.highMoney = totalProfit;
            highMoney = true;
        }
        //time check
        if (GameControl.SaveData.highSec < timeSec && GameControl.SaveData.highMin <= timeMin || GameControl.SaveData.highMin < timeMin)
        {
            GameControl.SaveData.highMin = timeMin;
            GameControl.SaveData.highSec = timeSec;
            highTime = true;
        }
        //enemy check
        if (GameControl.SaveData.highEnemies < enemies)
        {
            GameControl.SaveData.highEnemies = enemies;
            highEnemies = true;
        }
        if (GameControl.SaveData.highSeeds < seeds)
        {
            GameControl.SaveData.highSeeds = seeds;
            highSeeds = true;
        }
    }

    public int GetShiftNum()
    {
        return shiftNum;
    }

    public float GetScorePay()
    {
        return scorePay;
    }

    public List<float> GetScoreBreakdown() 
    {
        //List<float> scoreBreakdown = new List<float> { constructionScore, enemyScore, discoveryScore, otherScore };
        return scoreBreakdown;
    }

    public float GetTimeBonus()
    {
        return timeBonus;
    }

    public float GetSeedsEarned()
    {
        return seedsEarned;
    }

    public float GetTotalProfit()
    {
        return totalProfit;
    }

    public void UpdateWithdrawals(float moneySpent)
    {
        withdrawals += moneySpent;
    }
    
    public float GetWithdrawals()
    {
        return withdrawals;
    }

    public float GetFinalBalance()
    {
        return finalBalance;
    }

    public void SetFinalBalance()
    {
        UpdateWithdrawals(finalBalance - GameControl.SaveData.balance);
        finalBalance = GameControl.SaveData.balance;
    }

    public int GetMin()
    {
        return timeMin;
    }

    public int GetSec()
    {
        return timeSec;
    }

    public int GetEnemies()
    {
        return enemies;
    }

    public Sprite GetUsedSprite()
    {
        Sprite headSprite = GameControl.PlayerData.flowerStatsDict[mostUsed].headSprite;
        return headSprite;
    }

    public int GetUsedCount()
    {
        return mostUsedCount;
    }

    public bool GetHighTime()
    {
        return highTime;
    }

    public bool GetHighMoney()
    {
        return highMoney;
    }

    public bool GetHighEnemies()
    {
        return highEnemies;
    }

    public bool GetHighSeeds()
    {
        return highSeeds;
    }
}

public class ReportBehavior : MonoBehaviour
{
    [SerializeField] TMP_Text header;
    [SerializeField] TMP_Text shiftNum;

    //money stuff
    [SerializeField] TMP_Text constructionText;
    [SerializeField] TMP_Text enemyText;
    [SerializeField] TMP_Text discoveryText;
    [SerializeField] TMP_Text miscText;
    [SerializeField] TMP_Text timeBonus;

    //totals
    [SerializeField] TMP_Text profitText;
    [SerializeField] TMP_Text withdrawalsText;
    [SerializeField] TMP_Text balanceText;

    //stats
    [SerializeField] TMP_Text statsHeader;
    [SerializeField] TMP_Text lineBreak;
    [SerializeField] TMP_Text timeSurvived;
    [SerializeField] TMP_Text moneyEarned;
    [SerializeField] TMP_Text enemiesElim;
    [SerializeField] TMP_Text seedsCollected;
    [SerializeField] Image usedFlower;
    [SerializeField] TMP_Text usedCount;
    [SerializeField] GameObject sig;


    // Start is called before the first frame update
    void Start()
    {
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {
        if (!GameControl.SaveData.firstRun)
            FillReport();
        else
            InitReport();
        StartCoroutine(LerpUp());
    }

    public void InitReport()
    {
        header.text = "Bank Statement";
        shiftNum.text = "Yikes, Buddy";
        constructionText.text = "<align=left>Lawn Mowing - <line-height=0>\n<align=right>+" +
            string.Format("{0:C}", 75f) + "<line-height=1em>";
        enemyText.text = "<align=left>Dog Walking - <line-height=0>\n<align=right>+" +
            string.Format("{0:C}", 30f) + "<line-height=1em>";
        discoveryText.text = "<align=left>Game Developing - <line-height=0>\n<align=right>+" +
            string.Format("{0:C}", 5f) + "<line-height=1em>";
        miscText.text = "<align=left>Substitute Teaching - <line-height=0>\n<align=right>+" +
            string.Format("{0:C}", 25f) + "<line-height=1em>";
        timeBonus.text = "<align=left>Public Defending - <line-height=0>\n<align=right>+" +
            string.Format("{0:C}", 100f) + "<line-height=1em>";
        profitText.text = "<align=center> Total Profit = <line-height=0>\n<align=right>+" +
            string.Format("{0:C}", 235f) + "<line-height=1em>";
        withdrawalsText.text = "<align=center> Withdrawals = <line-height=0>\n<align=right>-" +
            string.Format("{0:C}", 235f) + "<line-height=1em>";
        balanceText.text = "<align=center> Final Balance = <line-height=0>\n<align=right>" +
            string.Format("{0:C}", 0f) + "<line-height=1em>";

        statsHeader.text = "Account Status: BAH";
        lineBreak.gameObject.SetActive(false);
        timeSurvived.text = "(Broke as Hell)";
        timeSurvived.enabled = true;

        //disable everything else
        moneyEarned.enabled = false;
        enemiesElim.enabled = false;
        seedsCollected.enabled = false;
        usedFlower.enabled = false;
        usedCount.enabled = false;
        sig.SetActive(false);
    }

    public void FillReport()
    {
        ShiftReport currentReport = GameControl.SaveData.shiftReports.Last();
        currentReport.SetFinalBalance();
        List<float> scoreBreakdown = currentReport.GetScoreBreakdown();
        //fill in the payout info
        shiftNum.text = "Shift #" + currentReport.GetShiftNum();
        constructionText.text = "<align=left>Crown Construction - <line-height=0>\n<align=right>+" + 
            string.Format("{0:C}", scoreBreakdown[0]) + "<line-height=1em>";
        enemyText.text = "<align=left>Skinwalkers Eliminated - <line-height=0>\n<align=right>+" +
            string.Format("{0:C}", scoreBreakdown[1]) + "<line-height=1em>";
        discoveryText.text = "<align=left>Crown Discovery - <line-height=0>\n<align=right>+" +
            string.Format("{0:C}", scoreBreakdown[2]) + "<line-height=1em>";
        miscText.text = "<align=left>Miscellaneous Tasks - <line-height=0>\n<align=right>+" +
            string.Format("{0:C}", scoreBreakdown[3]) + "<line-height=1em>";
        timeBonus.text = "<align=left>Time Bonus - <line-height=0>\n<align=right>+" + 
            string.Format("{0:C}", currentReport.GetTimeBonus()) + "<line-height=1em>";
        profitText.text = "<align=center> Total Profit = <line-height=0>\n<align=right>+" +
            string.Format("{0:C}", currentReport.GetTotalProfit()) + "<line-height=1em>";
        withdrawalsText.text = "<align=center> Withdrawals = <line-height=0>\n<align=right>-" +
            string.Format("{0:C}", currentReport.GetWithdrawals()) + "<line-height=1em>";
        balanceText.text = "<align=center> Final Balance = <line-height=0>\n<align=right>" +
            string.Format("{0:C}", currentReport.GetFinalBalance()) + "<line-height=1em>";

        //shift stats & high score implementation
        timeSurvived.text = "Time Survived: " + string.Format("{0:00}:{1:00}", currentReport.GetMin(), currentReport.GetSec());
        if (currentReport.GetHighTime())
            timeSurvived.text += "<color=\"yellow\"> - New PB!";
        moneyEarned.text = "Money Earned: " + string.Format("{0:C}", currentReport.GetTotalProfit());
        if (currentReport.GetHighMoney())
            moneyEarned.text += "<color=\"yellow\"> - New PB!";
        enemiesElim.text = "Skinwalkers Eliminated: " + currentReport.GetEnemies();
        if (currentReport.GetHighEnemies())
            enemiesElim.text += "<color=\"yellow\"> - New PB!";
        seedsCollected.text = "Essence Seeds Collected: " + currentReport.GetSeedsEarned();
        if (currentReport.GetHighSeeds())
            seedsCollected.text += "<color=\"yellow\"> - New PB!";
        usedFlower.sprite = currentReport.GetUsedSprite();
        usedCount.text = "x" + currentReport.GetUsedCount();

    }

    IEnumerator LerpUp()
    {
        float time = 0f;
        Vector2 finalPos = GetComponent<RectTransform>().position;
        Vector2 startingPos = new Vector2(finalPos.x, -Screen.height * 1.5f);

        while (time < 1f)
        {
            GetComponent<RectTransform>().position = Vector2.Lerp(startingPos, finalPos, time);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    
}
