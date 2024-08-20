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
            Vector2 currentDist = player.transform.position - transform.position;
            //Debug.Log(currentDist.magnitude);
            if (currentDist.magnitude < GameControl.PlayerData.pickupDist)
            {
                pickingUp = true;
                StartCoroutine(PlayerLerp());
            }
        }
    }

    public void LootDrop()
    {
        StartCoroutine(LootLerp());
    }

    IEnumerator LootLerp()
    {
        Vector2 currentLoc = transform.localPosition;
        Vector2 finalLootLoc = currentLoc + new Vector2(Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f));
        float currentTime = 0f;
        while (currentTime < 0.5f)
        {
            transform.localPosition = Vector2.Lerp(currentLoc, finalLootLoc, currentTime / 0.5f);
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one * 0.25f;
        player = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerMovement>().gameObject;
        pickingUp = false;
        GetComponent<SizeLerp>().lerping = false;
        GetComponent<SizeLerp>().Execute(true);
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
            transform.localPosition = Vector2.Lerp(transform.localPosition, player.transform.position, time);
            if ((transform.localPosition - player.transform.position).magnitude < 0.05f)
                break;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        GameControl.SaveData.essenceCount++;
        GameControl.PlayerData.shiftSeeds++;
        //first seed is picked up
        if (!GameControl.SaveData.firstSeed)
           GameControl.SaveData.firstSeed = true;
        gameObject.SetActive(false);
    }
}
