using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenFlowers : MonoBehaviour
{
    [SerializeField] float spawnDelay = 0.5f;
    [SerializeField] GameObject[] CommonFlowers;
    [SerializeField] GameObject[] UncommonFlowers;
    [SerializeField] float uncommonRarity;
    private GameObject[] Flowers;
    private int flowerCount = 0;
    [SerializeField] Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(nextSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator nextSpawn()
    {
        while (true)
        {
            if (flowerCount < 50)
            {
                Spawn();
            }
            yield return new WaitForSeconds(spawnDelay); // one second
        }
    }

    void Spawn()
    {
        Vector3 screenCorner = new Vector3(Screen.width, Screen.height);
        Vector3 pos = cam.ScreenToWorldPoint(screenCorner);
        Vector3 oppScreenCorner = new Vector3(0, 0);
        Vector3 negPos = cam.ScreenToWorldPoint(oppScreenCorner);
        float spawnX = Random.Range(negPos.x, pos.x);
        float spawnY = Random.Range(negPos.y, pos.y);
        Vector3 spawnPos = new Vector3(spawnX, spawnY);

        float rarityChoice = Random.Range(0f, 1f);
        if (rarityChoice > uncommonRarity)
            Flowers = CommonFlowers;
        else
            Flowers = UncommonFlowers;
        //selection of a flower from the given list and spawning
        int flowerChoice = Random.Range(0, Flowers.Length);
        GameObject newFlower = Instantiate(Flowers[flowerChoice], spawnPos, Quaternion.identity);
        flowerCount++;

    }
}
