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
        GameObject blackout = Instantiate(GameControl.PlayerData.BlackoutPrefab);
        blackout.GetComponent<BlackoutBehavior>().BeginBlackout("", " ", "Tutorial", 1.5f);
    }

    public void NextShift()
    {
        GameObject blackout = Instantiate(GameControl.PlayerData.BlackoutPrefab);
        //stop music and play car sound
        GameObject.FindWithTag("music").GetComponent<MusicHandler>().MenuMusicStop();

        blackout.GetComponent<BlackoutBehavior>().BeginBlackout("", " ", "Gameplay", 2f);
    }
}
