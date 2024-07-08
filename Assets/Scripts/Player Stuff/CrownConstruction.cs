using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using TMPro;

public class CrownConstruction : MonoBehaviour
{
    [SerializeField] GameObject docket;
    [SerializeField] GameObject crownPrefab;
    [SerializeField] GameObject[] inputs;
    [SerializeField] TMP_Text scoreNotif;
    [SerializeField] TMP_Text crownNotif;
    List<GameObject> chosenInputs;
    GameObject finalCrown;
    Transform[] slots;

    Dictionary<string, string> outsideText;
    Dictionary<string, string> insideText;
    Dictionary<string, string> primaryText;
    Dictionary<string, string> fourText;

    public bool constructionInProgress = false;
    public bool constructionReady = false;
    bool crownHeld = false;
    public bool skillCheckActive = false;

    FlowerHarvest harvestObj;


    float range;
    int damage;
    string projType;
    string aug1 = "";
    string aug2 = "";
    string aug3 = "";
    int numProjs = 0;
    int projRange = 0;
    int tier;

    bool crownDiscovered = false;
    int crownDiscoveryScore = 0;

    public string crownAnnouncement;

    int craftAnimChoice = 1;

    [SerializeField] GameObject flowerUIPool;
    int skillCheckCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        harvestObj = gameObject.GetComponentInChildren<FlowerHarvest>();
        CrownReplace();
        slots = docket.GetComponentsInChildren<Transform>();
        slots = slots.Where(child => child.tag == "slotEmpty").ToArray();
        chosenInputs = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (harvestObj.docketLoaded == true && Input.GetKeyDown(KeyCode.E) && !skillCheckActive && !constructionReady && !crownHeld)
        {
            if (GameControl.PlayerData.tutorialState == 4)
                GameControl.PlayerData.crownConstructionStarted = true;
            Debug.Log("trolling");
            ConstructionSkillCheck();
            harvestObj.docketLoaded = false;
            GetComponentInChildren<Animator>().SetBool("isMoving", false);
            GetComponentInChildren<Animator>().SetBool("isCrafting", true);
        }
        if (constructionReady == true)
        {
            GetComponentInChildren<Animator>().SetBool("isCrafting", false);
            int crownScore = (int) (Construction() * GameControl.PlayerData.crownMult);
            constructionReady = false;
            Debug.Log(crownScore);
            //int currentScore = PlayerPrefs.GetInt("totalScore");
            //PlayerPrefs.SetInt("totalScore", currentScore + crownScore);
            if (!GameControl.PlayerData.tutorialActive)
            {
                GameControl.PlayerData.score += crownScore;
                GameControl.PlayerData.constructionScore += crownScore;
                if (crownDiscovered)
                {
                    GameControl.PlayerData.score += crownDiscoveryScore;
                    GameControl.PlayerData.discoveryScore += crownDiscoveryScore;
                }
            }
            if (crownDiscovered)
            {
                crownNotif.GetComponent<ScoreNotification>().newFeed(crownAnnouncement, Color.green);
                scoreNotif.GetComponent<ScoreNotification>().newFeed("New Crown Discovered | +" + crownDiscoveryScore);
            }
            else
                crownNotif.GetComponent<ScoreNotification>().newFeed(crownAnnouncement);
            AkSoundEngine.PostEvent("CraftingDone", gameObject);
            scoreNotif.GetComponent<ScoreNotification>().newFeed("Crown Construction | +" + crownScore);
            finalCrown.GetComponent<SpriteRenderer>().enabled = true;
            finalCrown.transform.parent = null;
            gameObject.GetComponent<CrownThrowing>().CompletedCrown(finalCrown, range);
            finalCrown.GetComponent<CrownAttack>().SetProjStats(projRange, damage, projType, aug1, aug2, aug3, numProjs, tier);
            //Instantiation of new crown
            CrownReplace();
            crownHeld = true;
        }
        if (skillCheckActive)
        {
            if (GameControl.PlayerData.tutorialState != 4)
                skillChecking();
        }
    }


    void CrownReplace()
    {
        finalCrown = Instantiate(crownPrefab, docket.transform);
        foreach (SpriteRenderer sprite in finalCrown.GetComponentsInChildren<SpriteRenderer>())
            sprite.enabled = false;
        //finalCrown.GetComponent<SpriteRenderer>().enabled = false;
        finalCrown.transform.parent = docket.transform;
        harvestObj.crownReset();
    }

    public void CrownThrown()
    {
        crownHeld = false;
    }

    void ConstructionSkillCheck()
    {
        foreach (Transform t in slots)
        {
            skillCheckCounter = 0;
            int inputRand = UnityEngine.Random.Range(0, 4);
            GameObject newInput = Instantiate(inputs[inputRand], t.position + new Vector3(0, 2f), Quaternion.identity);
            chosenInputs.Add(newInput);
            Debug.Log(chosenInputs.Count);
            chosenInputs[chosenInputs.Count - 1].transform.parent = docket.transform;
        }
        skillCheckActive = true;
        gameObject.GetComponentInChildren<PlayerMovement>().CraftingSlow();
    }
    
    void skillChecking()
    {
        if (chosenInputs.Count > 0)
        {
            bool inputPressed = false;
            GameObject currentInput = chosenInputs[0];
            string name = currentInput.name;
            switch (name)
            {
                case "InputW(Clone)":
                    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                        inputPressed = true;
                    break;
                case "InputA(Clone)":
                    if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                        inputPressed = true;
                    break;
                case "InputS(Clone)":
                    if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                        inputPressed = true;
                    break;
                case "InputD(Clone)":
                    if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                        inputPressed = true;
                    break;
            }
            if (inputPressed)
            {
                AkSoundEngine.PostEvent("CraftingInput", gameObject);
                inputPressed = false;
                StartCoroutine(CraftDirection());
                //start the flower lerp
                skillCheckCounter++;
                chosenInputs.Remove(currentInput);
                Destroy(currentInput);
            }
        }
        else
        {
            Debug.Log("loopBROKE");
            skillCheckActive = false;
            gameObject.GetComponentInChildren<PlayerMovement>().CraftingDone();
            constructionReady = true;
        }
        
    }

    IEnumerator CraftDirection()
    {
        GetComponentInChildren<Animator>().SetInteger("CraftDirection", craftAnimChoice);
        if (flowerUIPool)
        {
            Debug.Log("currentCounter: " + skillCheckCounter);
            StartCoroutine(flowerLerpBegin(skillCheckCounter));
        }
        if (craftAnimChoice == 1)
            craftAnimChoice = 2;
        else
            craftAnimChoice = 1;
        yield return new WaitForSeconds(0.1f);
        GetComponentInChildren<Animator>().SetInteger("CraftDirection", 0);
    }

    IEnumerator flowerLerpBegin(int pos)
    {
        Debug.Log("startingFlowerLerp");
        yield return null;
        GameObject newUIFlower = flowerUIPool.GetComponent<ObjectPool>().GetPooledObject();
        newUIFlower.SetActive(true);
        //get the flower type that associates with the current transform
        Transform[] flowers = docket.GetComponentsInChildren<Transform>();
        flowers = flowers.Where(child => child.tag == "FlowerHead").ToArray();
        List<FlowerStats> flowerStats = flowerSlots(flowers);
        newUIFlower.GetComponent<CraftingLerp>().Activate(flowerStats[pos].type, craftAnimChoice);
    }

    private List<FlowerStats> flowerSlots(Transform[] flowers)
    {
        List<FlowerStats> flowerStats = new List<FlowerStats>();
        for (int i = 0; i < flowers.Length; i++)
        {
            flowerStats.Add(GameControl.PlayerData.flowerStatsDict[flowers[i].GetComponent<FlowerBehavior>().type]);
            //flowerStats.Add(flowers[i].gameObject.GetComponent<FlowerStats>());
        }
        return flowerStats;
    }

    private List<FlowerBehavior> flowerPositions(Transform[] flowers)
    {
        
        List<FlowerBehavior> flowerStats = new List<FlowerBehavior>();
        for (int i = 0; i < flowers.Length; i++)
        {
            //flowerStats.Add(GameControl.PlayerData.flowerStatsDict[flowers[i].GetComponent<FlowerBehavior>().type]);
            flowerStats.Add(flowers[i].gameObject.GetComponent<FlowerBehavior>());
            Debug.Log(flowerStats[i] + " added");
        }
        return flowerStats;
    }

    IEnumerator CrownName(List<FlowerStats> flowerStats)
    {
        bool[] wilds = new bool[5];
        for (int i = 0; i < flowerStats.Count; i++)
        {
            if (flowerStats[i].type == "wild")
                wilds[i] = true;
            else
                wilds[i] = false;
        }

        Dictionary<string, FlowerStats> dict = GameControl.PlayerData.flowerStatsDict;

        crownAnnouncement = "";
        string outside = "";
        string inside = "";
        string four = "";
        string fiver = "";

        //unlock data
        int primaryId = flowerStats[2].id;
        int insideId = -1;
        int outsideId = -1;
        string id = "";

        //check the crown type
        if (flowerStats[0].type == flowerStats[4].type || wilds[0] || wilds[4])
        {
            if (flowerStats[0].type == flowerStats[4].type)
                outside = flowerStats[0].type;
            else if (wilds[0])
                outside = flowerStats[4].type;
            else
                outside = flowerStats[0].type;
            outsideId = dict[outside].id;
        }
        if (flowerStats[1].type == flowerStats[3].type || wilds[1] || wilds[3])
        {
            if (flowerStats[1].type == flowerStats[3].type)
                inside = flowerStats[1].type;
            else if (wilds[1])
                inside = flowerStats[3].type;
            else
                inside = flowerStats[1].type;
            insideId = dict[inside].id;
        }
        if (outside == inside && outside != "")
            four = outside;
        if (four != "" && four == flowerStats[2].type)
            fiver = four;

        //Debug.Log("NAME SHIT: " + fiver + " " + four + " " + outside + " " + inside);
        //apply the name
        if (fiver != "")
            crownAnnouncement += dict[fiver].GetFiveText();
        else if (four != "")
            crownAnnouncement += dict[four].GetFourText();
        else
        {
            if (outside != "")
            {
                crownAnnouncement += dict[outside].GetOutsideText();
            }
            if (inside != "")
                crownAnnouncement += dict[inside].GetInsideText();
        }
        if (fiver == "")
            crownAnnouncement += dict[flowerStats[2].type].GetPrimaryText();
        crownAnnouncement += "Crown";
        //check for unlock here
        id += primaryId.ToString() + insideId.ToString() + outsideId.ToString();
        Crown constructedCrown = GameControl.CrownCompletion.allCrowns[id];
        if (constructedCrown.IsDiscovered())
        {
            constructedCrown.Crafted();
            crownDiscovered = false;
        }
        else
        {
            constructedCrown.Discovery(crownAnnouncement);
            crownDiscovered = true;
            crownDiscoveryScore = 5;
            if (outsideId != -1)
                crownDiscoveryScore += 5;
            if (insideId != -1)
                crownDiscoveryScore += 5;
        }

        crownAnnouncement += " Constructed!";
        //Debug.Log("NAME SHIT: " + crownAnnouncement);
        yield return crownAnnouncement;
    }

    int Construction()
    {
        //reset variables
        int crownScore = 0;
        //crownAnnouncement = "";
        aug1 = "";
        aug2 = "";
        aug3 = "";
        Transform[] flowers = docket.GetComponentsInChildren<Transform>();
        flowers = flowers.Where(child => child.tag == "FlowerHead").ToArray();
        List<FlowerStats> flowerStats = flowerSlots(flowers);
        List<FlowerBehavior> flowerPos = flowerPositions(flowers);
        List<int> dupePositions = new List<int>();
        tier = 0;

        //handle the crown naming convention before changing wilds to match the crown - separate function here
        StartCoroutine(CrownName(flowerStats));

        //wild check for crown construction
        foreach (var flower in flowerPos)
        {
            if (flower.type == "wild")
            {
                WildStats wildStats = (WildStats)GameControl.PlayerData.flowerStatsDict["wild"];
                Tuple<List<FlowerBehavior>, List<FlowerStats>> results = wildStats.WildHandler(flowerPos, flowerStats);
                flowerPos = results.Item1;
                flowerStats = results.Item2;
                break;
            }
        }
        FlowerStats centerStats = flowerStats[2];

        projRange = centerStats.GetProjRange(flowerPos[2].tier);
        range = flowerStats[0].GetRange(flowerPos[0].tier) + flowerStats[4].GetRange(flowerPos[4].tier);
        damage = flowerStats[1].GetDamage(flowerPos[1].tier) + flowerStats[2].GetDamage(flowerPos[2].tier) + flowerStats[3].GetDamage(flowerPos[3].tier);
        projType = flowerPos[2].type;
        aug1 = flowerPos[2].type;
        numProjs = centerStats.GetProjCount(flowerPos[2].tier);
        foreach (var flower in flowerPos)
            tier += flower.tier - 1;


        
        
        //check the first, remove as necessary
        while (flowerStats.Count > 0)
        {
            string currentType = flowerPos[0].type;
            Debug.Log(currentType);
            dupePositions.Add(flowerPos[0].position);
            for (int j = 1; j < flowerPos.Count; j++)
            {
                if (currentType.Equals(flowerPos[j].type))
                {
                    //Debug.Log(flowerStats[j].position);
                    //Debug.Log(flowerStats[j].type);
                    dupePositions.Add(flowerPos[j].position);
                }
            }
            /*foreach (int pos in dupePositions)
            {
                Debug.Log(pos);
            }*/
            switch (dupePositions.Count)
            {
                //duplicate found
                case 2:
                    Debug.Log("Doubles");
                    //symmetrical check
                    if ((dupePositions.Contains(0) && dupePositions.Contains(4)) || (dupePositions.Contains(1) && dupePositions.Contains(3)))
                    {
                        Debug.Log("Symmetric");
                        crownScore += flowerStats[0].GetPoints(flowerPos[0].tier) * 2 * 4;
                        /*if (dupePositions.Contains(0))
                            crownAnnouncement += flowerStats[0].outsideText;
                        else
                            crownAnnouncement += flowerStats[0].insideText;*/
                        if (aug2 == "")
                            aug2 = flowerPos[0].type;
                        else
                            aug3 = flowerPos[0].type;
                    }
                    else
                    {
                        Debug.Log(dupePositions[0] + " " + dupePositions[1]);
                        crownScore += flowerStats[0].GetPoints(flowerPos[0].tier) * 2 * 2;
                    }
                    break;
                //triple found
                case 3:
                    Debug.Log("Triples");
                    //symmetrical check
                    if ((!dupePositions.Contains(1) && !dupePositions.Contains(3)) || (!dupePositions.Contains(0) && !dupePositions.Contains(4)))
                    {
                        Debug.Log("Symmetric");
                        crownScore += flowerStats[0].GetPoints(flowerPos[0].tier) * 3 * 4;
                        /*if (dupePositions.Contains(0))
                            crownAnnouncement += flowerStats[0].outsideText;
                        else
                            crownAnnouncement += flowerStats[0].insideText;*/
                        if (aug2 == "")
                            aug2 = flowerPos[0].type;
                        else
                            aug3 = flowerPos[0].type;
                    }
                    else
                    {
                        crownScore += flowerStats[0].GetPoints(flowerPos[0].tier) * 3 * 3;
                    }
                    break;
                //quad found
                case 4:
                    Debug.Log("Quad");
                    //symmetrical
                    if (!dupePositions.Contains(2))
                    {
                        Debug.Log("Symmetric");
                        //8x the quad's base points
                        crownScore += flowerStats[0].GetPoints(flowerPos[0].tier) * 4 * 4;
                        //5x the middle's base points
                        //crownScore += flowerStats[2].basePoints * 5;
                        //flowerStats.RemoveAt(2);
                        aug2 = flowerPos[0].type;
                        aug3 = flowerPos[0].type;
                        //crownAnnouncement = flowerStats[0].fourText;
                    }
                    //non-symmetrical
                    else
                    {
                        crownScore += flowerStats[0].GetPoints(flowerPos[0].tier) * 4 * 3;
                    }
                    break;
                //fiver found
                case 5:
                    Debug.Log("Fiver");
                    crownScore += flowerStats[0].GetPoints(flowerPos[0].tier) * 5 * 5;
                    aug2 = flowerPos[0].type;
                    aug3 = flowerPos[0].type;
                    //crownAnnouncement += flowerStats[0].fiveText;
                    //fiver = true;
                    break;
                //single
                default:
                    crownScore += flowerStats[0].GetPoints(flowerPos[0].tier);
                    break;
            }

            //remove duplicates that have already been evaluated from the list
            for (int i = dupePositions.Count - 1; i > -1; i--)
            {
                Debug.Log("removing " + dupePositions[i]);
                int index = 0;
                for (int k = flowerPos.Count - 1; k > -1; k--)
                {
                    if (flowerPos[k].position == dupePositions[i])
                    {
                        index = k;
                        Debug.Log("current index" + index);
                        break;
                    }
                }
                Debug.Log("flowerStats size" + flowerStats.Count);
                flowerStats.Remove(flowerStats[index]);
                flowerPos.Remove(flowerPos[index]);
            }
            /*for (int k = flowerStats.Count - 1; k > -1; k--)
            {
                if (flowerStats[k].type == currentType)
                {
                    
                    flowerStats.Remove(flowerStats[k]);
                    flowerPos.Remove(flowerPos[k]);
                }
                    
            }*/
            //reset dupePositions
            dupePositions.Clear();
            //if (crownAnnouncement != "")
            //   crownAnnouncement += " ";
        }
        Debug.Log("gottem");
        
        Debug.Log(crownAnnouncement);
        return crownScore;
    }
}
