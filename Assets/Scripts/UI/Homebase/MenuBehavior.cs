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

    //og transform
    [SerializeField] Transform originalPlacement;
    [SerializeField] Transform finalPlacement;
    Vector3 startingPos;
    Quaternion startingRot;
    Vector3 startingScale;

    //selected transform
    [SerializeField] float sizeDiff = 2.25f;
    [SerializeField] float horizontalPlacement = 1 / 3f;

    [SerializeField] GameObject report;

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
        if (GameControl.PlayerData.menusReady && !GameControl.PlayerData.menuActive)
        {
            GetComponent<SizeLerp>().enabled = false;
            sizeLerp = StartCoroutine(HoverEnter());
            menuPopup.text = menuTitle;
        }
    }

    private void OnMouseExit()
    {
        if (GameControl.PlayerData.menusReady && !GameControl.PlayerData.menuActive)
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
        if (GameControl.PlayerData.menusReady && !GameControl.PlayerData.menuActive)
        {
            chosen = true;
            GameControl.PlayerData.menuActive = true;
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
        //report = GameObject.FindWithTag("report");
        report.GetComponent<ReportBehavior>().PutDown();

        //push to the front
        GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        GetComponent<SpriteRenderer>().sortingOrder = 10;
        if (GetComponentInChildren<Canvas>())
        {
            GetComponentInChildren<Canvas>().sortingLayerName = "Foreground";
            GetComponentInChildren<Canvas>().sortingOrder = 11;
        }
            

        //anim handler - establish all the starting and ending vars
        startingPos = transform.position;
        Vector3 growPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * horizontalPlacement, Screen.height / 2f, 0f));
        growPos = new Vector3(growPos.x, growPos.y, 0);

        startingRot = transform.localRotation;
        Quaternion endingRot = Quaternion.identity;

        startingScale = transform.localScale;
        Vector3 endingSize = startingScale * sizeDiff;

        float time = 0f;
        while (time < 1f)
        {
            //lerp position, rotation, and size at once
            transform.position = Vector3.Lerp(startingPos, growPos, time);
            transform.localRotation = Quaternion.Lerp(startingRot, endingRot, time);
            transform.localScale = Vector3.Lerp(startingScale, endingSize, time);

            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
        //TODO - save this so that it can go back to that after accessing another menu
        finalPlacement = transform;

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

    public void Nevermind()
    {
        StartCoroutine(MenuShrink());
    }    

    IEnumerator MenuShrink()
    {

        GetComponent<SpriteRenderer>().sortingLayerName = "Midground";
        GetComponent<SpriteRenderer>().sortingOrder = 0;
        if (GetComponentInChildren<Canvas>())
        {
            GetComponentInChildren<Canvas>().sortingLayerName = "Midground";
            GetComponentInChildren<Canvas>().sortingOrder = 2;
        }

        report.SetActive(true);

        if (keys)
        {
            menuPopup.text = "";
            locHandler.SetActive(false);
        }

        Vector3 finalPos = transform.position;
        Quaternion finalRot = transform.rotation;
        Vector3 finalSize = transform.localScale;

        float time = 0f;
        while (time < 1f)
        {
            //lerp position, rotation, and size at once
            transform.position = Vector3.Lerp(finalPos, startingPos, time);
            transform.localRotation = Quaternion.Lerp(finalRot, startingRot, time);
            transform.localScale = Vector3.Lerp(finalSize, startingSize, time);

            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }

        GameControl.PlayerData.menuActive = false;
    }
}
