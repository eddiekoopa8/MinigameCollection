using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _07_Coin : MGManager
{
    Rigidbody2D guy;
    SimpleCollisionListener guyCollider;

    enum XDIRECTION
    {
        NONE = 0,
        LEFT = -1,
        RIGHT = 1
    }; XDIRECTION guyXDirection;

    enum YDIRECTION
    {
        NONE = 0,
        UP = 1,
        DOWN = -1
    } YDIRECTION guyYDirection;

    static int COIN_COUNT = 50;
    int coins = 0;

    public override void MGStart()
    {
        coins = 0;
        guyXDirection = XDIRECTION.NONE;
        guyYDirection = YDIRECTION.NONE;
        guy = GameObject.Find("guy").GetComponent<Rigidbody2D>();
        guyCollider = GameObject.Find("guy").GetComponent<SimpleCollisionListener>();
        StartAsLost();
    }

    // Update is called once per frame
    public override void MGUpdate()
    {
        if (coins >= COIN_COUNT)
        {
            WonEndMG();
        }

        if (WonOrLost)
        {
            return;
        }
        guy.transform.SetPositionAndRotation(guy.transform.position + (Vector3.up * (float)((int)guyYDirection) / 2), guy.transform.rotation);
        guy.transform.SetPositionAndRotation(guy.transform.position + (Vector3.right * (float)((int)guyXDirection) / 2), guy.transform.rotation);

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            guyYDirection = YDIRECTION.UP;
            guyXDirection = XDIRECTION.NONE;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            guyYDirection = YDIRECTION.DOWN;
            guyXDirection = XDIRECTION.NONE;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            guyYDirection = YDIRECTION.NONE;
            guyXDirection = XDIRECTION.LEFT;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            guyYDirection = YDIRECTION.NONE;
            guyXDirection = XDIRECTION.RIGHT;
        }
        guy.gameObject.GetComponent<SpriteRenderer>().flipX = guyXDirection == XDIRECTION.RIGHT ? true : false;

        if (guyCollider.Has("Coin"))
        {
            Destroy(guyCollider.GetTriggered());
            coins++;
        }
    }
}
