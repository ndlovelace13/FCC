using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    protected GameObject player;
    protected Transform lerpTarget;
    protected bool pickingUp = false;
    protected bool placed = false;
    // Start is called before the first frame update
    protected void Start()
    {
        
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!pickingUp && placed)
        {
            Vector2 currentDist = player.transform.position - transform.position;
            //Debug.Log(currentDist.magnitude);
            if (currentDist.magnitude < GameControl.PlayerData.pickupDist)
            {
                pickingUp = true;
                StartCoroutine(PlayerLerp(1f));
            }
        }
    }

    protected virtual void OnEnable()
    {
        pickingUp = false;
        placed = false;
        player = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerMovement>().gameObject;
    }

    protected IEnumerator PlayerLerp(float lerpTime)
    {
        float time = 0f;
        while (time < lerpTime)
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, lerpTarget.position, time / lerpTime);
            if ((transform.localPosition - lerpTarget.position).magnitude < 0.05f)
                break;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        AssignValue();
        gameObject.SetActive(false);
    }

    protected virtual void AssignValue()
    {

    }
}
