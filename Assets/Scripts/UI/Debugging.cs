using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Debugging : MonoBehaviour
{
    TMP_Text info;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        info = GetComponent<TMP_Text>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        info.text = player.transform.position.ToString();
    }
}
