using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class SunnyStats : FlowerStats
{
    Vector3 startingPos = new Vector3(-0.061f, 0.44f);
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

    [SerializeField] Sprite stem2;
    [SerializeField] Sprite stem3;

    //blindStats
    float baseBlindTime = 1f;
    //float blindTime;
    float additionalBlindTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        description = "The Sunflower, while related to its zombie fighting cousins, shares little resemblence when it comes to its abilities. It sometimes questions whether it can really be related to those cheery healers - genetics are weird like that.";
        effects = "Extended Growth - If the player walks over this flower without harvesting, it will continue to grow in power\nBlind - Afflicted enemies will be unable to track the player for a period of time";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override Sprite GetHeadSprite(int tier)
    {
        switch (tier)
        {
            case 2: return headSprite2;
            case 3: return headSprite3;
            default: return headSprite;
        }
    }

    public override void SetStem(GameObject head, GameObject stem)
    {
        switch (head.GetComponent<FlowerBehavior>().tier)
        {
            case 2:
                stem.GetComponent<Animator>().Play("Tier2Done");
                head.transform.localPosition = startingPos + secondTierPos;
                break;
            case 3:
                stem.GetComponent<Animator>().Play("Tier3Done");
                head.transform.localPosition = startingPos + thirdTierPos;
                break;
            default:
                base.SetStem(head, stem);
                break;
        }
    }

    public override int GetProjRange(int tier)
    {
        Debug.Log("Proj Range Boosted by " + ((tier - 1) * projRangeBoost));
        return projRange + ((tier - 1) * projRangeBoost);
    }

    public override int GetRange(int tier)
    {
        Debug.Log("Range Boosted by " + ((tier - 1) * rangeBoost));
        return range + ((tier - 1) * rangeBoost);
    }

    public override int GetDamage(int tier)
    {
        Debug.Log("Damage Boosted by " + ((tier - 1) * damageBoost));
        return damage + ((tier - 1) * damageBoost);
    }

    public override int GetPoints(int tier)
    {
        Debug.Log("Points Boosted by " + ((tier - 1) * pointsBoost));
        return basePoints + ((tier - 1) * pointsBoost);
    }

    public override int GetProjCount(int tier)
    {
        Debug.Log("Proj Count Boosted by " + ((tier - 1) * projBoost));
        return projCount + ((tier - 1) * projBoost);
    }

    public override void OnHitboxEnter(GameObject flower)
    {
        //don't execute on objects collided with during intro
        if (!GameControl.PlayerData.loading)
            StartCoroutine(Growth(flower));
    }

    public override void OnEnemyCollision(GameObject enemy, int tier)
    {
        Debug.Log("blind Called? " + tier);
        StartCoroutine(BlindHandler(enemy, tier));
    }

    IEnumerator BlindHandler(GameObject enemy, int power)
    {
        //wait for surprise to be over - BROKEN
        while (enemy.GetComponent<EnemyBehavior>().surprised || enemy.GetComponent<EnemyBehavior>().isFrozen)
        {
            yield return new WaitForEndOfFrame();
        }
        //begin the blinding
        float time = 0f;
        float blindTime = baseBlindTime + (power * additionalBlindTime);
        GameObject part = enemy.GetComponent<EnemyBehavior>().nextParticle();
        enemy.GetComponent<EnemyBehavior>().setParticle(part, 5);
        enemy.GetComponent<EnemyBehavior>().isBlinded = true;
        while (time < blindTime)
        {
            if (!enemy.activeInHierarchy)
            {
                enemy.GetComponent<EnemyBehavior>().isBlinded = false;
                yield break;
            }
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
        //end the blinding
        enemy.GetComponent<EnemyBehavior>().isBlinded = false;
        enemy.GetComponent<EnemyBehavior>().setParticle(part, 0);
    }

    IEnumerator Growth(GameObject flower)
    {
        //establish all parts of the flower that are important
        FlowerBehavior behavior = flower.GetComponentInChildren<FlowerBehavior>();
        SpriteRenderer headSprite = flower.GetComponentInChildren<SunnyStats>().transform.GetComponent<SpriteRenderer>();
        //Stem Animator
        Animator stemAnim = flower.GetComponentsInChildren<Animator>().Last();
        GameObject shadow = flower.GetComponentInChildren<Rigidbody2D>().gameObject;

        if (behavior.growing == true)
            yield break;
        else
            behavior.growing = true;
        float time = 0f;
        shadow.GetComponent<SizeLerp>().Execute(true);
        if (behavior.tier == 1)
        {
            
            stemAnim.Play("Tier2");
            Debug.Log("starting growth");
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
        }
        if (behavior.tier == 2)
        {
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
        }
        shadow.GetComponent<SizeLerp>().Execute(false);
        yield return null;
    }
}
