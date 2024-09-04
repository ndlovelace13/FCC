using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScoreCategory
{
    construction,
    discovery,
    enemy,
    other
}

public class CoinBehavior : Item
{
    ScoreCategory cat;
    [SerializeField] int value = 1;

    [SerializeField] Sprite penny;
    [SerializeField] Sprite nickel;
    [SerializeField] Sprite quarter;

    protected override void OnEnable()
    {
        base.OnEnable();
        lerpTarget = player.transform.root;
    }
    public void CoinLerp(Vector3 startPos, Vector3 endPos, ScoreCategory scoreCat, int val)
    {
        cat = scoreCat;
        CoinType(val);
        StartCoroutine(ActualCoinLerp(startPos, endPos));
    }

    IEnumerator ActualCoinLerp(Vector2 startPos, Vector2 endPos)
    {
        float currentTime = 0f;
        //assign a random time for landing
        float targetTime = Random.Range(0.5f, 0.8f);


        //randomly assign a landing position
        //float targetMag = Random.Range(1f, 2f);
        //Vector2 targetAngle = new Vector2(Mathf.Cos(Random.Range(0, 2 * Mathf.PI)), Mathf.Sin(Random.Range(0, 2 * Mathf.PI)));
        float xVel = Random.Range(0f, 3f) * (Random.Range(0, 2) * 2 - 1);
        float yVel = Random.Range(5f, 10f);
        float accel = -10f;

        //Vector2 targetPos = endPos + targetAngle * targetMag;

        //randomly assign a rotation speed and starting rotation
        transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
        float rotSpeed = Random.Range(1f, 3f) * (Random.Range(0, 2) * 2 - 1);


        while (currentTime < targetTime)
        {
            float yFunc = accel * (Mathf.Pow(currentTime, 2)) + yVel * currentTime;
            transform.position = new Vector2(startPos.x + xVel * currentTime, startPos.y + yFunc);
            transform.localRotation = Quaternion.Euler(0, 0, transform.localRotation.eulerAngles.z + (360f * Time.deltaTime * rotSpeed));
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        placed = true;
    }

    protected void CoinType(int val)
    {
        value = val;
        switch (val)
        {
            case 1: GetComponent<SpriteRenderer>().sprite = penny; break;
            case 5: GetComponent<SpriteRenderer>().sprite = nickel; break;
            case 25: GetComponent<SpriteRenderer>().sprite = quarter; break;
            default: Debug.Log("monetary amount not yet assigned"); break;
        }
    }

    protected override void AssignValue()
    {
        GameControl.PlayerData.score += value;
        switch (cat)
        {
            case ScoreCategory.construction: GameControl.PlayerData.constructionScore += value; break;
            case ScoreCategory.discovery: GameControl.PlayerData.discoveryScore += value; break;
            case ScoreCategory.enemy: GameControl.PlayerData.enemyScore += value; break;
            case ScoreCategory.other: GameControl.PlayerData.otherScore += value; break;
        }
        GameObject.FindWithTag("MoneyCombo").GetComponent<MoneyCounter>().MoneyAdded(value);
        base.AssignValue();
    }
}
