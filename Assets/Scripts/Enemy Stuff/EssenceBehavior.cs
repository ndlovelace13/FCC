using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EssenceBehavior : Item
{
    public void LootDrop()
    {
        placed = false;
        StartCoroutine(LootLerp());
    }

    IEnumerator LootLerp()
    {
        Vector2 currentLoc = transform.localPosition;
        Vector2 finalLootLoc = currentLoc + new Vector2(Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f));
        float currentTime = 0f;
        while (currentTime < 0.5f)
        {
            transform.localPosition = Vector2.Lerp(currentLoc, finalLootLoc, currentTime / 0.5f);
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }
        placed = true;

        if (GameControl.PlayerData.gameWin)
        {
            Debug.Log("now lerping to player");
            pickingUp = true;
            yield return new WaitForSeconds(Random.Range(0f, 2f));
            StartCoroutine(PlayerLerp(3f));
        }
        else
        {
            Debug.Log("not happening");
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        lerpTarget = player.transform;
        transform.localScale = Vector3.one * 0.25f;
        placed = true;
        GetComponent<SizeLerp>().lerping = false;
        GetComponent<SizeLerp>().Execute(true);
    }

    IEnumerator StartLerp()
    {
        //make a little parabola here
        yield return null;
    }


    protected override void AssignValue()
    {
        GameControl.SaveData.essenceCount++;
        GameControl.PlayerData.shiftSeeds++;
        //first seed is picked up
        if (!GameControl.SaveData.firstSeed)
            GameControl.SaveData.firstSeed = true;
        base.AssignValue();
    }
}
