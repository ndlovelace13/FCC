using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNotif : MonoBehaviour
{
    [SerializeField] TMP_Text display;
    Color color;
    string displayedText;
    float notifTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("displaying" + displayedText);
        StartCoroutine("Destroy");
        transform.localScale = Vector3.one * 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(transform.position);
        transform.position += Vector3.up * 0.001f;
    }

    private void OnGUI()
    {
        display.color = color;
        display.text = displayedText;
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(notifTime);
        Destroy(gameObject);
    }

    public void Creation(string text, Color display)
    {
        color = display;
        displayedText = text;
    }
}
