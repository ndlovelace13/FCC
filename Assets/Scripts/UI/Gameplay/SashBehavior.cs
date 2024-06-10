using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SashBehavior : MonoBehaviour
{
    int slots;
    [SerializeField] GameObject slotPrefab;
    List<GameObject> slotObjects;
    [SerializeField]
    Sprite[] slotDefaults;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void slotInit()
    {
        slotObjects = new List<GameObject>();
        //slotObjects = GameObject.FindGameObjectsWithTag("affinitySlot");
        slots = GameControl.PlayerData.sashSlots;
        for (int i = 0; i < slots; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab);
            newSlot.transform.parent = gameObject.transform;
            newSlot.transform.GetComponent<RectTransform>().localPosition = Vector3.zero;
            slotObjects.Add(newSlot);
            slotObjects[i].GetComponent<SashSlot>().SpriteApply(slotDefaults[i]);
            GameControl.PlayerData.currentAffinities[i] = newSlot;
        }
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<CrownThrowing>().crownHeld == false && GameControl.PlayerData.loading == false)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && slots > 0)
            {
                StartCoroutine(AssignSlot(0));
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && slots > 1)
            {
                StartCoroutine(AssignSlot(1));
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && slots > 2)
            {
                StartCoroutine((AssignSlot(2)));
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) && slots > 3)
            {
                StartCoroutine((AssignSlot(3)));
            }
            if (Input.GetKeyDown(KeyCode.Alpha5) && slots > 4)
            {
                StartCoroutine((AssignSlot(4)));
            }
        }
    }

    IEnumerator AssignSlot(int slot)
    {
        yield return null;
        GameObject lastFlower = player.GetComponent<FlowerHarvest>().lastFlower();
        if (lastFlower != null)
        {
            string type = lastFlower.GetComponent<FlowerStats>().type;
            if (!GameControl.PlayerData.affinityAmounts.ContainsKey(type))
            {
                if (GameControl.PlayerData.sashTypes[slot] == "null")
                {
                    GameControl.PlayerData.sashTypes[slot] = type;
                    GameControl.PlayerData.affinityAmounts.Remove("slot" + slot);
                    GameControl.PlayerData.affinityAmounts.Add(type, 0);
                    slotObjects[slot].GetComponent<SashSlot>().NewFlower(type);
                    //slotObjects[slot + 1].GetComponent<Image>().sprite = GameControl.PlayerData.SpriteAssign(type);
                }
                else
                {
                    string lastType = GameControl.PlayerData.sashTypes[slot];
                    GameControl.PlayerData.sashTypes[slot] = type;
                    GameControl.PlayerData.affinityAmounts.Remove(lastType);
                    GameControl.PlayerData.affinityAmounts.Add(type, 0);
                    slotObjects[slot].GetComponent<SashSlot>().NewFlower(type);
                    //slotObjects[slot + 1].GetComponent<Image>().sprite = GameControl.PlayerData.SpriteAssign(type);
                }
            }
        }
    }
}
