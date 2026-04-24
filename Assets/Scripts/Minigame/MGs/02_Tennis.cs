using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class _02_Tennis : MGManager
{
    Rigidbody ball;
    Rigidbody player1;
    Rigidbody player2;
    Animator player1Anim;
    Animator player2Anim;
    public override void MGStart()
    {
        player1 = GameObject.Find("Player1").GetComponent<Rigidbody>();
        player2 = GameObject.Find("Player2").GetComponent<Rigidbody>();
        player1Anim = GameObject.Find("Player1").GetComponent<Animator>();
        player2Anim = GameObject.Find("Player2").GetComponent<Animator>();
        ball = GameObject.Find("Ball").GetComponent<Rigidbody>();

        Vector3 v = ball.velocity;
        v.z = -5;
        v.y = 30;
        ball.velocity = v;
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
            Vector3 v = player1.velocity;
            v.x = -SPEED;
            player1.velocity = v;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            player1Anim.Play("Move");
            Vector3 v = player1.velocity;
            v.x = SPEED;
            player1.velocity = v;
        }
        else
        {
            player1Anim.Play("Idle");
            player1.velocity = Vector3.zero;
        }
    }
}
