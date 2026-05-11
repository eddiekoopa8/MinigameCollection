using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SimpleCollisionListener : MonoBehaviour
    {
        List<Collider2D> Triggered;
        List<Collider2D> Collided;

        public Collider2D HasTriggered(string name)
        {
            foreach (Collider2D entry in Triggered.ToArray())
            {
                if (name == entry.name)
                {
                    return entry;
                }
            }
            return null;
        }

        public Collider2D HasCollided(string name)
        {
            foreach (Collider2D entry in Collided.ToArray())
            {
                if (name == entry.name)
                {
                    return entry;
                }
            }
            return null;
        }

        public Collider2D Has(string name)
        {
            Collider2D result = HasTriggered(name);
            if (result == null)
            {
                result = HasCollided(name);
            }
            return result;
        }

        private void Start()
        {
            Triggered = new List<Collider2D>();
            Collided = new List<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            foreach (Collider2D entry in Triggered.ToArray())
            {
                if (collider.name == entry.name)
                {
                    return;
                }
            }
            Debug.Log("touch " + collider.name);
            Triggered.Add(collider);
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            foreach (Collider2D entry in Triggered.ToArray())
            {
                if (collider.gameObject.GetInstanceID() == entry.gameObject.GetInstanceID())
                {
                    Triggered.Remove(collider);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Collider2D collider = collision.collider;
            foreach (Collider2D entry in Collided.ToArray())
            {
                if (collider.name == entry.name)
                {
                    return;
                }
            }
            Debug.Log("touch " + collider.name);
            Collided.Add(collider);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            Collider2D collider = collision.collider;
            foreach (Collider2D entry in Collided.ToArray())
            {
                if (collider.gameObject.GetInstanceID() == entry.gameObject.GetInstanceID())
                {
                    Collided.Remove(collider);
                }
            }
        }
    }
}