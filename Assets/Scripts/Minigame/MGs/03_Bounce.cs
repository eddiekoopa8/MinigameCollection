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
    int killed = 0;
    public override void MGStart()
    {
        body = GameObject.Find("Main").GetComponent<Rigidbody2D>();
        sprite = GameObject.Find("Main").GetComponent<SpriteRenderer>();
        enemies = new Rigidbody2D[ENEMY_COUNT];

        // Get enemies
        for (int i = 0; i < ENEMY_COUNT; i++)
        {
            enemies[i] = GameObject.Find("BadGuy"+(i+1/*why did i do this*/)).GetComponent<Rigidbody2D>();
        }
        killed = 0;
    }

    static int XSPEED = 20;
    static int YSPEED = 40;

    public override void MGUpdate()
    {
        foreach (Rigidbody2D enemy in enemies) {
            if (enemy.IsDestroyed() || !enemy)
            {
                continue;
            }
            // If we bounced off something solid.
            if (body.velocity.y == 0 || body.IsTouching(enemy.gameObject.GetComponent<Collider2D>()))
            {
            // Bounce!
                Vector3 v = body.velocity;
                v.y = YSPEED;
                body.velocity = v;
                //body.velocity = Vector2.up * 40;

                // If it was an enemy
                if (body.IsTouching(enemy.gameObject.GetComponent<Collider2D>()))
                {
                    // We got one!
                    Destroy(enemy.gameObject);
                    PlayMGWorldSound("EnemyHit");
                    killed++;
                }
            }
        }

        // Move with arrow keys
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
        // Not moving
        else
        {
            Vector3 v = body.velocity;
            v.x = 0;
            body.velocity = v;
        }

        // Enemy animations
        foreach (Rigidbody2D enemy in enemies)
        {
            // Null check
            if (enemy.IsDestroyed() || !enemy)
            {
                continue;
            }
            if (enemy.velocity.y == 0)
            {
                enemy.velocity = Vector2.up * 20;
            }
        }

        // We won if we killed them all.
        if (killed >= ENEMY_COUNT)
        {
            WonEndMG();
        }
    }
}
