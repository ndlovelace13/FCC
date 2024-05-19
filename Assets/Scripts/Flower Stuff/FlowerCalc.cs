using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerData
{
    Vector2 position;
    string type;
    bool activated;
    GameObject flower;

    public FlowerData(Vector2 newPos, string newType)
    {
        position = newPos;
        type = newType;
        activated = false;
        flower = null;
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
    string[] Flowers;
    [SerializeField] string[] common;
    [SerializeField] string[] uncommon;
    // Start is called before the first frame update

    void Start()
    {
        uncommonRarity = PlayerPrefs.GetFloat("uncommon");
        totalWidth = (int)background.size.x;
        totalHeight = (int)background.size.y;
        flowerInfo = InitialCalc();
        Debug.Log(flowerInfo.Count);
        visibleFlowerHandler.GetComponent<VisibleFlowers>().FlowerEstablish(flowerInfo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private List<FlowerData> InitialCalc()
    {
        List<FlowerData> flowers = new List<FlowerData>();
        currentHeight = totalHeight / 2;
        while (currentHeight > -(totalHeight / 2))
        {
            currentWidth = -totalWidth / 2;
            while (currentWidth < totalWidth / 2)
            {
                int inc = RandIncrement();
                if (currentWidth + inc < totalWidth / 2)
                {
                    currentWidth += inc;
                    //Debug.Log("current Height:" + currentHeight + "\ncurrent Width:" + currentWidth);
                    string flowerType = FlowerChoice();
                    //random color gen needs to happen here
                    FlowerData data = new FlowerData(new Vector2(currentWidth, currentHeight), flowerType);
                    flowers.Add(data);
                    //Debug.Log("This is the current num:" + flowers);
                }
                else
                    break;
            }
            currentHeight -= 2;
        }
        //Debug.Log("This is the count" + flowers.Count);
        return flowers;
    }

    private string FlowerChoice()
    {
        //rarity calculations, 0.3f chance for uncommon, 0.7f chance for common at this stage
        float rarityChoice = Random.Range(0f, 1f);
        if (rarityChoice > uncommonRarity)
            Flowers = common;
        else
            Flowers = uncommon;
        //selection of a flower from the given list and spawning
        int flowerChoice = Random.Range(0, Flowers.Length);
        return Flowers[flowerChoice];
    }

    private int RandIncrement()
    {
        return Random.Range(3, 15);
    }
}
