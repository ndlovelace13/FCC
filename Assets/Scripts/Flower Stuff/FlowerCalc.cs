using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class FlowerData
{
    Vector2 position;
    string type;
    bool activated;
    GameObject flower;
    int rarity;
    int tier;

    public FlowerData(Vector2 newPos, string newType, int raritySelect)
    {
        position = newPos;
        type = newType;
        activated = false;
        flower = null;
        rarity = raritySelect;
        tier = 1;
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

    public int getTier()
    {
        return tier;
    }

    public void setTier(int newTier)
    {
        tier = newTier;
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
    static float rareRarity;
    //float undiscoveredRarity;
    List<string> Flowers = new List<string>();
    [SerializeField] List<string> common;
    [SerializeField] List<string> uncommon;
    [SerializeField] List<string> rare;
    List<string> undiscovered;
    int raritySelection;
    // Start is called before the first frame update

    void Start()
    {
        Debug.Log("Whyyyyy");
        //grab everything from GameControl when gameplay begins
        if (!GameControl.SaveData.discoveredUncommon.Contains("red"))
        {
            Debug.Log("discoveredUncommon is tweaked");
            GameControl.PlayerData.discoveryDisplay = false;
            GameControl.PlayerData.FlowerDiscovery("red");
        }
        else
            Debug.Log("SHRIMGUS " + GameControl.SaveData.discoveredUncommon.Count);
        //for testing newly added flowers
        if (GameControl.PlayerData.testing)
        {
            GameControl.PlayerData.uncommon = 0.5f;
            GameControl.PlayerData.discoveryDisplay = false;
            List<string> strings = new List<string>(GameControl.PlayerData.undiscoveredUncommon);
            foreach (var flower in strings)
            {
                GameControl.PlayerData.FlowerDiscovery(flower);
            }
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
        uncommon = GameControl.SaveData.discoveredUncommon;
        rare = GameControl.SaveData.discoveredRare;
        undiscovered = GameControl.PlayerData.undiscoveredUncommon;
        //undiscoveredRarity = GameControl.PlayerData.undiscovered;
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
                    
                    Vector2 finalPos = SpacingCalc(currentWidth, currentHeight);
                    FlowerData data = new FlowerData(finalPos, flowerType, raritySelection);
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

    private Vector2 SpacingCalc(float width, float height)
    {
        float finalWidth = Random.Range(width - 0.5f, width + 0.5f);
        float finalHeight = Random.Range(height - 0.5f, height + 0.5f);
        return new Vector2(finalWidth, finalHeight);
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
            if (rarityChoice > rareRarity && rare.Count > 0)
            {
                raritySelection = 2;
                Flowers = rare;
            }
            else
            {
                raritySelection = 1;
                Flowers = uncommon;
            }
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
