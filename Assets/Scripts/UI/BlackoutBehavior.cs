using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BlackoutBehavior : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] TMP_Text topText;
    [SerializeField] TMP_Text bottomText;
    float blackoutLength;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginBlackout(string text1, string text2, string nextScene, float time)
    {
        topText.text = text1;
        bottomText.text = text2;
        blackoutLength = time;
        StartCoroutine(BlackingOut(nextScene));
    }

    IEnumerator BlackingOut(string nextScene)
    {
        float alpha = 0f;
        float time = 0f;
        while (time < blackoutLength)
        {
            alpha = Mathf.Lerp(0f, 1f, time / blackoutLength);
            background.color = new Color(0, 0, 0, alpha);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(nextScene);
    }
}
