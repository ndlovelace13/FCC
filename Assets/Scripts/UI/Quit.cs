using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour
{
    [SerializeField] GameObject quitPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameControl.PlayerData == null)
        {
            if (GameControl.PlayerData.quitCooldown)
                StartCoroutine(QuitCooldown());
            else
            {
                if (Input.GetKey(KeyCode.Escape))
                {
                    GameObject quitPrefab = GameObject.FindWithTag("quitMenu");
                    if (Time.timeScale != 0)
                        QuitPopup();
                }
            }
        }
    }

    public void QuitPopup()
    {
        if (Time.timeScale != 0)
        {
            Time.timeScale = 0;
            Instantiate(quitPrefab);
        }
    }

    IEnumerator QuitCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        GameControl.PlayerData.quitCooldown = false;
    }
}
