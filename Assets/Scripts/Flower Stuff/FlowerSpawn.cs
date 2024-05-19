using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSpawn : MonoBehaviour
{
    public Camera cam;
    public GameObject Player;
    [SerializeField] GameObject[] CommonFlowers;
    [SerializeField] GameObject[] UncommonFlowers;
    [SerializeField] float uncommonRarity;
    [SerializeField] float spawnDelay = 0.5f;
    private GameObject[] Flowers;
    private int flowerCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        flowerCount = PlayerPrefs.GetInt("flowerCount");
        StartCoroutine(nextSpawn());
    }

    IEnumerator nextSpawn()
    {
        while (true)
        {
            if (flowerCount < 200)
            {
                Spawn();
            }
            yield return new WaitForSeconds(spawnDelay); // one second
        }
    }

    void Spawn()
    {
        //calculations for the screen corners
        Vector3 screenCorner = new Vector3(Screen.width, Screen.height);
        Vector3 pos = cam.ScreenToWorldPoint(screenCorner);
        Vector3 oppScreenCorner = new Vector3(0, 0);
        Vector3 negPos = cam.ScreenToWorldPoint(oppScreenCorner);
        //get input from player
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        float spawnX = 0;
        float spawnY = 0;
        //assign spawn location randomly based on the direction the player is moving
        if (moveHorizontal > 0)
            spawnX = Random.Range(pos.x, pos.x + Mathf.Abs(pos.x - Player.transform.position.x));
        else if (moveHorizontal < 0)
            spawnX = Random.Range(negPos.x - Mathf.Abs(negPos.x - Player.transform.position.x), negPos.x);
        else
            spawnX = Random.Range(negPos.x, pos.x);
        if (moveVertical > 0)
            spawnY = Random.Range(pos.y, pos.y + Mathf.Abs(pos.y - Player.transform.position.y));
        else if (moveVertical < 0)
            spawnY = Random.Range(negPos.y - Mathf.Abs(negPos.y - Player.transform.position.y), negPos.y);
        else
            spawnY = Random.Range(negPos.y, pos.y);
        Vector3 spawnPos = new Vector3(spawnX, spawnY);
        //Debug.Log(pos +  " " + negPos);
        //spawn a flower as long as the player is moving
        if (moveHorizontal != 0 || moveVertical != 0)
        {
            //rarity calculations, 0.3f chance for uncommon, 0.7f chance for common at this stage
            float rarityChoice = Random.Range(0f, 1f);
            if (rarityChoice > uncommonRarity)
                Flowers = CommonFlowers;
            else
                Flowers = UncommonFlowers;
            //selection of a flower from the given list and spawning
            int flowerChoice = Random.Range(0, Flowers.Length);
            GameObject newFlower = Instantiate(Flowers[flowerChoice], spawnPos, Quaternion.identity);
            PlayerPrefs.SetInt("flowerCount", flowerCount++);
        }
    }

    // Update is called once per frame
    void Update()
    {
        flowerCount = PlayerPrefs.GetInt("flowerCount");
    }
}
