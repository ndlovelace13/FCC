using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildStats : FlowerStats
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string WildTypeRandomize()
    {
        List<string> noWild = new List<string>(GameControl.PlayerData.allDiscovered);
        noWild.Remove("wild");
        int discoveredCount = noWild.Count;
        string newType = noWild[UnityEngine.Random.Range(0, discoveredCount)];
        Debug.Log("Wild is become " + newType + ", destroyer of worlds");
        return newType;
    }

    public override string Colorizer(string input)
    {
        Color rando = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        string colorText = ColorUtility.ToHtmlStringRGB(rando);
        string returnString = "<color=#" + colorText + ">";
        returnString += input + "</color>";
        return returnString;
    }



    //INSTEAD - Keep the Wild FlowerStats object, replace the flowerPos - pull stats from the wild's affinity tier and apply to basePoints, projRange, range, damage, projCount, 
    public FlowerStats WildDataApply(FlowerStats original, string type)
    {
        FlowerStats copiedData = original;
        int currentWildAffinity = 0;
        if (GameControl.SaveData.sashActive && GameControl.PlayerData.affinityAmounts.ContainsKey("wild"))
            currentWildAffinity = GameControl.PlayerData.affinityAmounts["wild"];
        copiedData.SetPoints(GameControl.PlayerData.flowerStatsDict[type].pointsTiers[currentWildAffinity]);
        copiedData.SetProjRange(GameControl.PlayerData.flowerStatsDict[type].GetProjRange(1));
        copiedData.SetRange(GameControl.PlayerData.flowerStatsDict[type].rangeTiers[currentWildAffinity]);
        copiedData.SetDamage(GameControl.PlayerData.flowerStatsDict[type].damageTiers[currentWildAffinity]);
        copiedData.SetProjCount(GameControl.PlayerData.flowerStatsDict[type].projTiers[currentWildAffinity]);
        copiedData.aug = GameControl.PlayerData.flowerStatsDict[type].aug;
        return copiedData;
    }

    public Tuple<List<FlowerBehavior>, List<FlowerStats>> WildHandler(List<FlowerBehavior> flowerPos, List<FlowerStats> flowerData)
    {
        //if middle is wild, change that first
        if (flowerPos[2].type == "wild")
        {
            flowerPos[2].type = WildTypeRandomize();
            flowerData[2] = WildDataApply(flowerData[2], flowerPos[2].type);
        }
        //check inside, change both to match
        if (flowerPos[1].type == "wild")
        {
            if (flowerPos[3].type == "wild")
            {
                //both need to match
                string newType = WildTypeRandomize();
                flowerPos[1].type = newType;
                flowerPos[3].type = newType;
                FlowerStats newData = WildDataApply(flowerData[1], newType);
                flowerData[1] = newData;
                flowerData[3] = newData;
            }
            else
            {
                //first inside needs to match second
                flowerPos[1].type = flowerPos[3].type;
                flowerData[1] = WildDataApply(flowerData[1], flowerPos[1].type);
            }
        }
        else if (flowerPos[3].type == "wild")
        {
            //second inside needs to match first
            flowerPos[3].type = flowerPos[1].type;
            flowerData[3] = WildDataApply(flowerData[3], flowerPos[3].type);
        }

        //check outside, change both to match
        if (flowerPos[0].type == "wild")
        {
            if (flowerPos[4].type == "wild")
            {
                //both need to match
                string newType = WildTypeRandomize();
                flowerPos[0].type = newType;
                flowerPos[4].type = newType;
                FlowerStats newData = WildDataApply(flowerData[0], newType);
                flowerData[0] = newData;
                flowerData[4] = newData;
            }
            else
            {
                //first inside needs to match second
                flowerPos[0].type = flowerPos[4].type;
                flowerData[0] = WildDataApply(flowerData[0], flowerPos[0].type);
            }
        }
        else if (flowerPos[4].type == "wild")
        {
            //second inside needs to match first
            flowerPos[4].type = flowerPos[0].type;
            flowerData[4] = WildDataApply(flowerData[4], flowerPos[4].type);
        }

        return new Tuple<List<FlowerBehavior>, List<FlowerStats>>(flowerPos, flowerData);
}
}
