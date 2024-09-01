using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CoinLerp(Vector3 startPos, Vector3 endPos)
    {
        StartCoroutine(ActualCoinLerp(startPos, endPos));
    }

    IEnumerator ActualCoinLerp(Vector2 startPos, Vector2 endPos)
    {
        float currentTime = 0f;
        //assign a random time for landing
        float targetTime = Random.Range(0.5f, 0.8f);


        //randomly assign a landing position
        float targetMag = Random.Range(1f, 2f);
        Vector2 targetAngle = new Vector2(Mathf.Cos(Random.Range(0, 2 * Mathf.PI)), Mathf.Sin(Random.Range(0, 2 * Mathf.PI)));
        float targetX = endPos.x + Random.Range(0.25f, 2f) * (Random.Range(0, 2) * 2 - 1);
        float targetY = endPos.y + Random.Range(0f, 2f) * (Random.Range(0, 2) * 2 - 1);
        float targetHeight = Random.Range(1f, 3f);

        Vector2 targetPos = endPos + targetAngle * targetMag;

        //randomly assign a rotation speed and starting rotation
        transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
        float rotSpeed = Random.Range(0.1f, 3f) * (Random.Range(0, 2) * 2 - 1);


        while (currentTime < targetTime)
        {
            transform.position = Vector2.Lerp(startPos, targetPos, currentTime / targetTime);
            transform.localRotation = Quaternion.Euler(0, 0, transform.localRotation.eulerAngles.z + (360f * Time.deltaTime * rotSpeed));
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
