using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ControlsBehavior : MonoBehaviour
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
                Return();
            }
            /*else if (e.isKey)
            {
                if (GameControl.PlayerData.firstRun)
                {
                    SceneManager.LoadScene("Gameplay");
                }
                else
                {
                    SceneManager.LoadScene("TitleScreen");
                }
            }*/
        }
    }

    IEnumerator TransitionEnabler()
    {
        yield return new WaitForSeconds(1);
        canTransition = true;
    }

    public void Return()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
