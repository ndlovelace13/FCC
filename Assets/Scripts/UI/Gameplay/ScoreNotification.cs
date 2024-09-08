using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreNotification : MonoBehaviour
{
    public TMP_Text feedText;
    List<string> newNotifs;
    string notifications;
    Color ogColor;

    [SerializeField] bool crownAnnounce = false;
    // Start is called before the first frame update
    void Start()
    {
        newNotifs = new List<string>();
        ogColor = feedText.color;
    }

    // Update is called once per frame
    void Update()
    {
        notifications = "";
        foreach (string notif in newNotifs)
        {
            notifications += notif + "\n";
        }
        if (GameControl.PlayerData.repellentMode && crownAnnounce)
            feedText.text = "Use a Replicant Repellent!";
        else if (!GameControl.PlayerData.gameOver)
            feedText.text = notifications;
        else
            feedText.text = "";
    }

    public void newFeed(string newMessage)
    {
        newNotifs.Add(newMessage);
        feedText.color = ogColor;
        StartCoroutine("removeFeed");
    }

    public void newFeed(string newMessage, int score)
    {
        newMessage += "<color=\"green\">+" + string.Format("{0:C}", score / 100f);
        newNotifs.Add(newMessage);
        feedText.color = ogColor;
        StartCoroutine("removeFeed");
    }
    
    public void newFeed(string newMessage, Color newColor)
    {
        newNotifs.Add(newMessage);
        feedText.color = newColor;
        StartCoroutine("removeFeed");
    }

    IEnumerator removeFeed()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("removing from Feed");
        newNotifs.RemoveAt(0);
    }
}
