using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CrownThrowing : MonoBehaviour
{
    GameObject finalCrown;
    [SerializeField] public GameObject landingZone;
    public bool crownHeld = false;
    [SerializeField] float sizeMod;
    [SerializeField] float throwTime;
    [SerializeField] float throwSpeed;
    Vector2 endPos;

    GameObject projPool;
    [SerializeField] RepellentBehavior repellent;
    public float range;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        projPool = GameObject.FindWithTag("projectilePool");
        if (repellent != null)
        {
            range = repellent.range;
            speed = repellent.speed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (crownHeld)
        {
            //Debug.Log("crown held");
            finalCrown.transform.position = gameObject.transform.position;
            if ((GameControl.PlayerData.tutorialState >= 6 && GameControl.PlayerData.tutorialActive) || !GameControl.PlayerData.tutorialActive)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    crownHeld = false;
                    endPos = landingZone.GetComponent<LandingZone>().LockPos();
                    finalCrown.GetComponent<CrownAttack>().CrownActive();
                    gameObject.GetComponent<CrownConstruction>().CrownThrown();
                    StartCoroutine(CrownThrow());
                    AkSoundEngine.PostEvent("Throw", gameObject);
                }
            }
        }
        
    }

    //redo for precision throw mechanic
    IEnumerator CrownThrow()
    {
        float time = 0;
        Vector2 startPos = finalCrown.transform.position;
        throwTime = (startPos - endPos).magnitude / throwSpeed;
        //Vector2 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);
        direction.Normalize();
        finalCrown.GetComponent<Rigidbody2D>().velocity = direction * throwSpeed;
        Vector2 originalScale = finalCrown.transform.localScale;
        Vector2 lerpTarget = finalCrown.transform.localScale * sizeMod;
        bool halfwaySwap = false;

        while (time < throwTime)
        {
            if (time > throwTime / 2 && !halfwaySwap)
            {
                Debug.Log("goofy activate");
                originalScale = lerpTarget;
                lerpTarget = originalScale / sizeMod;
                halfwaySwap = true;
            }
            //finalCrown.transform.position = Vector2.Lerp(startPos, endPos, time / throwTime);
            finalCrown.transform.localScale = Vector2.Lerp(originalScale, lerpTarget, time / throwTime);
            time += Time.deltaTime;
            yield return null;
        }
        finalCrown.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        finalCrown.GetComponent<CrownAttack>().CrownArmed();
        landingZone.GetComponent<LandingZone>().Deactivate();
    }

    public void RepellentActivate()
    {
        StartCoroutine(Repelling());
    }

    IEnumerator Repelling()
    {
        transform.GetComponentInChildren<PlayerStatus>().repelling = true;
        float currentTime = 0f;
        float subTimer = 0f;
        while (currentTime < GameControl.PlayerData.repellentLength)
        {
            subTimer += Time.deltaTime;
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (subTimer > 0.2f)
            {
                Vector2 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = endPos - new Vector2(transform.position.x, transform.position.y);
                //direction.Normalize();
                float angleRadians = Mathf.Atan2(-direction.y, direction.x);
                if (angleRadians < 0)
                    angleRadians += 2 * Mathf.PI;
                angleRadians += Mathf.PI / 2;

                //spawn a repellent proj here
                float range = repellent.GetComponent<RepellentBehavior>().range;
                StartCoroutine(RepellentSpawn(angleRadians, range));
                subTimer = 0f;
            }
        }
        transform.GetComponentInChildren<PlayerStatus>().repelling = false;
    }

    IEnumerator RepellentSpawn(float angle, float range)
    {
        Vector2 projDir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        projDir.Normalize();
        GameObject proj = projPool.GetComponent<ObjectPool>().GetPooledObject();
        proj.transform.position = transform.position;

        proj.SetActive(true);
        proj.GetComponent<ProjectileBehavior>().RepellentSpawn(range, projDir);
        proj.GetComponentInChildren<Rigidbody2D>().velocity = projDir * speed;
        yield return null;
    }

    public void CompletedCrown(GameObject finishedCrown, float maxDist)
    {
        finalCrown = finishedCrown;
        //finalCrown.transform.localScale = Vector3.one * 0.6f;
        //crownHeld = true;
        //activate the landingZone
        landingZone.SetActive(true);
        landingZone.GetComponent<LandingZone>().Activate(maxDist);
        if (GameControl.PlayerData.tutorialState == 5)
            GameControl.PlayerData.crownComplete = true;
        /*Transform[] children = finalCrown.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.tag == "finalCrown")
                child.GetComponent<SpriteRenderer>().sortingOrder = 2;
            else
            {
                child.GetComponent<SpriteRenderer>().sortingOrder = 3;
            }
        }*/
    }
}
