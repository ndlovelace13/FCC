using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBehavior : MonoBehaviour
{
    //public FlowerStats stats;
    public string type;
    public int position = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //stats = GameControl.PlayerData.flowerStatsDict[type];
        Animator animator = transform.parent.GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetInteger("rarity", GetComponent<FlowerStats>().rarity);
    }
}
