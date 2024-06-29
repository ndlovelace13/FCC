using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public Camera cam;
    public GameObject Player;
    public float spawnDelay = 3f;
    List<GameObject> enemies = new List<GameObject>();
    static float xDiff;
    static float yDiff;
    static Vector3 pos;
    static Vector3 negPos;
    ObjectPool hatPool;

    //All enemy related variables
    [SerializeField] public EnemyStats thisEnemy;

    public float currentMax;
    public float currentMin;
    public int currentHealth;
    public int currentMaxEnemies;
    public int activeEnemies = 0;

    public int killScore;


    // Start is called before the first frame update
    void Start()
    {
        currentMax = thisEnemy.maxSpeed;
        currentMin = thisEnemy.minSpeed;
        currentHealth = thisEnemy.maxHealth;
        killScore = thisEnemy.killScore;
        currentMaxEnemies = thisEnemy.startingEnemies;
    }

    public void enemyBegin()
    {
        cam = Camera.main;
        Player = GameObject.FindWithTag("Player");
        hatPool = GameObject.FindWithTag("hats").GetComponent<ObjectPool>();
        activeEnemies = 0;
        StartCoroutine(spawnTimer());
        StartCoroutine(PosUpdate());
        StartCoroutine(VisibleCheck());
        StartCoroutine(CountScale());
        StartCoroutine(StatsScale());
    }

    IEnumerator spawnTimer()
    {
        //Debug.Log("Yall got me fucked up");
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);
            //check how many enemies are currently active in hierarchy
            //GameObject[] currentEnemies = GameObject.FindGameObjectsWithTag("enemy");
            Debug.Log("There are currently " + activeEnemies + " enemies in play");
            if (activeEnemies < currentMaxEnemies)
            {
                GameObject newEnemy = gameObject.GetComponent<ObjectPool>().GetPooledObject();
                if (newEnemy != null)
                {
                    //Debug.Log("Yall got me fucked up");
                    StartCoroutine(Spawn(newEnemy));
                }
                else
                {
                    Debug.Log("What the fuck is a kilometer");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 newSpawn()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        float spawnX = 0;
        float spawnY = 0;
        float spawnOffset = Mathf.Abs(pos.x - Player.transform.position.x);
        int choice = Random.Range(0, 2);
        if (moveHorizontal > 0)
            spawnX = Random.Range(pos.x, pos.x + Mathf.Abs(pos.x - Player.transform.position.x));
        else if (moveHorizontal < 0)
            spawnX = Random.Range(negPos.x - Mathf.Abs(negPos.x - Player.transform.position.x), negPos.x);
        else
        {
            if (choice == 0)
                spawnX = Random.Range(pos.x, pos.x + spawnOffset);
            else
                spawnX = Random.Range(negPos.x - spawnOffset, negPos.x);
        }
        choice = Random.Range(0, 2);
        spawnOffset = Mathf.Abs(pos.y - Player.transform.position.y);
        if (moveVertical > 0)
            spawnY = Random.Range(pos.y, pos.y + Mathf.Abs(pos.y - Player.transform.position.y));
        else if (moveVertical < 0)
            spawnY = Random.Range(negPos.y - Mathf.Abs(negPos.y - Player.transform.position.y), negPos.y);
        else
        {
            if (choice == 0)
                spawnY = Random.Range(pos.y, pos.y + spawnOffset);
            else
                spawnY = Random.Range(negPos.y - spawnOffset, negPos.y);
        }
        Vector3 spawnPos = new Vector3(spawnX, spawnY);
        return spawnPos;
    }

    IEnumerator Spawn(GameObject newEnemy)
    {
        
        newEnemy.SetActive(true);
        newEnemy.transform.position = newSpawn();
        newEnemy.GetComponent<EnemyBehavior>().SetObjects(this);
        newEnemy.GetComponent<EnemyBehavior>().Activate();
        StartCoroutine(HatAssign(newEnemy));
        if (!enemies.Contains(newEnemy))
            enemies.Add(newEnemy);
        activeEnemies++;
        yield return null;
    }

    IEnumerator HatAssign(GameObject newEnemy)
    {
        GameObject newHat = hatPool.GetPooledObject();
        newHat.transform.SetParent(newEnemy.transform);
        newHat.SetActive(true);
        newHat.transform.position = newEnemy.transform.position;
        newHat.GetComponent<HatBehavior>().Activate();
        //newHat.transform.position = newEnemy.transform.position;
        yield return null;
    }

    IEnumerator VisibleCheck()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            foreach (var enemy in enemies)
            {
                if (enemy.GetComponent<EnemyBehavior>().isActive && !isVisible(enemy))
                    StartCoroutine(Respawn(enemy));
            }
        }
    }

    IEnumerator Respawn(GameObject movedEnemy)
    {
        yield return null;
        Debug.Log("enemy moved");
        movedEnemy.transform.position = newSpawn();
    }

    private static bool isVisible(GameObject enemy)
    {
        Vector2 enemyPos = enemy.transform.position;
        //Debug.Log(flowerPos);
        return enemyPos.x < (pos.x + xDiff) && enemyPos.x >= (negPos.x - xDiff) && enemyPos.y > (negPos.y - yDiff) && enemyPos.y <= (pos.y + yDiff);
    }

    IEnumerator PosUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            pos = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
            negPos = cam.ScreenToWorldPoint(new Vector3(0, 0));
            xDiff = Mathf.Abs(pos.x - negPos.x) / 2;
            yDiff = Mathf.Abs(pos.y - negPos.y) / 2;

            //Debug.Log(pos + " " + negPos);
        }
    }

    //handles the scaling of enemy count over time
    IEnumerator CountScale()
    {
        while (true)
        {
            yield return new WaitForSeconds(thisEnemy.countScaleTime);
            currentMaxEnemies += thisEnemy.enemyScaleAmt;
        }
    }

    //handles the scaling of enemy stats over time
    IEnumerator StatsScale()
    {
        while (true)
        {
            yield return new WaitForSeconds(thisEnemy.statsScaleTime);
            if (GameControl.PlayerData.playerSpeed > currentMax)
            {
                currentMax += thisEnemy.maxInterval;
                currentMin += thisEnemy.minInterval;
            }
            currentHealth += thisEnemy.healthInterval;
            killScore += thisEnemy.scoreIncrease;
            
        }
    }
}
