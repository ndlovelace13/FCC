using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchPage : Page
{
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
    public int index;
    public bool pageFilled = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pageFilled)
        {
            if (GameControl.SaveData.essenceCount <= 0 || currentResearch.maxResearchTimes == currentResearch.currentResearchTimes)
                donationButton.interactable = false;
            goalText.text = "Seeds Required for Goal: " + (currentResearch.requiredSeeds - currentResearch.currentSeeds);
        }
    }

    public void SetIndex(int ind)
    {
        index = ind;
    }

    public void Donate()
    {
        if (pageFilled)
        {
            Debug.Log("Donated once - " + gameObject.name);
            GameControl.SaveData.essenceCount--;
            currentResearch.currentSeeds++;
            if (currentResearch.requiredSeeds == currentResearch.currentSeeds)
                currentResearch.ResearchAction();
            GameControl.PlayerData.donationMade = true;
            UpdateMeter();
        }
    }

    public override void FillPage()
    {
        currentResearch = GameControl.PlayerData.researchItems[index];
        //donationButton.onClick.RemoveAllListeners();
        //donationButton.onClick.AddListener(Donate);
        image.sprite = currentResearch.resultImg;

        title.text = currentResearch.title;
        subheader.text = currentResearch.description;

        pageFilled = true;
        //initialize donation meter
        InitMeter();
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
