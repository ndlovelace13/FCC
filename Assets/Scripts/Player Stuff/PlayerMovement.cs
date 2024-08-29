using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rigid;
    [SerializeField] Animator animator;
    private float craftingSlow = 0.5f;
    Vector2 movement;

    bool leftTested = false;
    bool rightTested = false;
    bool upTested = false;
    bool downTested = false;
    // Start is called before the first frame update
    void Start()
    {
        //animator = transform.GetComponentInChildren<Animator>(true);
        //rigid = GetComponent<Rigidbody2D>();
        speed = GameControl.PlayerData.playerSpeed;
        craftingSlow = GameControl.PlayerData.craftingSlow;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameControl.PlayerData.loading && !GameControl.PlayerData.gamePaused && !GameControl.PlayerData.gameOver && !GameControl.PlayerData.gameWin)
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
    }

    public void IntroMove()
    {
        StartCoroutine(IntroMoving());
    }
    IEnumerator IntroMoving()
    {
        Debug.Log("starting intro");
        Camera.main.transform.parent = null;
        Vector3 screenEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, 0));
        screenEdge += new Vector3(0, 20);
        transform.parent.transform.position = screenEdge;
        Debug.Log("player placed");
        float time = 0f;
        Debug.Log(gameObject.name);
        animator = transform.parent.GetComponentInChildren<Animator>(true);
        while (time < 3f)
        {
            animator.SetBool("isMoving", true);
            transform.parent.transform.position = Vector2.Lerp(screenEdge, Vector2.zero, time / 3f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("player done moving");
        animator.SetBool("isMoving", false);
        Camera.main.transform.parent = transform.parent;
        Debug.Log("ending intro");
        GameControl.PlayerData.NewUnlocks();
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
            GameControl.PlayerData.savedEnemyDict[other.transform.root.GetComponent<EnemyBehavior>().type].deathCount++;
            GameOver();
        }
        if (other.gameObject.tag == "projectile" || other.gameObject.tag == "aoe")
        {
            GetComponent<PlayerStatus>().CollisionCheck(other);
        }
    }

    public void GameOver()
    {
        StartCoroutine(GameOverAnim());
    }

    IEnumerator GameOverAnim()
    {
        //TODO - set the death anim/sprite here
        Time.timeScale = 1f;
        GameControl.PlayerData.gameOver = true;
        GameControl.PlayerData.shiftJustEnded = true;
        animator.Play("Death");
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(1f);
        float time = 0f;
        float currentCam = Camera.main.orthographicSize;
        while (time < 2f)
        {
            Camera.main.transform.localPosition = new Vector3(0f, Mathf.Lerp(0f, 0.85f, time / 2f), -10);
            Camera.main.orthographicSize = Mathf.Lerp(currentCam, 0.5f, time / 2f);
            time += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene("Homebase");
    }

    public void GameWin(Vector3 finalPos, string type)
    {
        StartCoroutine(GameWinAnim(finalPos, type));
    }

    IEnumerator GameWinAnim(Vector3 finalPos, string type)
    {
        animator.SetBool("isMoving", false);
        rigid.velocity = Vector3.zero;
        GameControl.PlayerData.shiftJustEnded = true;
        yield return new WaitForSeconds(2f);
        
        Vector3 currentPos = transform.parent.transform.position;
        finalPos = finalPos + Vector3.up * 3;
        //lerp the player to directly above the poppy position
        float time = 0f;
        while (time < 2.5f)
        {
            animator.SetBool("isMoving", true);
            transform.parent.transform.position = Vector3.Lerp(currentPos, finalPos, time / 2.5f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.SetBool("isMoving", false);

        //poppy discovery pop-up and fade to black
        GameControl.PlayerData.GameWinBehavior(type);

        //display a win message
        yield return null;
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
