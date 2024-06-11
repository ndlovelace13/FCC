using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlowerStats : MonoBehaviour
{
    [SerializeField] public int rarity;
    [SerializeField] public string type;

    public int basePoints;
    public int damage;
    public int range;
    public int projCount;
    public int projRange;

    public Sprite headSprite;
    public Sprite projSprite;
    public GameObject pool;

    [SerializeField]
    int[] pointsTiers = new int[4];
    [SerializeField]
    int[] damageTiers = new int[4];
    [SerializeField]
    int[] rangeTiers = new int[4];
    [SerializeField]
    int[] projTiers = new int[4];

    [SerializeField] public int aug;

    [SerializeField] public string primaryText, insideText, outsideText, fourText, fiveText;
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

    public virtual void applyAug()
    {

    }
}
