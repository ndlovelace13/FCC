using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceBehavior : MonoBehaviour
{
    GameObject player;
    bool pickingUp = false;
    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!pickingUp)
        {
            Vector2 currentDist = player.transform.localPosition - transform.position;
            //Debug.Log(currentDist.magnitude);
            if (currentDist.magnitude < GameControl.PlayerData.pickupDist)
            {
                pickingUp = true;
                StartCoroutine(PlayerLerp());
            }
        }
    }

    private void OnEnable()
    {
        player = GameObject.FindWithTag("Player");
        pickingUp = false;
        GetComponent<SizeLerp>().Execute();
    }

    IEnumerator StartLerp()
    {
        //make a little parabola here
        yield return null;
    }

    IEnumerator PlayerLerp()
    {
        float time = 0f;
        while (time < 1f)
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, player.transform.localPosition, time);
            if ((transform.localPosition - player.transform.localPosition).magnitude < 0.05f)
                break;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        GameControl.PlayerData.essenceCount++;
        gameObject.SetActive(false);
    }
}
