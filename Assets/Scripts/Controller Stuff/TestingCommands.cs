using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingCommands : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //define all the testing commands
        if (GameControl.PlayerData.testing)
        {
            //add a dollar
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                GameControl.SaveData.balance += 1f;
            }

            //add an essence seed
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                GameControl.SaveData.essenceCount += 1;
            }
        }
    }
}
