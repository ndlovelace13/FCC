using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Fist : BossExtension
{
    [SerializeField] GameObject forearm;
    [SerializeField] GameObject fist;
    [SerializeField] GameObject fistShadow;

    float passedTime = 0f;
    float stateTime = 0f;

    public bool direction;
    // Start is called before the first frame update
    void Start()
    {
        activeBoss = GameObject.FindWithTag("boss").GetComponent<EnemyBehavior>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerStatus>().transform;

        if (transform.position.x < player.position.x)
            direction = true;
        else
            direction = false;
        //Debug.Log(activeBoss.gameObject.name);
        StartCoroutine(Grow());
        StartCoroutine(ShadowHandler());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFrozen)
        {
            moveSpeed = backupSpeed;
            passedTime += Time.deltaTime;
        }
    }

    IEnumerator Grow()
    {
        //reset the time vars
        stateTime = 1f;
        passedTime = 0f;
        
        float finalRot;
        if (direction)
            finalRot = -90f;
        else
            finalRot = 90f;

        //starting vals
        fist.transform.localPosition = Vector3.zero;
        Vector3 currentSize = fist.transform.localScale;

        //forearm disabled
        forearm.SetActive(false);

        //fist start at same pos as base - size 1 --> grow up a bunch, lerp size to 3, rotate towards direction of movement at same time

        while (passedTime < stateTime)
        {
            fist.transform.localPosition = Vector3.Lerp(Vector3.zero, Vector3.up * 1.75f, passedTime / stateTime);
            fist.transform.localRotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0, 0, finalRot), passedTime / stateTime);
            fist.transform.localScale = Vector3.Lerp(currentSize, currentSize * 2, passedTime / stateTime);
            yield return new WaitForEndOfFrame();
        }
        //grow the base, play growth anim - pivot at the base
        

        //yield return new WaitForSeconds(1);
        StartCoroutine(Hook());
    }

    IEnumerator Hook()
    {
        //reset the time vars
        stateTime = 1.5f;
        passedTime = 0f;

        int dir;
        if (direction)
            dir = 1;
        else
            dir = -1;

        forearm.SetActive(true);
        //if (!direction)
          //  forearm.GetComponent<SpriteRenderer>().flipX = true;
        //handle the forearm in its own IEnumerator
        StartCoroutine(ForearmStretch());

        float fullSpeedTime = 1f;

        while (passedTime < fullSpeedTime)
        {
            //fist fly forward for duration of the hook, might lerp to slow at the end?
            fist.transform.localPosition = fist.transform.localPosition + new Vector3(moveSpeed * Time.deltaTime * dir, 0f);
            //Debug.Log(fist.transform.localPosition);

            yield return new WaitForEndOfFrame();
        }

        //handle the slow down lerp here
        while (passedTime < stateTime)
        {
            float currentSpeed = Mathf.Lerp(moveSpeed, 0, ((passedTime - fullSpeedTime)/ (stateTime - fullSpeedTime)));
            fist.transform.localPosition = fist.transform.localPosition + new Vector3(currentSpeed * Time.deltaTime * dir, 0f);
            yield return new WaitForEndOfFrame();
        }

        

        //base in place

        //forearm stretch between fist starting pos and fist pivot
        

        //yield return new WaitForSeconds(1.5f);
        StartCoroutine(Return());
    }

    IEnumerator Return()
    {
        //reset the time vars
        stateTime = 1.5f;
        passedTime = 0f;

        int dir;
        if (direction)
            dir = 1;
        else
            dir = -1;

        float fullSpeedTime = 1f;

        while (passedTime < fullSpeedTime)
        {
            //fist fly forward for duration of the hook, might lerp to slow at the end?
            fist.transform.localPosition = fist.transform.localPosition + new Vector3(moveSpeed * Time.deltaTime * -dir, 0f);
            //Debug.Log(fist.transform.localPosition);

            yield return new WaitForEndOfFrame();
        }

        //handle the slow down lerp here
        while (passedTime < stateTime)
        {
            float currentSpeed = Mathf.Lerp(moveSpeed, 0, ((passedTime - fullSpeedTime) / (stateTime - fullSpeedTime)));
            fist.transform.localPosition = fist.transform.localPosition + new Vector3(currentSpeed * Time.deltaTime * -dir, 0f);
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(Despawn());
    }

    IEnumerator Despawn()
    {
        //reset the time vars
        stateTime = 1f;
        passedTime = 0f;

        //starting vals
        Vector3 currentPos = fist.transform.localPosition;
        Vector3 currentSize = fist.transform.localScale;
        Quaternion startingRot = fist.transform.localRotation;

        //forearm disabled
        forearm.SetActive(false);
        GetComponent<Animator>().SetTrigger("shrink");

        //fist start at same pos as base - size 1 --> grow up a bunch, lerp size to 3, rotate towards direction of movement at same time

        while (passedTime < stateTime)
        {
            fist.transform.localPosition = Vector3.Lerp(currentPos, Vector3.zero, passedTime / stateTime);
            fist.transform.localRotation = Quaternion.Lerp(startingRot, Quaternion.identity, passedTime / stateTime);
            fist.transform.localScale = Vector3.Lerp(currentSize, currentSize / 2, passedTime / stateTime);
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
        //StartCoroutine(Destroy());
    }

    IEnumerator ShadowHandler()
    {
        float startingScale = fist.transform.localScale.x;
        while (gameObject.activeSelf)
        {
            //set the fist shadow to the same x position as the fist
            fistShadow.transform.localPosition = new Vector3(fist.transform.localPosition.x, fistShadow.transform.localPosition.y);

            //set the fistShadow localScale to the same as the fist
            //Debug.Log("current scale = " + fist.transform.localScale.x + " starting scale: " + startingScale);
            fistShadow.transform.localScale = fist.transform.localScale;
            yield return new WaitForEndOfFrame();
        }
        
    }

    IEnumerator ForearmStretch()
    {
        Vector3 startingFist = fist.transform.localPosition;
        while (forearm.activeSelf)
        {
            Vector2 length = fist.transform.localPosition - startingFist;
            /*float angleRadians = Mathf.Atan2(length.y, -length.x);
            if (angleRadians < 0)
                angleRadians += 2 * Mathf.PI;
            Vector2 direction = new Vector2(Mathf.Sin(angleRadians), Mathf.Cos(angleRadians));*/
            forearm.transform.localPosition = Vector3.Lerp(startingFist, fist.transform.localPosition, 0.5f);
            //forearm.transform.localRotation = Quaternion.LookRotation(Vector3.forward, direction);

            forearm.transform.localScale = new Vector2(length.magnitude / 4.75f, forearm.transform.localScale.y);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }



}
