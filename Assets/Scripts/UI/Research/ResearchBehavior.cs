using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResearchBehavior : MonoBehaviour
{
    [SerializeField] GameObject[] donateSlots;
    [SerializeField] GameObject donateElem;
    // Start is called before the first frame update

    [SerializeField] TMP_Text balance;

    bool newLineReady = true;

    [SerializeField] GameObject speech;
    string[] currentLine = new string[1];
    string firstLine = "Here you'll find a list of active research drives! The R&D team is always looking to provide you with new tech, based on essence seeds dropped from enemies on the job";
    string[] introLines = { "Got Seeds?", "Our top scientists would be thrilled by your donations", "I don't know how it works, do I sound like a scientist?", "This is way beyond my understanding" };
    string[] donateLines = { "Yeah, science!", "R&D is cooking up something special JUST for you!", "Who's a helpful crown maker? You are, you are!", "Can't wait to see those skinwalker faces when they see THIS", "Check in every week for new research drives! You may rest, but our scientists don't" };
    void Start()
    {
        //GameObject catalogSlots = GameObject.FindWithTag("upgradeSlots");
        //upgradeSlots = GetComponentsInChildren<Transform>(catalogSlots);
        /*upgradeSlots = GetComponentsInChildren<RectTransform>();
        upgradeSlots = upgradeSlots.Where(child => child.tag == "upgrade").ToArray();
        upgradeSlots[0].gameObject.GetComponent<Upgrade>().SetValues("uncommon", 3f, 1.25f, 10, 0.02f);*/
        /*foreach (Upgrade upgrade in GameControl.PlayerData.upgrades)
        {
            //GameObject newUpgrade = Instantiate(upgradeElem);
            //newUpgrade.GetComponent<UpgradeElement>().setUpgrade(upgrade);
            //newUpgrade.transform.SetParent(gameObject.transform);
            int available = nextSlot();
            GameObject newUpgrade = upgradeSlots[available];
            newUpgrade.SetActive(true);
            newUpgrade.tag = "slotFull";
            newUpgrade.GetComponent<UpgradeElement>().setUpgrade(upgrade);
            //newUpgrade.transform.position = upgradeSlots[available].position;
        }*/
        IntroLineAssign();
    }

    // Update is called once per frame
    void Update()
    {
        balance.text = "Essence Seeds: " + GameControl.SaveData.essenceCount;
        if (GameControl.PlayerData.donationMade)
        {
            if (newLineReady)
                PurchaseLineAssign();
            GameControl.PlayerData.donationMade = false;
        }
    }

    private int nextSlot()
    {
        for (int i = 0; i < donateSlots.Length; i++)
        {
            if (donateSlots[i].tag == "slotEmpty")
            {
                return i;
            }
        }
        return -1;
    }

    private void PurchaseLineAssign()
    {
        newLineReady = false;
        StartCoroutine(NewLineReset());
        currentLine[0] = donateLines[Random.Range(0, donateLines.Length)];
        speech.GetComponent<TextAdvancement>().setDialogue(currentLine);
    }

    IEnumerator NewLineReset()
    {
        yield return new WaitForSeconds(5);
        newLineReady = true;
    }

    private void IntroLineAssign()
    {
        if (GameControl.SaveData.firstResearch)
        {
            GameControl.SaveData.firstResearch = false;
            currentLine[0] = firstLine;
        }
        else
        {
            currentLine[0] = introLines[Random.Range(0, introLines.Length)];
        }
        speech.GetComponent<TextAdvancement>().setDialogue(currentLine);
    }

    public void ReturnToEnd()
    {
        GameControl.SaveHandler.SaveGame();
        SceneManager.LoadScene("Homebase");
    }
}
