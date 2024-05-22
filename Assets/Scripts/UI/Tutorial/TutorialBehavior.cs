using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialBehavior : MonoBehaviour
{
    [SerializeField] GameObject speech;
    [SerializeField] GameObject phone;
    [SerializeField] TMP_Text objective;
    [SerializeField] GameObject player;
    [SerializeField] GameObject firstFlower;
    [SerializeField] GameObject firstCrown;
    string[] text1 = 
    {
        "So you wanted a little practice before getting tossed into the hellho --- um, I mean birthday party?",
        "Alright, I guess, practice makes perfect. You're already here, so let's get on with it. Let's make it quick though, I'm not paying you to learn on the job",
        "Ugh, hirees expect so much these days",
        "My apologies for only being present over the phone, unfortunately I have much more pressing matters to attend to. Don't worry, though, I can still monitor your progress - I have my ways",
        "Since this is our first time working together, I'll assume nothing. Show me that you know how to walk first - not much point in moving forward if you're immobile"
    };
    string[] text2 =
    {
        "Phew, got a bit nervous there for a second. Thought I may have to go back to the drawing board and hire a professional...",
        "Now that I know you can move, let's into the business of flowers",
        "You see that flower in front of you? Pick it up. Please."
    };
    string[] text3 =
    {
        "Very good. You're going to be doing A LOT of that",
        "The next thing to cover is tossing flowers",
        "While your primary objective is to make flower crowns, some circumstances might call for passing out a single flower",
        "For example, if you pick up a flower by accident and want to clear space for something else",
        "Or if you need to keep a particularly excited child at bay - give it a try"
    };
    string[] text4 =
    {
        "Keep that skill in mind, it may come in handy",
        "Good, you've got the basics down now. I'm so proud. Let's get to the fun part",
        "Once you have collected five flowers, you can construct a flower crown at any time",
        "Doing so is beneficial for a couple of reasons:",
        "1. Tossing a completed crown can \"pacify\" multiple children at once\n2. I'll pay you for each crown you make",
        "Now give it a try. I value symmetry above all else, so try to make your crowns pretty - it pays"
    };
    string[] text5 =
    {
        "Crafting a crown takes a bit of skill. Your movement will be slowed while crafting, but if you're quick about it, that shouldn't be TOO much of an issue",
        "Now get to it"
    };
    string[] text6 =
    {
        "You truly are a jack of all trades, well done",
        "Once you've constructed a flower crown, all that's left is to throw it",
        "The little beasts love them so much, they'll swarm to claim it for themselves",
        "Did I say beasts? Angels! I mean angels, they're absolutely...wonderful",
        "Throwing a crown will distract the children, allowing you to work towards creating your next crown",
        "Give it a try!"
    };

    List<string[]> tutorialText;
    string[] objectiveText =
    {
        "hugh mungus what",
        "Use WASD or the arrow keys to move in all four cardinal directions",
        "Use the spacebar to pick up your first flower",
        "Use the mouse to aim and click the right mouse button to fire a single flower",
        "Pick up five flowers and press E to craft your first flower crown",
        "Follow the input queues to finish crafting your first crown", 
        "Use the mouse to aim and click the left mouse button to toss your first crown"
    };
    bool conditionMet = true;
    // Start is called before the first frame update
    void Start()
    {
        PlayerDisable();
        firstFlower.SetActive(false);
        tutorialText = new List<string[]> { text1, text2, text3, text4, text5, text6 };
        //tutorialText.Add(text1);
        //tutorialText.Add(text2);
        //tutorialText.Add(text3);
        GameControl.PlayerData.tutorialActive = true;
        speech.GetComponent<TextAdvancement>().setDialogue(tutorialText[GameControl.PlayerData.tutorialState]);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameControl.PlayerData.dialogueComplete && speech.activeSelf)
            TutorialAdvance();
    }

    public void TutorialAdvance()
    {
        conditionMet = false;
        speech.SetActive(false);
        phone.SetActive(false);
        GameControl.PlayerData.tutorialState++;
        StartCoroutine(objectiveUpdate());
    }

    IEnumerator checkCondition()
    {
        while (!conditionMet)
        {
            yield return new WaitForSeconds(0.1f);
            switch (GameControl.PlayerData.tutorialState)
            {
                case 1: 
                    if (GameControl.PlayerData.inputsTested == 4)
                    {
                        conditionMet = true;
                        break;
                    }
                    break;
                case 2:
                    if (GameControl.PlayerData.flowerHarvested == true)
                    {
                        conditionMet = true;
                        break;
                    }
                    break;
                case 3:
                    if (GameControl.PlayerData.singleTossed == true)
                    {
                        conditionMet = true;
                        break;
                    }
                    break;
                case 4:
                    if (GameControl.PlayerData.crownConstructionStarted == true)
                    {
                        conditionMet = true;
                        break;
                    }
                    break;
                case 5:
                    if (GameControl.PlayerData.crownComplete == true)
                    {
                        conditionMet = true;
                        break;
                    }
                    break;
                case 6:
                    if (GameControl.PlayerData.crownThrown == true)
                    {
                        conditionMet = true;
                        break;
                    }
                    break;
                default: Debug.Log("unhandled state"); conditionMet = true; break;
            }
        }
        StartCoroutine(objectiveComplete());
        yield return null;
    }

    IEnumerator objectiveUpdate()
    {
        objective.color = Color.white;
        objective.text = objectiveText[GameControl.PlayerData.tutorialState];
        PlayerEnable();
        StartCoroutine(checkCondition());
        yield return null;
    }

    IEnumerator objectiveComplete()
    {
        objective.color = Color.green;
        yield return new WaitForSeconds(1);
        objective.text = "";
        PlayerDisable();
        speech.SetActive(true);
        speech.GetComponent<TextAdvancement>().setDialogue(tutorialText[GameControl.PlayerData.tutorialState]);
        phone.SetActive(true);
        ObjectEnable();
    }

    public void PlayerDisable()
    {
        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponent<FlowerHarvest>().enabled = false;
        player.GetComponent<CrownThrowing>().enabled = false;
        player.GetComponent<CrownConstruction>().enabled = false;
        player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }

    public void PlayerEnable()
    {
        if (GameControl.PlayerData.tutorialState > 0)
        {
            player.GetComponent<PlayerMovement>().enabled = true;
        }
        if (GameControl.PlayerData.tutorialState > 1)
        {
            player.GetComponent<FlowerHarvest>().enabled = true;
        }
        if (GameControl.PlayerData.tutorialState > 3)
        {
            player.GetComponent<CrownConstruction>().enabled = true;
        }
        if (GameControl.PlayerData.tutorialState > 4)
        {
            player.GetComponent<CrownThrowing>().enabled = true;
        }
    }

    public void ObjectEnable()
    {
        if (GameControl.PlayerData.tutorialState == 1)
        {
            firstFlower.SetActive(true);
            firstFlower.transform.position = player.transform.position + new Vector3(-5, 0);
        }
        if (GameControl.PlayerData.tutorialState == 3)
        {
            firstCrown.SetActive(true);
            firstCrown.transform.position = player.transform.position;
        }
    }
}
