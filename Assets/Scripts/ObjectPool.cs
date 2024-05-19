using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject objectToPool;
    public List<GameObject> pooledObjects;
    public int amountToPool;
    public int activeObjects;
    bool objectActivated;
    // Start is called before the first frame update

    void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
            //Debug.Log(i);
        }
    }

    public GameObject GetPooledObject()
    {
        ///THIS MIGHT BREAK THE ENEMY SPAWN
        for(int i = 0; i < amountToPool; i++)
        {
            //Debug.Log("at least I tried");
            if (!pooledObjects[i].activeInHierarchy)
            {
                //Debug.Log("why" + i);
                return pooledObjects[i];
            }
        }
        //Debug.Log("No objects");
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("sec") % 30 == 0)
        {
            if (amountToPool >= activeObjects && !objectActivated)
            {
                objectActivated = true;
                activeObjects++;
            }
        }
        else
        {
            objectActivated = false;
        }
    }

}
