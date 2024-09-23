using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catalog : Paginator
{
    // Start is called before the first frame update
    public override void Start()
    {
        referencePages = GameControl.PlayerData.catalogPages;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
