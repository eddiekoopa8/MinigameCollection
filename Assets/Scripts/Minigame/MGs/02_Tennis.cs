using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Core;

public class _02_Tennis : MGManager
{
    Rigidbody ball;
    Rigidbody player1;
    Rigidbody player2;
    SimpleCollisionListener3D ballCollider;
    SimpleCollisionListener3D player1Collider;
    SimpleCollisionListener3D player2Collider;
    Animator player1Anim;
    Animator player2Anim;
    
    enum HitDirection
    {
        PLAYER1,
        PLAYER2,
    } HitDirection playerDirection;

    public override void MGStart()
    {
        // Player 1 stuff
        player1 = GameObject.Find("Player1").GetComponent<Rigidbody>();
        player1Collider = GameObject.Find("Player1").GetComponent<SimpleCollisionListener3D>();
        player1Anim = GameObject.Find("Player1").GetComponent<Animator>();

        // Player 2 stuff
        player2 = GameObject.Find("Player2").GetComponent<Rigidbody>();
        player2Collider = GameObject.Find("Player1").GetComponent<SimpleCollisionListener3D>();
        player2Anim = GameObject.Find("Player2").GetComponent<Animator>();

        // Duke, do you want the ball?
        ball = GameObject.Find("Ball").GetComponent<Rigidbody>();
        ballCollider = GameObject.Find("Ball").GetComponent<SimpleCollisionListener3D>();

        playerDirection = HitDirection.PLAYER2;

        // Set ball position to start with player 2
        ball.transform.position = player2.transform.position;
    }

    void SetFakePerspective(Transform transform)
    {
        // Bad fake 3D perspective for 2D sprites
        float scaleVal = -(transform.position.z / 20);
        transform.localScale = new Vector3(scaleVal, scaleVal, 1);
    }

    static int SPEED = 50;
    public override void MGBeforeUpdate()
    {
        // The ball falls off screen when ther minigame is not activated yet.
        // Make sure it does not happen.
        if (!MGActive)
        {
            ball.transform.position = player2.transform.position;
        }
    }
    public override void MGUpdate()
    {
        // Fake perspective for plasyer 1, 2 and ball.
        SetFakePerspective(player1.transform);
        SetFakePerspective(player2.transform);
        SetFakePerspective(ball.transform);

        // Moving player 1 with keys
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            player1Anim.Play("Move");
            player1.velocity = Vector3.left * SPEED;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            player1Anim.Play("Move");
            player1.velocity = Vector3.right * SPEED;
        }
        // Not moving
        else
        {
            player1Anim.Play("Idle");
            player1.velocity = Vector3.zero;
        }

        // if ball hit player 2
        if ((ballCollider.Has("Player2") && playerDirection == HitDirection.PLAYER2) || (DebuggingMGs && Input.GetKeyDown(KeyCode.R)))
        {
            // Set ball to the racket position
            Vector3 racketPos = player2.gameObject.FindChild("Racket").transform.position;
            ball.transform.position = new Vector3(racketPos.x, racketPos.y, racketPos.z);

            // New ball speed
            ball.velocity = new Vector3(Unityls.Rand(-7, 7), 7, -15); // new vel

            // Now make it target player 1
            playerDirection = HitDirection.PLAYER1;

            PlayMGWorldSound("EnemyHit");
            PlayMGWorldSound("Jump");
        }

        // if ball hit player 1
        if (ballCollider.Has("Player1") && playerDirection == HitDirection.PLAYER1)
        {
            // Set ball to the racket position
            Vector3 racketPos = player1.gameObject.FindChild("Racket").transform.position;
            ball.transform.position = new Vector3(racketPos.x, racketPos.y, racketPos.z);

            // New ball speed
            ball.velocity = new Vector3(Unityls.Rand(-7, 7), 11, 15); // new vel

            // Now make it target player 2
            playerDirection = HitDirection.PLAYER2;
            
            PlayMGWorldSound("EnemyHit");
            PlayMGWorldSound("Jump");
        }

        // Make ball not off screen
        if (ballCollider.Has("Right"))
        {
            ball.velocity = new Vector3(-3, ball.velocity.y, ball.velocity.z);
        }
        if (ballCollider.Has("Left"))
        {
            ball.velocity = new Vector3(3, ball.velocity.y, ball.velocity.z);
        }

        // If ball went out of the court
        if (ballCollider.Has("Lose"))
        {
            LostEndMG();
        }

        // if ball is targeting player 2
        if (playerDirection == HitDirection.PLAYER2)
        {
            // Player 2 moves to ball position (and never misses)
            Vector3 pos = Vector3.MoveTowards(player2.transform.position, ball.transform.position, 0.40f);
            Vector3 curPos = player2.transform.position;
            player2.transform.position = new Vector3(pos.x, curPos.y, curPos.z);
        }
    }
}
