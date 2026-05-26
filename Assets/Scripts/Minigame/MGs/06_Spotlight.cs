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
    
    static bool flipX = false;
    static bool flipY = false;

    public override void MGStart()
    {
        guyXDirection = !flipX ? XDIRECTION.LEFT : XDIRECTION.RIGHT;
        guyYDirection = !flipY ? YDIRECTION.DOWN : YDIRECTION.UP;
        flipX = !flipX;
        if (flipX) flipY = !flipY;
        spotlight = GameObject.Find("spotlight").GetComponent<Collider2D>();

        // get guy
        guy = GameObject.Find("guy").GetComponent<Rigidbody2D>();
        guyCollider = GameObject.Find("guy").GetComponent<SimpleCollisionListener>();
        GameObject.Find("spotlight").SetActive(true);
    }

    // Update is called once per frame
    public override void MGUpdate()
    {
        // hide cursor and lock it to game window
        // (is reverted back by MG world)

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        // Set spotlight position to mouse
        Vector3 pos = Input.mousePosition;
        pos.z += 1000; // some layer fix
        spotlight.gameObject.transform.position = Camera.main.ScreenToWorldPoint(pos);

        // If guy is in spotlight, we won!
        if (guyCollider.Has("spotlight"))
        {
            WonMG();
        }
        // Or we lose.
        else
        {
            LostMG();
        }

        // guy moves zig zag pattern
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

        // Guy moving
        guy.transform.SetPositionAndRotation(guy.transform.position + (Vector3.up * (float)((int)guyYDirection) / 5), guy.transform.rotation);
        guy.transform.SetPositionAndRotation(guy.transform.position + (Vector3.right * (float)((int)guyXDirection) / 5), guy.transform.rotation);
    }
}
