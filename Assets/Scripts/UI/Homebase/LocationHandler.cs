using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationHandler : MonoBehaviour
{
    [SerializeField] GameObject TrainingButton;
    [SerializeField] GameObject NextShiftButton;
    // Start is called before the first frame update
    void Start()
    {
        if (GameControl.SaveData.tutorialComplete)
        {
            NextShiftButton.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void NextShift()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
