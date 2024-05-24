using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour
{
    //elements of upgrade UI
    [SerializeField]
    public string title, description;

    public Sprite icon;

    //need to give a sprite as well

    string upgradeKey;
    public string unit;
    public float currentValue;
    public float upgradeAmount;
    public float basePrice;
    public float currentPrice;
    float priceInflation;
    public int timesUpgraded = 0;
    public int maxTimesUpgraded;
    // Start is called before the first frame update
    void Start()
    {
        currentPrice = basePrice;
        currentValue = GameControl.PlayerData.upgradeDict[upgradeKey];
    }

    public void SetValues(string key, float priceBase, float inflation, int maximumUpgrades, float increaseAmount, string title, string description, string units, Sprite newIcon)
    {
        upgradeKey = key;
        basePrice = priceBase;
        priceInflation = inflation;
        maxTimesUpgraded = maximumUpgrades;
        upgradeAmount = increaseAmount;
        this.title = title;
        this.description = description;
        unit = units;
        icon = newIcon;
    }

    // Update is called once per frame
    void Update()
    {
        if (upgradeKey != null)
            currentValue = GameControl.PlayerData.upgradeDict[upgradeKey];
    }

    public void Purchase()
    {
        //PlayerPrefs.SetFloat("balance", PlayerPrefs.GetFloat("balance") - currentPrice);
        GameControl.PlayerData.balance -= currentPrice;
        //PlayerPrefs.SetFloat(upgradeKey, PlayerPrefs.GetFloat(upgradeKey) + upgradeAmount);
        GameControl.PlayerData.upgradeDict[upgradeKey] = currentValue + upgradeAmount;
        timesUpgraded++;
        GameControl.PlayerData.UpgradeApply();
        GameControl.PlayerData.purchaseMade = true;
        StartCoroutine(PriceUp());
    }

    IEnumerator PriceUp()
    {
        currentPrice = currentPrice * priceInflation;
        yield return null;
    }
}
