using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _06_Spotlight : MGManager
{
    Collider2D spotlight;
    Rigidbody2D guy;

    enum XDIRECTION
    {
        LEFT,
        RIGHT
    }; XDIRECTION guyXDirection;

    enum YDIRECTION
    {
        UP,
        DOWN
    }; YDIRECTION guyYDirection;

    public override void MGStart()
    {
        guyXDirection = XDIRECTION.LEFT;
        guyYDirection = YDIRECTION.DOWN;
        spotlight = GameObject.Find("spotlight").GetComponent<Collider2D>();
        guy = GameObject.Find("guy").GetComponent<Rigidbody2D>();
        GameObject.Find("spotlight").SetActive(true);
    }

    // Update is called once per frame
    public override void MGUpdate()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        Vector3 pos = Input.mousePosition;
        pos.z += 1000;
        spotlight.gameObject.transform.position = Camera.main.ScreenToWorldPoint(pos);
    }
}
