using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlowerInfoPage : Page
{
    // Start is called before the first frame update
    public string type;

    //UI Elements
    [SerializeField] Image flowerImage;
    [SerializeField] TMP_Text description;
    [SerializeField] Image flowerProj;
    [SerializeField] TMP_Text effects;

    //basic stats section
    [SerializeField] TMP_Text damageText;
    [SerializeField] TMP_Text rangeText;
    [SerializeField] TMP_Text projRangeText;
    [SerializeField] TMP_Text projCountText;
    [SerializeField] TMP_Text valueText;
    [SerializeField] GameObject tierButton;
    [SerializeField] TMP_Text tierNum;

    //wild handler
    int currentTier = 0;

    //special stats section - TODO
    [SerializeField] GameObject powerButton;
    [SerializeField] TMP_Text powerNum;
    [SerializeField] GameObject specialStatPrefab;
    [SerializeField] GameObject specialRow;
    [SerializeField] GameObject noSpecial;
    [SerializeField] TMP_Text noSpecialText;
    List<GameObject> specialObjs;
    int currentPower = 1;
    


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
        Debug.Log("Filling the page for " + type);

        //set the flower rarity text regardless of encounter
        switch (currentFlower.rarity)
        {
            case 0: subheader.text = "Common Flower"; break;
            case 1: subheader.text = "Uncommon Flower"; break;
            case 2: subheader.text = "Rare Flower"; break;
            case 3: subheader.text = "Bizarre Flower"; break;
            case 4: subheader.text = "Legendary Flower"; break;
        }

        //assign the flower and projectile image either way
        flowerImage.sprite = currentFlower.headSprite;
        flowerProj.sprite = currentFlower.projSprite;

        //check whether the flower is encountered or not - fill out the page if true, otherwise gray out and don't fill
        if (GameControl.PlayerData.savedFlowerDict[type].encountered)
        {
            title.text = GameControl.PlayerData.flowerStatsDict[type].title + " Info";
            //pull description from the flowerStats obj

            //if encountered, set the image alpha low
            flowerImage.color = new Color(flowerImage.color.r, flowerImage.color.g, flowerImage.color.b, 1 / 255f);
            description.text = currentFlower.description;
            flowerProj.color = new Color(flowerImage.color.r, flowerImage.color.g, flowerImage.color.b, 1 / 255f);
            effects.text = currentFlower.effects;

            //basic stats
            if (currentFlower.type != "wild")
            {
                damageText.text = currentFlower.damageTiers[currentTier].ToString();
                rangeText.text = currentFlower.rangeTiers[currentTier].ToString();
                projRangeText.text = currentFlower.GetProjRange(currentTier).ToString();
                projCountText.text = currentFlower.projTiers[currentTier].ToString();
                valueText.text = string.Format("{0:C}", currentFlower.pointsTiers[currentTier] / 100f);
                if (GameControl.SaveData.sashActive)
                {
                    tierButton.SetActive(true);
                    tierNum.text = currentTier.ToString();
                }
                else
                    tierButton.SetActive(false);
            }
            else
            {
                damageText.text = "X";
                rangeText.text = "X";
                projRangeText.text = "X";
                projCountText.text = "X";
                valueText.text = "X";
                tierButton.SetActive(false);
            }
            

            //special stats handler
            
            //retrieve the special stats
            List<SpecialStats> specialStats = currentFlower.GetSpecialValues(currentPower);
            specialObjs = new List<GameObject>();
            if (specialStats.Count > 0)
            {
                //Initialize the specialstat prefab for each one and fill the values
                foreach (var specialStat in specialStats)
                {
                    GameObject statObj = Instantiate(specialStatPrefab);
                    statObj.transform.SetParent(specialRow.transform);
                    statObj.transform.localScale = Vector3.one;
                    statObj.GetComponent<SpecialStats>().TransferData(specialStat);
                    specialObjs.Add(statObj);
                }

                if (GameControl.PlayerData.savedFlowerDict[type].highestPower > 1)
                {
                    powerButton.SetActive(true);
                    powerNum.text = currentPower.ToString();
                }
                else
                    powerButton.SetActive(false);
            }
            else
            {
                //handle the case in which there are no special values to report
                noSpecial.SetActive(true);
                if (currentFlower.type == "wild")
                    noSpecialText.text = "Replicates the Effects of Other Flowers";
                else
                    noSpecialText.text = "No Special Effects";
                powerButton.SetActive(false);
            }

            /*foreach (var prefab in prefabs)
            {
                prefab.GetComponent<LayoutElement>().height
            }*/

        }
        else
        {
            flowerImage.color = new Color(flowerImage.color.r, flowerImage.color.g, flowerImage.color.b, 1f);
            title.text = "???";
            description.text = "You haven't encountered this flower in the field! Come back once you've figured it out";
            flowerProj.color = new Color(flowerImage.color.r, flowerImage.color.g, flowerImage.color.b, 1f);
            effects.text = "Unknown";

            //basic stats
            damageText.text = "?";
            rangeText.text = "?";
            projRangeText.text = "?";
            projCountText.text = "?";
            valueText.text = "?";
            tierButton.SetActive(false);
            noSpecial.SetActive(true);
            noSpecialText.text = "Flower Abilities are Unknown";
            powerButton.SetActive(false);
        }
    }

    public void IncrementTier()
    {
        FlowerStats currentFlower = GameControl.PlayerData.flowerStatsDict[type];
        if (currentTier < GameControl.PlayerData.savedFlowerDict[type].highestTier)
        {
            currentTier++;
        }
        else
        {
            currentTier = 0;
        }
        //pull the values from the associated tier
        damageText.text = currentFlower.damageTiers[currentTier].ToString();
        rangeText.text = currentFlower.rangeTiers[currentTier].ToString();
        projRangeText.text = currentFlower.GetProjRange(currentTier).ToString();
        projCountText.text = currentFlower.projTiers[currentTier].ToString();
        valueText.text = string.Format("{0:C}", currentFlower.pointsTiers[currentTier] / 100f);
        tierNum.text = currentTier.ToString();
    }


    public void IncrementPower()
    {
        FlowerStats currentFlower = GameControl.PlayerData.flowerStatsDict[type];
        if (currentPower < GameControl.PlayerData.savedFlowerDict[type].highestPower)
        {
            currentPower++;
        }
        else
            currentPower = 1;

        //retrieve the special stats
        List<SpecialStats> specialStats = currentFlower.GetSpecialValues(currentPower);
        //Initialize the specialstat prefab for each one and fill the values
        for (int i = 0; i < specialStats.Count; i++)
        {
            specialObjs[i].GetComponent<SpecialStats>().TransferData(specialStats[i]);
        }
        powerNum.text = currentPower.ToString();
    }

    
}
