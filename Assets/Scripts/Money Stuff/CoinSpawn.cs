using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawn : MonoBehaviour
{

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
        while (remainingCoins > 0)
        {
            int currentCoins = 0;
            if (remainingCoins > 10)
            {
                currentCoins = 10;
            }
            else
                currentCoins = remainingCoins;
            for (int i = 0; i < currentCoins; i++)
            {
                GameObject coin = coinPool.GetPooledObject();
                coin.SetActive(true);
                coin.GetComponent<CoinBehavior>().CoinLerp(transform.position, transform.position, cat);
            }

            remainingCoins -= currentCoins;
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }
}
