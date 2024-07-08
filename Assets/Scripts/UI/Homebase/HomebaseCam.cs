using System.Collections;
using System.Collections.Generic;
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
    public bool menuActive = false;

    //menu options
    [SerializeField] GameObject catalog;
    [SerializeField] GameObject research;
    [SerializeField] GameObject completionTracker;
    [SerializeField] GameObject carKeys;

    //Dialogue Stuff
    [SerializeField] GameObject phone;
    [SerializeField] GameObject speechBubble;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = GetComponent<Camera>();
        if (GameControl.PlayerData.shiftJustEnded || GameControl.PlayerData.continuePressed)
        {
            StartCoroutine(InitialMove());
            if (!GameControl.PlayerData.continuePressed)
            {
                Debug.Log("what the fuck");
                StartCoroutine(BalanceUpdate());
                StartCoroutine(UnlockChecker());
            }
            StartCoroutine(DialogueQueue());
        }
        else
        {
            StartCoroutine(MenuInit());
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DialogueQueue()
    {
        yield return null;
        if (GameControl.SaveData.firstRun)
        {
            //load intro dialogue, store in a text file eventually?
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
                "Sending you the details for a quick training session, we'll talk more when you arrive",
                "Looking forward to having you on the payroll, just try not to waste my time"
            };
            GameControl.SaveData.dialogueQueue.Enqueue(introDialogue);
        }
        //add additional checks here for the rest of dialogue stuff
    }

    IEnumerator UnlockChecker()
    {
        yield return null;
        if (GameControl.SaveData.shiftCounter == 1)
        {
            GameControl.SaveData.catalogUnlocked = true;
        }
        //unlock completion tracker when the player has crafted 15 different crowns

        //unlock research when the player has collected their first essence seed
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

        if (GameControl.SaveData.dialogueQueue.Count > 0)
        {
            StartCoroutine(DialogueActivate());
        }

        //activate the other menus once they are unlocked
        carKeys.SetActive(true);
        //TODO - implement checks for unlocks here
        if (GameControl.SaveData.researchUnlocked)
            research.SetActive(true);
        if (GameControl.SaveData.completionUnlocked)
            completionTracker.SetActive(true);
        if (GameControl.SaveData.catalogUnlocked)
            catalog.SetActive(true);
        yield return null;
    }

    IEnumerator DialogueActivate()
    {
        //play phone ringing sound here
        yield return new WaitForSeconds(2f);
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
        phone.GetComponent<PhoneLerp>().PhoneClose();
    }


}
