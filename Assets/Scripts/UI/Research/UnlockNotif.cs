using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockNotif : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] TMP_Text announceText;
    [SerializeField] TMP_Text unlockText;
    [SerializeField] Image unlockImage;

    float notifWait = 2f;
    float lerpTime = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        //BeginNotif(GameControl.PlayerData.flowerSprites[0], "this is a test");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginNotif(Sprite unlock, string text)
    {
        unlockImage.sprite = unlock;
        unlockText.text = text;
        StartCoroutine(BackgroundFade());
        StartCoroutine(LerpUp());
    }

    IEnumerator BackgroundFade()
    {
        float maxAlpha = background.color.a;
        background.color = new Color(0, 0, 0, 0);
        float time = 0f;
        float alpha;
        while (time < lerpTime)
        {
            alpha = Mathf.Lerp(0f, maxAlpha, time / lerpTime);
            background.color = new Color(0, 0, 0, alpha);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(notifWait);
        time = 0f;
        while (time < lerpTime)
        {
            alpha = Mathf.Lerp(maxAlpha, 0f, time / lerpTime);
            background.color = new Color(0, 0, 0, alpha);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
        yield return null;
    }

    IEnumerator LerpUp()
    {
        //positions to end up at
        Vector2 announceLoc = announceText.GetComponent<RectTransform>().position;
        Vector2 imageLoc = unlockImage.GetComponent<RectTransform>().position;
        Vector2 unlockLoc = unlockText.GetComponent<RectTransform>().position;

        //starting positions
        Vector2 announceStart = announceLoc + new Vector2(0, 500f);
        Vector2 imageStart = imageLoc - new Vector2(0, 500f);
        Vector2 unlockStart = unlockLoc - new Vector2(0, 500f);

        float time = 0f;
        while (time < lerpTime)
        {
            announceText.GetComponent<RectTransform>().position = Vector2.Lerp(announceStart, announceLoc, time / lerpTime);
            unlockImage.GetComponent<RectTransform>().position = Vector2.Lerp(imageStart, imageLoc, time / lerpTime);
            unlockText.GetComponent<RectTransform>().position = Vector2.Lerp(unlockStart, unlockLoc, time / lerpTime);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(notifWait);
        time = 0f;
        while (time < lerpTime)
        {
            announceText.GetComponent<RectTransform>().position = Vector2.Lerp(announceLoc, announceStart, time / lerpTime);
            unlockImage.GetComponent<RectTransform>().position = Vector2.Lerp(imageLoc, imageStart, time / lerpTime);
            unlockText.GetComponent<RectTransform>().position = Vector2.Lerp(unlockLoc, unlockStart, time / lerpTime);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
