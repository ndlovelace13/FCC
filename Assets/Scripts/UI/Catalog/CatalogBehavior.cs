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
    }

    // Update is called once per frame
    void Update()
    {
        balance.text = "Current Balance: " + string.Format("{0:C}", GameControl.PlayerData.balance);
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

    public void StartGame()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
