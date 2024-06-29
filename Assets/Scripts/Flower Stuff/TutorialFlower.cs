using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialFlower : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer headSprite = GetComponentInChildren<FlowerStats>().gameObject.GetComponent<SpriteRenderer>();
        headSprite.enabled = false;
        Animator stemAnim = gameObject.GetComponentsInChildren<Animator>().Last();
        stemAnim.Play("BasicGrow");
        StartCoroutine(HeadActivate(stemAnim, headSprite));
    }

    IEnumerator HeadActivate(Animator stemAnim, SpriteRenderer head)
    {
        while (stemAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return new WaitForEndOfFrame();
        }
        head.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
