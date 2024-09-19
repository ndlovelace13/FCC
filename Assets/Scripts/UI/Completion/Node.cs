using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Crown crown;
    SpriteRenderer spriteRenderer;
    public Vector3 basePos;
    public bool firstTime = true;
    Coroutine sizeLerp;
    RectTransform nodeFrame;
    bool isSelected = true;

    Coroutine currentLerp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVisible(bool isVisible)
    {
        isSelected = isVisible;
        //GetComponentInChildren<SpriteRenderer>().enabled = isVisible;
    }
    public void NodeAssignment(Crown newCrown)
    {
        crown = newCrown;
    }
    
    IEnumerator InitialNodePlace()
    {
        spriteRenderer.sortingOrder++;
        if (crown.Status())
        {
            GetComponent<SizeLerp>().Execute(crown.IsDiscovered());
        }
        firstTime = false;
        yield return null;
    }
    IEnumerator ColorSet()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (crown.IsDiscovered())
            spriteRenderer.color = new Color(22 / 255f, 209 / 255f, 33 / 255f);
        else if (crown.Discoverable())
            spriteRenderer.color = Color.yellow;

        //Set the nodeFrame and start the checkingRoutine
        nodeFrame = GameObject.FindWithTag("nodeFrame").GetComponent<RectTransform>();
        StartCoroutine(FrameCheck());
        yield return null;
    }

    IEnumerator FrameCheck()
    {
        while (gameObject.activeSelf)
        {
            //get the current boundaries of the nodeFrame in WorldPosition
            Vector3[] worldCorners = new Vector3[4];
            nodeFrame.GetWorldCorners(worldCorners);
            Vector3 currentPos = transform.position;
            //check if the transform is within the bounds of the frame
            bool visibility = false;
            //x check
            if (currentPos.x >= worldCorners[0].x && currentPos.x <= worldCorners[2].x)
            {
                //y check
                if (currentPos.y >= worldCorners[0].y && currentPos.y <= worldCorners[2].y)
                    visibility = true;
            }
            //update the spriteRenderer accordingly
            if (visibility && isSelected)
                GetComponentInChildren<SpriteRenderer>().enabled = true;
            else
                GetComponentInChildren<SpriteRenderer>().enabled = false;

            yield return new WaitForEndOfFrame();
        }
    }

    public void newLocationLerp(Vector3 newLocation)
    {
        if (currentLerp != null)
            StopCoroutine(currentLerp);

        gameObject.layer = 2;
        if (newLocation == basePos)
        {
            currentLerp = StartCoroutine(Reset());
        }
        else
        {
            currentLerp = StartCoroutine(NewLocation(newLocation));
        }
    }

    IEnumerator Reset()
    {
        if (firstTime)
            StartCoroutine(ColorSet());
        float time = 0f;
        Vector3 init = transform.localPosition;
        while (time < 0.5f)
        {
            transform.localPosition = Vector3.Lerp(init, basePos, time / 0.5f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = basePos;
        if (firstTime)
            StartCoroutine(InitialNodePlace());
        gameObject.layer = 0;
    }

    IEnumerator NewLocation(Vector3 newLocation)
    {
        float time = 0f;
        Vector3 currentPos = transform.localPosition;
        while (time < 0.5f)
        {
            transform.localPosition = Vector3.Lerp(currentPos, newLocation, time / 0.5f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = newLocation;
        gameObject.layer = 0;
    }

    private void OnMouseEnter()
    {
        //Debug.Log(gameObject.name);
        if (spriteRenderer.enabled)
            sizeLerp = StartCoroutine(HoverEnter());
    }
    private void OnMouseExit()
    {
        if (spriteRenderer.enabled)
        {
            GameControl.CrownCompletion.infoPopup.SetActive(false);
            StopCoroutine(sizeLerp);
            StartCoroutine(HoverExit());
        }
    }

    IEnumerator HoverEnter()
    {
        float time = 0f;
        GetComponent<SizeLerp>().enabled = false;
        Vector3 startSize = transform.localScale;
        while (time < 0.25f)
        {
            transform.localScale = Vector3.Lerp(startSize, Vector3.one * 2.5f, time / 0.25f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(InfoDisplay());
    }

    IEnumerator InfoDisplay()
    {
        GameObject info = GameControl.CrownCompletion.infoPopup;
        Debug.Log(transform.position);
        info.GetComponent<InfoPopup>().CrownChosen(transform.position, crown);
        yield return null;
    }

    IEnumerator HoverExit()
    {
        float time = 0f;
        Vector3 init = transform.localScale;
        while (time < 0.25f)
        {
            transform.localScale = Vector3.Lerp(init, Vector3.one, time / 0.25f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
