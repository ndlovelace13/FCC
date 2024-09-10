using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class RepellentBehavior : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    bool clicked = false;
    Vector2 startMousePos;
    Vector2 currentMousePos;
    [SerializeField] RectTransform rect;

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
                currentMousePos = Input.mousePosition;
                rect.position = currentMousePos;
                //TODO implement shake checker and counter
            }
            else
                yield break;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Output to console the GameObject's name and the following message
        Debug.Log("Cursor Entering " + name + " GameObject");
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (!clicked)
        {
            startMousePos = pointerEventData.position;
            clicked = true;
            StartCoroutine(ShakeChecker());
        }
        
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        clicked = false;
    }

    //TODO - lerp up and down on repellent mode
}
