using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (transform.parent.tag == "enemy" || transform.parent.tag == "boss")
            transform.parent.GetComponent<EnemyBehavior>().CollisionCheck(other); 
        if (transform.parent.tag == "flower" && other.gameObject.transform.root.tag == "Player" && !GameControl.PlayerData.loading)
        {
            FlowerStats thisFlower = transform.parent.GetComponentInChildren<FlowerStats>();
            GameControl.PlayerData.flowerStatsDict[thisFlower.type].OnHitboxEnter(transform.parent.gameObject);
        }
        if (transform.parent.tag == "bossPart")
        {
            transform.parent.GetComponent<BossExtension>().CollisionCheck(other);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (transform.parent.tag == "finalCrown")
            transform.parent.GetComponent<CrownAttack>().CollisionCheck(collision);
    }



}
