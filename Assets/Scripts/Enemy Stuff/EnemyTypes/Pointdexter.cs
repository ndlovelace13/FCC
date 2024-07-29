using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointdexter : EnemyBehavior
{
    [SerializeField]
    // Start is called before the first frame update
    /*void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/


    //the basic behavior of the enemy
    public override IEnumerator StandardBehavior()
    {
        while (gameObject.activeSelf)
        {
            //SPEED UP MAY NEED REWORK
            GameObject[] crowns = GameObject.FindGameObjectsWithTag("finalCrown");
            foreach (GameObject obj in crowns)
            {
                if (obj.GetComponent<CrownAttack>().crownActive == true)
                    crown = obj.transform;
            }
            if (crown != null)
            {
                //Debug.Log("target swap called");
                TargetSwap();
            }
            else
            {
                if (target == crown)
                    StartCoroutine(Surprised(surpriseTime));
                target = player;
            }
            if (!isFrozen && !surprised)
                moveSpeed = backupSpeed;
            //movement
            if (!isBlinded)
            {
                //Debug.Log(target.position);
                Vector2 direction = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y);
                direction.Normalize();
                gameObject.GetComponent<Rigidbody2D>().velocity = direction * moveSpeed;
            }
            else
                Debug.Log("Enemy isn't currently tracking");
            if (health <= 0)
            {
                Deactivate();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public override IEnumerator StateUpdate()
    {
        //target acquisition

        //pointing (buffing)

        //retreating - do this if player gets too close 
        yield return null;
    }
}
