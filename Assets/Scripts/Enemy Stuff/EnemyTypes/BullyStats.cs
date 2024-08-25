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
        return "This variant's imposing demeanor takes after that of a school-yard bully. Rather than taking lunch money from the children it torments, this bully absorbs life energy from its fellow skinwalkers. Don't blame the kid, blame the parenting.";
    }

    public override string GetBehavior()
    {
        return "The Bully's experience with hand-to-hand combat makes it a fearsome foe. It will keep its distance until it assaults you with one of its four attacks: a punch, charge, insult, or hook";
    }

    public override string GetWeakness()
    {
        return "- Eliminating or stalling skinwalkers when the Bully is spawning will decrease it's maximum health\n- Its hook attack leaves the Bully stationary and vulnerable - damaging its fist will also injure the target itself";
    }
}
