using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// VERY simple player class

public class PlayerMovement : BB.PhysicsObject
{
    // Start is called before the first frame update
    public override void ActorStart()
    {
        // uh
    }
    static float speed = 7.5f;
    static float jumpspeed = 20.0f;
    // Update is called once per frame
    public override void ActorUpdate()
    {
        // left movement
        if (Input.GetKey(KeyCode.LeftArrow) && !isLeft)
        {
            rigidbody.velocity = new Vector2(-speed, rigidbody.velocity.y);
        }
        // right movement
        else if (Input.GetKey(KeyCode.RightArrow) && !isRight)
        {
            rigidbody.velocity = new Vector2(speed, rigidbody.velocity.y);
        }
        // no movement
        else
        {
            rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        }

        // jumping!
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpspeed);
        }
    }

}
