using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeLerp : MonoBehaviour
{
    [SerializeField] float sizeModifier;
    [SerializeField] float loopTime;
    [SerializeField] bool looping = false;
    bool lerping = false;
    // Start is called before the first frame update
    void Start()
    {
        if (looping)
        {
            Execute();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Execute()
    {
        //need a better solution for satisfying feedback without infinite growth
        if (!lerping)
            StartCoroutine(SizeLerping());
    }

    IEnumerator SizeLerping()
    {
        lerping = true;
        do
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
        } while (looping);
        lerping = false;
    }
}
