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
    [SerializeField] Button crownInfoButton;
    [SerializeField] Button careerButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void FillPage()
    {
        if (flowerStickers)
            subheader.text = "-- Flowers --";
        else
        {
            subheader.text = "-- Skinwalkers --";
        }
        if (!stickersOrganized)
            StickerOrganize();
    }

    //organize the stickers into rows - this will need reworking as more flowers and enemies get added likely
    private void StickerOrganize()
    {
        Debug.Log("Stickers Being Organized");
        int stickerCount = stickerList.Count;
        int stickerDivisor = (int)Mathf.Floor(Mathf.Sqrt(stickerCount));
        int numRows = (stickerCount / stickerDivisor);
        if (flowerStickers)
            numRows++;
        if (stickerCount < 4)
            numRows = 1;
        int stickerIndex = 0;

        //initialize a horizontal grouping for each row in numRows
        for (int i = 0; i < numRows; i++)
        {
            GameObject row = new GameObject("Row" + i);
            row.transform.parent = transform;
            row.transform.localScale = Vector3.one;
            row.AddComponent<HorizontalLayoutGroup>();
            LayoutElement layout = row.AddComponent<LayoutElement>();
            layout.flexibleHeight = 1;

            //assign the stickers to a row
            int rowCount;
            if (i < numRows - 1)
            {
                if (i % 2 == 0)
                    rowCount = stickerDivisor;
                else
                    rowCount = stickerDivisor - 1;
            }
            else
                rowCount = 5;
            for (int j = 0; j < rowCount; j++)
            {
                if (stickerIndex < stickerList.Count)
                {
                    stickerList[stickerIndex].transform.SetParent(row.transform);
                    stickerList[stickerIndex].GetComponent<Sticker>().DiscoveryCheck();
                    Debug.Log("Sticker " + stickerIndex + " assigned");
                    stickerIndex++;
                }
                else
                {
                    break;
                }
            }

        }
        stickersOrganized = true;
        if (!flowerStickers)
        {
            Almanac almanac = GameObject.FindWithTag("almanac").GetComponent<Almanac>();
            crownInfoButton.gameObject.SetActive(true);
            crownInfoButton.transform.SetAsLastSibling();
            crownInfoButton.onClick.AddListener(() => almanac.ChangePages(GameControl.PlayerData.almanacPages.Count - 4));
            careerButton.gameObject.SetActive(true);
            careerButton.transform.SetAsLastSibling();
            careerButton.onClick.AddListener(() => almanac.ChangePages(GameControl.PlayerData.almanacPages.Count - 2));
        }
        else
        {
            crownInfoButton.gameObject.SetActive(false);
            careerButton.gameObject.SetActive(false);
        }
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
            newSticker.transform.SetParent(transform);
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
            newSticker.transform.SetParent(transform);
            Sticker enemySticker = newSticker.GetComponent<Sticker>();
            enemySticker.SetType(enemy.Key, flowerStickers);
            stickerList.Add(newSticker);
        }
        //StickerOrganize();
    }

    public List<Page> PageInit(int offset)
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
                flowerInfo.SetType(sticker.GetComponent<Sticker>().type);

                GameObject statObject = Instantiate(GameControl.PlayerData.flowerStatPage);
                FlowerStatPage flowerStat = statObject.GetComponent<FlowerStatPage>();
                flowerStat.SetType(sticker.GetComponent<Sticker>().type);
                //add to the array and get the index of the first page added
                associatedPages.Add(flowerInfo);
                sticker.GetComponent<Sticker>().SetPageIndex(associatedPages.Count - 1 + offset);
                associatedPages.Add(flowerStat);

                infoObject.transform.SetParent(transform.parent);
                statObject.transform.SetParent(transform.parent);
            }
            else
            {
                //pull the enemyStats from the sticker so that the created stats page matches the enemy
                GameObject enemyObj = Instantiate(GameControl.PlayerData.enemyPage);
                EnemyPage enemyInfo = enemyObj.GetComponent<EnemyPage>();
                //add to the array and get the index
                associatedPages.Add(enemyInfo);
                sticker.GetComponent<Sticker>().SetPageIndex(associatedPages.Count - 1 + offset);
                enemyInfo.SetType(sticker.GetComponent<Sticker>().type);

                enemyObj.transform.SetParent(transform.parent);
            }
        }

        return associatedPages;
    }

    
}
