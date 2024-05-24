using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeLerp : MonoBehaviour
{
    [SerializeField] float sizeModifier;
    [SerializeField] float loopTime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SizeLerping());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SizeLerping()
    {
        while (true)
        {
            float time = 0;
            Vector2 originalScale = transform.localScale;
            Vector2 lerpTarget = transform.localScale * sizeModifier;
            bool halfwaySwap = false;

            while (time < loopTime)
            {
                if (time > loopTime / 2 && !halfwaySwap)
                {
                    Debug.Log("goofy activate");
                    originalScale = lerpTarget;
                    lerpTarget = originalScale / sizeModifier;
                    halfwaySwap = true;
                }
                transform.localScale = Vector2.Lerp(originalScale, lerpTarget, time / loopTime);
                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}
