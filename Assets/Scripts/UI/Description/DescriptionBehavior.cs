using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DescriptionBehavior : MonoBehaviour
{
    [SerializeField] GameObject[] textFlow;
    int counter = 0;
    float cooldown = 0f;
    bool ready = true;
    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject go in textFlow)
        {
            if (go.GetComponent<TMP_Text>() != null)
                go.GetComponent<TMP_Text>().alpha = 0f;
            else
            {
                Image image = go.GetComponent<Image>();
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            }
                
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.keyCode == KeyCode.Escape)
        {
            SceneManager.LoadScene("TitleScreen");
        }
        else if (e.isKey && ready)
        {
            if (counter < textFlow.Length)
            {
                StartCoroutine(FadeIn(counter));
                counter++;
                ready = false;
            }
            else
            {
                if (GameControl.PlayerData.firstRun)
                {
                    SceneManager.LoadScene("Tutorial");
                }
                else
                {
                    SceneManager.LoadScene("TitleScreen");
                }
            }
        }
    }

    IEnumerator FadeIn(int index)
    {
        for (int i = 0; i <= 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            if (textFlow[index].GetComponent<TMP_Text>() != null)
                textFlow[index].GetComponent<TMP_Text>().alpha = i * 0.01f;
            else
            {
                Image image = textFlow[index].GetComponent<Image>();
                image.color = new Color(image.color.r, image.color.g, image.color.b, i * 0.01f);
            }
        }
        ready = true;
        yield return null;
    }


}
