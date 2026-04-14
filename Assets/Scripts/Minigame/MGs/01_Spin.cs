using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class _01_Spin : MGManager
{
    Transform WheelRotate;
    Collider2D InputCollision;
    Collider2D StopCollision;

    SpriteRenderer NormalExp;
    SpriteRenderer FailExp;
    SpriteRenderer SuccessExp;

    bool Stop;
    public override void MGStart()
    {
        WheelRotate = GameObject.Find("WheelTarget").transform;
        InputCollision = GameObject.Find("WheelTarget").GetComponent<BoxCollider2D>();
        StopCollision = GameObject.Find("StopTarget").GetComponent<BoxCollider2D>();

        NormalExp = GameObject.Find("WheelExpressionNormal").GetComponent<SpriteRenderer>();
        FailExp = GameObject.Find("WheelExpressionFail").GetComponent<SpriteRenderer>();
        SuccessExp = GameObject.Find("WheelExpressionSuccess").GetComponent<SpriteRenderer>();

        Stop = false;
    }

    public override void MGUpdate()
    {
        if (WonOrLost)
        {
            return;
        }

        bool touching = InputCollision.IsTouching(StopCollision);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("STOP!!");
            Stop = true;
        }

        NormalExp.enabled = !Stop;
        FailExp.enabled = !touching && Stop;
        SuccessExp.enabled = touching && Stop;

        if (Stop)
        {
            if (touching)
            {
                Won();
            }
            else
            {
                Lost();
            }
        }
        else
        { 
            WheelRotate.Rotate(new Vector3(0, 0, 3));
        }
    }
}
