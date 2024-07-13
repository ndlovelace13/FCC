using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopup : MonoBehaviour
{
    [SerializeField] TMP_Text crownTitle;
    [SerializeField] TMP_Text crownType;
    [SerializeField] TMP_Text crownStats;
    [SerializeField] GameObject crownDisplay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void CrownChosen(Vector3 nodePos, Crown currentCrown)
    {
        gameObject.SetActive(true);
        fillStats(currentCrown);
        assignLoc(nodePos);
    }

    private void assignLoc(Vector3 nodePos)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 canvasPos = Camera.main.WorldToViewportPoint(nodePos);

        

        //reset the fit thing
        GetComponent<ContentSizeFitter>().enabled = false;
        GetComponent<VerticalLayoutGroup>().enabled = false;
        GetComponent<ContentSizeFitter>().enabled = true;
        GetComponent<VerticalLayoutGroup>().enabled = true;

        //check for the quadrant of the screen and assign the pos point accordingly
        Vector3 corner = new Vector3(rectTransform.rect.width, rectTransform.rect.height) / 2f;
        Debug.Log(corner);
        if (canvasPos.x <= 0.5 && canvasPos.y <= 0.5)
        {
            rectTransform.pivot = Vector2.zero;
        }
        else if (canvasPos.x > 0.5 && canvasPos.y <= 0.5)
        {
            rectTransform.pivot = new Vector2(1, 0);
        }
        else if (canvasPos.x <= 0.5 && canvasPos.y > 0.5)
        {
            rectTransform.pivot = new Vector2(0, 1);
        }
        else
        {
            rectTransform.pivot = Vector2.one;
        }
        Debug.Log("this is the canvasPos" + canvasPos);
        rectTransform.anchoredPosition = Vector3.zero;
        //anchor to the right spot
        rectTransform.anchorMin = canvasPos;
        rectTransform.anchorMax = canvasPos;

        //gameObject.SetActive(true);
    }

    private void fillStats(Crown currentCrown)
    {
        List<string> flowers = currentCrown.GetFlowers();
        if (currentCrown.IsDiscovered())
        {
            crownTitle.text = currentCrown.GetTitle();
            crownType.text = "Type: " + GetCrownType(currentCrown.GetStructure());
            crownStats.text = "Times Played: " + currentCrown.GetTimesCrafted() +
                            "\nDiscovered On: Shift #" + currentCrown.GetShift();
            crownDisplay.SetActive(true);
            SetFlowers(currentCrown);

        }
            
        else
        {
            crownTitle.text = "???";
            crownType.text = "Type: " + GetCrownType(currentCrown.GetStructure());
            flowers = flowers.Distinct().ToList();
            flowers.Remove("");
            flowers.OrderBy(i => Guid.NewGuid()).ToList();
            crownStats.text = "Contains:";
            foreach (var flower in flowers)
            {
                if (GameControl.PlayerData.allDiscovered.Contains(flower))
                    crownStats.text += " " + GameControl.PlayerData.flowerStatsDict[flower].GetTitle();
                else
                    crownStats.text += " undiscovered";
                if (flowers.Last() != flower)
                    crownStats.text += ",";
            }
            crownDisplay.SetActive(false);
        }
    }

    private void SetFlowers(Crown crown)
    {
        //get the desired flower types
        List<string> flowers = crown.GetFlowers();
        List<Image> unassigned = new List<Image>();
        //get the children of the crown
        Image[] flowerSprites = crownDisplay.GetComponentsInChildren<Image>();
        flowerSprites = flowerSprites.Where(child => child.gameObject.tag == "FlowerHead").ToArray();
        //assign the flower sprite if the flower is defined
        for (int i = 0; i < flowerSprites.Length; i++)
        {
            if (i == 0 || i == 4)
            {
                if (flowers[2] != "")
                    flowerSprites[i].sprite = GameControl.PlayerData.flowerStatsDict[flowers[2]].headSprite;
                else
                {
                    unassigned.Add(flowerSprites[i]);
                }
            }
            else if (i == 1 || i == 3)
            {
                if (flowers[1] != "")
                    flowerSprites[i].sprite = GameControl.PlayerData.flowerStatsDict[flowers[1]].headSprite;
                else
                {
                    unassigned.Add(flowerSprites[i]);
                }
            }
            else
                flowerSprites[i].sprite = GameControl.PlayerData.flowerStatsDict[flowers[0]].headSprite;
        }
        StartCoroutine(randomFlower(unassigned));
    }

    IEnumerator randomFlower(List<Image> sprites)
    {
        while (true)
        {
            //choose first
            int[] choices = new int[sprites.Count];
            for (int i = 0; i < choices.Length; i++)
            {
                choices[i] = UnityEngine.Random.Range(0, GameControl.PlayerData.allDiscovered.Count);
                //case for 2
                if (choices.Length == 2)
                {
                    if (i == 1)
                    {
                        while (choices[i] == choices[0])
                        {
                            choices[i] = UnityEngine.Random.Range(0, GameControl.PlayerData.allDiscovered.Count);
                        }
                    }
                }
                //case for 4
                else if (choices.Length == 4)
                {
                    if (i == 2)
                    {
                        while (choices[i] == choices[1])
                        {
                            choices[i] = UnityEngine.Random.Range(0, GameControl.PlayerData.allDiscovered.Count);
                        }
                    }
                    else if (i == 3)
                    {
                        while (choices[i] == choices[0])
                        {
                            choices[i] = UnityEngine.Random.Range(0, GameControl.PlayerData.allDiscovered.Count);
                        }
                    }
                }
            }
            //then assign
            for (int i = 0; i < sprites.Count; i++)
            {
                
                string type = GameControl.PlayerData.allDiscovered[choices[i]];
                sprites[i].sprite = GameControl.PlayerData.flowerStatsDict[type].headSprite;
            }
            yield return new WaitForSeconds(1);
        }
    }

    private string GetCrownType(crownType type)
    {
        string returnedType = "";
        switch (type)
        {
            case global::crownType.Basic: returnedType = "Basic"; break;
            case global::crownType.Advanced: returnedType = "Advanced"; break;
            case global::crownType.Three: returnedType = "Three of a Kind"; break;
            case global::crownType.Complete: returnedType = "Complete"; break;
            case global::crownType.FullHouse: returnedType = "Full House"; break;
            case global::crownType.Four: returnedType = "Four of a Kind"; break;
            case global::crownType.Fiver: returnedType = "Five of a Kind"; break;
        }
        return returnedType;
    }
}
