using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TutorialBehavior : MonoBehaviour
{
    bool canTransition;
    // Start is called before the first frame update
    void Start()
    {
        canTransition = false;
        StartCoroutine(TransitionEnabler());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        if (canTransition)
        {
            Event e = Event.current;
            if (e.keyCode == KeyCode.Escape)
            {
                SceneManager.LoadScene("TitleScreen");
            }
            else if (e.isKey)
            {
                if (PlayerPrefs.GetInt("firstRun") == 0)
                {
                    PlayerPrefs.SetInt("firstRun", 1);
                    SceneManager.LoadScene("Gameplay");
                }
                else
                {
                    SceneManager.LoadScene("TitleScreen");
                }
            }
        }
    }

    IEnumerator TransitionEnabler()
    {
        yield return new WaitForSeconds(1);
        canTransition = true;
    }
}
