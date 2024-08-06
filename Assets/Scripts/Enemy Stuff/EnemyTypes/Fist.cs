using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        player = player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<CapsuleCollider2D>().transform;

        if (transform.position.x < player.position.x)
            direction = true;
        else
            direction = false;
        //Debug.Log(activeBoss.gameObject.name);
        StartCoroutine(Grow());
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
            finalRot = 90f;
        else
            finalRot = -90f;

        //starting vals
        fist.transform.localPosition = Vector3.zero;
        Vector3 currentSize = fist.transform.localScale;

        //forearm disabled
        forearm.SetActive(false);

        //fist start at same pos as base - size 1 --> grow up a bunch, lerp size to 3, rotate towards direction of movement at same time

        while (passedTime < stateTime)
        {
            fist.transform.localPosition = Vector3.Lerp(Vector3.zero, Vector3.up * 3, passedTime / stateTime);
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
        //fist fly forward for duration of the hook, might lerp to slow at the end?
        fist.GetComponent<Rigidbody2D>().velocity = Vector3.up * moveSpeed;

        //base in place

        //forearm stretch between fist starting pos and fist pivot
        forearm.SetActive(true);

        yield return new WaitForSeconds(2);
        StartCoroutine(Return());
    }

    IEnumerator Return()
    {
        //fist fly back for duration of return - MIGHT NEED TO B LONGER
        fist.GetComponent<Rigidbody2D>().velocity = Vector3.down * moveSpeed * 2;

        //base in place

        //forearm does the same as before
        yield return new WaitForSeconds(1);
        StartCoroutine(Despawn());
    }

    IEnumerator Despawn()
    {
        //fist rotate back and shrink, lerping down to base
        
        //base plays shrink anim

        //forearm deactivate
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }

}
