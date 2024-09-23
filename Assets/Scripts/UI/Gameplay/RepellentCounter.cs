using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RepellentCounter : MonoBehaviour
{
    TMP_Text repellentCount;
    // Start is called before the first frame update
    void Start()
    {
        repellentCount = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        repellentCount.text = "";
        for (int i = 0; i < GameControl.PlayerData.remainingRepellent; i++)
        {
            repellentCount.text += "<sprite=0>";
        }
        
    }
}
