using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    public GameObject Collided;
    public GameObject Triggered;

    void OnCollisionEnter2D(Collision2D c)
    {
        Collided = c.gameObject;
    }

    void OnCollisionExit2D(Collision2D c)
    {
        Collided = null;
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        Triggered = c.gameObject;
    }

    void OnTriggerExit2D(Collider2D c)
    {
        Triggered = null;
    }
}
