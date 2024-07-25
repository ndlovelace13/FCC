using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Page : MonoBehaviour
{
    //public GameObject gameObj;
    // Start is called before the first frame update
    [SerializeField] protected TMP_Text title;
    [SerializeField] protected TMP_Text subheader;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void FillPage()
    {

    }
}
