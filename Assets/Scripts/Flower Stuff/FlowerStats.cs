using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerStats : MonoBehaviour
{
    [SerializeField] public int rarity;
    [SerializeField] public string type;

    public int basePoints;
    public int damage;
    public int range;
    public int projCount;

    [SerializeField]
    int[] pointsTiers = new int[4];
    [SerializeField]
    int[] damageTiers = new int[4];
    [SerializeField]
    int[] rangeTiers = new int[4];
    [SerializeField]
    int[] projTiers = new int[4];
    [SerializeField] public int position;
    //associated projectile script
    //associated augment script
    // Start is called before the first frame update
    void Start()
    {
        UpdateAffinity(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateAffinity(int tier)
    {
        basePoints = pointsTiers[tier];
        damage = damageTiers[tier];
        range = rangeTiers[tier];
        projCount = projTiers[tier];
    }
}
