using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenBehavior : MonoBehaviour
{
    [SerializeField] TMP_Text balanceInfo;
    float scoreBonus;
    float timeBonus;
    bool displayUpdate = true;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        scoreBonus = GameControl.PlayerData.score / 100f;
        timeBonus = GameControl.PlayerData.min * 60 + GameControl.PlayerData.sec;
        timeBonus /= 100f;
        if (!GameControl.PlayerData.balanceUpdated)
            StartCoroutine(BalanceUpdate());
        else
            displayUpdate = false;
    }

    // Update is called once per frame
    void Update()
    {
        balanceInfo.text = "PAYOUT\n";
        if (displayUpdate)
            balanceInfo.text += "Score Bonus: +" + string.Format("{0:C}", scoreBonus) +
            "\nTime Bonus: +" + string.Format("{0:C}", timeBonus);
        balanceInfo.text += "\nBalance: " + string.Format("{0:C}", GameControl.PlayerData.balance) + 
            "\n\nEssence Seeds: " + GameControl.PlayerData.essenceCount;
    }

    /*void OnGUI()
    {
        Event e = Event.current;
        if (e.keyCode == KeyCode.Escape)
        {
            Application.Quit();
        }
        else if (e.keyCode == KeyCode.R)
        {
            SceneManager.LoadScene("Gameplay");
        }
        else if (e.keyCode == KeyCode.M)
        {
            SceneManager.LoadScene("TitleScreen");
        }
    }*/

    public void CatalogLoad()
    {
        SceneManager.LoadScene("Catalog");
    }

    public void ResearchLoad()
    {
        SceneManager.LoadScene("Research");
    }

    public void MenuLoad()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void NextShift()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void CompletionLoad()
    {
        SceneManager.LoadScene("CompletionTracker");
    }

    IEnumerator BalanceUpdate()
    {
        GameControl.PlayerData.balance += scoreBonus + timeBonus;
        GameControl.PlayerData.balanceUpdated = true;
        yield return null;
    }

}
