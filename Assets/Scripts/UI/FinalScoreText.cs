using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalScoreText : MonoBehaviour
{
    public TMP_Text finalScore;
    private string text;
    private bool highScoreSet = false;
    private bool highTimeSet = false;
    // Start is called before the first frame update
    void Start()
    {
        HighScore();
        SetResults();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetResults()
    {
        text = "Final Score: " + PlayerPrefs.GetInt("totalScore");
        if (highScoreSet)
            text += " - NEW BEST!";
        text += "\nFinal Time: " + String.Format("{0:00}:{1:00}", PlayerPrefs.GetInt("min"), PlayerPrefs.GetInt("sec"));
        if (highTimeSet)
            text += " - NEW BEST!";
        finalScore.text = text;
    }

    private void HighScore()
    {
        //checking if most recent score is better than the previous high score;
        int currentHighScore = PlayerPrefs.GetInt("highScore");
        if (currentHighScore < PlayerPrefs.GetInt("totalScore"))
        {
            PlayerPrefs.SetInt("highScore", PlayerPrefs.GetInt("totalScore"));
            highScoreSet = true;
        }
        int currentBestMin = PlayerPrefs.GetInt("highMin");
        int currentBestSec = PlayerPrefs.GetInt("highSec");
        if ((currentBestSec <= PlayerPrefs.GetInt("sec") && currentBestMin <= PlayerPrefs.GetInt("min")) || currentBestMin < PlayerPrefs.GetInt("min"))
        {
            PlayerPrefs.SetInt("highMin", PlayerPrefs.GetInt("min"));
            PlayerPrefs.SetInt("highSec", PlayerPrefs.GetInt("sec"));
            highTimeSet = true;
        }
    }
}
