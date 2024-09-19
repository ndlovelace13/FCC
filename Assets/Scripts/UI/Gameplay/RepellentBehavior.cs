using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class RepellentBehavior : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    bool clicked = false;
    Vector2 startMousePos;
    Vector2 currentMousePos;
    Vector2 prevMousePos;
    [SerializeField] RectTransform rect;
    Vector3 startingPos;
    float maxDist = 200f;
    bool firstAppearance = true;
    float shakeDist = 0f;
    float requiredShake = 3000f;

    //projectile stats
    public float range = 5f;
    public float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (firstAppearance)
        {
            startingPos = rect.position;
            firstAppearance = false;
        }
        else
            rect.position = startingPos;
        clicked = false;
        shakeDist = 0f;
        StartCoroutine(ShakeChecker());
    }

    IEnumerator ShakeChecker()
    {
        while (true)
        {
            if (clicked)
            {
                currentMousePos = Input.mousePosition;
                
                Vector3 mouseDiff = prevMousePos - currentMousePos;
                Vector3 ogMouseDiff = startMousePos - currentMousePos;
                Vector3 ogDist = startingPos - rect.position - mouseDiff;
                if (ogMouseDiff.sqrMagnitude < Mathf.Pow(maxDist, 2))
                {
                    rect.position -= mouseDiff;
                    shakeDist += mouseDiff.magnitude;
                }
                if (shakeDist > requiredShake)
                {
                    StartCoroutine(RepellentActivate());
                    break;
                }
            }
            else
                yield break;
            yield return new WaitForEndOfFrame();
            prevMousePos = currentMousePos;
        }
        yield return null;
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
            prevMousePos = startMousePos;
            clicked = true;
            StartCoroutine(ShakeChecker());
        }
        
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        clicked = false;
        shakeDist = 0f;
    }

    IEnumerator RepellentActivate()
    {
        //kill the lerping effects
        GetComponent<SizeLerp>().enabled = false;
        GetComponent<ShakeLerp>().enabled = false;

        //decrement repellent var
        GameControl.PlayerData.remainingRepellent--;

        //Get the docket location for lerping
        GameObject player = GameObject.FindWithTag("Player");
        Transform[] children = player.GetComponentsInChildren<Transform>();
        Transform docket = children.Where(child => child.tag == "docket").ToArray()[0];

        Vector3 finalPos = Camera.main.WorldToScreenPoint(docket.position);
        Vector3 currentPos = rect.position;

        //Shrink, spin, and lerp to the player
        float currentTime = 0f;
        while (currentTime < 1f)
        {
            rect.position = Vector3.Lerp(currentPos, finalPos, currentTime);
            rect.rotation = Quaternion.Euler(0, 0, rect.rotation.eulerAngles.z + 360f * currentTime);
            rect.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.2f, currentTime);
            currentTime += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }

        rect.position = finalPos;
        //player recieve anim plays here
        yield return new WaitForSeconds(0.5f);

        //call the repellent active script here
        player.GetComponent<CrownThrowing>().RepellentActivate();

        //deactivate
        gameObject.SetActive(false);
        yield return null;
    }

    //TODO - lerp up and down on repellent mode
}
