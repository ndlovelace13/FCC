using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogoFade : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject logo;
    Image logoImage;
    float currentAlpha = 0f;
    void Start()
    {
        PlayerPrefs.SetInt("firstRun", 0);
        logoImage = logo.GetComponent<Image>();
        StartCoroutine(LogoControl());
    }

    // Update is called once per frame
    void Update()
    {
        logoImage.color = new Color(logoImage.color.r, logoImage.color.g, logoImage.color.b, currentAlpha);
    }

    IEnumerator LogoControl()
    {
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            logoIn();
        }
        yield return new WaitForSeconds(2);
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            logoFade();
        }
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("TitleScreen");
    }

    private void logoFade()
    {
        currentAlpha -= 0.01f;
    }

    private void logoIn()
    {
        currentAlpha += 0.01f;
    }
}
