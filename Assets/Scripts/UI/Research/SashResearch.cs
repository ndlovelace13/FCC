using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SashResearch : Research
{
    private void Start()
    {
        requiredSeeds = 10;
        maxResearchTimes = 1;
    }
    public override void ResearchAction()
    {
        GameControl.PlayerData.sashActivated = true;
        GameControl.PlayerData.sashActive = true;
        currentResearchTimes++;
        GameObject unlockNotif = Instantiate(unlockPrefab);
        unlockNotif.GetComponent<UnlockNotif>().BeginNotif(resultImg, "Affinity Sash Unlocked!");
    }
}
