using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CrownThrowing : MonoBehaviour
{
    GameObject finalCrown;
    public bool crownHeld;
    [SerializeField] float sizeMod;
    [SerializeField] float throwTime;
    [SerializeField] float throwSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (crownHeld)
        {
            finalCrown.transform.position = gameObject.transform.position;
            if ((GameControl.PlayerData.tutorialState >= 6 && GameControl.PlayerData.tutorialActive) || !GameControl.PlayerData.tutorialActive)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    crownHeld = false;
                    finalCrown.GetComponent<CrownAttack>().CrownActive();
                    gameObject.GetComponent<CrownConstruction>().CrownThrown();
                    StartCoroutine(CrownThrow());
                }
            }
        }
        
    }

    //redo for precision throw mechanic
    IEnumerator CrownThrow()
    {
        float time = 0;
        Vector2 startPos = finalCrown.transform.position;
        Vector2 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
            finalCrown.transform.localScale = Vector2.Lerp(originalScale, lerpTarget, time / throwTime);
            time += Time.deltaTime;
            yield return null;
        }
        finalCrown.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        finalCrown.GetComponent<CrownAttack>().CrownArmed();
    }

    public void CompletedCrown(GameObject finishedCrown)
    {
        finalCrown = finishedCrown;
        finalCrown.transform.localScale = Vector3.one * 0.6f;
        crownHeld = true;
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
