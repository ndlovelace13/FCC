using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sticker : MonoBehaviour
{
    public string type;
    Sprite unlockedSticker;
    public int pageIndex;
    public bool isFlower;
    Button myButton;
    Almanac almanac;

    [SerializeField] Image stickerOutline;
    [SerializeField] Image stickerForeground;
    // Start is called before the first frame update
    void Start()
    {
        myButton = GetComponent<Button>();
        //find the almanac obj
        almanac = GameObject.FindWithTag("almanac").GetComponent<Almanac>();
        myButton.onClick.AddListener(MyButton_onClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*private void OnEnable()
    {
        if (unlockedSticker != null)
            DiscoveryCheck();
    }*/

    void MyButton_onClick()
    {
        almanac.ChangePages(pageIndex);
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
        stickerOutline.sprite = unlockedSticker;
        stickerForeground.sprite = unlockedSticker;

    }

    public int GetPageIndex()
    {
        return pageIndex;
    }

    public void SetPageIndex(int newIndex)
    {
        pageIndex = newIndex;
        //DiscoveryCheck();
    }

    public void DiscoveryCheck()
    {
        Debug.Log("calling discovery check for " + type);
        Color currentColor = GetComponent<Image>().color;
        if (isFlower)
        {
            Debug.Log(GameControl.PlayerData.savedFlowerDict.Count + "in the dict");
            //check whether the type has been encountered in save data
            if (!GameControl.PlayerData.savedFlowerDict[type].encountered)
            {
                stickerForeground.enabled = false;
                stickerOutline.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
            }
            else
            {
                stickerForeground.enabled = true;
                stickerOutline.color = new Color(1, 1, 1, 1f);
            }
        }
        else
        {
            Debug.Log(GameControl.PlayerData.savedEnemyDict.Count + " in the dict");
            if (!GameControl.PlayerData.savedEnemyDict[type].encountered)
            {
                stickerForeground.enabled = false;
                stickerOutline.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
            }
            else
            {
                stickerForeground.enabled = true;
                stickerOutline.color = new Color(1, 1, 1, 1f);
            }
        }
    }
}
