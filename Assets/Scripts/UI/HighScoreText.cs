using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class HighScoreText : MonoBehaviour
{
    TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Best Score: " + PlayerPrefs.GetInt("highScore") + "\n" + "Best Time: " + String.Format("{0:00}:{1:00}", PlayerPrefs.GetInt("highMin"), PlayerPrefs.GetInt("highSec"));
    }
}
