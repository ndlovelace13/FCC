using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class Almanac : MonoBehaviour
{
    //List<Page> pages;
    [SerializeField] List<GameObject> pageObjects;

    [SerializeField] int currentIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        PageInit();
        ChangePages(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            PrevPages();
        if (Input.GetKeyDown(KeyCode.RightArrow))
            NextPages();
    }

    private void PageInit()
    {
        //pages = new List<Page>(GameControl.PlayerData.almanacPages);
        pageObjects = new List<GameObject>();
        foreach (Page page in GameControl.PlayerData.almanacPages)
        {
            GameObject pageDupe = Instantiate(page.gameObject);
            pageDupe.transform.SetParent(transform);
            pageDupe.transform.localScale = Vector3.one;
            pageObjects.Add(pageDupe);
            //pageDupe.SetActive(false);
        }
    }

    public void ChangePages(int newIndex)
    {
        //disable the current pages
        pageObjects[currentIndex].SetActive(false);
        pageObjects[currentIndex + 1].SetActive(false);
        //do a page turning anim here
        if (newIndex % 2 != 0)
            currentIndex = newIndex - 1;
        else
            currentIndex = newIndex;

        //open the book up to the sticker pages - this should be the initial state
        pageObjects[currentIndex].SetActive(true);
        pageObjects[currentIndex + 1].SetActive(true);
    }

    public void NextPages()
    {
        if (currentIndex < pageObjects.Count - 2)
            ChangePages(currentIndex + 2);
    }

    public void PrevPages()
    {
        if (currentIndex > 1)
            ChangePages(currentIndex - 2);
    }
}
