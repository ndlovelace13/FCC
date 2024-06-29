using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialBehavior : MonoBehaviour
{
    [SerializeField] GameObject speech;
    [SerializeField] GameObject phone;
    [SerializeField] GameObject popUp;
    [SerializeField] TMP_Text objective;
    [SerializeField] GameObject player;
    [SerializeField] GameObject firstFlower;
    [SerializeField] GameObject firstCrown;
    [SerializeField] GameObject fireCrown;
    GameObject fireInstance;
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
        "Now that I know you can move, let's get into the business of flowers",
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
    string[] text7 =
    {
        "Easy enough, huh? Don't mind the side-effects of the crown, those projectiles are harmless - to humans",
        "There's a lot that goes into the... \"effectiveness\" of a flower crown - be creative and experiment",
        "Never know what may happen when symmetry and rarity meet",
        "Speaking of, I've noticed a couple of, let's say, non-traditional flowers popping up at the venue",
        "For example, that red one right in front of you - try crafting a crown with it",
        "Remember, symmetry is key"
    };
    string[] text8 =
    {
        "Woah, flammable flowers?? Incredible",
        "Be on the lookout for new and interesting types of flowers - wouldn't want to leave nature's unique creations unacknowledged",
        "I'm sure the children will LOVE to see what crowns you come up with ",
        "They just better hope they don't stand too close hehe",
        "That wraps up your training - you can put Flower Crown Crafter Certification on your resume now",
        "I'll see you at the party - oh, you won't see me though",
        "One last thing, don't make direct contact with the... children - I hate paperwork",
        "Let us get to work (when I say us, I mean you of course)!"
    };

    List<string[]> tutorialText;
    string[] objectiveText =
    {
        "hugh mungus what",
        "Use WASD or the arrow keys to move in all four cardinal directions",
        "Use the spacebar to pick up your first flower",
        "Use the mouse to aim and click the right mouse button or Q to fire a single flower",
        "Pick up five flowers and press E to craft your first flower crown",
        "Follow the input queues to finish crafting your first crown", 
        "Use the mouse to aim and click the left mouse button to toss your first crown",
        "Create and throw a crown with the unnatural flower in its center"
    };
    bool conditionMet = true;
    // Start is called before the first frame update
    void Start()
    {
        PlayerDisable();
        phone.SetActive(false);
        speech.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameControl.PlayerData.dialogueComplete && speech.activeSelf)
            TutorialAdvance();
        if (GameControl.PlayerData.tutorialState == 7 && GameControl.PlayerData.fireReset == true)
        {
            GameControl.PlayerData.fireReset = false;
            GameObject newInstance = Instantiate(fireCrown);
            Destroy(fireInstance);
            fireInstance = newInstance;
            fireInstance.transform.localPosition = player.transform.localPosition;
            fireInstance.transform.SetParent(null);
            fireInstance.SetActive(true);
        }
    }

    public void TutorialAdvance()
    {
        conditionMet = false;
        speech.SetActive(false);
        phone.SetActive(false);
        if (GameControl.PlayerData.tutorialState != 7)
        {
            GameControl.PlayerData.tutorialState++;
            StartCoroutine(objectiveUpdate());
        }
        else
        {
            StartCoroutine(beginGameplay());
        }
    }

    IEnumerator beginGameplay()
    {
        //animation of some sort here
        yield return null;
        GameControl.PlayerData.tutorialActive = false;
        GameControl.PlayerData.tutorialState = 0;
        SceneManager.LoadScene("Gameplay");
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
                case 7:
                    if (GameControl.PlayerData.redCrown == true)
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
        objective.color = Color.white;
        objective.text = "Press Space to advance/skip dialogue";
        PlayerDisable();
        speech.SetActive(true);
        speech.GetComponent<TextAdvancement>().setDialogue(tutorialText[GameControl.PlayerData.tutorialState]);
        phone.SetActive(true);
        ObjectEnable();
    }

    public void PlayerDisable()
    {
        player.GetComponentInChildren<Animator>().SetBool("isMoving", false);
        player.GetComponentInChildren<PlayerMovement>().enabled = false;
        player.GetComponentInChildren<FlowerHarvest>().enabled = false;
        player.GetComponent<CrownThrowing>().enabled = false;
        player.GetComponent<CrownConstruction>().enabled = false;
        player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }

    public void PlayerEnable()
    {
        if (GameControl.PlayerData.tutorialState > 0)
        {
            player.GetComponentInChildren<PlayerMovement>().enabled = true;
        }
        if (GameControl.PlayerData.tutorialState > 1)
        {
            player.GetComponentInChildren<FlowerHarvest>().enabled = true;
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
        if (GameControl.PlayerData.tutorialState == 6)
        {
            fireInstance = Instantiate(fireCrown);
            fireInstance.transform.localPosition = player.transform.localPosition;
            fireInstance.transform.SetParent(null);
            fireInstance.SetActive(true);
        }
    }

    public void TutorialSkip()
    {
        GameControl.PlayerData.tutorialActive = false;
        GameControl.PlayerData.tutorialSkip = true;
        SceneManager.LoadScene("Gameplay");
    }

    public void TutorialContinue()
    {
        popUp.SetActive(false);
        tutorialText = new List<string[]> { text1, text2, text3, text4, text5, text6, text7, text8 };
        //tutorialText.Add(text1);
        //tutorialText.Add(text2);
        //tutorialText.Add(text3);
        GameControl.PlayerData.tutorialActive = true;
        objective.text = "Press Space to advance/skip dialogue";
        phone.SetActive(true);
        speech.SetActive(true);
        speech.GetComponent<TextAdvancement>().setDialogue(tutorialText[GameControl.PlayerData.tutorialState]);
    }
}
