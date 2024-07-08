using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenTransition : MonoBehaviour
{
    [SerializeField] GameObject loadButton;
    bool canTransition;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        //set playerprefs here for use throughout the game - lmao how far we've come
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
        loadButton.SetActive(GameControl.SaveHandler.FileFind());
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
        yield return new WaitForSeconds(0.2f);
        canTransition = true;
    }

    public void NewPlayTransition()
    {
        if (canTransition)
        {
            GameControl.PlayerData.NewGame();
            /*if (!GameControl.PlayerData.descriptionVisited && GameControl.SaveData.firstRun)
            {
                SceneManager.LoadScene("Description");
            }
            else if (GameControl.SaveData.firstRun)
                SceneManager.LoadScene("Tutorial");
            else
                SceneManager.LoadScene("Gameplay");*/
            SceneManager.LoadScene("Homebase");
        }
    }

    public void LoadPlayTransition()
    {
        if (canTransition)
        {
            GameControl.PlayerData.LoadGame();
            /*if (!GameControl.PlayerData.descriptionVisited && GameControl.SaveData.firstRun)
            {
                SceneManager.LoadScene("Description");
            }
            else if (GameControl.SaveData.firstRun)
                SceneManager.LoadScene("Tutorial");*/
            SceneManager.LoadScene("Homebase");
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
