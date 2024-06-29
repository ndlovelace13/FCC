using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatBehavior : MonoBehaviour
{
    [SerializeField] Sprite healthy;
    [SerializeField] Sprite injured;
    [SerializeField] Vector3 startPositionOffset;
    Quaternion startRotationOffset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        if (transform.parent.localScale.x < 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y);
        else
            transform.localScale = new Vector3(0.5f, 0.5f);
        transform.localPosition = startPositionOffset;
        if (startRotationOffset == null)
            startRotationOffset = transform.localRotation;
        transform.localRotation = startRotationOffset;
        GetComponent<SpriteRenderer>().sprite = healthy;
        //HatFall();
    }

    public void HatFall()
    {
        GetComponent<SpriteRenderer>().sprite = injured;
        transform.parent = null;
        StartCoroutine(FallLerp());
    }

    IEnumerator FallLerp()
    {
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + new Vector3(1f, 0f);
        Quaternion startRotation = Quaternion.identity;
        Quaternion endRotation = Quaternion.Euler(0, 0, -90);
        float time = 0f;
        float initY = (-2 * (Mathf.Pow(time - 0.1f, 2f) + 0.5f));
        while (time < 1f)
        {
            float yPos = (-2*(Mathf.Pow(time - 0.1f, 2f) + 0.5f)) - initY;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            transform.position = Vector3.Lerp(startPos, endPos, time) + new Vector3(0, yPos);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(Despawn());
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
}
