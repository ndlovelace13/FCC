using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        GameControl.SaveHandler.SaveGame();
        SceneManager.LoadScene("TitleScreen");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Cancel()
    {
        Time.timeScale = 1f;
        GameControl.PlayerData.quitCooldown = true;
        Destroy(gameObject);
    }
}
