using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement_2 : BB.PhysicsObject
{
    static float speed = 15f;
    static float jumpspeed = 25.5f;

    public bool AllowXMovement = false;
    public bool AllowYMovement = false;

    public override void ActorUpdate()
    {
        Debug.Log("am ");
        if (Input.GetKey(KeyCode.LeftArrow) && !isLeft && AllowXMovement)
        {
            rigidbody.velocity = new Vector2(-speed, rigidbody.velocity.y);
        }
        else if (Input.GetKey(KeyCode.RightArrow) && !isRight && AllowXMovement)
        {
            rigidbody.velocity = new Vector2(speed, rigidbody.velocity.y);
        }
        else
        {
            rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && AllowYMovement)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpspeed);
        }
    }

}
