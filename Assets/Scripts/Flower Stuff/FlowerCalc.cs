using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerData
{
    Vector2 position;
    string type;
    bool activated;
    GameObject flower;
    int rarity;

    public FlowerData(Vector2 newPos, string newType, int raritySelect)
    {
        position = newPos;
        type = newType;
        activated = false;
        flower = null;
        rarity = raritySelect;
    }

    public Vector2 getPosition()
    {
        return position;
    }
    public string getType()
    {
        return type;
    }

    public void setType(string newType)
    {
        type = newType;
    }

    public bool isActivated()
    {
        return activated;
    }

    public void SetActivated(bool newActive) 
    {
        activated = newActive;
    }

    public GameObject getFlower()
    {
        return flower;
    }

    public void setFlower(GameObject newFlower)
    {
        flower = newFlower;
    }

    public int getRarity()
    {
        //Debug.Log("Rarity is : " + rarity);
        return rarity;
    }

    public void setRarity(int newRarity)
    {
        rarity = newRarity;
    }
}

public class FlowerCalc : MonoBehaviour
{
    [SerializeField] GameObject visibleFlowerHandler;
    [SerializeField] SpriteRenderer background;
    int currentWidth;
    int currentHeight;
    int totalWidth;
    int totalHeight;
    List<FlowerData> flowerInfo;

    //rarity shit
    static float uncommonRarity;
    float undiscoveredRarity;
    List<string> Flowers;
    [SerializeField] List<string> common;
    [SerializeField] List<string> uncommon;
    List<string> undiscovered;
    int raritySelection;
    // Start is called before the first frame update

    void Start()
    {
        //grab everything from GameControl when gameplay begins
        if (GameControl.PlayerData.tutorialSkip && GameControl.PlayerData.discoveredUncommon.Count == 0)
        {
            GameControl.PlayerData.discoveryDisplay = false;
            GameControl.PlayerData.FlowerDiscovery("red");
        }
        //for testing newly added flowers
        if (GameControl.PlayerData.testing)
        {
            GameControl.PlayerData.uncommon = 0.5f;
            GameControl.PlayerData.discoveryDisplay = false;
            GameControl.PlayerData.FlowerDiscovery("red");
            GameControl.PlayerData.FlowerDiscovery("sunny");
        }
        GameControl.PlayerData.ResetRun();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PreroundCalc()
    {
        StartCoroutine(Calculating());   
    }

    IEnumerator Calculating()
    {
        GameControl.PlayerData.discoveryDisplay = true;
        uncommonRarity = GameControl.PlayerData.uncommon;
        common = GameControl.PlayerData.commonPool;
        uncommon = GameControl.PlayerData.discoveredUncommon;
        undiscovered = GameControl.PlayerData.undiscoveredUncommon;
        undiscoveredRarity = GameControl.PlayerData.undiscovered;
        totalWidth = (int)background.size.x;
        totalHeight = (int)background.size.y;
        flowerInfo = InitialCalc();
        Debug.Log(flowerInfo.Count);
        visibleFlowerHandler.GetComponent<VisibleFlowers>().FlowerEstablish(flowerInfo);
        yield return null;
    }

    private List<FlowerData> InitialCalc()
    {
        List<FlowerData> flowers = new List<FlowerData>();
        currentHeight = totalHeight / 2;
        while (currentHeight > -(totalHeight / 2))
        {
            currentWidth = -totalWidth / 2 - RandIncrement();
            while (currentWidth < totalWidth / 2)
            {
                int inc = RandIncrement();
                if (currentWidth + inc < totalWidth / 2)
                {
                    currentWidth += inc;
                    //Debug.Log("current Height:" + currentHeight + "\ncurrent Width:" + currentWidth);
                    string flowerType = FlowerChoice();
                    //random color gen needs to happen here
                    FlowerData data = new FlowerData(new Vector2(currentWidth, currentHeight), flowerType, raritySelection);
                    flowers.Add(data);
                    //Debug.Log("This is the current num:" + flowers);
                }
                else
                    break;
            }
            currentHeight -= 2;
        }
        //Debug.Log("This is the count" + flowers.Count);
        //TO DO - SECOND WAVE OF RANDOMIZATION, random movement within a 1 square grid
        return flowers;
    }

    private string FlowerChoice()
    {
        //rarity calculations, 0.3f chance for uncommon, 0.7f chance for common at this stage
        float rarityChoice = Random.Range(0f, 1f);
        if (rarityChoice > uncommonRarity)
        {
            Flowers = common;
            raritySelection = 0;
        }
        else
        {
            rarityChoice = Random.Range(0f, 1f);
            raritySelection = 1;
            Flowers = uncommon;
        }
        //selection of a flower from the given list and spawning
        int flowerChoice = Random.Range(0, Flowers.Count);
        return Flowers[flowerChoice];
    }

    private int RandIncrement()
    {
        return Random.Range(5, 15);
    }
}
