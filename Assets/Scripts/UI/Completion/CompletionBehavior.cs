using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompletionBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Return();
        }
    }

    public void Return()
    {
        GameControl.SaveHandler.SaveGame();
        SceneManager.LoadScene("Homebase");
    }
}
