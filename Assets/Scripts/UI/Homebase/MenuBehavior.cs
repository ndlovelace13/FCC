using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class MenuBehavior : MonoBehaviour
{
    [SerializeField] string menuName;
    [SerializeField] string menuTitle;
    [SerializeField] TMP_Text menuPopup;
    Vector3 startingSize;
    Vector3 finalSize;
    Coroutine sizeLerp;
    bool chosen = false;

    //car key stuff
    public bool keys = false;
    [SerializeField] GameObject locHandler;
    // Start is called before the first frame update
    void Start()
    {
        startingSize = transform.localScale;
        finalSize = startingSize * 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        //Grow on hover, maybe add outline
        if (GameControl.PlayerData.menusReady && !chosen)
        {
            sizeLerp = StartCoroutine(HoverEnter());
            menuPopup.text = menuTitle;
        }
    }

    private void OnMouseExit()
    {
        if (GameControl.PlayerData.menusReady && !chosen)
        {
            if (sizeLerp != null)
                StopCoroutine(sizeLerp);
            StartCoroutine(HoverExit());
            menuPopup.text = "";
        }
    }

    IEnumerator HoverEnter()
    {
        float time = 0f;
        GetComponent<SpriteRenderer>().sortingOrder = 1;
        Vector3 init = transform.localScale;
        while (time < 0.5f)
        {
            transform.localScale = Vector3.Lerp(init, finalSize, time / 0.5f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator HoverExit()
    {
        float time = 0f;
        Vector3 init = transform.localScale;
        while (time < 0.5f)
        {
            transform.localScale = Vector3.Lerp(init, startingSize, time / .5f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        GetComponent<SpriteRenderer>().sortingOrder = 0;
    }

    private void OnMouseDown()
    {
        if (GameControl.PlayerData.menusReady)
        {
            chosen = true;
            //play anim and open to the size of the screen
            StopAllCoroutines();
            StartCoroutine(MenuGrow());
            /*if (GameControl.SaveData.firstRun)
                menuName = "Tutorial";
            SceneManager.LoadScene(menuName);*/
        }
    }

    IEnumerator MenuGrow()
    {
        //lower the report
        GameObject.FindWithTag("report").GetComponent<ReportBehavior>().PutDown();

        //push to the front
        GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        GetComponent<SpriteRenderer>().sortingOrder = 10;

        //anim handler - establish all the starting and ending vars
        Vector3 startingPos = transform.position;
        Vector3 growPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 3f, Screen.height / 2f, 0f));
        growPos = new Vector3(growPos.x, growPos.y, 0);

        Quaternion startingRot = transform.localRotation;
        Quaternion endingRot = Quaternion.identity;

        Vector3 startingSize = transform.localScale;
        Vector3 endingSize = startingSize * 2.25f;

        float time = 0f;
        while (time < 1f)
        {
            //lerp position, rotation, and size at once
            transform.position = Vector3.Lerp(startingPos, growPos, time);
            transform.localRotation = Quaternion.Lerp(startingRot, endingRot, time);
            transform.localScale = Vector3.Lerp(startingSize, endingSize, time);

            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }

        //store the value of the current menu so you can play a closing anim whne navigating back
        //except for the car keys - pop up with a separate menu for these to decide where to go
        if (keys)
        {
            menuPopup.text = "Where To?";
            locHandler.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene(menuName);
        }
    }
}
