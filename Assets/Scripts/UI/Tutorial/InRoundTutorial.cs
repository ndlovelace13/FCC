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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SashIntro()
    {
        TutorialContinue(sashText);
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
