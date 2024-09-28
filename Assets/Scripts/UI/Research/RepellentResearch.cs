using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepellentResearch : Research
{
    // Start is called before the first frame update
    void Start()
    {
        if (requiredSeeds == 0)
            requiredSeeds = 5;
        maxResearchTimes = 3;

    }

    // Update is called once per frame
    public override void ResearchAction()
    {
        requiredSeeds += 5;
        currentSeeds = 0;
        currentResearchTimes++;
        GameControl.SaveData.repellentCount++;
        GameObject unlockNotif = Instantiate(unlockPrefab);
        unlockNotif.GetComponent<UnlockNotif>().BeginNotif(resultImg, "Additional Repellent Unlocked!");

        //update the totals for the almanac
        GameControl.SaveData.totalDrives++;
    }
}
