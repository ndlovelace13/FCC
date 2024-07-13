using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CompletionNotif : MonoBehaviour
{
    TMP_Text completionNotif;
    // Start is called before the first frame update
    void Start()
    {
        completionNotif = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameControl.SaveData.newDiscoveries > 0)
            completionNotif.text = "<size=300%><color=\"green\">" + GameControl.SaveData.newDiscoveries + "</size></color>\nNew Discoveries";
        else
            completionNotif.text = "";
    }
}
