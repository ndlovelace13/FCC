using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairPlacement : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform crosshair;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 realMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        /*Vector2 playerPos = player.position;
        Vector2 direction = realMousePos - playerPos;
        direction.Normalize();
        crosshair.position = playerPos + direction * 4;*/
        crosshair.position = realMousePos;

    }
}
