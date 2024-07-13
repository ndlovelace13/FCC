using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FilterDrop : MonoBehaviour
{
    [SerializeField] GameObject togglePrefab;
    [SerializeField] List<Toggle> options = new List<Toggle>();
    [SerializeField] bool flowerTypes = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Toggle> GetOptions()
    {
        if (flowerTypes)
        {
            AssignFlowers();
        }
        return options;
    }

    private void AssignFlowers()
    {
        togglePrefab = GetComponentInChildren<Toggle>().gameObject;
        List<string> discovered = GameControl.PlayerData.allDiscovered;
        foreach (var flower in discovered)
        {
            GameObject toggle = Instantiate(togglePrefab);
            toggle.transform.SetParent(transform, true);
            toggle.transform.localScale = Vector3.one;
            toggle.GetComponentInChildren<TMP_Text>().text = GameControl.PlayerData.flowerStatsDict[flower].GetTitle();
            toggle.GetComponentInChildren<FlowerToggle>().type = flower;
            //toggle.GetComponent<Toggle>().isOn = true;
            options.Add(toggle.GetComponent<Toggle>());
            toggle.SetActive(false);
        }
        Destroy(togglePrefab);
    }

    public void Expand()
    {
        foreach (var toggle in options)
        {
            toggle.gameObject.SetActive(true);
        }
        Canvas.ForceUpdateCanvases();
        transform.parent.GetComponent<VerticalLayoutGroup>().enabled = false;
        transform.parent.GetComponent<ContentSizeFitter>().enabled = false;
        transform.parent.GetComponent<VerticalLayoutGroup>().enabled = true;
        transform.parent.GetComponent<ContentSizeFitter>().enabled = true;

        foreach (var toggle in options)
        {
            //toggle.gameObject.SetActive(true);
            Vector3 currentPos = toggle.GetComponent<RectTransform>().position;
            Debug.Log(currentPos);
            Vector3 anchoredPos = toggle.GetComponent<RectTransform>().anchoredPosition;
            Debug.Log(anchoredPos);
            toggle.GetComponent<RectTransform>().localPosition = new Vector3(currentPos.x, currentPos.y, 0);
        }
    }

    public void Shrink()
    {
        foreach (var toggle in options)
        {
            toggle.gameObject.SetActive(false);
        }
    }

}
