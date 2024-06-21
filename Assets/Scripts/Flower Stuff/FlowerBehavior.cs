using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBehavior : MonoBehaviour
{
    //public FlowerStats stats;
    public string type;
    public int position = 0;
    public bool picked = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //stats = GameControl.PlayerData.flowerStatsDict[type];
        
        if (!picked)
        {
            Animator animator = transform.parent.GetChild(0).GetComponentInChildren<Animator>();
            animator.SetInteger("rarity", GetComponent<FlowerStats>().rarity);
        }
            
    }

    private void OnEnable()
    {
        picked = true;
    }

    private void OnDisable()
    {
        picked = true;
    }
}
