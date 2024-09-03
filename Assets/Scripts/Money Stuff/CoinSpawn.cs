using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawn : MonoBehaviour
{
    int maxCoins = 30;
    [SerializeField] ObjectPool coinPool;
    // Start is called before the first frame update
    void Start()
    {
        //grab the coin pool from the gameControl object here - store as a variable in gamecontrol as well
        coinPool = GameControl.PlayerData.moneyPool;
        //StartCoroutine(CoinTesting());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*IEnumerator CoinTesting()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            Payout(10);
        }
        

    }*/

    public void Payout(int amount, ScoreCategory cat)
    {
        //input is the amount of coins necessary to spawn
        Debug.Log("Spawner paying out: " + amount);
        StartCoroutine(CoinSpawning(amount, cat));
    }

    IEnumerator CoinSpawning(int amount, ScoreCategory cat)
    {
        yield return null;
        int remainingCoins = amount;
        int nickelCount = 0;
        int quarterCount = 0;
        //if the value exceeds the maxCoin count using only pennies alone, call coinSplit to determine how many other coins are necessary
        if (amount > maxCoins)
        {
            Tuple<int, int> returnedCounts = CoinSplit(amount);
            nickelCount = returnedCounts.Item1;
            quarterCount = returnedCounts.Item2;
        }
        while (remainingCoins > 0)
        {
            int currentCoins = 0;
            int currentValue;
            if (remainingCoins > 10)
            {
                currentCoins = 10;
            }
            else
                currentCoins = remainingCoins;
            for (int i = 0; i < currentCoins; i++)
            {
                //decide the value of the coin
                if (nickelCount > 0)
                {
                    nickelCount--;
                    currentValue = 5;
                }
                else if (quarterCount > 0)
                {
                    quarterCount--;
                    currentValue = 25;
                }
                else
                    currentValue = 1;
                GameObject coin = coinPool.GetPooledObject();
                coin.SetActive(true);
                coin.GetComponent<CoinBehavior>().CoinLerp(transform.position, transform.position, cat, currentValue);
            }

            remainingCoins -= currentCoins;
            yield return new WaitForEndOfFrame();
        }
        transform.parent = null;
        gameObject.SetActive(false);
    }

    //determine how many nickels and/or quarters are necessary to get the coin count below the max
    private Tuple<int, int> CoinSplit(int amount)
    {
        int nickels = 0;
        int quarters = 0;

        while (amount > maxCoins)
        {
            if (nickels < 25)
            {
                nickels++;
                amount -= 5;
            }
            else
            {
                quarters++;
                nickels -= 5;
            }
        }

        return new Tuple<int, int>(nickels, quarters);
    }
}
