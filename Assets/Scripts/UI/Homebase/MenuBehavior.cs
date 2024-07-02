using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour
{
    [SerializeField] string menuName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        //Grow on hover, maybe add outline
        //StartCoroutine()
    }

    private void OnMouseDown()
    {
        SceneManager.LoadScene(menuName);
    }
}
