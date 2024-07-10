using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneLerp : MonoBehaviour
{
    public bool inPlace = false;
    public bool callerKnown = true;

    [SerializeField] Sprite unknownCaller;
    [SerializeField] Sprite flowerGuy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {
        if (callerKnown)
            GetComponent<Image>().sprite = flowerGuy;
        else
            GetComponent<Image>().sprite = unknownCaller;

        inPlace = false;
        StartCoroutine(LerpUp());
    }

    public void PhoneClose()
    {
        StartCoroutine(LerpDown());
    }
    IEnumerator LerpUp()
    {
        float time = 0f;
        Vector2 finalPos = GetComponent<RectTransform>().position;
        Vector2 startingPos = new Vector2(finalPos.x, -Screen.height * 1.5f);

        while (time < 1f)
        {
            GetComponent<RectTransform>().position = Vector2.Lerp(startingPos, finalPos, time);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        inPlace = true;
    }

    IEnumerator LerpDown()
    {
        float time = 0f;
        Vector2 startingPos = GetComponent<RectTransform>().position;
        Vector2 finalPos = new Vector2(startingPos.x, -Screen.height * 1.5f);

        while (time < 1f)
        {
            GetComponent<RectTransform>().position = Vector2.Lerp(startingPos, finalPos, time);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }
}
