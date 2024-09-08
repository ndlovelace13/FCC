using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RepellentBehavior : MonoBehaviour
{
    bool clicked = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        clicked = false;
        StartCoroutine(ShakeChecker());
    }

    IEnumerator ShakeChecker()
    {
        while (true)
        {
            if (clicked)
            {
                GameControl.PlayerData.remainingRepellent--;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
        gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        clicked = true;
    }

    private void OnMouseEnter()
    {
        Debug.Log("Mouse Detected");
        GameControl.PlayerData.crosshairActive = false;
    }

    private void OnMouseExit()
    {
        GameControl.PlayerData.crosshairActive = true;
    }

    //TODO - lerp up and down on repellent mode
}
