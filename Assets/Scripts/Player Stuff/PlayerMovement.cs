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
    Vector2 movement;

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
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();

        if (movement != Vector2.zero)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (GameControl.PlayerData.tutorialState == 1)
        {
            if (!leftTested)
            {
                if (movement.x < 0)
                {
                    leftTested = true;
                    GameControl.PlayerData.inputsTested++;
                }
            }
            if (!rightTested)
            {
                if (movement.x > 0)
                {
                    rightTested = true;
                    GameControl.PlayerData.inputsTested++;
                }
            }
            if (!downTested)
            {
                if (movement.y < 0)
                {
                    downTested = true;
                    GameControl.PlayerData.inputsTested++;
                }
            }
            if (!upTested)
            {
                if (movement.y > 0)
                {
                    upTested = true;
                    GameControl.PlayerData.inputsTested++;
                }
            }
        }

        rigid.velocity = movement * speed;
    }

    private void FixedUpdate()
    {
        //gameObject.transform.position += new Vector3(moveHorizontal * speed, moveVertical * speed) * Time.deltaTime;
        
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
