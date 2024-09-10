using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeLerp : MonoBehaviour
{
    Quaternion startingRot;
    Quaternion targetRot;
    public bool looping = false;
    [SerializeField] float lerpAngle;
    [SerializeField] float lerpTime;
    // Start is called before the first frame update
    void Start()
    {
        if (looping)
        {
            Execute(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Execute(bool loop)
    {
        looping = loop;
        startingRot = transform.rotation;
        StartCoroutine(ShakeLerping());
    }

    IEnumerator ShakeLerping()
    {
        do
        {
            float currentTime = 0;
            targetRot = Quaternion.Euler(startingRot.x, startingRot.y, startingRot.z + lerpAngle);

            while (currentTime < lerpTime)
            {
                transform.rotation = Quaternion.Lerp(startingRot, targetRot, currentTime / lerpTime);
                currentTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            transform.rotation = targetRot;
            currentTime = 0;
            while (currentTime < lerpTime)
            {
                transform.rotation = Quaternion.Lerp(targetRot, startingRot, currentTime / lerpTime);
                currentTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.rotation = startingRot;

            lerpAngle *= -1f;

        } while (looping);
    }

    private void OnEnable()
    {
        if (looping)
            Execute(true);
    }
}
