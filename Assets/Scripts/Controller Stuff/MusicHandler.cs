using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event menuMusic;
    [SerializeField] AK.Wwise.Event gameplayMusic;

    [SerializeField] AK.Wwise.Event menuMusicStop;
    [SerializeField] AK.Wwise.Event gameplayMusicStop;

    // Start is called before the first frame update
    void Start()
    {
        //MenuMusicStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MenuMusicStart()
    {
        GameplayMusicStop();
        menuMusic.Post(gameObject);
    }

    public void GameplayMusicStart()
    {
        MenuMusicStop();
        gameplayMusic.Post(gameObject);
    }

    public void MenuMusicStop()
    {
        menuMusicStop.Post(gameObject);
    }

    public void GameplayMusicStop()
    {
        gameplayMusicStop.Post(gameObject);
    }
}
