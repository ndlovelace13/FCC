using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletionCamera : MonoBehaviour
{
    Vector3 movement;
    Camera cam;
    Node finalNode;
    float maxMag = 0f;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        //camera movement inhibitor in directions
        if (finalNode != null)
        {
            if (transform.position.x < -maxMag && movement.x < 0)
                movement.x = 0;
            else if (transform.position.x > maxMag && movement.x > 0)
                movement.x = 0;
            if (transform.position.y < -maxMag && movement.y < 0)
                movement.y = 0;
            if (transform.position.y > maxMag && movement.y > 0)
                movement.y = 0;
        }
        movement.Normalize();

        transform.position += movement * Time.deltaTime * 20f;
        if (cam.orthographicSize <= 5f && Input.mouseScrollDelta.y > 0f)
            cam.orthographicSize = 5f;
        else if (cam.orthographicSize >= 50f && Input.mouseScrollDelta.y < 0f)
            cam.orthographicSize = 50f;
        else
            cam.orthographicSize -= Input.mouseScrollDelta.y;
    }

    public void NodeSet(Node lastNode)
    {
        if (lastNode.basePos.magnitude > maxMag)
        {
            finalNode = lastNode;
            maxMag = finalNode.basePos.magnitude;
            //Debug.Log("maxMag set to " + maxMag);
            //Debug.Log(lastNode.transform.position);
        }
        
    }
}
