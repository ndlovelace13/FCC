using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class FlowerBehavior : MonoBehaviour
{
    //public FlowerStats stats;
    public string type;
    public int position = 0;
    public bool picked = true;
    public bool growing = false;
    public int tier = 0;

    //shit for construction
    public Vector3 finalDocketPos;
    public Vector3 randomCraftPos;
    public bool draggable = false;
    public bool placed = false;

    // Start is called before the first frame update
    void Start()
    {
        if (GameControl.PlayerData.tutorialActive && transform.parent.tag != "finalCrown")
            picked = false;
    }

    // Update is called once per frame
    void Update()
    {
        //stats = GameControl.PlayerData.flowerStatsDict[type];
        
        if (!picked)
        {
            Animator animator = transform.parent.GetChild(0).GetComponentInChildren<Animator>();
            animator.SetInteger("rarity", GetComponent<FlowerStats>().rarity);
        }

        if (GameControl.PlayerData.gameOver)
        {
            draggable = false;
        }

    }

    private void OnEnable()
    {
        picked = true;
    }

    private void OnDisable()
    {
        picked = true;
        placed = false;
    }

    //executes only when draggable is true
    void OnMouseDrag()
    {
        if (draggable)
        {
            Debug.Log(name + " being dragged " + transform.position);
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(currentPos.x, currentPos.y, 0);
        }
    }

    void OnMouseOver()
    {
        if (draggable)
        {
            Debug.Log(name + " is being hovered over");
        }
    }

    private void OnMouseUp()
    {
        if (draggable)
        {
            //check whether the release is within a unit of the finalpos, if so, then snap to
            float currentDist = Vector3.Distance(transform.localPosition, finalDocketPos);
            if (currentDist < 1)
            {
                draggable = false;
                transform.localPosition = finalDocketPos;
                placed = true;
            }
            //otherwise, reset
            else
            {
                StartCoroutine(ResetLerp());
            }
        }
    }

    IEnumerator ResetLerp()
    {
        float currentTime = 0f;
        Vector3 currentPos = transform.localPosition;
        while (currentTime < 0.2f)
        {
            transform.localPosition = Vector3.Lerp(currentPos, randomCraftPos, currentTime / 0.2f);
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }
    }
}
