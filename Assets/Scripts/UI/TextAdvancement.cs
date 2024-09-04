using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextAdvancement : MonoBehaviour
{
    [SerializeField] TMP_Text speechBubble;
    [SerializeField] GameObject spaceToSkip;
    string[] dialogue;
    string currentLine;
    string alphaTag = "<alpha=#00>";
    int index = 0;
    bool skipText = false;
    public bool skippable = false;
    // Start is called before the first frame update
    void Start()
    {
        //currentLine = "My name is Hugh Mungus - Hugh Mungus what? - Hugh Mungus";
        //StartCoroutine(speech());
        if (!skippable)
        {
            spaceToSkip.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameControl.PlayerData.speaking && Input.GetKeyDown(KeyCode.Space))
        {
            printNext();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            skipText = true;
        }
    }

    public void setDialogue(string[] newDialogue)
    {
        StopAllCoroutines();
        dialogue = newDialogue;
        GameControl.PlayerData.dialogueComplete = false;
        index = -1;
        printNext();
    }

    public string nextDialogue()
    {
        if (index < dialogue.Length - 1)
        {
            index++;
            return dialogue[index];
        }
        return "bruh";
    }

    public void printNext()
    {
        //AkSoundEngine.PostEvent("DialogueRandom", gameObject);
        currentLine = nextDialogue();
        if (currentLine == "bruh")
            GameControl.PlayerData.dialogueComplete = true;
        else
            StartCoroutine(speech());
    }

    IEnumerator speech()
    {
        var talkSound = AkSoundEngine.PostEvent("DialogueStart", gameObject);
        //AkSoundEngine.Seek(talkSound, Random.Range(0f, 1f), false);
        GameControl.PlayerData.speaking = true;
        speechBubble.text = alphaTag + currentLine;
        for (int i = 0; i < currentLine.Length; i++)
        {
            yield return new WaitForSeconds(0.05f);
            speechBubble.text = currentLine.Substring(0, i+1) + alphaTag + currentLine.Substring(i+1);
            if (skipText)
            {
                speechBubble.text = currentLine;
                break;
            }
        }
        AkSoundEngine.StopPlayingID(talkSound);
        skipText = false;
        GameControl.PlayerData.speaking = false;
        yield return null;
    }
}
