using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenBehavior : MonoBehaviour
{
    [SerializeField] TMP_Text balanceInfo;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        StartCoroutine(BalanceUpdate());
    }

    // Update is called once per frame
    void Update()
    {
        balanceInfo.text = "PAYOUT\n" + "Profit: +" + string.Format("{0:C}", GameControl.PlayerData.score / 100f) + "\nBalance: " + string.Format("{0:C}", GameControl.PlayerData.balance);
    }

    void OnGUI()
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
    }

    public void LinkToForm()
    {
        Application.OpenURL("https://forms.gle/LkHsuFDq4exfR15r6");
    }

    public void CatalogLoad()
    {
        SceneManager.LoadScene("Catalog");
    }

    IEnumerator BalanceUpdate()
    {
        float oldBalance = GameControl.PlayerData.balance;
        float newScore = GameControl.PlayerData.score;
        newScore /= 100;
        GameControl.PlayerData.balance += newScore;
        yield return null;
    }

}
