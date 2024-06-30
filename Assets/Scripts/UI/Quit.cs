using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour
{
    [SerializeField] GameObject quitPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            QuitPopup();
        }
    }

    public void QuitPopup()
    {
        if (Time.timeScale != 0)
        {
            Time.timeScale = 0;
            Instantiate(quitPrefab);
        }
    }
}
