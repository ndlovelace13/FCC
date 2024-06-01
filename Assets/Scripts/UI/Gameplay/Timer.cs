using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    // Start is called before the first frame update
    int min;
    int sec;

    //speed stuff
    [SerializeField] int speedInterval = 15;

    float maxSpeed;
    float minSpeed;

    public TMP_Text timerText;
    void Start()
    {
        GameControl.PlayerData.firstRun = false;
        GameControl.PlayerData.discoveryDisplay = true;
        maxSpeed = GameControl.PlayerData.maxSpeed;
        minSpeed = GameControl.PlayerData.minSpeed;
        GameControl.PlayerData.ResetRun();
        StartCoroutine(SpeedUp());
    }

    // Update is called once per frame
    void Update()
    {
        sec = (int) Time.timeSinceLevelLoad % 60;
        min = (int) Time.timeSinceLevelLoad / 60;

        timerText.text = String.Format("{0:00}:{1:00}", min, sec);

        GameControl.PlayerData.min = min;
        GameControl.PlayerData.sec = sec;
    }

    IEnumerator SpeedUp()
    {
        while (true)
        {
            yield return new WaitForSeconds(speedInterval);
            if (GameControl.PlayerData.playerSpeed + 1 > maxSpeed)
            {
                maxSpeed += GameControl.PlayerData.maxInterval;
                GameControl.PlayerData.currentMax = maxSpeed;
                minSpeed += GameControl.PlayerData.minInterval;
                GameControl.PlayerData.currentMin = minSpeed;
            }
            else
            {
                Debug.Log("maxSpeed reached");
                GameControl.PlayerData.currentHealth += GameControl.PlayerData.healthInterval;
            }
            
            //PlayerPrefs.SetFloat("maxSpeed", PlayerPrefs.GetFloat("maxSpeed") + maxInterval);
            //PlayerPrefs.SetFloat("minSpeed", PlayerPrefs.GetFloat("minSpeed") + minInterval);
        }
    }
}
