using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResignHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameControl.PlayerData.gamePaused = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }
    }

    public void Resign()
    {
        //take the code from the player end script
        GameControl.PlayerData.gamePaused = false;
        //Call the GameOver method from the player obj
        GameObject.FindWithTag("Player").GetComponentInChildren<PlayerMovement>().GameOver();
        //then kill
        Destroy(gameObject);
        /*GameControl.PlayerData.gameOver = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene("EndScreen");*/
    }

    public void Cancel()
    {
        GameControl.PlayerData.gamePaused = false;
        Time.timeScale = 1f;
        Destroy(gameObject);
    }
}
