using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _06_Spotlight : MGManager
{
    Collider2D spotlight;
    Rigidbody2D guy;
    SimpleCollisionListener guyCollider;

    enum XDIRECTION
    {
        LEFT = -1,
        RIGHT = 1
    }; XDIRECTION guyXDirection;

    enum YDIRECTION
    {
        UP = 1,
        DOWN = -1
    }; YDIRECTION guyYDirection;

    public override void MGStart()
    {
        guyXDirection = XDIRECTION.LEFT;
        guyYDirection = YDIRECTION.DOWN;
        spotlight = GameObject.Find("spotlight").GetComponent<Collider2D>();
        guy = GameObject.Find("guy").GetComponent<Rigidbody2D>();
        guyCollider = GameObject.Find("guy").GetComponent<SimpleCollisionListener>();
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
        
        if (guyCollider.Has("spotlight"))
        {
            WonForce();
        }
        else
        {
            LostForce();
        }

        if (guyCollider.Has("up"))
        {
            guyYDirection = YDIRECTION.DOWN;
        }
        if (guyCollider.Has("down"))
        {
            guyYDirection = YDIRECTION.UP;
        }
        if (guyCollider.Has("left"))
        {
            guyXDirection = XDIRECTION.RIGHT;
        }
        if (guyCollider.Has("right"))
        {
            guyXDirection = XDIRECTION.LEFT;
        }
        
        guy.transform.SetPositionAndRotation(guy.transform.position + (Vector3.up * (float)((int)guyYDirection) / 5), guy.transform.rotation);
        guy.transform.SetPositionAndRotation(guy.transform.position + (Vector3.right * (float)((int)guyXDirection) / 5), guy.transform.rotation);
    }
}
