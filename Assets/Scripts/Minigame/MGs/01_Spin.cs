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
        // Get transform for rotation
        WheelRotate = GameObject.Find("WheelTarget").transform;

        // Get hitboxes
        InputCollision = GameObject.Find("WheelTarget").GetComponent<BoxCollider2D>();
        StopCollision = GameObject.Find("StopTarget").GetComponent<BoxCollider2D>();

        // get expressions
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

        // Either enter or space pressed
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("STOP!!");
            Stop = true;
        }

        // Enable expressions depending on state
        NormalExp.enabled = !Stop; // Normal expression if wheel is moving
        FailExp.enabled = !touching && Stop; // Sad expression if wheel has stopped and is not touching arrow
        SuccessExp.enabled = touching && Stop; // Happy expression if wheel has stopped and is touching arrow

        if (Stop)
        {
            if (touching)
            {
                // You're winner!
                PlayMGWorldSound("PickupCoin");
                WonEndMG();
            }
            else
            {
                // You lose...
                PlayMGWorldSound("HitHurt");
                LostEndMG();
            }
        }
        else
        { 
            // Rotate based on delta time
            WheelRotate.Rotate(new Vector3(0, 0, Time.deltaTime * 150));
        }
    }
}
