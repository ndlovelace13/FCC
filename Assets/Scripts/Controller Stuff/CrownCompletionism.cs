using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum crownType
{
    Basic,
    Advanced,
    Three,
    Complete,
    FullHouse,
    Four,
    Fiver
}

[System.Serializable]
public class CrownData
{
    public bool discovered, statusChanged;

    public string id, title;

    public int timesCrafted, shiftDiscovered;

    //call when initial discovery happens
    public CrownData(Crown discoveredCrown)
    {
        discovered = true;
        statusChanged = true;
        id = discoveredCrown.GetId();
        title = discoveredCrown.GetTitle();
        timesCrafted = 1;
        shiftDiscovered = discoveredCrown.GetShift();
    }
}

public class Crown
{
    bool discovered = false;
    bool discoverable = false;
    bool statusChanged = false;

    //eventually will want to be able to create a replica of these on will, may need to store the pools or the prefabs, we'll cross that bridge later
    /*GameObject CenterFlower;
    GameObject InsideFlower;
    GameObject OutsideFlower;*/
    string title;

    string primary;
    string inside;
    string outside;
    string id = "";

    crownType type;

    int timesCrafted = 0;
    int shiftDiscovered = 0;

    CrownData saveData;

    public void RestoreData(CrownData saveData)
    {
        this.saveData = saveData;
        //Restore all the important info from the save Data
        discovered = saveData.discovered;
        statusChanged = saveData.statusChanged;
        title = saveData.title;
        timesCrafted = saveData.timesCrafted;
        shiftDiscovered = saveData.shiftDiscovered;
    }

    public string SetCrownVals(string prim, string ins, string outs)
    {
        primary = prim;
        inside = ins;
        outside = outs;

        //savedFlowerData stuff

        //yeah fuck you unity i guess
        //identifier = { primary, inside, outside};
        Dictionary<string, FlowerStats> flowerDict = GameControl.PlayerData.flowerStatsDict;

        int primaryId = flowerDict[primary].id;
        int insideId = -1;
        int outsideId = -1;
        if (flowerDict.ContainsKey(inside))
            insideId = flowerDict[inside].id;
        if (flowerDict.ContainsKey(outside))
            outsideId = flowerDict[outside].id;
        id += primaryId.ToString() + insideId.ToString() + outsideId.ToString();
        //Debug.Log(id);
        SetType();
        return id;
    }

    private void SetType()
    {
        if (inside.CompareTo("") != 0 && outside.CompareTo("") != 0)
        {
            if (primary.CompareTo(inside) == 0 && primary.CompareTo(outside) == 0)
                type = crownType.Fiver;
            else if (primary.CompareTo(inside) != 0 && inside.CompareTo(outside) == 0)
                type = crownType.Four;
            else if (primary.CompareTo(inside) == 0 || primary.CompareTo(outside) == 0)
                type = crownType.FullHouse;
            else
                type = crownType.Complete;
        }
        else
        {
            if (primary.CompareTo(inside) == 0 || primary.CompareTo(outside) == 0)
                type = crownType.Three;
            else if (outside.CompareTo("") != 0 || inside.CompareTo("") != 0)
                type = crownType.Advanced;
            else
                type = crownType.Basic;
        }
        
    }

    public crownType GetStructure()
    {
        return type;
    }

    public string GetId()
    {
        return id;
    }

    public string GetTitle()
    {
        return title;
    }

    public bool IsDiscovered()
    {
        return discovered;
    }

    public bool Status()
    {
        bool status = statusChanged;
        statusChanged = false;
        if (saveData != null)
            saveData.statusChanged = false;
        return status;
    }

    public bool Discoverable()
    {
        if (discoverable)
            return discoverable;
        else if (discovered)
        {
            discoverable = false;
            return discoverable;
        }
            
        string[] includedFlowers = { primary, inside, outside };
        foreach (string flower in includedFlowers)
        {
            if (flower != "" && !GameControl.PlayerData.allDiscovered.Contains(flower))
            {
                discoverable = false;
                return discoverable;
            }
        }
        discoverable = true;
        statusChanged = true;
        return discoverable;
    }

    public void Discovery(string newTitle)
    {
        title = newTitle;
        discovered = true;
        discoverable = false;
        statusChanged = true;
        shiftDiscovered = GameControl.SaveData.shiftCounter;
        GameControl.CrownCompletion.totalDiscovered++;
        Crafted();
        Debug.Log(title + " discovered!");

        //Create a new crownData obj and add to the save list
        saveData = new CrownData(this);
        GameControl.SaveData.discoveredCrowns.Add(saveData);

        //increment the discoveredCounter in all associated flower save data
        List<string> flowers = GetFlowers().Distinct().ToList();
        foreach (var flower in flowers)
        {
            if (GameControl.PlayerData.savedFlowerDict.ContainsKey(flower))
                GameControl.PlayerData.savedFlowerDict[flower].discoveredCrowns++;
        }
    }

    public int GetShift()
    {
        return shiftDiscovered;
    }

    public int GetTimesCrafted()
    {
        return timesCrafted;
    }

