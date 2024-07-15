using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sticker : MonoBehaviour
{
    string type;
    Sprite unlockedSticker;
    Page pageLink;
    bool isFlower;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetStickerType()
    {
        return type;
    }

    public void SetType(string newType, bool flower)
    {
        Debug.Log(newType);
        isFlower = flower;
        type = newType;
        if (isFlower)
        {
            unlockedSticker = GameControl.PlayerData.flowerStatsDict[type].headSprite;

        }
        else
        {
            unlockedSticker = GameControl.PlayerData.enemyStatsDict[type].sprite;
        }
        //apply a check here to see if its unlocked, don't show if it isn't
        GetComponent<Image>().sprite = unlockedSticker;

    }

    public Page GetLink()
    {
        return pageLink;
    }

    public void SetLink(Page newLink)
    {
        pageLink = newLink;
    }
}
