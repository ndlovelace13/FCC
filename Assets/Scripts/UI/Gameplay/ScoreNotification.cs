using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreNotification : MonoBehaviour
{
    public TMP_Text feedText;
    List<string> newNotifs;
    string notifications;
    // Start is called before the first frame update
    void Start()
    {
        newNotifs = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        notifications = "";
        foreach (string notif in newNotifs)
        {
            notifications += notif + "\n";
        }
        feedText.text = notifications;
    }
    
    public void newFeed(string newMessage)
    {
        newNotifs.Add(newMessage);
        StartCoroutine("removeFeed");
    }

    IEnumerator removeFeed()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("removing from Feed");
        newNotifs.RemoveAt(0);
    }
}
