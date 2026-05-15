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
    public override void MGStart()
    {
        StartAsWon();

        player1 = GameObject.Find("Player1").GetComponent<Rigidbody>();
        player1Collider = GameObject.Find("Player1").GetComponent<SimpleCollisionListener3D>();
        player1Anim = GameObject.Find("Player1").GetComponent<Animator>();

        player2 = GameObject.Find("Player2").GetComponent<Rigidbody>();
        player2Collider = GameObject.Find("Player1").GetComponent<SimpleCollisionListener3D>();
        player2Anim = GameObject.Find("Player2").GetComponent<Animator>();

        ball = GameObject.Find("Ball").GetComponent<Rigidbody>();
        ballCollider = GameObject.Find("Ball").GetComponent<SimpleCollisionListener3D>();
    }

    void SetFakePerspective(Transform transform)
    {
        float scaleVal = -(transform.position.z / 20);
        transform.localScale = new Vector3(scaleVal, scaleVal, 1);
    }

    static int SPEED = 50;

    public override void MGUpdate()
    {
        SetFakePerspective(player1.transform);
        SetFakePerspective(player2.transform);
        SetFakePerspective(ball.transform);
        
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
        else
        {
            player1Anim.Play("Idle");
            player1.velocity = Vector3.zero;
        }

        if (ballCollider.Has("Player2"))
        {
            ball.transform.position = player2.transform.position;
            ball.velocity = Vector3.zero; // reset vel
        }
    }
}
