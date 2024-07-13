using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] CrownThrowing throwing;
    [SerializeField] CrownConstruction construction;
    [SerializeField] FlowerHarvest harvest;
    [SerializeField] PlayerMovement movement;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameControl.PlayerData.tutorialActive && !GameControl.PlayerData.loading)
        {
            if (GameControl.PlayerData.gameOver || GameControl.PlayerData.gamePaused)
            {
                throwing.enabled = false;
                construction.enabled = false;
                harvest.enabled = false;
                movement.enabled = false;
            }
            else if (!GameControl.PlayerData.gameOver && !GameControl.PlayerData.gamePaused)
            {
                throwing.enabled = true;
                construction.enabled = true;
                harvest.enabled = true;
                movement.enabled = true;
            }
        }
        
    }
}
