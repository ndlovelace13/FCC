using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenTransition : MonoBehaviour
{
    bool canTransition;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        //set playerprefs here for use throughout the game
        if (PlayerPrefs.GetInt("firstRun") == 0)
        {
            PlayerPrefs.SetFloat("balance", 0);
            PlayerPrefs.SetFloat("uncommon", 0.2f);
            PlayerPrefs.SetInt("highScore", 0);
            PlayerPrefs.SetInt("highMin", 0);
            PlayerPrefs.SetInt("highSec", 0);
        }
        
        canTransition = false;
        StartCoroutine(TransitionEnabler());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        /*if (canTransition)
        {
            Event e = Event.current;
            if (e.isKey && e.keyCode != KeyCode.Escape)
            {
                if (PlayerPrefs.GetInt("firstRun") == 0)
                {
                    SceneManager.LoadScene("Description");
                }
                else
                    SceneManager.LoadScene("Gameplay");
            }
        }*/
    }

    IEnumerator TransitionEnabler()
    {
        yield return new WaitForSeconds(1);
        canTransition = true;
    }

    public void PlayTransition()
    {
        if (canTransition)
        {
            if (!GameControl.PlayerData.descriptionVisited)
            {
                SceneManager.LoadScene("Description");
            }
            else if (GameControl.PlayerData.firstRun)
                SceneManager.LoadScene("Tutorial");
            else
                SceneManager.LoadScene("Gameplay");
        }
    }
    
    public void DescriptionTransition()
    {
        GameControl.PlayerData.descriptionfromMenu = true;
        SceneManager.LoadScene("Description");
    }

    public void ControlsTransition()
    {
        SceneManager.LoadScene("Controls");
    }

    public void CreditsTransition()
    {
        SceneManager.LoadScene("Credits");
    }

    public void LinkToForm()
    {
        Application.OpenURL("https://forms.gle/LkHsuFDq4exfR15r6");
    }
}
