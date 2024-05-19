using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
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
            Application.Quit();
        }
        else if (e.keyCode == KeyCode.R)
        {
            SceneManager.LoadScene("Gameplay");
        }
        else if (e.keyCode == KeyCode.M)
        {
            SceneManager.LoadScene("TitleScreen");
        }
    }

    public void LinkToForm()
    {
        Application.OpenURL("https://forms.gle/LkHsuFDq4exfR15r6");
    }

}
