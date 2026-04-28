using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class _03_Tennis : MGManager
{
    Rigidbody2D body;
    SpriteRenderer sprite;
    Rigidbody2D[] enemies;
    static int ENEMY_COUNT = 3;
    public override void MGStart()
    {
        body = GameObject.Find("Main").GetComponent<Rigidbody2D>();
        sprite = GameObject.Find("Main").GetComponent<SpriteRenderer>();
        enemies = new Rigidbody2D[ENEMY_COUNT];
        for (int i = 0; i < ENEMY_COUNT; i++)
        {
            enemies[i] = GameObject.Find("BadGuy"+(i+1)).GetComponent<Rigidbody2D>();
        }
    }

    static int XSPEED = 20;
    static int YSPEED = 40;

    public override void MGUpdate()
    {
        foreach (Rigidbody2D enemy in enemies) {
            if (body.velocity.y == 0 || body.IsTouching(enemy.gameObject.GetComponent<Collider2D>()))
            {

                Vector3 v = body.velocity;
                v.y = YSPEED;
                body.velocity = v;
                //body.velocity = Vector2.up * 40;
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 v = body.velocity;
            v.x = -XSPEED;
            body.velocity = v;
            //body.velocity = Vector2.left * 10;
            sprite.flipX = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 v = body.velocity;
            v.x = XSPEED;
            body.velocity = v;
            //body.velocity = Vector2.right * 10;
            sprite.flipX = false;
        }
        else
        {
            Vector3 v = body.velocity;
            v.x = 0;
            body.velocity = v;
        }

        foreach (Rigidbody2D enemy in enemies)
        {
            if (enemy.velocity.y == 0)
            {
                enemy.velocity = Vector2.up * 20;
            }
        }
    }
}
