using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepellentRange : MonoBehaviour
{
    [SerializeField] GameObject shadow;
    public PlayerStatus status;
    public int enemiesInRange = 0;

    bool statusSet = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*if (statusSet)
        {
            if (enemiesInRange > 0)
            {
                //Debug.Log("it should be working you fucking moron");
                status.DangerOn();
                //Debug.Log(status.inDanger);
            }
            else
            {
                status.DangerOff();
            }
        }*/
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("broski what");
        if (collision.transform.tag == "shadow" && (collision.transform.root.tag == "enemy" || collision.transform.root.tag == "boss" || collision.transform.root.tag == "bossPart"))
        {
            enemiesInRange++;
            //Debug.Log(enemiesInRange);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "shadow" && (collision.transform.root.tag == "enemy" || collision.transform.root.tag == "boss" || collision.transform.root.tag == "bossPart"))
        {
            enemiesInRange--;
        }
    }
}
