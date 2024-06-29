using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class FlowerHarvest : MonoBehaviour
{
    [SerializeField] GameObject[] slots;
    [SerializeField] GameObject docket;
    GameObject flowerPool;
    GameObject crown;
    public bool docketLoaded = true;
    public bool crownHeld = false;

    [SerializeField] AK.Wwise.Event harvestSound;
    // Start is called before the first frame update
    void Start()
    {
        //crown = GameObject.FindWithTag("finalCrown").transform;
        flowerPool = GameObject.FindGameObjectWithTag("flowerPool");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Q))
        {
            if (GameControl.PlayerData.tutorialActive)
            {
                if (GameControl.PlayerData.tutorialState == 3)
                    SingleFire();
            }
            else
                SingleFire();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("TRIGGERED");
        if (collision.gameObject.tag == "flower")
        {
           // Debug.Log("2 Levels");
            if (Input.GetKey(KeyCode.Space) && !docketLoaded && !GameControl.PlayerData.loading)
            {
                //I'm putting a bullet through my skull
                GameObject flower = collision.gameObject.transform.parent.gameObject;
                Debug.Log("This is the thing: " + flower.name);
                foreach (Transform t in flower.transform)
                {
                    if (t.CompareTag("FlowerHead"))
                    {
                        GameObject head = t.gameObject;
                        int slotPos = nextSlot();
                        if (slotPos != -1)
                        {
                            AkSoundEngine.PostEvent("Flower_Harvest", transform.parent.gameObject);
                            //TODO: PUll from the pool instead
                            GameObject newHead = Instantiate(head, slots[slotPos].transform);
                            newHead.GetComponent<SpriteRenderer>().sortingLayerName = "Midground";
                            newHead.GetComponent<SpriteRenderer>().sortingOrder = 4;
                            newHead.transform.localScale = new Vector3(1f, 1f, 1f);
                            newHead.GetComponent<FlowerBehavior>().picked = true;
                            //newHead.transform.position = slots[slotPos].transform.position;
                            //Debug.Log("WHAT THE FUCK IS HAPPENING: " + crown.gameObject.name);
                            newHead.transform.SetParent(crown.transform);
                            newHead.GetComponent<FlowerBehavior>().position = slotPos;
                            Debug.Log("slot Position assigned: " + slotPos);

                            //check for discovery
                            string type = head.GetComponent<FlowerStats>().type;
                            StartCoroutine(DiscoveryCheck(type));
                            slots[slotPos].tag = "slotFull";
                            if (GameControl.PlayerData.tutorialState == 2)
                                GameControl.PlayerData.flowerHarvested = true;
                        }
                    }
                }
                if (GameControl.PlayerData.tutorialActive)
                    Destroy(flower);
                else
                {
                    flowerPool.GetComponent<VisibleFlowers>().FlowerHarvested(flower);
                    //PlayerPrefs.SetInt("flowerCount", PlayerPrefs.GetInt("flowerCount") - 1);
                }
            }
        }
    }

    private void SingleFire()
    {
        Debug.Log("Last full slot" + lastSlot());
        int slotPos = lastSlot();
        if (slotPos != -1 && !transform.parent.GetComponent<CrownConstruction>().skillCheckActive && !transform.parent.GetComponent<CrownConstruction>().constructionReady)
        {
            AkSoundEngine.PostEvent("Throw", transform.parent.gameObject);
            docketLoaded = false;
            slots[slotPos].tag = "slotEmpty";
            GameObject tossedFlower = lastFlower();
            Debug.Log(tossedFlower.GetComponent<FlowerStats>().type);
            crown.GetComponent<CrownAttack>().SingleFire(tossedFlower);
            if (GameControl.PlayerData.tutorialState == 3)
            {
                GameControl.PlayerData.singleTossed = true;
            }
        }
    }

    IEnumerator DiscoveryCheck(string type)
    {
        if (GameControl.PlayerData.undiscoveredUncommon.Contains(type))
        {
            GameControl.PlayerData.FlowerDiscovery(type);
        }
        yield return null;
    }

    public void crownReset()
    {
        docketLoaded = false;
        foreach (GameObject slot in slots)
        {
            slot.tag = "slotEmpty";
        }
        Transform[] allObjects = docket.GetComponentsInChildren<Transform>();
        allObjects = allObjects.Where(child => child.tag == "finalCrown").ToArray();
        crown = allObjects[0].gameObject;
        if (crown)
        {
            Debug.Log(crown.gameObject.name);
        }
    }

    private int nextSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].tag == "slotEmpty")
            {
                if (i == 4)
                    docketLoaded = true;
                return i;
            }
        }
        return -1;
    }

    private int lastSlot()
    {
        for (int i = slots.Length - 1; i >= 0; i--)
        {
            if (slots[i].tag == "slotFull")
            {
                return i;
            }
        }
        return -1;
    }

    public GameObject lastFlower()
    {
        int slotPos = lastSlot();
        Transform[] currentFlowers = crown.GetComponentsInChildren<Transform>();
        int flowerNum = currentFlowers.Length;
        return currentFlowers[flowerNum - 1].gameObject;
    }
}
