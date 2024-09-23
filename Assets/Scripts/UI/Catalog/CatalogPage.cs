using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogPage : Page
{
    public int startingIndex;

    public List<GameObject> upgradeSlots;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStartIndex(int ind)
    {
        startingIndex = ind;
    }

    public override void FillPage()
    {
        //Debug.Log("Starting index is: " + startingIndex);
        int currentIndex = startingIndex;
        foreach (GameObject slot in upgradeSlots)
        {
            if (currentIndex < GameControl.PlayerData.upgrades.Count)
            {
                slot.SetActive(true);
                slot.tag = "slotFull";
                slot.GetComponent<UpgradeElement>().setUpgrade(GameControl.PlayerData.upgrades[currentIndex]);
            }
            else
                slot.SetActive(false);
            currentIndex++;
        }
    }
}
