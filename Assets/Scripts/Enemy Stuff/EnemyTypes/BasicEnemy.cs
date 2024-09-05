using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : EnemyBehavior
{
    // Start is called before the first frame update
    /*void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            '
    }*/

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
                rb2D.MovePosition(rb2D.position + direction * moveSpeed * Time.deltaTime);
            }
            //death check
            if (health <= 0)
            {
                Deactivate();
            }

            //boss summon check
            if (GameControl.PlayerData.bossSpawning && summoning == false)
            {
                StartCoroutine(BossSummoning());
                summoning = true;
            }
            if (sacrifice)
            {
                StartCoroutine(Sacrifice());
                yield break;
            }  
            yield return new WaitForEndOfFrame();
        }
    }
}
