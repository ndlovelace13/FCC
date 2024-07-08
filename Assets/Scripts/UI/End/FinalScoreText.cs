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
        //HighScore();
        SetResults();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetResults()
    {
        text = "Final Score: " + GameControl.PlayerData.score;
        if (highScoreSet)
            text += " - NEW BEST!";
        text += "\nFinal Time: " + String.Format("{0:00}:{1:00}", GameControl.PlayerData.min, GameControl.PlayerData.sec);
        if (highTimeSet)
            text += " - NEW BEST!";
        finalScore.text = text;
    }

    /*private void HighScore()
    {
        //checking if most recent score is better than the previous high score;
        int currentHighScore = GameControl.SaveData.highScore;
        if (currentHighScore < GameControl.PlayerData.score)
        {
            GameControl.SaveData.highScore = GameControl.PlayerData.score;
            highScoreSet = true;
        }
        int currentBestMin = GameControl.SaveData.highMin;
        int currentBestSec = GameControl.SaveData.highSec;
        if ((currentBestSec < GameControl.PlayerData.sec && currentBestMin <= GameControl.PlayerData.min) || currentBestMin < GameControl.PlayerData.min)
        {
            GameControl.SaveData.highMin = GameControl.PlayerData.min;
            GameControl.SaveData.highSec = GameControl.PlayerData.sec;
            highTimeSet = true;
        }
    }*/
}
