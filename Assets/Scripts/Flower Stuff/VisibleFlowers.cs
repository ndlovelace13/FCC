using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VisibleFlowers : MonoBehaviour
{
    Camera cam;
    static List<FlowerData> flowerInfo;
    static Vector3 pos;
    static Vector3 negPos;
    static float xDiff;
    static float yDiff;
    static List<FlowerData> visibleFlowers;

    //flower pools
    GameObject whitePool;
    GameObject pinkPool;
    GameObject orangePool;
    GameObject bluePool;
    GameObject greenPool;
    GameObject redPool;
    GameObject yellowPool;
    GameObject dandyPool;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        visibleFlowers = new List<FlowerData>();
        StartCoroutine(PosUpdate());

        //assigning pools
        GameObject[] pools = GameObject.FindGameObjectsWithTag("headPool");
        foreach (GameObject pool in pools)
        {
            switch (pool.name)
            {
                case "WhitePool": whitePool = pool; break;
                case "PinkPool": pinkPool = pool; break;
                case "OrangePool": orangePool = pool; break;
                case "BluePool": bluePool = pool; break;
                case "GreenPool": greenPool = pool; break;
                case "RedPool": redPool = pool; break;
                case "YellowPool": yellowPool = pool; break;
                case "DandyPool": dandyPool = pool; break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FlowerEstablish(List<FlowerData> ogFlowers)
    {
        Debug.Log("establish these nuts " + ogFlowers.Count);
        flowerInfo = ogFlowers;
        /*foreach (var flower in flowerInfo)
        {
            Debug.Log("what");
            GameObject newFlower = gameObject.GetComponent<ObjectPool>().GetPooledObject();
            if (newFlower != null)
            {
                Debug.Log("alksdjflkasdjkfl");
                newFlower.SetActive(true);
                newFlower.transform.position = flower.getPosition();
            }
            else
            {
                Debug.Log("what the fuck");
            }
        }*/
        StartCoroutine(VisibleUpdate());
    }

    private static bool isVisible(FlowerData flower)
    {
        Vector2 flowerPos = flower.getPosition();
        //Debug.Log(flowerPos);
        return flowerPos.x < (pos.x + xDiff) && flowerPos.x >= (negPos.x - xDiff) && flowerPos.y > (negPos.y - yDiff) && flowerPos.y <= (pos.y + yDiff);
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

    IEnumerator VisibleUpdate()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            foreach (var flower in flowerInfo)
            {
                if (isVisible(flower) && flower.getFlower() == null)
                {
                    StartCoroutine(VisibleApply(flower));
                }
                else if (isVisible(flower) == false && flower.getFlower() != null)
                {
                    StartCoroutine(VisibleRemove(flower));
                }
                //else
                //{
                //    Debug.Log(isVisible(flower) + " " + flower.isActivated());
                //}
                //Debug.Log("current num of visibleFlowers: " + visibleFlowers.Count);
            }
            //StartCoroutine(VisibleApplication());
        }
    }



    IEnumerator VisibleApplication()
    {
        Debug.Log("Visible Flowers: " + visibleFlowers.Count);
        foreach (var flower in visibleFlowers)
        {
            if (!flower.isActivated())
            {
                GameObject newFlower = gameObject.GetComponent<ObjectPool>().GetPooledObject();
                if (newFlower != null)
                {
                    flower.SetActivated(true);
                    newFlower.SetActive(true);
                    flower.setFlower(newFlower);
                    newFlower.transform.position = flower.getPosition();
                }
                else
                {
                    Debug.Log("what the fuck");
                }
            }
        }
        yield return null;
    }

    IEnumerator VisibleApply(FlowerData flower)
    {
        yield return null;
        if (!visibleFlowers.Contains(flower))
        {
            GameObject newFlower = gameObject.GetComponent<ObjectPool>().GetPooledObject();
            if (newFlower != null)
            {
                visibleFlowers.Add(flower);
                flower.SetActivated(true);
                newFlower.SetActive(true);
                flower.setFlower(newFlower);
                newFlower.transform.position = flower.getPosition();
                GameObject head = headReturn(flower.getType());
                head.SetActive(true);
                head.transform.position = flower.getPosition() + new Vector2(-0.061f, 0.44f);
                head.transform.parent = newFlower.transform;
                
                //apply uncommon particle if uncommon
                //newFlower.GetComponentInChildren<Animator>().SetInteger("rarity", flower.getRarity());
            }
            else
            {
                Debug.Log("visible flowers count" + visibleFlowers.Count);
            }
        }
    }

    IEnumerator VisibleRemove(FlowerData flower)
    {
        yield return null;
        if (visibleFlowers.Contains(flower))
        {
            GameObject head = flower.getFlower().transform.GetChild(1).gameObject;
            head.transform.parent = null;
            head.SetActive(false);
            flower.getFlower().SetActive(false);
            flower.setFlower(null);
            flower.SetActivated(false);
            visibleFlowers.Remove(flower);
        }
    }

    public void FlowerHarvested(GameObject flower)
    {
        foreach (FlowerData f in visibleFlowers)
        {
            if (flower == f.getFlower())
            {
                GameObject head = f.getFlower().transform.GetChild(1).gameObject;
                head.transform.parent = null;
                head.SetActive(false);
                f.getFlower().SetActive(false);
                f.setFlower(null);
                f.SetActivated(false);
                visibleFlowers.Remove(f);
                flowerInfo.Remove(f);
                //CALL A COROUTINE HERE TO REIMPLEMENT AFTER A SET AMOUNT OF TIME NOT SUPER IMPORTANT
                break;
            }
        }
    }

    private GameObject headReturn(string type)
    {
        GameObject headReturn = null;
        switch (type)
        {
            case "white": headReturn = whitePool.GetComponent<ObjectPool>().GetPooledObject(); break;
            case "pink": headReturn = pinkPool.GetComponent<ObjectPool>().GetPooledObject(); break;
            case "orange": headReturn = orangePool.GetComponent<ObjectPool>().GetPooledObject(); break;
            case "red": headReturn = redPool.GetComponent<ObjectPool>().GetPooledObject(); break;
            case "yellow": headReturn = yellowPool.GetComponent<ObjectPool>().GetPooledObject(); break;
            case "green": headReturn = greenPool.GetComponent<ObjectPool>().GetPooledObject(); break;
            case "blue": headReturn = bluePool.GetComponent<ObjectPool>().GetPooledObject(); break;
            case "dandy": headReturn = dandyPool.GetComponent<ObjectPool>().GetPooledObject(); break;   
        }
        return headReturn;
    }
}
