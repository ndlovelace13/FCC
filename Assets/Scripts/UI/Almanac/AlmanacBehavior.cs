using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlmanacBehavior : MonoBehaviour
{
    bool newLineReady = true;

    [SerializeField] GameObject speech;
    string[] currentLine = new string[1];
    string firstLine = "Ooh you decided to check out the Almanac!!! It may look somewhat incomplete at the moment, but I promise to keep it up to date as you continue to work! I hope you like the stickers";
    string[] almanacLines = { 
        "Be sure to turn to the back for the most important data I collect - yours!", 
        "If you ever forget how to do your job (it happens to the best of us), I included some informational pages towards the back!", 
        "As you encounter new flowers and skinwalker variants, I'll be adding in their pages for easy reference. You can always access a page easily by clicking on the stickers on the homepage!",
        "Each flower's page features helpful information and stats about their behavior - as you discover new ways to grow their power, you can also toggle to see their updated stats!", 
        "Ever wonder what your most used flower is? Turn to the back to see - of course, I keep track of that! There are very few things about you I DON'T keep track of...",
        "Skinwalker variant pages feature important stats, like their health, speed, how many times you've eliminated them, and how many times they've eliminated you (nothing personal, that's just the job)"
    };
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(NewLineAssign());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator NewLineAssign()
    {
        while (true)
        {
            if (GameControl.SaveData.firstAlmanac)
            {
                GameControl.SaveData.firstAlmanac = false;
                currentLine[0] = firstLine;
            }
            else
            {
                currentLine[0] = almanacLines[Random.Range(0, almanacLines.Length)];
            }
            speech.GetComponent<TextAdvancement>().setDialogue(currentLine);
            yield return new WaitForSeconds(30);
        }
    }

    public void Homebase()
    {
        SceneManager.LoadScene("Homebase");
    }
}
