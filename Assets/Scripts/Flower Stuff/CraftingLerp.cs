using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CraftingLerp : MonoBehaviour
{
    RectTransform thisTrans;
    GameObject player;
    Vector2 startPos;
    Vector2 endPos;
    GameObject sash;
    GameObject mainCanvas;
    float maxTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate(string type, int startSide)
    {
        gameObject.SetActive(true);
        player = GameObject.FindWithTag("Player");
        sash = GameObject.FindWithTag("sash");
        mainCanvas = GameObject.FindWithTag("mainCanvas");
        transform.SetParent(mainCanvas.transform);
        thisTrans = GetComponent<RectTransform>();
        if (startSide == 1)
        {
            startPos = new Vector2(50f, 50f);
        }
        else
        {
            startPos = new Vector2(-50f, 50f);
        }
        thisTrans.anchoredPosition = startPos;
        //transform.localScale = 100f * transform.localScale;
        GetComponentInChildren<Image>().sprite = GameControl.PlayerData.SpriteAssign(type);
        if (gameObject.activeSelf)
            Debug.Log(type);

        //begin the initial lerp after the position and sprite are set
        StartCoroutine(initialLerp(type));


        //after the initial lerp, check if the bad boy is in the sash - fly there if true, deactivate if not
    }

    IEnumerator initialLerp(string type)
    {
        yield return null;
        Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        direction.Normalize();
        endPos = direction * 200f;
        float currentTime = 0f;
        while (currentTime < maxTime)
        {
            thisTrans.localPosition = Vector2.Lerp(startPos, endPos, currentTime);
            //thisTrans.localPosition = bruh;
            currentTime += Time.deltaTime;
            //Debug.Log(Time.time);
            yield return new WaitForSeconds(0.001f);
        }
        Debug.Log("laksdjflkasdjflkadsjfl");
        if (GameControl.PlayerData.sashTypes.Contains(type))
        {
            int index = GameControl.PlayerData.sashTypes.IndexOf(type);
            startPos = endPos;
            Transform lastPos = GameControl.PlayerData.currentAffinities[index].transform;
            //Debug.Log("bruh moment" + slots.Count());
            StartCoroutine(sashLerp(lastPos));
            GameControl.PlayerData.affinityIncrease(type);
            lastPos.GetComponent<SizeLerp>().Execute(false);
        }
        else
            gameObject.SetActive(false);
    }

    IEnumerator sashLerp(Transform endPos)
    {
        Vector2 finalPos = endPos.GetComponent<RectTransform>().localPosition;
        finalPos.x = sash.GetComponent<RectTransform>().localPosition.x;
        Debug.Log("This is the final" + finalPos);
        float currentTime = 0f;
        while (currentTime < maxTime) {
            thisTrans.localPosition = Vector2.Lerp(startPos, finalPos, currentTime);
            //thisTrans.localPosition = bruh;
            currentTime += Time.deltaTime;
            //Debug.Log(Time.time);
            yield return new WaitForSeconds(0.001f);
        }
        yield return null;
        //up the dictionary that corresponds

        gameObject.SetActive(false);
    }
}
