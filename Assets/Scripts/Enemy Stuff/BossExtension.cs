using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossExtension : EnemyBehavior
{
    public EnemyBehavior activeBoss;
    // Start is called before the first frame update
    /*void Start()
    {
        activeBoss = GameObject.FindWithTag("boss").GetComponent<EnemyBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    /*public override void Activate()
    {
        activeBoss = GameObject.FindWithTag("boss").GetComponent<EnemyBehavior>();
        Debug.Log(activeBoss.gameObject.name);
        base.Activate();
    }*/

    public override void CollisionCheck(Collider2D other)
    {
        if (other.gameObject.tag == "projectile")
        {
            GameObject otherParent = other.gameObject.transform.parent.gameObject;
            if (!otherParent.GetComponent<ProjectileBehavior>().enemyProj)
            {
                DealDamage(otherParent.GetComponent<ProjectileBehavior>().damage, Color.white);
                actualAugs = otherParent.GetComponent<ProjectileBehavior>().getActualAugs();
                activeBoss.AugmentApplication(actualAugs);
                otherParent.GetComponent<ProjectileBehavior>().ObjectDeactivate();
            }
        }
        else if (other.gameObject.tag == "aoe")
        {
            GameObject otherParent = other.gameObject.transform.parent.gameObject;
            actualAugs = otherParent.GetComponent<AoeBehavior>().getActualAugs();
            activeBoss.AugmentApplication(actualAugs);
        }
    }

    public override void DealDamage(int damage, Color color)
    {
        AkSoundEngine.PostEvent("EnemyHit", gameObject);
        activeBoss.health -= damage;
        if (activeBoss.health < activeBoss.maxHealth / 2 && GetComponentInChildren<HatBehavior>() != null)
            activeBoss.GetComponentInChildren<HatBehavior>().HatFall();
        GameObject newNotif = Instantiate(notif, transform.position + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)), Quaternion.identity);
        newNotif.GetComponent<DamageNotif>().Creation(damage.ToString(), color);
    }
}
