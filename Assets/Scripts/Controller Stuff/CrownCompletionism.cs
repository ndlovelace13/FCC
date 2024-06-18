using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    int timesCrafted = 0;

    public string SetCrownVals(string prim, string ins, string outs)
    {
        primary = prim;
        inside = ins;
        outside = outs;

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
        return id;
    }

    public string GetId()
    {
        return id;
    }

    public bool IsDiscovered()
    {
        return discovered;
    }

    public bool Status()
    {
        bool status = statusChanged;
        statusChanged = false;
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
        CrownCompletionism.completionTracker.totalDiscovered++;
        Crafted();
        Debug.Log(title + " discovered!");
    }

    public int GetTimesCrafted()
    {
        return timesCrafted;
    }

    public void Crafted()
    {
        timesCrafted++;
        Debug.Log("You've crafted it " + timesCrafted + " times");
    }

    public List<string> GetFlowers()
    {
        List<string> flowers = new List<string>{ primary, inside, outside };
        return flowers;
    }
}

public class CrownCompletionism : MonoBehaviour
{
    public static CrownCompletionism completionTracker;
    // Start is called before the first frame update

    public Dictionary<string, Crown> allCrowns;
    public List<Crown> crowns = new List<Crown>();

    public GameObject nodePool;
    [SerializeField] GameObject nodePrefab;

    public int totalDiscovered = 0;

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
        if (completionTracker == null)
        {
            completionTracker = this;
        }
        else
        {
            Destroy(gameObject);
        }

        allCrowns = new Dictionary<string, Crown>();
        StartCoroutine(CrownPermutations());
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
            crowns.Add(newCrown);
            //add the crown with only inside symmetry - for each flower
            foreach (var insideFlower in flowers)
            {
                newCrown = new Crown();
                id = newCrown.SetCrownVals(flower.type, insideFlower.type, "");
                allCrowns.Add(id, newCrown);
                crowns.Add(newCrown);

                foreach (var outsideFlower in flowers)
                {
                    //add the crown with only outside symmetry - for each flower
                    newCrown = new Crown();
                    id = newCrown.SetCrownVals(flower.type, "", outsideFlower.type);
                    if (!allCrowns.ContainsKey(id))
                    {
                        allCrowns.Add(id, newCrown);
                        crowns.Add(newCrown);
                    }

                    //add the crown with inside and outside symmetry - for each flower
                    newCrown = new Crown();
                    id = newCrown.SetCrownVals(flower.type, insideFlower.type, outsideFlower.type);
                    if (!allCrowns.ContainsKey(id))
                    {
                        allCrowns.Add(id, newCrown);
                        crowns.Add(newCrown);
                    }
                }
            }
        }
  

        Debug.Log("There are " + allCrowns.Count + " potential crowns");
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
