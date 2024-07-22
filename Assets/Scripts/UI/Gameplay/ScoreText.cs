using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    public TMP_Text scoreText;
    // Start is called before the first frame update
    void Start()
    {
        //PlayerPrefs.SetInt("totalScore", 0);
        //scoreText = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Earnings: " + string.Format("{0:C}", GameControl.PlayerData.score / 100f) + "\nEssence: " + GameControl.SaveData.essenceCount;
    }
}
