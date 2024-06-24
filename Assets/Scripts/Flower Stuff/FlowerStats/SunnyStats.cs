using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SunnyStats : FlowerStats
{
    float firstTierTime = 5f;
    Vector3 secondTierPos = new Vector3(0, 0.25f);
    float secondTierTime = 10f;
    Vector3 thirdTierPos = new Vector3(0, 0.5f);

    List<GameObject> growingFlowers;

    int projBoost = 1;
    int rangeBoost = 2;
    int pointsBoost = 1;
    int damageBoost = 2;
    int projRangeBoost = 2;

    [SerializeField] Sprite headSprite2;
    [SerializeField] Sprite headSprite3;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnHitboxEnter(GameObject flower)
    {
        //don't execute on objects collided with during intro
        if (!GameControl.PlayerData.loading)
            StartCoroutine(Growth(flower));
    }

    IEnumerator Growth(GameObject flower)
    {
        //establish all parts of the flower that are important
        FlowerBehavior behavior = flower.GetComponentInChildren<FlowerBehavior>();
        SpriteRenderer headSprite = flower.GetComponentInChildren<SunnyStats>().transform.GetComponent<SpriteRenderer>();
        //Stem Animator
        Animator stemAnim = flower.GetComponentsInChildren<Animator>().Last();

        if (behavior.growing == true)
            yield break;
        else
            behavior.growing = true;
        float time = 0f;
        stemAnim.Play("Tier2");
        Debug.Log("starting growth");
        Vector3 startingPos = headSprite.transform.localPosition;
        while (time < firstTierTime)
        {
            if (!flower.activeSelf)
                yield break;
            //lerping the flowerHead
            headSprite.transform.localPosition = Vector3.Lerp(startingPos, startingPos + secondTierPos, time / firstTierTime);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("tier 2 reached");
        behavior.tier++;
        headSprite.sprite = headSprite2;
        stemAnim.Play("Tier3");
        time = 0f;
        while (time < secondTierTime)
        {
            if (!flower.activeSelf)
                yield break;
            //lerping the flowerHead
            headSprite.transform.localPosition = Vector3.Lerp(startingPos + secondTierPos, startingPos + thirdTierPos, time / secondTierTime);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        behavior.tier++;
        headSprite.sprite = headSprite3;
        Debug.Log("tier 3 reached");
        yield return null;
    }
}
