using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

// inspired by some submission in devil may cry

public class _04_Sword : MGManager
{
    GameObject player;
    Animator playerAnim;
    enum STT_PLAYER
    {
        IDLE,
        WALK,
        ATTACK,
        DEAD
    }; STT_PLAYER playerState;
    enum DIRECTION {
        LEFT,
        RIGHT
    }; DIRECTION playerDirection;

    Vector3[] directions =
    {
        Vector3.left,
        Vector3.right
    };

    // must be float otherwise some weird problems occur.
    float[] directionsScale =
    {
        -1.0f,
         1.0f
    };

    public override void MGStart()
    {
        player = GameObject.Find("Player");
        playerState = STT_PLAYER.IDLE;
        playerDirection = DIRECTION.LEFT;

        playerAnim = player.GetComponent<Animator>();
    }

    static float MOVE_SPEED = 1.75f;

    public override void MGUpdate()
    {
        player.transform.localScale = new Vector3(directionsScale[(int)playerDirection], player.transform.localScale.y, player.transform.localScale.z);
        switch (playerState)
        {
            case STT_PLAYER.IDLE:
                {
                    playerAnim.Play("idle");
                    break;
                }
            case STT_PLAYER.WALK:
                {
                    playerAnim.Play("walk");
                    player.transform.SetPositionAndRotation(player.transform.position + (directions[(int)playerDirection] * (MOVE_SPEED / 5)), player.transform.rotation);
                    break;
                }
            case STT_PLAYER.ATTACK:
                {
                    playerAnim.Play("attack");

                    // did attack transition to walk?
                    if (!playerAnim.GetCurrentAnimatorStateInfo(0).IsName("attack"))
                    {
                        playerState = STT_PLAYER.IDLE;
                    }
                    break;
                }
        }


        if (playerState != STT_PLAYER.ATTACK)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                playerState = STT_PLAYER.WALK;
                playerDirection = DIRECTION.LEFT;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                playerState = STT_PLAYER.WALK;
                playerDirection = DIRECTION.RIGHT;
            }
            else
            {
                playerState = STT_PLAYER.IDLE;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerState = STT_PLAYER.ATTACK;
        }
    }
}
