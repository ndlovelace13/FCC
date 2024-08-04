using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] TMP_Text bossName;
    [SerializeField] RectTransform healthFill;
    [SerializeField] RectMask2D healthMask;

    float maxMask;
    float targetMask;
    float currentMask;

    EnemyBehavior currentBoss;

    [SerializeField] RectTransform fullObj;

    bool bossActive = false;

    // Start is called before the first frame update
    void Start()
    {
        currentBoss = GameObject.FindWithTag("boss").GetComponent<EnemyBehavior>();
        bossName.text = "";

        StartCoroutine(LerpUp());
        StartCoroutine(HealthbarFill());
    }

    // Update is called once per frame
    void Update()
    {
        if (bossActive)
        {
            //update the healthbar to reflect the boss's current health
            float currentPercent = (float)currentBoss.health / (float)currentBoss.maxHealth;
            currentMask = maxMask - currentPercent * maxMask;
            Debug.Log(currentBoss.health + " | " + currentBoss.maxHealth + " = " + currentBoss.health / currentBoss.maxHealth);

            //get the current padding, adjust the x value
            var padding = healthMask.padding;
            padding.z = currentMask;
            healthMask.padding = padding;
        }
    }

    IEnumerator LerpUp()
    {
        Vector2 finalPos = fullObj.localPosition;
        fullObj.localPosition = new Vector2(finalPos.x, finalPos.y - Screen.height / 4);
        Vector2 startingPos =  fullObj.localPosition;

        float currentTime = 0f;
        while (currentTime < 1f)
        {
            fullObj.localPosition = Vector2.Lerp(startingPos, finalPos, currentTime);
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator LerpDown()
    {
        Vector2 startingPos = fullObj.localPosition;
        Vector2 finalPos = new Vector2(startingPos.x, startingPos.y - Screen.height / 4);

        float currentTime = 0f;
        while (currentTime < 1f)
        {
            fullObj.localPosition = Vector2.Lerp(startingPos, finalPos, currentTime);
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator HealthbarFill()
    {
        Canvas.ForceUpdateCanvases();
        float spawnTime = ((Bully)currentBoss).spawnTime;
        float currentTime = 0f;

        //init health bar
        maxMask = healthFill.rect.width;
        //Debug.Log("MaxMask: " + maxMask);
        targetMask = 0;

        while (currentTime < spawnTime)
        {
            //lerp the padding to get to maxHealth eventually
            float currentAmount = Mathf.Lerp(0, currentBoss.health, currentTime / spawnTime);
            float currentPercent = currentAmount / currentBoss.maxHealth;
            currentMask = maxMask - currentPercent * maxMask;
            //Debug.Log(currentMask + " | " + currentPercent);

            //get the current padding, adjust the x value
            var padding = healthMask.padding;
            padding.z = currentMask;
            healthMask.padding = padding;

            //increment time
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
        bossActive = true;
        bossName.text = GameControl.PlayerData.enemyStatsDict[currentBoss.type].title;
    }
}
