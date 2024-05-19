using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public Camera cam;
    public GameObject Player;
    public float spawnDelay = 2f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawnTimer());
    }

    IEnumerator spawnTimer()
    {
        //Debug.Log("Yall got me fucked up");
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);
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

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Spawn(GameObject newEnemy)
    {
        if (newEnemy == null)
            yield return null;
        //calculations for the screen corners
        Vector3 screenCorner = new Vector3(Screen.width, Screen.height);
        Vector3 pos = cam.ScreenToWorldPoint(screenCorner);
        Vector3 oppScreenCorner = new Vector3(0, 0);
        Vector3 negPos = cam.ScreenToWorldPoint(oppScreenCorner);
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
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
        newEnemy.SetActive(true);
        newEnemy.transform.position = spawnPos;
        newEnemy.GetComponent<EnemyBehavior>().Activate();
        yield return null;
    }
}
