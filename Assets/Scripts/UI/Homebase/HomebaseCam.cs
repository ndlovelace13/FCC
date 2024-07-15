using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
//using System.Linq;

public class HomebaseCam : MonoBehaviour
{
    Camera mainCam;
    [SerializeField] SpriteRenderer carSprite;
    [SerializeField] Sprite carMenu;
    [SerializeField] GameObject playerSprite;
    [SerializeField] GameObject report;
    [SerializeField] GameObject controlPrompt;
    public bool menuActive = false;

    //menu options
    [SerializeField] GameObject catalog;
    [SerializeField] GameObject research;
    [SerializeField] GameObject completionTracker;
    [SerializeField] GameObject almanac;
    [SerializeField] GameObject carKeys;

    //Dialogue Stuff
    [SerializeField] GameObject phone;
    [SerializeField] GameObject speechBubble;

    [SerializeField] GameObject quitPopup;

    string[] introDialogue = new string[]
            {
                "Uhhhh....Hello?",
                "Are you the odd job guy I saw in the paper?",
                "Is it REALLY true you'll do ANYTHING for the right price?",
                "Alright, well I happen to have an....ODD job for you",
                "Two questions:\n1. Are you good with kids? and \n2. Have you ever made a flower crown before?",
                "Honestly, don't answer those, it doesn't matter. You can learn on the job",
                "We have a position available for an entertainer at a birthday party, think you can handle it?",
                "If you're committed, you can make some real dough - but there is a bit of a learning curve",
                "Sending you the details for a quick, optional, training session, we'll talk more when you arrive",
                "Looking forward to having you on the payroll, just try not to waste my time"
            };

    string[] contractDiscussion = new string[]
            {
                "Before you start, we unfortunately have to go over some terms and conditions",
                "Make sure you read it over VERY carefully, you know how the old T&C routine goes",
                "I'm winking at you right now but just realized that doesn't really come across over the phone",
                "When you're finished, just click to sign on the line. Take as long as you need, I have ALL day..."
            };

    string[] postContract = new string[]
            {
                "Finally, phew thought you were never going to sign",
                "Congrats on joining the Anti-Anomaly Action Team, I was going to send a cake but we don't really have the budget",
                "So...I may have fibbed a little in the job description. As you may have guessed by the name, the Anti-Anomaly Action Team doesn't usually handle birthday parties",
                "The kids you signed up to deal with are a little...off, you'll see what I mean soon enough",
                "Sending you the coordinates for the party, make sure you put on your big boy pants for your first shift",
                "Good luck, you're gonna need it"
            };

    string[] catalogUnlock = new string[]
    {
        "So...skinwalker children...apologies for the lack of transparency - company rules",
        "Although, to be fair, if you actually read the terms and conditions, you'd know that this job isn't exactly a cake walk",
        "Lucky for you, we need all the help we can get - you'll never have to worry about being ready for the next shift",
        "As a new recruit, I'm not really allowed to say how its possible, only that you'll always wake up here in perfect condition if anything happens out on the job",
        "Don't think about it too hard, just keep on keeping on and make that paper",
        "Speaking of, I left you a little present, courtesy of the AAAT - The Catalog",
        "This booklet gives you access to a wide range of improvements to your working conditions",
        "Due to budgetary restrictions, you will have to pay for these upgrades yourself - however, we do provide free, instantaneous shipping (you're welcome)",
        "Just give me a ring whenever you want to order something off The Catalog - trust me, it'll be worth the investment"
    };

    string[] researchUnlock = new string[]
    {
        "Hello, hello, hello, Mr. Rookie!!!",
        "A little (grumpy) birdy told me you stumbled upon an essence seed out in the field",
        "Of course, as a newbie to this whole \"skinwalker elimination\" thing, I can't really explain the intricacies of their corpses dropping seeds",
        "To tell you the truth, we don't really have a clear answer yet either...fascinating specimens, aren't they?",
        "Thus, as a member of the Anti-Anomaly Action Team's Research and Development division, I implore you to collect as many of those seeds as you can find!",
        "I've passed along a handy-dandy little booklet of Research Drives that you can donate the seeds you find to!",
        "It's a win-win situation - my colleagues and I get to dig into the skinwalker species and YOU get to mess around with the cool new things we discover!",
        "Go ahead, send us those seeds of yours, and we promise to make your job much more interesting",
        "Oh yeah, the name's Jill Frye, yes the one from the science show...oh, the good old days"
    };

