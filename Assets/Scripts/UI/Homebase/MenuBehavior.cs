using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour
{
    [SerializeField] string menuName;
    Vector3 startingSize;
    Coroutine sizeLerp;
    // Start is called before the first frame update
    void Start()
    {
        startingSize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        //Grow on hover, maybe add outline
        sizeLerp = StartCoroutine(HoverEnter());
    }

    private void OnMouseExit()
    {
        if (sizeLerp != null)
            StopCoroutine(sizeLerp);
        StartCoroutine(HoverExit());
    }

    IEnumerator HoverEnter()
    {
        float time = 0f;
        GetComponent<SpriteRenderer>().sortingOrder = 1;
        while (time < 0.5f)
        {
            transform.localScale = Vector3.Lerp(startingSize, startingSize * 1.5f, time / 0.5f);
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
        //play anim and open to the size of the screen
        MenuGrow();
        if (GameControl.SaveData.firstRun)
            menuName = "Tutorial";
        SceneManager.LoadScene(menuName);
    }

    private void MenuGrow()
    {
        //anim handler
        //lower the report
        //store the value of the current menu so you can play a closing anim whne navigating back
    }
}
