using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullyStats : EnemyStats
{
    public float insultDebuff = 0.5f;
    public float insultLength = 7f;

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
        return "The Bully";
    }

    public override string GetSubtext()
    {
        return "Boss Variant";
    }

    public override string GetDescription()
    {
        return "Placeholder placeholder placeholder placeholder";
    }

    public override string GetBehavior()
    {
        return "Placeholder placeholder placeholder placeholder placeholder placeholder";
    }

    public override string GetWeakness()
    {
        return "- Placeholder placeholder placeholder\n- Placeholder placeholder placeholder placeholder placeholder";
    }
}
