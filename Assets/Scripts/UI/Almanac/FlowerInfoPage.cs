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
    int currentTier = 0;

    //special stats section - TODO
    [SerializeField] GameObject powerButton;
    


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
            damageText.text = currentFlower.damageTiers[currentTier].ToString();
            rangeText.text = currentFlower.rangeTiers[currentTier].ToString();
            projRangeText.text = currentFlower.GetProjRange(currentTier).ToString();
            projCountText.text = currentFlower.projTiers[currentTier].ToString();
            valueText.text = string.Format("{0:C}", currentFlower.pointsTiers[currentTier] / 100f);
            if (GameControl.SaveData.sashActive)
                tierButton.SetActive(true);
            else
                tierButton.SetActive(false);

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
        }
    }
}
