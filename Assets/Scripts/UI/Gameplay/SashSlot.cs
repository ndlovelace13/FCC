using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SashSlot : MonoBehaviour
{
    [SerializeField] Image flower;
    [SerializeField] Image tier;
    [SerializeField] GameObject scoreNotif;
    public int currentTier = 0;
    string currentType = "null";
    FlowerStats currentStats = null;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().color = Color.clear;
        scoreNotif = GameObject.FindWithTag("scoreAnnounce");
        BaseTier();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpriteApply(Sprite newSprite)
    {
        flower.sprite = newSprite;
        GetComponent<SizeLerp>().Execute(false);
    }

    public void NewFlower(string type)
    {
        //reset the values of the old flowerType if one is there
        currentTier = 0;
        if (currentStats != null)
            currentStats.UpdateAffinity(currentTier);
        currentType = type;
        //find the flowerStats associated with type
        currentStats = GameControl.PlayerData.flowerStatsDict[currentType];
        SpriteApply(GameControl.PlayerData.SpriteAssign(type));
    }

    public void BaseTier()
    {
        tier.sprite = GameControl.PlayerData.affinityTiers[currentTier];
        tier.color = Color.clear;
    }

    public void tierUp()
    {
        scoreNotif.GetComponent<ScoreNotification>().newFeed("Affinity Tier Increased | +" + GameControl.PlayerData.tierScores[currentTier]);
        currentTier++;
        tier.sprite = GameControl.PlayerData.affinityTiers[currentTier];
        tier.color = Color.white;
        GetComponent<SizeLerp>().Execute(false);
        //update all flowerStats here
        currentStats.UpdateAffinity(currentTier);
        //score bonus for tiering up?
    }
}
