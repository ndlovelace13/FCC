using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CatalogBehavior : MonoBehaviour
{
    [SerializeField] GameObject[] upgradeSlots;
    [SerializeField] GameObject upgradeElem;
    // Start is called before the first frame update

    [SerializeField] TMP_Text balance;

    [SerializeField] GameObject speech;
    string[] currentLine = new string[1];
    string firstLine = "This is the catalog, your one stop shop for improvements of all sorts. Give me a call when you want to order something - you're welcome for the express shipping";
    string[] introLines = {"Yes? What'll it be?", "You always call at the BEST times - what is it?", "Time to spend that hard earned cash?", "You again, shouldn't you be making crowns or something?"};
    string[] purchaseLines = { "Anything else?", "Hope it helps", "Good choice, you needed that one", "On the way", "They tell me the catalog is expanding sometime soon, I'll believe it when I see it" };
    void Start()
    {
        //GameObject catalogSlots = GameObject.FindWithTag("upgradeSlots");
        //upgradeSlots = GetComponentsInChildren<Transform>(catalogSlots);
        /*upgradeSlots = GetComponentsInChildren<RectTransform>();
        upgradeSlots = upgradeSlots.Where(child => child.tag == "upgrade").ToArray();
        upgradeSlots[0].gameObject.GetComponent<Upgrade>().SetValues("uncommon", 3f, 1.25f, 10, 0.02f);*/
        foreach (Upgrade upgrade in GameControl.PlayerData.upgrades)
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
        }
        IntroLineAssign();
    }

    // Update is called once per frame
    void Update()
    {
        balance.text = "Current Balance: " + string.Format("{0:C}", GameControl.PlayerData.balance);
        if (GameControl.PlayerData.purchaseMade)
        {
            GameControl.PlayerData.purchaseMade = false;
            PurchaseLineAssign();
        }
    }

    private int nextSlot()
    {
        for (int i = 0; i < upgradeSlots.Length; i++)
        {
            if (upgradeSlots[i].tag == "slotEmpty")
            {
                return i;
            }
        }
        return -1;
    }

    private void PurchaseLineAssign()
    {
        currentLine[0] = purchaseLines[Random.Range(0, purchaseLines.Length)];
        speech.GetComponent<TextAdvancement>().setDialogue(currentLine);
    }

    private void IntroLineAssign()
    {
        if (GameControl.PlayerData.firstCatalog)
        {
            GameControl.PlayerData.firstCatalog = false;
            currentLine[0] = firstLine;
        }
        else
        {
            currentLine[0] = introLines[Random.Range(0, introLines.Length)];
        }
        speech.GetComponent<TextAdvancement>().setDialogue(currentLine);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
