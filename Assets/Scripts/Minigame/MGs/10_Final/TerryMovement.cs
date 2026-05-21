using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I am NEVER touching this EVER again
// This might be THE worst thing that I have written.
// Hours of bugs, unity janks.
// I don't care about quality at this point. Quantity is enough for my sanity.

public class TerryMovement : BB.PhysicsObject
{
    static float MOVE_SPEED = 12f;
    static float DEATH_Y_POS = -18f;
    
    float speed;
    
    bool charging;
    bool chargeGoBack;
    bool damaged;
    bool dead;
    
    Core.Timer timer;
    Core.Timer invTimer;
    
    Transform followTransform;
    Vector3 targetPosition;
    
    Core.SimpleCollisionListener collider;
    SpriteRenderer render;
    
    int health = 4;
    
    public void SetTransform(Transform myTransform)
    {
        followTransform = myTransform;
    }
    
    public override void ActorStart()
    {
        collider = GetComponent<Core.SimpleCollisionListener>();
        speed = MOVE_SPEED;
        charging = false;
        chargeGoBack = false;
        timer = new Core.Timer();
        invTimer = new Core.Timer();
        timer.SetMaximumInMilliseconds(3250);
        invTimer.SetMaximumInMilliseconds(3000);
        SetTransform(GameObject.Find("Player").transform);
        damaged = false;
        dead = false;
        invTimer.Reset();
        render = GetComponent<SpriteRenderer>();
    }
    
    public bool CanDamage()
    {
        return !damaged && !dead;
    }

    public override void ActorUpdate()
    {
        if (health <= 0)
        {
            dead = true;
        }
        if (dead)
        {
            render.enabled = !render.enabled;
            rigidbody.velocity = new Vector2(0, -8);
            if (transform.position.y <= DEATH_Y_POS)
            {
                Destroy(gameObject);
            }
            return;
        }
        if ((collider.Has("Weapon") || Input.GetKeyDown(KeyCode.D)) && !damaged)
        {
            Debug.Log("damage");
            health--;
            damaged = true;
            chargeGoBack = true;
            charging = false;
        }
        if (damaged)
        {
            render.enabled = !render.enabled;
            rigidbody.velocity = Vector2.zero;
            invTimer.Tick();
            //timer.Tick();
            if (invTimer.Reached)
            {
                invTimer.Reset();
                damaged = false;
                render.enabled = true;

                chargeGoBack = false;
                charging = true;
                timer.Reset();
            }
        }
        //charging = Input.GetKey(KeyCode.C) && transform.gameObject.enabled;
        if (charging)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 0.28f);
            if (Mathf.Abs(targetPosition.sqrMagnitude - transform.position.sqrMagnitude) < 0.01)
            {
                charging = false;
                chargeGoBack = true;
            }
        }
        else if (chargeGoBack)
        {
            rigidbody.velocity = new Vector2(0, 8);
            if (transform.position.y >= 6)
            {
                chargeGoBack = false;
            }
        }
        else if (!damaged)
        {
            timer.Tick();
            rigidbody.velocity = new Vector2(speed, 0);
            if (isLeft)
            {
                speed = MOVE_SPEED;
            }
            else if (isRight)
            {
                speed = -MOVE_SPEED;
            }
            
            if (timer.Reached)
            {
                timer.Reset();
                if (followTransform != null) targetPosition = followTransform.position;
                chargeGoBack = false;
                charging = true;
            }
        }
    }
}
