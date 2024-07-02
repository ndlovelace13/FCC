using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportBehavior : MonoBehaviour
{
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
        StartCoroutine(LerpUp());
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
    }
    
}
