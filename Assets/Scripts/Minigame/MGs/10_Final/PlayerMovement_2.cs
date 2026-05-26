using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// would be better if i did a namespace
public class PlayerMovement_2 : BB.PhysicsObject
{
    // Same as PlayerMovement, but this time there are options to disable jumping and horizontal movement.
    // (TODO: replace the old PlayerMovement with this)

    static float speed = 15f;
    static float jumpspeed = 25.5f;

    public bool AllowXMovement = false;
    public bool AllowYMovement = false;

    public override void ActorUpdate()
    {
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
