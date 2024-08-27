using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingStem : MonoBehaviour
{
    FlowerBehavior connectedFlower;
    Transform flowerPos;
    Vector3 slotPos;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetFlower(FlowerBehavior newFlower)
    {
        connectedFlower = newFlower;
        flowerPos = connectedFlower.transform;
        slotPos = connectedFlower.finalDocketPos;
        GetComponent<SpriteRenderer>().enabled = true;
        StartCoroutine(StemStretch());
    }

    IEnumerator StemStretch()
    {
        while (connectedFlower != null && connectedFlower.placed == false)
        {
            Vector2 length = flowerPos.localPosition - slotPos;
            float angleRadians = Mathf.Atan2(length.y, -length.x);
            if (angleRadians < 0)
                angleRadians += 2 * Mathf.PI;
            Vector2 direction = new Vector2(Mathf.Sin(angleRadians), Mathf.Cos(angleRadians));
            transform.localPosition = Vector3.Lerp(flowerPos.localPosition, slotPos, 0.5f);
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
            transform.localScale = new Vector2(length.magnitude / 6f, transform.localScale.y);

            yield return new WaitForEndOfFrame();
        }
        connectedFlower = null;
        transform.SetParent(null);
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
