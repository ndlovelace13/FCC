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


    float range;
    int damage;
    string projType;
    int aug1 = 0;
    int aug2 = 0;
    int aug3 = 0;

    public string crownAnnouncement;
    // Start is called before the first frame update
    void Start()
    {
        DictionaryInit();
        CrownReplace();
        slots = docket.GetComponentsInChildren<Transform>();
        slots = slots.Where(child => child.tag == "slotEmpty").ToArray();
        chosenInputs = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<FlowerHarvest>().docketLoaded == true && Input.GetKeyDown(KeyCode.E) && !skillCheckActive && !constructionReady && !crownHeld)
        {
            if (GameControl.PlayerData.tutorialState == 4)
                GameControl.PlayerData.crownConstructionStarted = true;
            Debug.Log("trolling");
            ConstructionSkillCheck();
            gameObject.GetComponent<FlowerHarvest>().docketLoaded = false;
        }
        if (constructionReady == true)
        {
            int crownScore = Construction();
            constructionReady = false;
            Debug.Log(crownScore);
            //int currentScore = PlayerPrefs.GetInt("totalScore");
            //PlayerPrefs.SetInt("totalScore", currentScore + crownScore);
            if (!GameControl.PlayerData.tutorialActive)
                GameControl.PlayerData.score += crownScore;
            crownNotif.GetComponent<ScoreNotification>().newFeed(crownAnnouncement);
            scoreNotif.GetComponent<ScoreNotification>().newFeed("Crown Construction | +" + crownScore);
            finalCrown.GetComponent<SpriteRenderer>().enabled = true;
            finalCrown.transform.parent = null;
            gameObject.GetComponent<CrownThrowing>().CompletedCrown(finalCrown);
            finalCrown.GetComponent<CrownAttack>().SetProjStats(range, damage, projType, aug1, aug2, aug3);
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

    //move this to gamecontrol when adding more flowers - link each flower's strings to the flowerStats object
    void DictionaryInit()
    {
        outsideText = new Dictionary<string, string>() {
            {"white", "Pale "},
            {"orange", "Tawny "},
            {"pink", "Flushed "},
            {"red", "Chilled "},
            {"yellow", "Charged "},
            {"green", "Poisonous "},
            {"blue", "Chilled "}
        };
        insideText = new Dictionary<string, string>() {
            {"white", "Blank "},
            {"orange", "Peachy "},
            {"pink", "Salmon "},
            {"red", "Burning "},
            {"yellow", "Volatile "},
            {"green", "Noxious "},
            {"blue", "Frigid "}
        };
        primaryText = new Dictionary<string, string>() {
            {"white", "Pasty "},
            {"orange", "Orangish "},
            {"pink", "Rosy "},
            {"red", "Fiery "},
            {"yellow", "Electric "},
            {"green", "Toxic "},
            {"blue", "Frozen "}
        };
        fourText = new Dictionary<string, string>() {
            {"white", "Colorless "},
            {"orange", "Paprika " },
            {"pink", "Plasticky "},
            {"red", "Scorching "},
            {"yellow", "Voltaic "},
            {"green", "Lethal "},
            {"blue", "Arctic "}
        };
    }

    void CrownReplace()
    {
        finalCrown = Instantiate(crownPrefab, docket.transform);
        finalCrown.GetComponent<SpriteRenderer>().enabled = false;
        finalCrown.transform.parent = docket.transform;
        gameObject.GetComponent<FlowerHarvest>().crownReset();
    }

    public void CrownThrown()
    {
        crownHeld = false;
    }

    void ConstructionSkillCheck()
    {
        foreach (Transform t in slots)
        {
            int inputRand = Random.Range(0, 4);
            GameObject newInput = Instantiate(inputs[inputRand], t.position + new Vector3(0, 1.5f), Quaternion.identity);
            chosenInputs.Add(newInput);
            Debug.Log(chosenInputs.Count);
            chosenInputs[chosenInputs.Count - 1].transform.parent = docket.transform;
        }
        skillCheckActive = true;
        gameObject.GetComponent<PlayerMovement>().CraftingSlow();
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
                chosenInputs.Remove(currentInput);
                Destroy(currentInput);
                inputPressed = false;
            }
        }
        else
        {
            Debug.Log("loopBROKE");
            skillCheckActive = false;
            gameObject.GetComponent<PlayerMovement>().CraftingDone();
            constructionReady = true;
        }
        
    }

    int Construction()
    {
        //reset variables
        int crownScore = 0;
        crownAnnouncement = "";
        aug1 = 0;
        aug2 = 0;
        aug3 = 0;
        Transform[] flowers = docket.GetComponentsInChildren<Transform>();
        flowers = flowers.Where(child => child.tag == "FlowerHead").ToArray();
        List<FlowerStats> flowerStats = new List<FlowerStats>();
        List<int> dupePositions = new List<int>();
        for (int i = 0; i < flowers.Length; i++)
        {
            flowerStats.Add(flowers[i].gameObject.GetComponent<FlowerStats>());
        }
        range = flowerStats[0].range + flowerStats[4].range;
        damage = flowerStats[1].damage + flowerStats[3].damage;
        projType = flowerStats[2].type;
        aug1 = augmentCheck(projType);
        
        //check the first, remove as necessary
        while (flowerStats.Count > 0)
        {
            string currentType = flowerStats[0].type;
            dupePositions.Add(flowerStats[0].position);
            for (int j = 1; j < flowerStats.Count; j++)
            {
                if (currentType.Equals(flowerStats[j].type))
                {
                    //Debug.Log(flowerStats[j].position);
                    //Debug.Log(flowerStats[j].type);
                    dupePositions.Add(flowerStats[j].position);
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
                        crownScore += flowerStats[0].basePoints * 2 * 3;
                        if (dupePositions.Contains(0))
                            crownAnnouncement += outsideText[currentType];
                        else
                            crownAnnouncement += insideText[currentType];
                        if (aug2 == 0)
                            aug2 = augmentCheck(currentType);
                        else
                            aug3 = augmentCheck(currentType);
                    }
                    else
                    {
                        crownScore += flowerStats[0].basePoints * 2 * 2;
                    }
                    break;
                //triple found
                case 3:
                    Debug.Log("Triples");
                    //symmetrical check
                    if ((!dupePositions.Contains(1) && !dupePositions.Contains(3)) || (!dupePositions.Contains(0) && !dupePositions.Contains(4)))
                    {
                        Debug.Log("Symmetric");
                        crownScore += flowerStats[0].basePoints * 3 * 4;
                        if (dupePositions.Contains(0))
                            crownAnnouncement += outsideText[currentType];
                        else
                            crownAnnouncement += insideText[currentType];
                        if (aug2 == 0)
                            aug2 = augmentCheck(currentType);
                        else
                            aug3 = augmentCheck(currentType);
                    }
                    else
                    {
                        crownScore += flowerStats[0].basePoints * 3 * 3;
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
                        crownScore += flowerStats[0].basePoints * 4 * 8;
                        //5x the middle's base points
                        crownScore += flowerStats[2].basePoints * 5;
                        flowerStats.RemoveAt(2);
                        aug2 = augmentCheck(currentType);
                        aug3 = augmentCheck(currentType);
                        crownAnnouncement = fourText[currentType];
                    }
                    //non-symmetrical
                    else
                    {
                        crownScore += flowerStats[0].basePoints * 4 * 5;
                    }
                    break;
                //fiver found
                case 5:
                    Debug.Log("Fiver");
                    crownScore += flowerStats[0].basePoints * 5 * 10;
                    aug2 = augmentCheck(currentType);
                    aug3 = augmentCheck(currentType);
                    crownAnnouncement += fourText[currentType];
                    break;
                //single
                default:
                    crownScore += flowerStats[0].basePoints;
                    break;
            }

            //remove duplicates that have already been evaluated from the list
            for (int k = flowerStats.Count - 1; k > -1; k--)
            {
                if (flowerStats[k].type == currentType)
                    flowerStats.Remove(flowerStats[k]);
            }
            //reset dupePositions
            dupePositions.Clear();
            //if (crownAnnouncement != "")
            //   crownAnnouncement += " ";
        }
        Debug.Log("gottem");
        crownAnnouncement += primaryText[projType] + "Crown Constructed!";
        Debug.Log(crownAnnouncement);
        return crownScore;
    }

    public int augmentCheck(string type)
    {
        switch (type)
        {
            case "red": return 1;
            case "blue": return 2;
            case "green": return 3;
            case "yellow": return 4;
            default: return 0;
        }    
    }
}
