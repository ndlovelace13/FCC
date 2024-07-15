using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerPage : Page
{
    [SerializeField] List<GameObject> stickerList;
    [SerializeField] GameObject stickerPrefab;
    public bool flowerStickers;
    bool stickersOrganized = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {
        if (flowerStickers)
            subheader.text = "-- Flowers --";
        else
            subheader.text = "-- Skinwalkers --";
        if (!stickersOrganized)
            StickerOrganize();
    }

    //organize the stickers into rows - this will need reworking as more flowers and enemies get added likely
    private void StickerOrganize()
    {
        Debug.Log("Stickers Being Organized");
        int stickerCount = stickerList.Count;
        int stickerDivisor = (int)Mathf.Floor(Mathf.Sqrt(stickerCount));
        int numRows = (stickerCount / stickerDivisor) + 1;
        int stickerIndex = 0;

        //initialize a horizontal grouping for each row in numRows
        for (int i = 0; i < numRows; i++)
        {
            GameObject row = new GameObject("Row" + i);
            row.transform.parent = transform;
            row.AddComponent<HorizontalLayoutGroup>();
            LayoutElement layout = row.AddComponent<LayoutElement>();
            layout.flexibleHeight = 1;

            //assign the stickers to a row
            int rowCount;
            if (i % 2 == 0)
                rowCount = stickerDivisor;
            else
                rowCount = stickerDivisor - 1;
            for (int j = 0; j < rowCount; j++)
            {
                stickerList[stickerIndex].transform.parent = row.transform;
                Debug.Log("Sticker " + stickerIndex + " assigned");
                stickerIndex++;
            }

        }
        stickersOrganized = true;
    }

    //constructor for flowers
    public void StickerAssign(Dictionary<string, FlowerStats> allFlowers)
    {
        Debug.Log("flower stickers initialized");
        //indicate that this page is for flowers
        flowerStickers = true;
        stickerList = new List<GameObject>();
        foreach (var flower in allFlowers)
        {
            //make a flower sticker for each flower, add it to the list
            GameObject newSticker = Instantiate(stickerPrefab);
            newSticker.transform.parent = transform;
            Sticker flowerSticker = newSticker.GetComponent<Sticker>();
            flowerSticker.SetType(flower.Key, flowerStickers);
            stickerList.Add(newSticker);
        }
        //StickerOrganize();
    }

    //constructor for enemies
    public void StickerAssign(Dictionary<string, EnemyStats> allEnemies)
    {
        Debug.Log("enemy stickers initialized");
        flowerStickers = false;
        stickerList = new List<GameObject>();
        foreach (var enemy in allEnemies)
        {
            //make an enemy sticker for each enemy, add it to the list
            GameObject newSticker = Instantiate(stickerPrefab);
            newSticker.transform.parent = transform;
            Sticker enemySticker = newSticker.GetComponent<Sticker>();
            enemySticker.SetType(enemy.Key, flowerStickers);
            stickerList.Add(newSticker);
        }
        //StickerOrganize();
    }

    public List<Page> PageInit()
    {
        List<Page> associatedPages = new List<Page>();
        
        //create the associatedPages for each sticker
        Debug.Log("sticker count: " +  stickerList.Count);
        foreach (GameObject sticker in stickerList)
        {
            //make two pages for each flower and only one for enemy
            if (flowerStickers)
            {
                //pull the flowerStats from the sticker so that the created info and stats page match that flower
                GameObject infoObject = Instantiate(GameControl.PlayerData.flowerInfoPage);
                FlowerInfoPage flowerInfo = infoObject.GetComponent<FlowerInfoPage>();

                GameObject statObject = Instantiate(GameControl.PlayerData.flowerStatPage);
                FlowerStatPage flowerStat = statObject.GetComponent<FlowerStatPage>();
                sticker.GetComponent<Sticker>().SetLink(flowerInfo);
                associatedPages.Add(flowerInfo);
                associatedPages.Add(flowerStat);

                infoObject.transform.SetParent(transform.parent);
                statObject.transform.SetParent(transform.parent);
            }
            else
            {
                //pull the enemyStats from the sticker so that the created stats page matches the enemy
                GameObject enemyObj = Instantiate(GameControl.PlayerData.enemyPage);
                EnemyPage enemyInfo = enemyObj.GetComponent<EnemyPage>();
                sticker.GetComponent<Sticker>().SetLink(enemyInfo);
                associatedPages.Add(enemyInfo);

                enemyObj.transform.SetParent(transform.parent);
            }
        }

        return associatedPages;
    }

    
}
