using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LandingZone : MonoBehaviour
{
    Transform crosshair, player;
    float maxDistance;
    float maxSquared;
    bool locked = false;
    // Start is called before the first frame update
    void Start()
    {
        crosshair = GameObject.FindWithTag("crosshair").transform;
        player = GameObject.FindWithTag("Player").transform;
        player.GetComponent<CrownThrowing>().landingZone = gameObject;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LocationUpdate()
    {
        while (!locked)
        {
            float mag = (crosshair.position - player.position).magnitude;
            if (mag <= maxDistance)
            {
                transform.position = crosshair.position;
            }
            else
            {
                Vector3 direction = crosshair.position - player.position;
                direction.Normalize();
                transform.position = player.position + direction * maxDistance;
            }
            yield return null;
            //yield return new WaitForEndOfFrame();
        }
    }

    public Vector2 LockPos()
    {
        locked = true;
        Vector2 endPos = transform.position;
        return endPos;
        //might need to send distance to the throw script to know where to go to
    }

    public void Activate(float maxDist)
    {
        
        maxDistance = maxDist;
        StartCoroutine(LocationUpdate());
    }

    public void Deactivate()
    {
        locked = false;
        gameObject.SetActive(false);
    }
}
