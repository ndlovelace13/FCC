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

        //set the flower rarity text regardless of encounter
        switch (currentFlower.rarity)
        {
            case 0: subheader.text = "Common Flower"; break;
            case 1: subheader.text = "Uncommon Flower"; break;
            case 2: subheader.text = "Rare Flower"; break;
            case 3: subheader.text = "Bizarre Flower"; break;
            case 4: subheader.text = "Legendary Flower"; break;
        }

        //check whether the flower is encountered or not - fill out the page if true, otherwise gray out and don't fill
        if (GameControl.PlayerData.savedFlowerDict[type].encountered)
        {
            title.text = GameControl.PlayerData.flowerStatsDict[type].title;
            //pull description from the flowerStats obj
            flowerImage.sprite = currentFlower.headSprite;
            description.text = currentFlower.description;
            
        }
        else
        {
            title.text = "???";
            description.text = "You haven't encountered this flower in the field! Come back once you've figured it out";
        }
    }
}
