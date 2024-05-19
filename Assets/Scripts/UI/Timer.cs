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
    [SerializeField] float maxInterval = 0.5f;
    [SerializeField] float minInterval = 0.25f;

    public TMP_Text timerText;
    void Start()
    {
        PlayerPrefs.SetFloat("maxSpeed", 2f);
        PlayerPrefs.SetFloat("minSpeed", 1f);
        StartCoroutine(SpeedUp());
    }

    // Update is called once per frame
    void Update()
    {
        sec = (int) Time.timeSinceLevelLoad % 60;
        min = (int) Time.timeSinceLevelLoad / 60;

        timerText.text = String.Format("{0:00}:{1:00}", min, sec);

        PlayerPrefs.SetInt("min", min);
        PlayerPrefs.SetInt("sec", sec);
    }

    IEnumerator SpeedUp()
    {
        while (true)
        {
            yield return new WaitForSeconds(speedInterval);
            PlayerPrefs.SetFloat("maxSpeed", PlayerPrefs.GetFloat("maxSpeed") + maxInterval);
            PlayerPrefs.SetFloat("minSpeed", PlayerPrefs.GetFloat("minSpeed") + minInterval);
        }
    }
}
