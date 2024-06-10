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
    }
}
