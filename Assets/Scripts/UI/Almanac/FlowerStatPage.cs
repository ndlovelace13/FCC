using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlowerStatPage : Page
{
    public string type;

    //Stats Section
    [SerializeField] TMP_Text totalCrowns;
    [SerializeField] TMP_Text singleFlowers;
    [SerializeField] GameObject tierObj;
    [SerializeField] TMP_Text highestTier;
    [SerializeField] TMP_Text crownCompletion;
    [SerializeField] TMP_Text shiftHarvestRecord;
    [SerializeField] TMP_Text totalHarvestCount;

    //MASTERY SECTION - TO DO

    //Titles Section
    [SerializeField] TMP_Text primary;
    [SerializeField] TMP_Text interior;
    [SerializeField] TMP_Text exterior;
    [SerializeField] TMP_Text four;
    [SerializeField] TMP_Text fiver;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetType(string newType)
    {
        type = newType;
    }

    public override void FillPage()
    {
        FlowerStats currentFlower = GameControl.PlayerData.flowerStatsDict[type];
        SavedFlowerStats currentSaveData = GameControl.PlayerData.savedFlowerDict[type];

        Debug.Log("Filling the stats page for " + type);


        //check whether the player has encountered
        if (GameControl.PlayerData.savedFlowerDict[type].encountered)
        {
            //title
            title.text = "Your " + currentFlower.title + " Stats";

            //set the stats according to the savedFlower item
            totalCrowns.text = currentSaveData.crownCount.ToString();
            singleFlowers.text = currentSaveData.singleCount.ToString();

            //handle the tier stuff
            if (GameControl.SaveData.sashActive)
            {
                tierObj.SetActive(true);
                highestTier.text = currentSaveData.highestTier.ToString();
            }
            else
                tierObj.SetActive(false);

            //crown completion stuff - TODO
            crownCompletion.text = "bruh";

            shiftHarvestRecord.text = "Shift #" + currentSaveData.highestShift + " - " + currentSaveData.highestHarvest;
            totalHarvestCount.text = currentSaveData.harvestCount.ToString();

            //handle name stuff - add in checks for discovyery here
            primary.text = currentFlower.primaryText;
            interior.text = currentFlower.insideText;
            exterior.text = currentFlower.outsideText;
            four.text = currentFlower.fourText;
            fiver.text = currentFlower.fiveText;
        }
        else
        {
            title.text = "Your ??? Stats";

            //set the stats according to the savedFlower item
            totalCrowns.text = "N/A";
            singleFlowers.text = "N/A";

            //handle the tier stuff
            tierObj.SetActive(false);

            //crown completion stuff - TODO
            crownCompletion.text = "N/A";

            shiftHarvestRecord.text = "N/A";
            totalHarvestCount.text = "N/A";

            //handle name stuff - add in checks for discovyery here
            primary.text = "Unknown";
            interior.text = "Unknown";
            exterior.text = "Unknown";
            four.text = "Unknown";
            fiver.text = "Unknown";
        }
    }
}
