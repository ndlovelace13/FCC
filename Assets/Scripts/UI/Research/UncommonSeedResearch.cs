using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UncommonSeedResearch : Research
{
    private void Start()
    {
        requiredSeeds = 3;
        maxResearchTimes = GameControl.PlayerData.undiscoveredUncommon.Count - 1;
    }
    public override void ResearchAction()
    {
        requiredSeeds += 2;
        currentSeeds = 0;
        currentResearchTimes++;
        string type = GameControl.PlayerData.undiscoveredUncommon[Random.Range(0, GameControl.PlayerData.undiscoveredUncommon.Count)];
        GameControl.PlayerData.FlowerDiscovery(type);
    }
}
