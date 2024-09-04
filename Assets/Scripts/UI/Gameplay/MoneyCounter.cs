using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyCounter : MonoBehaviour
{
    float prevCount = 0;
    float currentCount = 0;
    float comboTime = 0f;
    float comboDecay = 3f;
    Vector3 initSize;

    TMP_Text textObj;
    // Start is called before the first frame update
    void Start()
    {
        textObj = GetComponent<TMP_Text>();
        textObj.alpha = 0f;
        StartCoroutine(LerpHandler());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //call this whenever money is added to the total
    public void MoneyAdded(int value)
    {
        currentCount += value;
        GetComponent<RectTransform>().localScale = GetComponent<RectTransform>().localScale + Vector3.one * (value / 100f);
    }

    IEnumerator LerpHandler()
    {
        while (true)
        {
            if (prevCount < currentCount)
            {
                textObj.alpha = 1f;
                comboTime = 0f;
                initSize = GetComponent<RectTransform>().localScale;
            }
            else if (textObj.alpha == 0f)
            {
                currentCount = 0;
                initSize = Vector3.one;
            }
            else
            {
                textObj.alpha = Mathf.Lerp(1f, 0f, comboTime / comboDecay);
                GetComponent<RectTransform>().localScale = Vector3.Lerp(initSize, Vector3.one, comboTime / comboDecay);
            }
            textObj.text = "+ " + string.Format("{0:C}", (float)currentCount / 100f);
            prevCount = currentCount;
            comboTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
