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
        return "Just your everyday child-replicating skinwalker. Below average speed, intelligence, and looks, this variant's only strength lies in numbers.";
    }

    public override string GetBehavior()
    {
        return "This variant will relentlessly chase its closest target - whether that be you or a flower crown";
    }

    public override string GetWeakness()
    {
        return "- Everything\n- Easily Distracted & Easily Surprised";
    }
}
