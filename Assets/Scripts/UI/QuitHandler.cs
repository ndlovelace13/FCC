using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Cancel()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
    }
}
