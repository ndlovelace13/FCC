using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InRoundTutorial : MonoBehaviour
{
    [SerializeField] GameObject speech;
    [SerializeField] GameObject phone;
    string[] sashText =
    {
        "Finally decided to purchase the floral sash, huh? About time...",
        "R&D's been hard at work producing a tool to help you increase your power on the job, thanks to the essence seeds you contributed",
        "Now you'll be able to select a couple flowers every shift to specialize in - the more you use them, the stronger they get",
        "Unfortunately, we don't have the tech to preserve these affinity boosts, thus, your sash will reset everytime you bite the dust",
        "To equip a flower to your sash, press the number key associated with the slot - you'll see their affinity tiers upgrade as you craft crowns with those flowers",
        "You can replace a slot at any time by pressing the corresponding number key again, however you will lose all affinity to the replaced flower",
        "Get out there and make some money - now with even more efficiency!"
    };
    string[] repelText =
    {
        "Replicant Repellent, eh? Guess those essence seeds really do help!",
        "According to Jill, this spray will allow you to ward off those pesky replicants for a short amount of time",
        "Your goal is still to eliminate as many as you can - however, this new tech should come in handy if you find yourself in a tight spot",
        "Just be sure to give it a good shake first!",
        "Oh yeah, each can of Repellent is single-use. Be sure to use it only when absolutely necessary!",
        "However, we will be sure to refill your supply before each shift, so don't be TOO stingy with them. Good luck out there!"
    };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SashIntro()
    {
        TutorialContinue(sashText);
    }

    public void RepellIntro()
    {
        TutorialContinue(repelText);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameControl.PlayerData.dialogueComplete && speech.activeSelf)
        {
            Debug.Log("teehee");
            speech.SetActive(false);
            phone.SetActive(false);
            GameControl.PlayerData.FinishIntro();
        }
    }

    public void TutorialContinue(string[] currentText)
    {
        //set score text to be this during tutorial
        //objective.text = "Press Space to advance/skip dialogue";
        phone.SetActive(true);
        speech.SetActive(true);
        speech.GetComponent<TextAdvancement>().setDialogue(currentText);
    }
}
