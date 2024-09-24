using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchPaginator : Paginator
{
    // Start is called before the first frame update
    public override void Start()
    {
        referencePages = GameControl.PlayerData.researchPages;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
