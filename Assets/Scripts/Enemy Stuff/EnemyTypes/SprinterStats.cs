using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprinterStats : EnemyStats
{
    [SerializeField] public float sprintLength;
    [SerializeField] public float sprintSpeed;
    [SerializeField] public float chargeTime;
    [SerializeField] public float sprintCooldown;
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
        return "The Sprinter";
    }

    public override string GetSubtext()
    {
        return "Common Variant";
    }

    public override string GetDescription()
    {
        return "The Sprinter appears to have taken on the form and characteristics of a sporty, \"popular\" kid. Unfortunately, this includes their abysmal sense of color coordination.";
    }

    public override string GetBehavior()
    {
        return "This variant is notable for it's short bursts of speed. It will normally chase its target, briefly slow down, then blindly sprint towards it";
    }

    public override string GetWeakness()
    {
        return "- Freezing and Stunning the target will reset its sprint cycle\n- The target will not sprint after you while blinded";
    }
}
