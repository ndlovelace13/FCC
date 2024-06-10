using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Research : MonoBehaviour
{
    public int requiredSeeds;
    public int maxResearchTimes;

    public int currentResearchTimes = 0;
    public int currentSeeds = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void ResearchAction()
    {

    }
}
