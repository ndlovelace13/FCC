using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeElement : MonoBehaviour
{
    [SerializeField]
    TMP_Text title, description, current, cost;
    [SerializeField]
    Button purchaseButton;
    [SerializeField] Image icon;
    Upgrade currentUpgrade;

    bool upgradeSet = false;
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        if (upgradeSet)
        {
            if (currentUpgrade.currentPrice > GameControl.PlayerData.balance || currentUpgrade.timesUpgraded == currentUpgrade.maxTimesUpgraded)
            {
                purchaseButton.interactable = false;
            }

            //set current and cost text to reflect the variables
            current.text = "Current Value: " + currentUpgrade.currentValue + " +" + currentUpgrade.upgradeAmount + currentUpgrade.unit;
            cost.text = "Upgrade Cost: " + string.Format("{0:C}", currentUpgrade.currentPrice);
            //Debug.Log(currentUpgrade);
        }
        //disable purchase button if not enough money or max upgrades reached
    }

    public void setUpgrade(Upgrade newUpgrade)
    {
        currentUpgrade = newUpgrade;
        purchaseButton.onClick.AddListener(currentUpgrade.Purchase);
        title.text = currentUpgrade.title;
        description.text = currentUpgrade.description;
        upgradeSet = true;
        icon.sprite = currentUpgrade.icon;
    }
}
