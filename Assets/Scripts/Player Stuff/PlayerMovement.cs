using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rigid;
    private Animator animator;
    private float moveLimiter = 0.7f;
    private float craftingSlow = 0.5f;
    float moveHorizontal;
    float moveVertical;

    bool leftTested = false;
    bool rightTested = false;
    bool upTested = false;
    bool downTested = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        //rigid = GetComponent<Rigidbody2D>();
        speed = GameControl.PlayerData.playerSpeed;
        craftingSlow = GameControl.PlayerData.craftingSlow;
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
            animator.SetBool("isMoving", true);
        }
        else if (moveHorizontal == 0 && moveVertical == 0)
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
        }

        if (GameControl.PlayerData.tutorialState == 1)
        {
            if (!leftTested)
            {
                if (moveHorizontal < 0)
                {
                    leftTested = true;
                    GameControl.PlayerData.inputsTested++;
                }
            }
            if (!rightTested)
            {
                if (moveHorizontal > 0)
                {
                    rightTested = true;
                    GameControl.PlayerData.inputsTested++;
                }
            }
            if (!downTested)
            {
                if (moveVertical < 0)
                {
                    downTested = true;
                    GameControl.PlayerData.inputsTested++;
                }
            }
            if (!upTested)
            {
                if (moveVertical > 0)
                {
                    upTested = true;
                    GameControl.PlayerData.inputsTested++;
                }
            }
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
