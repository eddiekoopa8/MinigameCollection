using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Modified PlayerMovement_2

public class WeaponMovement : BB.PhysicsObject
{
    static float MOVE_SPEED = 16f;
    
    float speed;
    Core.SimpleCollisionListener collider;
    
    public override void ActorStart()
    {
        collider = GetComponent<Core.SimpleCollisionListener>();
        speed = MOVE_SPEED;
    }

    public override void ActorUpdate()
    {
        rigidbody.velocity = new Vector2(speed, MOVE_SPEED/1.5f);

        // zigzag pattern
        if (/*isLeft*/ collider.Has("Left"))
        {
            speed = MOVE_SPEED;
        }
        else if (/*isRight*/ collider.Has("Right"))
        {
            speed = -MOVE_SPEED;
        }
    }
    
    public void ResetToPosition(Vector3 pos, int direction = 1)
    {
        // Only reset position  if it's offscreen
        if (transform.position.y >= 16) transform.position = pos;
        speed = MOVE_SPEED * direction;
    }
}
