using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DonationItemBehavior : MonoBehaviour
{
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text description;
    [SerializeField] Image image;
    [SerializeField] TMP_Text goalText;
    [SerializeField] Button donationButton;

    //donation stuff
    [SerializeField] RectTransform meterFill;
    [SerializeField] RectMask2D meterMask;
    float maxMask;
    float targetMask;
    float currentMask;


    [SerializeField] Research currentResearch;
    [SerializeField] int index;

    // Start is called before the first frame update
    void Start()
    {
        currentResearch = GameControl.PlayerData.researchItems[index];
        donationButton.onClick.RemoveAllListeners();
        donationButton.onClick.AddListener(Donate);
        image.sprite = currentResearch.resultImg;

        //initialize donation meter
        InitMeter();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameControl.SaveData.essenceCount == 0 || currentResearch.maxResearchTimes == currentResearch.currentResearchTimes)
            donationButton.interactable = false;
        goalText.text = "Seeds Required for Goal: " + (currentResearch.requiredSeeds - currentResearch.currentSeeds);
    }

    public void Donate()
    {
        Debug.Log("Donated once");
        GameControl.SaveData.essenceCount--;
        currentResearch.currentSeeds++;
        if (currentResearch.requiredSeeds == currentResearch.currentSeeds)
            currentResearch.ResearchAction();
        GameControl.PlayerData.donationMade = true;
        UpdateMeter();
    }

    public void InitMeter()
    {
        maxMask = meterFill.rect.height - meterMask.padding.w - meterMask.padding.y; 
        targetMask = meterMask.padding.w;
        UpdateMeter();
    }

    public void UpdateMeter()
    {
        float targetHeight = currentResearch.currentSeeds * maxMask / currentResearch.requiredSeeds;
        currentMask = maxMask + targetMask - targetHeight;
        var padding = meterMask.padding;
        padding.w = currentMask;
        meterMask.padding = padding;

    }
}
