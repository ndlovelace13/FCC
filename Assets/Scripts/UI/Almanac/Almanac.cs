using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Almanac : Paginator
{
    // Start is called before the first frame update
    public override void Start()
    {
        referencePages = GameControl.PlayerData.almanacPages;
        base.Start();
    }
}