    public void Crafted()
    {
        timesCrafted++;
        if (timesCrafted > 1)
            saveData.timesCrafted++;
        Debug.Log("You've crafted it " + timesCrafted + " times");

        //increment the crownCounter in all associated flower save data
        List<string> flowers = GetFlowers().Distinct().ToList();
        foreach (var flower in flowers)
        {
            if (GameControl.PlayerData.savedFlowerDict.ContainsKey(flower))
                GameControl.PlayerData.savedFlowerDict[flower].crownCount++;
        }
    }

    public List<string> GetFlowers()
    {
        List<string> flowers = new List<string>{ primary, inside, outside };
        return flowers;
    }
}

public class CrownCompletionism : MonoBehaviour
{
    //public static CrownCompletionism completionTracker;
    // Start is called before the first frame update

    public Dictionary<string, Crown> allCrowns;
    //public List<Crown> crowns = new List<Crown>();

    public GameObject nodePool;
    [SerializeField] GameObject nodePrefab;

    public int totalDiscovered = 0;

    public GameObject infoPopup;

    private void Awake()
    {
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PermutationEst()
    {
        if (allCrowns == null)
        {
            allCrowns = new Dictionary<string, Crown>();
            StartCoroutine(CrownPermutations());
        }
    }

    IEnumerator CrownPermutations()
    {
        FlowerStats[] flowers = GameControl.PlayerData.flowerStats;
        //for each flower as a primary
        foreach (var flower in flowers)
        {
            //add the crown with only primary symmetry
            Crown newCrown = new Crown();
            string id = newCrown.SetCrownVals(flower.type, "", "");
            allCrowns.Add(id, newCrown);
            //crowns.Add(newCrown);
            //add the crown with only inside symmetry - for each flower
            foreach (var insideFlower in flowers)
            {
                newCrown = new Crown();
                id = newCrown.SetCrownVals(flower.type, insideFlower.type, "");
                allCrowns.Add(id, newCrown);
                //crowns.Add(newCrown);

                foreach (var outsideFlower in flowers)
                {
                    //add the crown with only outside symmetry - for each flower
                    newCrown = new Crown();
                    id = newCrown.SetCrownVals(flower.type, "", outsideFlower.type);
                    if (!allCrowns.ContainsKey(id))
                    {
                        allCrowns.Add(id, newCrown);
                        //crowns.Add(newCrown);
                    }

                    //add the crown with inside and outside symmetry - for each flower
                    newCrown = new Crown();
                    id = newCrown.SetCrownVals(flower.type, insideFlower.type, outsideFlower.type);
                    if (!allCrowns.ContainsKey(id))
                    {
                        allCrowns.Add(id, newCrown);
                        //crowns.Add(newCrown);
                    }
                }
            }
        }
  

        Debug.Log("There are " + allCrowns.Count + " potential crowns");

        //Call the save data restore routine
        StartCoroutine(CrownDataRestore());
        //Call the saving data routine
        if (!GameControl.SaveData.crownCounterSet)
            StartCoroutine(CrownCounterSet());
        yield return null;
    }

    IEnumerator CrownDataRestore()
    {
        if (GameControl.SaveData.discoveredCrowns == null)
        {
            GameControl.SaveData.discoveredCrowns = new List<CrownData>();
            totalDiscovered = 0;
        }
        else
        {
            totalDiscovered = GameControl.SaveData.discoveredCrowns.Count;
            foreach (CrownData saveCrown in GameControl.SaveData.discoveredCrowns)
            {
                //Debug.Log(saveCrown.GetId());
                //Debug.Log(saveCrown.GetTitle());
                allCrowns[saveCrown.id].RestoreData(saveCrown);

                //add pre-discovered crowns to the counters if crowncounter is not set
                if (!GameControl.SaveData.crownCounterSet)
                {
                    List<string> flowers = allCrowns[saveCrown.id].GetFlowers().Distinct().ToList();
                    foreach (var flower in flowers)
                    {
                        if (GameControl.PlayerData.savedFlowerDict.ContainsKey(flower))
                            GameControl.PlayerData.savedFlowerDict[flower].discoveredCrowns++;
                    }
                }
            }
            Debug.Log(totalDiscovered + "crowns restored");
        }

        yield return null;
    }

    IEnumerator CrownCounterSet()
    {
        foreach (var crown in allCrowns)
        {
            List<string> flowers = crown.Value.GetFlowers().Distinct().ToList();
            foreach (var flower in flowers)
            {
                if (GameControl.PlayerData.savedFlowerDict.ContainsKey(flower))
                    GameControl.PlayerData.savedFlowerDict[flower].possibleCrowns++;
            }
        }
        GameControl.SaveData.crownCounterSet = true;
        yield return null;
    }

    public void nodePooling()
    {
        nodePool = Instantiate(GameControl.PlayerData.flowerPool);
        nodePool.name = "nodePool";
        nodePool.transform.parent = transform;
        nodePool.GetComponent<ObjectPool>().Establish(nodePrefab, allCrowns.Count);
        nodePool.GetComponent<ObjectPool>().Pooling();
    }
}
