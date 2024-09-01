using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointdexterStats : EnemyStats
{
    [SerializeField] public float pointCharge;
    [SerializeField] public float retreatTime;
    [SerializeField] public float pointCooldown;

    [SerializeField] public float buffRange;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override string GetTitle()
    {
        return "The Pointdexter";
    }

    public override string GetSubtext()
    {
        return "Common Variant";
    }

    public override string GetDescription()
    {
        return "The Pointdexter variant is a bit more lucid than other replicants - however, this intelligence has manifested in a passive aggressive personality. It prefers to pull the strings rather than take matters into their (three) hands";
    }

    public override string GetBehavior()
    {
        return "This variant is aware of the threat you pose - it will avoid direct confrontation. Instead, it influences its allies by way of a speed boost and cleansing of any status effects.";
    }

    public override string GetWeakness()
    {
        return "- The pointdexter is weakest before and after pointing - it can't run away\n- Freezing the replicant will delay its buffing powers";
    }
}
