using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlPrompts : MonoBehaviour
{
    [SerializeField] TMP_Text controlText;
    string promptString;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        promptString = string.Empty;
        if (!GameControl.PlayerData.menuActive)
        {
            //check for previous shift
            if (GameControl.PlayerData.currentReportIndex > 0)
            {
                promptString += "S - Previous Shift Report";
            }
            //check for next shift
            if (GameControl.PlayerData.currentReportIndex < GameControl.SaveData.shiftReports.Count - 1)
            {
                if (string.IsNullOrEmpty(promptString))
                {
                    promptString += "W - Next Shift Report";
                }
                else
                    promptString += " | W - Next Shift Report";
            }
            //check for quit availability
            if (GameControl.PlayerData.menusReady)
            {
                if (string.IsNullOrEmpty(promptString))
                {
                    promptString += "Esc - Rest Up";
                }
                else
                    promptString += " | Esc - Rest Up";
            }
        }
        controlText.text = promptString;
    }
}
