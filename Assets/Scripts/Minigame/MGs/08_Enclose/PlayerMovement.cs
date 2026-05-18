using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : BB.PhysicsObject
{
    // Start is called before the first frame update
    public override void ActorStart()
    {
    }
    static float speed = 7.5f;
    static float jumpspeed = 20.0f;
    // Update is called once per frame
    public override void ActorUpdate()
    {
        if (Input.GetKey(KeyCode.LeftArrow) && !isLeft)
        {
            rigidbody.velocity = new Vector2(-speed, rigidbody.velocity.y);
        }
        else if (Input.GetKey(KeyCode.RightArrow) && !isRight)
        {
            rigidbody.velocity = new Vector2(speed, rigidbody.velocity.y);
        }
        else
        {
            rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpspeed);
        }
    }

}
