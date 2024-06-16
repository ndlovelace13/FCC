using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crown
{
    bool discovered = false;

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

    public bool IsDiscovered()
    {
        return discovered;
    }

    public void Discovery(string newTitle)
    {
        title = newTitle;
        discovered = true;
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
}

public class CrownCompletionism : MonoBehaviour
{
    public static CrownCompletionism completionTracker;
    // Start is called before the first frame update

    public Dictionary<string, Crown> allCrowns;

    private void Awake()
    {
        if (completionTracker == null)
        {
            completionTracker = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        allCrowns = new Dictionary<string, Crown>();
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
            //add the crown with only inside symmetry - for each flower
            foreach (var insideFlower in flowers)
            {
                newCrown = new Crown();
                id = newCrown.SetCrownVals(flower.type, insideFlower.type, "");
                allCrowns.Add(id, newCrown);
                
                foreach (var outsideFlower in flowers)
                {
                    //add the crown with only outside symmetry - for each flower
                    newCrown = new Crown();
                    id = newCrown.SetCrownVals(flower.type, "", outsideFlower.type);
                    if (!allCrowns.ContainsKey(id))
                        allCrowns.Add(id, newCrown);

                    //add the crown with inside and outside symmetry - for each flower
                    newCrown = new Crown();
                    id = newCrown.SetCrownVals(flower.type, insideFlower.type, outsideFlower.type);
                    if (!allCrowns.ContainsKey(id))
                        allCrowns.Add(id, newCrown);
                }
            }
        }
  

        Debug.Log("There are " + allCrowns.Count + " potential crowns");
        yield return null;
    }
}
