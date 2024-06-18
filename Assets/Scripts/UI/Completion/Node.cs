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
        GetComponent<SpriteRenderer>().enabled = isVisible;
    }
    public void NodeAssignment(Crown newCrown)
    {
        crown = newCrown;
    }
    
    public void InitialNodePlace(Vector3 initPos)
    {
        //Debug.Log("Initial Node Lerp");
        transform.localPosition = initPos;
        basePos = initPos;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (crown.IsDiscovered())
            spriteRenderer.color = Color.green;
        else if (crown.Discoverable())
            spriteRenderer.color = Color.yellow;
        if (crown.Status())
        {
            GetComponent<SizeLerp>().Execute();
        }
        firstTime = false;
    }

    public void newLocationLerp(Vector3 newLocation)
    {
        if (newLocation == basePos)
        {
            StartCoroutine(Reset());
        }
        else
        {
            StartCoroutine(NewLocation(newLocation));
        }
    }

    IEnumerator Reset()
    {
        float time = 0f;
        Vector3 init = transform.localPosition;
        while (time < 0.5f)
        {
            transform.localPosition = Vector3.Lerp(init, basePos, time / 0.5f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator NewLocation(Vector3 newLocation)
    {
        float time = 0f;
        while (time < 0.5f)
        {
            transform.localPosition = Vector3.Lerp(basePos, newLocation, time / 0.5f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnMouseEnter()
    {
        Debug.Log(gameObject.name);
        sizeLerp = StartCoroutine(HoverEnter());
    }
    private void OnMouseExit()
    {
        StopCoroutine(sizeLerp);
        StartCoroutine(HoverExit());
    }

    IEnumerator HoverEnter()
    {
        float time = 0f;
        while (time < 0.25f)
        {
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 2.5f, time / 0.25f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
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
