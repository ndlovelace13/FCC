using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PageTurnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Sprite directionSprite;
    CursorBehavior cursor;
    // Start is called before the first frame update
    void Start()
    {
        cursor = GameObject.FindWithTag("cursor").GetComponent<CursorBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData data)
    {
        cursor.ApplySprite(directionSprite);
    }

    public void OnPointerExit(PointerEventData data)
    {
        cursor.ResetSprite();
    }

    public void OnDisable()
    {
        cursor.ResetSprite();
    }
}
