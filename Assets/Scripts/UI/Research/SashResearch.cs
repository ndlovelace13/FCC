using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SashResearch : Research
{
    private void Start()
    {
        requiredSeeds = 10;
        maxResearchTimes = 1;

        //increment the total research possibilities
        if (GameControl.SaveData.firstRun)
            GameControl.SaveData.researchAmount += maxResearchTimes;
    }
    public override void ResearchAction()
    {
        GameControl.SaveData.sashActivated = true;
        GameControl.SaveData.sashActive = true;
        currentResearchTimes++;
        GameObject unlockNotif = Instantiate(unlockPrefab);
        unlockNotif.GetComponent<UnlockNotif>().BeginNotif(resultImg, "Affinity Sash Unlocked!");
    }
}
