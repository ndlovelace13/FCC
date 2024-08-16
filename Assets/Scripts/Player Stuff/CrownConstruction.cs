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

    public bool constructionInProgress = false;
    public bool constructionReady = false;
    bool crownHeld = false;
    public bool skillCheckActive = false;

    FlowerHarvest harvestObj;


    float range;
    int damage;
    string projType;
    //string aug1 = "";
    //string aug2 = "";
    //string aug3 = "";
    Dictionary<string, int> actualAugs;
    int numProjs = 0;
    int projRange = 0;
    int tier;

    bool crownDiscovered = false;
    int crownDiscoveryScore = 0;

    public string crownAnnouncement;

    int craftAnimChoice = 1;

    [SerializeField] GameObject flowerUIPool;
    

    //stuff for the skill checking rework
    FlowerBehavior[] currentFlowers;
    int prevSkillCheckCount = 0;
    int skillCheckCounter = 0;
    [SerializeField] GameObject craftStem;
    GameObject[] stems;

    // Start is called before the first frame update
    void Start()
    {
        harvestObj = gameObject.GetComponentInChildren<FlowerHarvest>();
        CrownReplace();
        slots = docket.GetComponentsInChildren<Transform>();
        slots = slots.Where(child => child.tag == "slotEmpty").ToArray();
        chosenInputs = new List<GameObject>();

        stems = new GameObject[slots.Length];
        for (int i = 0; i < stems.Length; i++)
        {
            stems[i] = Instantiate(craftStem);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //activated when the docket is loaded and the player presses E - initiates crown construction
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
        //activated when the crown is fully crafted - calculates the score and resets for more harvesting
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
                GameControl.PlayerData.shiftCrowns++;
                if (crownDiscovered)
                {
                    GameControl.PlayerData.score += crownDiscoveryScore;
                    GameControl.PlayerData.discoveryScore += crownDiscoveryScore;
                }
            }
            if (crownDiscovered)
            {
                GameControl.SaveData.newDiscoveries++;
                crownNotif.GetComponent<ScoreNotification>().newFeed(crownAnnouncement, Color.green);
                scoreNotif.GetComponent<ScoreNotification>().newFeed("New Crown Discovered | ", crownDiscoveryScore);
            }
            else
                crownNotif.GetComponent<ScoreNotification>().newFeed(crownAnnouncement);
            AkSoundEngine.PostEvent("CraftingDone", gameObject);
            scoreNotif.GetComponent<ScoreNotification>().newFeed("Crown Construction | ", crownScore);
            //finalCrown.GetComponent<SpriteRenderer>().enabled = true;
            
            gameObject.GetComponent<CrownThrowing>().CompletedCrown(finalCrown, range);
            //TODO - switch hardcoded augments to passing actualAugs dict
            foreach (var aug in actualAugs)
                Debug.Log("Augment " + aug.Key + " " + aug.Value);
            finalCrown.GetComponent<CrownAttack>().SetProjStats(projRange, damage, projType, actualAugs, numProjs, tier);
            //crownHeld = true;
            //reactivate the crosshair
            GameControl.PlayerData.crosshairActive = true;
        }
        //triggers when the skillcheck is active - not ideal but may be necessary for the tutorial state checking
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

    //sets up the skill check - retain this function to place the flowers and crown for crafting minigame
    void ConstructionSkillCheck()
    {
        StartCoroutine(CraftingSetupLerp());

        //slots contains the final positions for the flowers
        /*foreach (Transform t in slots)
        {
            skillCheckCounter = 0;
            int inputRand = UnityEngine.Random.Range(0, 4);
            GameObject newInput = Instantiate(inputs[inputRand], t.position + new Vector3(0, 2f), Quaternion.identity);
            chosenInputs.Add(newInput);
            Debug.Log(chosenInputs.Count);
            chosenInputs[chosenInputs.Count - 1].transform.parent = docket.transform;
        }*/
    }

    //use this function to place all the flowers and crown in the right spots for construction minigame
    IEnumerator CraftingSetupLerp()
    {
        //activate the crown spriteRenderer
        finalCrown.GetComponent<SpriteRenderer>().enabled = true;

        //calculate the edge of the screen and the finalLocation
        Vector3 topEdge = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height));
        Vector3 center = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2));
        float yOffset = Math.Abs(topEdge.y - center.y) / 4f;
        Vector3 finalPos = new Vector3(finalCrown.transform.localPosition.x, yOffset);

        //calculate a random location for all of the flowers
        currentFlowers = finalCrown.transform.GetComponentsInChildren<FlowerBehavior>();
        //Transform[] flowers = children.Where(child => child.tag == "FlowerHead").ToArray();
        for (int i = currentFlowers.Length - 1; i >= 0; i--)
        {
            //store the finalPosition as the current localPos in relation to the crown
            currentFlowers[i].finalDocketPos = currentFlowers[i].gameObject.transform.localPosition;

            //generate a random location on the surrounding oval
            float generatedRad = UnityEngine.Random.Range(0f, Mathf.PI * 2);
            Vector3 generatedPos = new Vector3(4 * Mathf.Cos(generatedRad), 2.5f * Mathf.Sin(generatedRad));
            currentFlowers[i].randomCraftPos = generatedPos;

            //assign a stem to the current flower
            stems[i].GetComponent<CraftingStem>().SetFlower(currentFlowers[i]);
            stems[i].transform.SetParent(finalCrown.transform);
        }


        float currentTime = 0f;
        while (currentTime < 0.2f)
        {
            finalCrown.transform.localPosition = Vector3.Lerp(Vector3.zero, finalPos, currentTime / 0.2f);
            foreach (var flower in currentFlowers)
            {
                flower.gameObject.transform.localPosition = Vector3.Lerp(flower.finalDocketPos, flower.randomCraftPos, currentTime / 0.2f);
            }
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }

        //finally allow for the skillChecking to begin
        foreach (var flower in currentFlowers)
        {
            flower.draggable = true;
            flower.placed = false;
        }

        prevSkillCheckCount = 0;
        skillCheckActive = true;
        
        gameObject.GetComponentInChildren<PlayerMovement>().CraftingSlow();
        GameControl.PlayerData.crosshairActive = false;
    }

    //define the behavior for resetting crown to the pre-crafted state
    IEnumerator CraftingCancelLerp()
    {
        Debug.Log("Crafting Cancel Started");

        /*//calculate the edge of the screen and the finalLocation
        Vector3 topEdge = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height));
        Vector3 center = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2));
        float yOffset = Math.Abs(topEdge.y - center.y) / 4f;
        Vector3 finalPos = new Vector3(finalCrown.transform.localPosition.x, yOffset);

        //calculate a random location for all of the flowers
        currentFlowers = finalCrown.transform.GetComponentsInChildren<FlowerBehavior>();
        //Transform[] flowers = children.Where(child => child.tag == "FlowerHead").ToArray();
        for (int i = currentFlowers.Length - 1; i >= 0; i--)
        {
            //store the finalPosition as the current localPos in relation to the crown
            currentFlowers[i].finalDocketPos = currentFlowers[i].gameObject.transform.localPosition;

            //generate a random location on the surrounding oval
            float generatedRad = UnityEngine.Random.Range(0f, Mathf.PI * 2);
            Vector3 generatedPos = new Vector3(4 * Mathf.Cos(generatedRad), 2.5f * Mathf.Sin(generatedRad));
            currentFlowers[i].randomCraftPos = generatedPos;

            //assign a stem to the current flower
            stems[i].GetComponent<CraftingStem>().SetFlower(currentFlowers[i]);
            stems[i].transform.SetParent(finalCrown.transform);
        }*/

        Vector3 currentCrownPos = finalCrown.transform.localPosition;

        float currentTime = 0f;
        while (currentTime < 0.2f)
        {
            finalCrown.transform.localPosition = Vector3.Lerp(currentCrownPos, Vector3.zero, currentTime / 0.2f);
            foreach (var flower in currentFlowers)
            {
                flower.gameObject.transform.localPosition = Vector3.Lerp(flower.randomCraftPos, flower.finalDocketPos, currentTime / 0.2f);
            }
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }

        //finally allow for the skillChecking to begin
        foreach (var flower in currentFlowers)
        {
            flower.draggable = false;
            flower.placed = false;
        }

        skillCheckActive = false;

        gameObject.GetComponentInChildren<PlayerMovement>().CraftingDone();
        GameControl.PlayerData.crosshairActive = true;

        //disable the crown spriteRenderer
        finalCrown.GetComponent<SpriteRenderer>().enabled = false;

        GetComponentInChildren<Animator>().SetBool("isCrafting", false);
        harvestObj.docketLoaded = true;
        GetComponentInChildren<Animator>().SetBool("isMoving", true);

        yield return null;
        
    }

    IEnumerator CrownFinishLerp()
    {
        skillCheckActive = false;
        gameObject.GetComponentInChildren<PlayerMovement>().CraftingDone();
        constructionReady = true;

        Vector3 currentPos = finalCrown.transform.position;
        //Debug.Log(currentPos + " " + transform.position);

        float currentTime = 0f;
        while (currentTime < 0.2f)
        {
            finalCrown.transform.position = Vector3.Lerp(currentPos, transform.position, currentTime / 0.2f);
            finalCrown.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.6f, currentTime / 0.2f);
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //set things up for the reckoning when done
        //Instantiation of new crown
        finalCrown.transform.parent = null;
        CrownReplace();
        crownHeld = true;
        gameObject.GetComponent<CrownThrowing>().crownHeld = true;
        //free the crown from the bonds of parentage
        
    }
    
    void skillChecking()
    {
        /*if (chosenInputs.Count > 0)
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
        }*/
        if (Input.GetKeyDown(KeyCode.E) && !GameControl.PlayerData.tutorialActive)
        {
            StartCoroutine(CraftingCancelLerp());
        }
        
        //the new version
        if (prevSkillCheckCount < 5)
        {
            skillCheckCounter = 0;
            foreach (var flower in currentFlowers)
            {
                if (flower.placed)
                    skillCheckCounter++;
            }
            if (skillCheckCounter > prevSkillCheckCount)
            {
                AkSoundEngine.PostEvent("CraftingInput", gameObject);
                prevSkillCheckCount = skillCheckCounter;
            }
        }
        else
        {
            Debug.Log("Skill Check Complete");
            //lerp crown back down
            StartCoroutine(CrownFinishLerp());
        }
        
    }

    public void CraftLerpCommand(int pos)
    {
        StartCoroutine(CraftDirection(pos));
    }

    IEnumerator CraftDirection(int pos)
    {
        GetComponentInChildren<Animator>().SetInteger("CraftDirection", craftAnimChoice);
        if (flowerUIPool)
        {
            //Debug.Log("currentCounter: " + skillCheckCounter);
            StartCoroutine(flowerLerpBegin(pos));
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
            crownAnnouncement += " Constructed!";
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
            crownAnnouncement += " Discovered!";
        }

        
        //Debug.Log("NAME SHIT: " + crownAnnouncement);
        yield return crownAnnouncement;
    }

    int Construction()
    {
        //reset variables
        int crownScore = 0;
        //crownAnnouncement = "";
        //aug1 = "";
        //aug2 = "";
        //aug3 = "";
        actualAugs = new Dictionary<string, int>();
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
        //remove the basic augs eventually
        //aug1 = flowerPos[2].type;
        AugPowerCheck(flowerPos[2].type);
        numProjs = centerStats.GetProjCount(flowerPos[2].tier);
        foreach (var flower in flowerPos)
        {
            if (flower.tier > 1)
                tier += flower.tier;
        }


        
        
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
                        AugPowerCheck(flowerPos[0].type);
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
                        AugPowerCheck(flowerPos[0].type);
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
                        //4x the quad's base points
                        crownScore += flowerStats[0].GetPoints(flowerPos[0].tier) * 4 * 4;
                        AugPowerCheck(flowerPos[0].type);
                        AugPowerCheck(flowerPos[0].type);
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
                    AugPowerCheck(flowerPos[0].type);
                    AugPowerCheck(flowerPos[0].type);
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
            //reset dupePositions
            dupePositions.Clear();
            //if (crownAnnouncement != "")
            //   crownAnnouncement += " ";
        }
        Debug.Log("gottem");

        //check for sunny augment, apply tier value instead if it exists
        if (actualAugs.ContainsKey("sunny"))
            actualAugs["sunny"] += tier;

        //check for highestPower in save data for each 
        foreach (var aug in actualAugs)
        {
            if (GameControl.PlayerData.savedFlowerDict[aug.Key].highestPower < aug.Value)
                GameControl.PlayerData.savedFlowerDict[aug.Key].highestPower = aug.Value;
        }

        Debug.Log(crownAnnouncement);
        return crownScore;

        
    }

    private void AugPowerCheck(string type)
    {
        if (actualAugs.ContainsKey(type))
            actualAugs[type]++;
        else
            actualAugs.Add(type, 1);
    }
}
