using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SimpleCollisionListener : MonoBehaviour
    {
        /* List<*/Collider2D/*>*/ Triggered;
        /* List<*/Collider2D/*>*/ Collided;

        public GameObject GetTriggered()
        {
            return Triggered.gameObject;
        }

        public GameObject GetCollided()
        {
            return Collided.gameObject;
        }

        public bool HasTriggered(string name)
        {
            /*bool r = false;
            Triggered.ForEach(delegate (Collider2D entry)
            {
                if (name == entry.name)
                {
                    r = true;
                }
            });
            return r;*/
            return Triggered && Triggered.name == name;
        }

        public bool HasCollided(string name)
        {
            /* bool r = false;
             Collided.ForEach(delegate (Collider2D entry)
             {
                 if (name == entry.name)
                 {
                     r = true;
                 }
             });
             return r;*/
            return Collided && Collided.name == name;
        }

        public bool Has(string name)
        {
            return HasTriggered(name) ? true : HasCollided(name);
        }

        public bool HasTriggeredTag(string name)
        {
            /*bool r = false;
            Triggered.ForEach(delegate (Collider2D entry)
            {
                if (name == entry.name)
                {
                    r = true;
                }
            });
            return r;*/
            return Triggered && Triggered.CompareTag(name);
        }

        public bool HasCollidedTag(string name)
        {
            /* bool r = false;
             Collided.ForEach(delegate (Collider2D entry)
             {
                 if (name == entry.name)
                 {
                     r = true;
                 }
             });
             return r;*/
            return Collided && Collided.CompareTag(name);
        }

        public bool HasTag(string name)
        {
            return HasTriggeredTag(name) ? true : HasCollidedTag(name);
        }

        private void Start()
        {
            /*Triggered = new List<Collider2D>();
            Collided = new List<Collider2D>();*/
            Triggered = null;
            Collided = null;

            //Debug.Log(gameObject.name + " is listening!");
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            /*Triggered.ForEach(delegate (Collider2D entry)
            {
                if (collider.name == entry.name)
                {
                    return;
                }
            });
            Debug.Log("touch " + collider.name);
            Triggered.Add(collider);*/
            //Debug.Log("trigger " + collider.name);
            Triggered = collider;
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            /*Triggered.ForEach(delegate (Collider2D entry)
            {
                if (collider.gameObject.GetInstanceID() == entry.gameObject.GetInstanceID())
                {
                    Debug.Log("exit " + collider.name);
                    Triggered.Remove(collider);
                }
            });*/
            Debug.Log("untrigger " + collider.name);
            Triggered = null;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            /*Collider2D collider = collision.collider;
            Collided.ForEach(delegate (Collider2D entry)
            {
                if (collider.name == entry.name)
                {
                    return;
                }
            });
            Debug.Log("touch " + collider.name);
            Collided.Add(collider);*/

            //Debug.Log("enter " + collision.collider.name);
            Collided = collision.collider;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            /*Collider2D collider = collision.collider;
            Collided.ForEach(delegate (Collider2D entry)
            {
                if (collider.gameObject.GetInstanceID() == entry.gameObject.GetInstanceID())
                {
                    Debug.Log("exit " + collider.name);
                    Collided.Remove(collider);
                }
            });*/
            Debug.Log("exit " + collision.collider.name);
            Collided = null;
        }
    }
}