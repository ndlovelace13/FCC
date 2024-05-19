using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotBehavior : MonoBehaviour
{
    public bool slotFull;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "slotEmpty";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
