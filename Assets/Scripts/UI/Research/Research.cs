using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchData
{
    public int currentResearchTimes;
    public int maxResearchTimes;
    public int currentSeeds;
    public int requiredSeeds;

    public void SetData(Research input)
    {
        currentResearchTimes = input.currentResearchTimes;
        maxResearchTimes = input.maxResearchTimes;
        currentSeeds = input.currentSeeds;
        requiredSeeds= input.requiredSeeds;

        //increment the maxResearchTimes if this is being instantiated for the first time
        GameControl.SaveData.researchAmount += maxResearchTimes;
    }
}

public abstract class Research : MonoBehaviour
{
    [SerializeField]
    [TextArea] public string title;

    [SerializeField]
    [TextArea] public string description;

    [SerializeField] public GameObject unlockPrefab;
    [SerializeField] public Sprite resultImg;
    public int requiredSeeds;
    public int maxResearchTimes;

    public int currentResearchTimes = 0;
    public int currentSeeds = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestoreData(ResearchData savedData)
    {
        currentResearchTimes = savedData.currentResearchTimes;
        currentSeeds = savedData.currentSeeds;
        requiredSeeds = savedData.requiredSeeds;
        maxResearchTimes = savedData.maxResearchTimes;
    }

    public virtual void ResearchAction()
    {

    }
}
