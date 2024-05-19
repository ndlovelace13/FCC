using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rigid;
    private float moveLimiter = 0.7f;
    private float craftingSlow = 0.5f;
    float moveHorizontal;
    float moveVertical;
    // Start is called before the first frame update
    void Start()
    {
        //rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal != 0 && moveVertical != 0)
        {
            moveHorizontal *= moveLimiter;
            moveVertical *= moveLimiter;
        }  
    }

    private void FixedUpdate()
    {
        rigid.velocity = new Vector2(moveHorizontal * speed, moveVertical * speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("what the sigma");
        if (other.gameObject.tag == "enemy")
        {
            //Debug.Log(other.gameObject.transform.position);
            //Debug.Log(transform.position);
            SceneManager.LoadScene("EndScreen");
        }
    }

    public void CraftingSlow()
    {
        speed *= craftingSlow;
    }

    public void CraftingDone()
    {
        speed /= craftingSlow;
    }
}