    string[] completionUnlock = new string[]
    {
        "Hello again, new guy",
        "You've been doing great work out there so far, thanks for keeping those damn things in check - you have no idea how much work you're saving me",
        "Our data indicates you've already crafted over 20 different flower crowns - I like that dedication to variety",
        "The geeks in the software division got bored and cooked up an application \"made for completionists\"",
        "I don't really know what a completionist is, as you know I'm out here trying to prevent societal ruin instead of wasting away on those computer games",
        "However, you seem to be a crafty sort of guy, so I thought I'd let you try it out",
        "I left a company tablet in your vehicle preloaded with your current crown discoveries",
        "They tell me it will automatically update with new discoveries as you work and research new types of flowers",
        "Be sure to check it out as you continue to discover new flower crown combinations - who knows, it may be more than a fancy checklist"
    };

    //public bool menusReady = false;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = GetComponent<Camera>();
        if (GameControl.SaveData.firstRun)
        {
            DialogueQueue();
        }
        if (GameControl.PlayerData.shiftJustEnded || GameControl.PlayerData.continuePressed)
        {
            StartCoroutine(InitialMove());
            if (!GameControl.PlayerData.continuePressed)
            {
                Debug.Log("what the fuck");
                StartCoroutine(BalanceUpdate());
                StartCoroutine(UnlockChecker());
                //StartCoroutine(DialogueQueue());
            }
        }
        else
        {
            StartCoroutine(MenuInit());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject quitPrefab = GameObject.FindWithTag("quitMenu");
            if (quitPrefab == null)
                MainMenuHandler();
        }
    }

    public void MainMenuHandler()
    {
        Time.timeScale = 0f;
        Instantiate(quitPopup);
    }

    private void DialogueQueue()
    {
        if (GameControl.SaveData.firstRun && GameControl.SaveData.tutorialComplete)
        {
            //BRANCH HERE FOR IF TUTORIAL SKIPPED OR NOT
            string[] branchDialogue;
            if (GameControl.PlayerData.tutorialSkip)
            {
                branchDialogue = new string[]
                {
                    "Too cool for training, huh? Fair enough, I appreciate your gung ho attitude.",
                    "You'll pick things up as you go, and you can always go back to training if you realize you weren't actually the guy"
                };
            }
            else
            {
                branchDialogue = new string[]
                {
                    "Well done in with the training, you're a natural crown creator"
                };
            }
            contractDiscussion = branchDialogue.Concat(contractDiscussion).ToArray();
            GameControl.SaveData.dialogueQueue.Enqueue(contractDiscussion);
        }
        else if (GameControl.SaveData.firstRun)
        {
            //load intro dialogue, store in a text file eventually?
            GameControl.SaveData.dialogueQueue.Enqueue(introDialogue);
        }
        //add additional checks here for the rest of dialogue stuff
    }

    IEnumerator UnlockChecker()
    {
        yield return null;
        //unlock the catalog after the first run
        if (GameControl.SaveData.shiftCounter == 1 && !GameControl.SaveData.catalogUnlocked)
        {
            //QUEUE DIALOGUE HERE
            GameControl.SaveData.dialogueQueue.Enqueue(catalogUnlock);
            GameControl.SaveData.catalogUnlocked = true;
            GameControl.PlayerData.unlockDone = true;
        }
        //unlock research when the player has collected their first essence seed
        if (GameControl.SaveData.highSeeds > 0 && !GameControl.PlayerData.unlockDone && !GameControl.SaveData.researchUnlocked)
        {
            GameControl.SaveData.dialogueQueue.Enqueue(researchUnlock);
            phone.GetComponent<PhoneLerp>().callerKnown = false;
            GameControl.SaveData.researchUnlocked = true;
            GameControl.PlayerData.unlockDone = true;
        }
        //unlock completion tracker when the player has crafted 15 different crowns
        if (GameControl.CrownCompletion.totalDiscovered >= 20 && !GameControl.PlayerData.unlockDone && !GameControl.SaveData.completionUnlocked)
        {
            GameControl.SaveData.dialogueQueue.Enqueue(completionUnlock);
            GameControl.SaveData.completionUnlocked = true;
            GameControl.PlayerData.unlockDone = true;
        }
        GameControl.SaveHandler.SaveGame();

        //TODO - Almanac unlock when player discovers their first flower
    }

    IEnumerator BalanceUpdate()
    {
        float scoreBonus = GameControl.PlayerData.score / 100f;
        float timeBonus = GameControl.PlayerData.min * 60 + GameControl.PlayerData.sec;
        timeBonus /= 100f;
        //actually update the balance
        GameControl.SaveData.balance += scoreBonus + timeBonus;
        List<float> scoreBreakdown = new List<float>();
        scoreBreakdown.Add(GameControl.PlayerData.constructionScore / 100f);
        scoreBreakdown.Add(GameControl.PlayerData.enemyScore / 100f);
        scoreBreakdown.Add(GameControl.PlayerData.discoveryScore / 100f);
        scoreBreakdown.Add(GameControl.PlayerData.otherScore / 100f);
        //create the shiftReport object
        ShiftReport currentReport = new ShiftReport(GameControl.SaveData.shiftCounter, scoreBonus, scoreBreakdown, timeBonus, GameControl.PlayerData.shiftSeeds);
        GameControl.SaveData.shiftReports.Add(currentReport);
        GameControl.SaveHandler.SaveGame();
        yield return null;
    }

    IEnumerator InitialMove()
    {
        yield return new WaitForSeconds(2f);
        float time = 0f;
        float startingSize = mainCam.orthographicSize;
        Vector3 startingPos = transform.position;
        while (time < 3f)
        {
            mainCam.orthographicSize = Mathf.Lerp(startingSize, 10, time / 3f);
            transform.position = Vector3.Lerp(startingPos, new Vector3(0, 0, -10), time / 3f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }    
        yield return new WaitForSeconds(1f);
        GameControl.PlayerData.shiftJustEnded = false;
        GameControl.PlayerData.continuePressed = false;
        StartCoroutine(MenuInit());
    }

    IEnumerator MenuInit()
    {
        Cursor.visible = true;
        mainCam.orthographicSize = 10;
        transform.position = new Vector3(0, 0, -10);
        menuActive = true;
        carSprite.sprite = carMenu;
        playerSprite.SetActive(false);
        report.SetActive(true);
        controlPrompt.SetActive(true);

        //reset if no menus are active
        GameControl.PlayerData.menuActive = false;

        Debug.Log("currently " + GameControl.SaveData.dialogueQueue.Count + " in queue");
        if (GameControl.SaveData.dialogueQueue.Count > 0)
        {
            GameControl.PlayerData.menusReady = false;
            StartCoroutine(DialogueActivate());
        }
        else
        {
            GameControl.PlayerData.menusReady = true;
        }

        //activate the other menus once they are unlocked
        carKeys.SetActive(true);
        //TODO - implement checks for unlocks here
        if (GameControl.SaveData.researchUnlocked)
            research.SetActive(true);
        if (GameControl.SaveData.completionUnlocked || GameControl.PlayerData.testing)
            completionTracker.SetActive(true);
        if (GameControl.SaveData.catalogUnlocked)
            catalog.SetActive(true);
        if (GameControl.SaveData.almanacUnlocked || GameControl.PlayerData.testing)
            almanac.SetActive(true);
        yield return null;
    }

    IEnumerator DialogueActivate()
    {
        Debug.Log("dropping dialogue");
        //play phone ringing sound here
        yield return new WaitForSeconds(2f);
        //set the unknown caller sprite if it is the first run or any other new calls
        if (GameControl.SaveData.firstRun && !GameControl.SaveData.tutorialComplete)
        {
            phone.GetComponent<PhoneLerp>().callerKnown = false;
        }
        //phone.GetComponent<PhoneLerp>().inPlace = false;
        phone.SetActive(true);
        while (!phone.GetComponent<PhoneLerp>().inPlace)
        {
            yield return new WaitForEndOfFrame();
            if (phone.GetComponent<PhoneLerp>().inPlace)
                break;
        }
        speechBubble.SetActive(true);
        speechBubble.GetComponent<TextAdvancement>().setDialogue(GameControl.SaveData.dialogueQueue.Dequeue());
        while (!GameControl.PlayerData.dialogueComplete)
        {
            yield return new WaitForEndOfFrame();
            if (GameControl.PlayerData.dialogueComplete)
                break;
        }
        speechBubble.SetActive(false);
        if (!(GameControl.SaveData.firstRun && GameControl.SaveData.tutorialComplete) || GameControl.SaveData.contractSigned)
        {
            phone.GetComponent<PhoneLerp>().PhoneClose();
            GameControl.PlayerData.menusReady = true;
        }  
    }

    public void PostContract()
    {
        GameControl.SaveData.dialogueQueue.Enqueue(postContract);
        StartCoroutine(DialogueActivate());
    }


}
