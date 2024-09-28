using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UncommonSeedResearch : Research
{
    private void Start()
    {
        if (requiredSeeds == 0)
            requiredSeeds = 3;
        maxResearchTimes = GameControl.PlayerData.undiscoveredUncommon.Count - 1;

        //increment the total research possibilities
        //f (GameControl.SaveData.firstRun)
            
    }
    public override void ResearchAction()
    {
        requiredSeeds += 2;
        currentSeeds = 0;
        currentResearchTimes++;
        string type = GameControl.PlayerData.undiscoveredUncommon[Random.Range(0, GameControl.PlayerData.undiscoveredUncommon.Count)];
        GameControl.PlayerData.FlowerDiscovery(type);
        GameObject unlockNotif = Instantiate(unlockPrefab);
        unlockNotif.GetComponent<UnlockNotif>().BeginNotif(GameControl.PlayerData.SpriteAssign(type), "New Flower Discovered!");

        //update the totals for the almanac
        GameControl.SaveData.totalDrives++;
    }
}
