using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BasicStats : EnemyStats
{
    // Start is called before the first frame update
    void Start()
    {
        title = "The Skinwalker";
        subtext = "Very Common Variant";
        description = "Just your everyday child-replicating skinwalker. Below average speed, intelligence, and looks, this variant's only strength lies in numbers.";
        behavior = "This variant will relentlessly chase its closest target - whether that be you or a flower crown";
        weakness = "- Everything\n- Easily Distracted & Easily Surprised";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override string GetTitle()
    {
        return "The Skinwalker";
    }

    public override string GetSubtext()
    {
        return "Very Common Variant";
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
