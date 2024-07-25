using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        GameControl.PlayerData.crosshairActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;

        GetComponent<RectTransform>().position = Input.mousePosition;

        if (GameControl.PlayerData.gamePaused)
            GameControl.PlayerData.crosshairActive = false;

        if (GameControl.PlayerData.crosshairActive)
        {
            GetComponent<Image>().enabled = false;
        }
        else
            GetComponent<Image>().enabled = true;
    }
}
