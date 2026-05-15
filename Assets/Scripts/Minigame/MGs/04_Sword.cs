using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

// inspired by some submission in devil may cry

public class _04_Sword : MGManager
{
    GameObject sword;
    GameObject player;
    Animator playerAnim;

    Rigidbody2D[] enemies;
    static int ENEMY_COUNT = 4;
    int killCount = 0;

    enum STT_PLAYER
    {
        IDLE,
        WALK,
        ATTACK,
        ATTACKING,
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
        sword = GameObject.Find("Sword");
        playerState = STT_PLAYER.IDLE;
        playerDirection = DIRECTION.LEFT;

        playerAnim = player.GetComponent<Animator>();

        enemies = new Rigidbody2D[ENEMY_COUNT];
        for (int i = 0; i < ENEMY_COUNT; i++)
        {
            enemies[i] = GameObject.Find("Skull" + (i)).GetComponent<Rigidbody2D>();
        }
    }

    static float MOVE_SPEED = 1.75f;

    public override void MGUpdate()
    {
        if (killCount >= ENEMY_COUNT)
        {
            WonEndMG();
        }
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
                    playerState = STT_PLAYER.ATTACKING;
                    break;
                }
            case STT_PLAYER.ATTACKING:
                {
                    // did attack transition to walk?
                    if (!playerAnim.GetCurrentAnimatorStateInfo(0).IsName("attack"))
                    {
                        playerState = STT_PLAYER.IDLE;
                    }
                    break;
                }
            case STT_PLAYER.DEAD:
                {
                    playerAnim.Play("explode");
                    player.transform.SetPositionAndRotation(player.transform.position + (Vector3.down * 0.225f), player.transform.rotation);
                    return;
                }
        }
        
        if (playerState != STT_PLAYER.ATTACK && playerState != STT_PLAYER.ATTACKING)
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

        foreach (Rigidbody2D enemy in enemies)
        {
            if (enemy.IsDestroyed() || !enemy)
            {
                continue;
            }
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, GameObject.FindWithTag("Player").transform.position, 0.10f);

            if (enemy.IsTouching(player.GetComponent<Collider2D>()))
            {
                playerState = STT_PLAYER.DEAD;
                LostEndMG();
            }
            else if (enemy.IsTouching(sword.GetComponent<Collider2D>()) && playerState == STT_PLAYER.ATTACKING)
            {
                Destroy(enemy.gameObject);
                killCount++;
                continue;
            }
        }
    }
}
