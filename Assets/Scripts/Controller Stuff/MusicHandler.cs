using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    public static MusicHandler MusicControl;

    [SerializeField] AK.Wwise.Event titleMusic;
    [SerializeField] AK.Wwise.Event titleMusicStop;


    [SerializeField] AK.Wwise.Event menuMusic;
    [SerializeField] AK.Wwise.Event gameplayMusic;

    [SerializeField] AK.Wwise.Event menuMusicStop;
    [SerializeField] AK.Wwise.Event gameplayMusicStop;

    public bool songPlaying = false;

    // Start is called before the first frame update
    private void Awake()
    {
        if (MusicControl == null)
        {
            MusicControl = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TitleMusicStart()
    {
        titleMusic.Post(gameObject);
        songPlaying = true;
    }

    public void TitleMusicStop()
    {
        titleMusicStop.Post(gameObject);
        songPlaying = false;
    }


    public void MenuMusicStart()
    {
        GameplayMusicStop();
        menuMusic.Post(gameObject);
        songPlaying = true;
    }

    public void GameplayMusicStart()
    {
        MenuMusicStop();
        gameplayMusic.Post(gameObject);
        songPlaying = true;
    }

    public void MenuMusicStop()
    {
        menuMusicStop.Post(gameObject);
        songPlaying = false;
    }

    public void GameplayMusicStop()
    {
        gameplayMusicStop.Post(gameObject);
        songPlaying = false;
    }
}
