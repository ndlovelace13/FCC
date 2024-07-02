using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HomebaseCam : MonoBehaviour
{
    Camera mainCam;
    [SerializeField] SpriteRenderer carSprite;
    [SerializeField] Sprite carMenu;
    [SerializeField] GameObject playerSprite;
    [SerializeField] GameObject report;
    public bool menuActive = false;

    //menu options
    [SerializeField] GameObject catalog;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = GetComponent<Camera>();
        if (GameControl.PlayerData.shiftJustEnded)
            StartCoroutine(InitialMove());
        else
            StartCoroutine(MenuInit());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator InitialMove()
    {
        yield return new WaitForSeconds(2f);
        float time = 0f;
        float startingSize = mainCam.orthographicSize;
        Vector3 startingPos = transform.position;
        while (time < 3f)
        {
            mainCam.orthographicSize = Mathf.Lerp(startingSize, 10, time / 3f);
            transform.position = Vector3.Lerp(startingPos, new Vector3(0, 0, -10), time / 3f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }    
        yield return new WaitForSeconds(1f);
        GameControl.PlayerData.shiftJustEnded = false;
        StartCoroutine(MenuInit());
    }

    IEnumerator MenuInit()
    {
        mainCam.orthographicSize = 10;
        transform.position = new Vector3(0, 0, -10);
        menuActive = true;
        carSprite.sprite = carMenu;
        playerSprite.SetActive(false);
        report.SetActive(true);

        //activate the other menus once they are unlocked
        if (GameControl.SaveData.catalogUnlocked)
            catalog.SetActive(true);
        yield return null;
    }
}
