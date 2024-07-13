using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InitialFlowers : MonoBehaviour
{
    Camera cam;
    List<string> flowerTypes;
    float spawnCooldown = 1f;
    [SerializeField] GameObject flowerBase;

    int flowerCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        Invoke("BeginSpawn", spawnCooldown);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginSpawn()
    {
        flowerTypes = GameControl.SaveHandler.LoadFlowers();
        StartCoroutine(nextSpawn());
    }

    IEnumerator nextSpawn()
    {
        while (true)
        {
            if (flowerCount < 50)
            {
                StartCoroutine(Spawn());
            }
            yield return new WaitForSeconds(spawnCooldown); // one second
        }
    }

    IEnumerator Spawn()
    {
        Vector3 screenCorner = new Vector3(Screen.width, Screen.height);
        Vector3 pos = cam.ScreenToWorldPoint(screenCorner);
        Vector3 oppScreenCorner = new Vector3(0, 0);
        Vector3 negPos = cam.ScreenToWorldPoint(oppScreenCorner);
        float spawnX = Random.Range(negPos.x, pos.x);
        float spawnY = Random.Range(negPos.y, pos.y);
        Vector3 spawnPos = new Vector3(spawnX, spawnY);

        //selection of a flower from the given list and spawning
        int flowerChoice = Random.Range(0, flowerTypes.Count);
        //get the flowerHead from the flowers list in gamecontrol
        GameObject head = HeadReturn(flowerTypes[flowerChoice]);
        GameObject newFlower = Instantiate(flowerBase, spawnPos, Quaternion.identity);
        head.transform.SetParent(newFlower.transform);
        head.transform.position = newFlower.transform.position;
        //execute the initial growth anim
        Animator stemAnim = newFlower.GetComponentsInChildren<Animator>().Last();
        stemAnim.Play("BasicGrow");
        while (stemAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return new WaitForEndOfFrame();
        }
        head.GetComponent<SpriteRenderer>().enabled = true;
        head.GetComponent<FlowerBehavior>().picked = false;
        head.GetComponent<FlowerBehavior>().growing = false;
        head.GetComponent<FlowerStats>().SetStem(head, stemAnim.gameObject);
        flowerCount++;

    }

    private GameObject HeadReturn(string type)
    {
        GameObject head = new GameObject();
        switch (type)
        {
            case "pink":
                head = Instantiate(GameControl.PlayerData.flowers[0]);
                break;
            case "white":
                head = Instantiate(GameControl.PlayerData.flowers[1]);
                break;
            case "orange":
                head = Instantiate(GameControl.PlayerData.flowers[2]);
                break;
            case "red":
                head = Instantiate(GameControl.PlayerData.flowers[3]);
                break;
            case "blue":
                head = Instantiate(GameControl.PlayerData.flowers[4]);
                break;
            case "green":
                head = Instantiate(GameControl.PlayerData.flowers[5]);
                break;
            case "yellow":
                head = Instantiate(GameControl.PlayerData.flowers[6]);
                break;
            case "dandy":
                head = Instantiate(GameControl.PlayerData.flowers[7]);
                break;
            case "wild":
                head = Instantiate(GameControl.PlayerData.flowers[8]);
                break;
            case "sunny":
                head = Instantiate(GameControl.PlayerData.flowers[9]);
                break;
            default:
                Debug.Log("fucked it up");
                break;

        }
        head.GetComponent<SpriteRenderer>().enabled = false;
        return head;
    }
}
