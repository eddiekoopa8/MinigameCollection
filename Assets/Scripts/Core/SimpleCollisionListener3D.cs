using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SimpleCollisionListener3D : MonoBehaviour
    {
        /* List<*/Collider/*>*/ Triggered;
        /* List<*/Collider/*>*/ Collided;

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

        private void Start()
        {
            /*Triggered = new List<Collider2D>();
            Collided = new List<Collider2D>();*/
            Triggered = null;
            Collided = null;

            Debug.Log(gameObject.name + " is listening!");
        }

        private void OnTriggerStay(Collider collider)
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
            Debug.Log(gameObject.name + ": trigger " + collider.name);
            Triggered = collider;
        }

        private void OnTriggerExit(Collider collider)
        {
            /*Triggered.ForEach(delegate (Collider2D entry)
            {
                if (collider.gameObject.GetInstanceID() == entry.gameObject.GetInstanceID())
                {
                    Debug.Log("exit " + collider.name);
                    Triggered.Remove(collider);
                }
            });*/
            Debug.Log(gameObject.name + ": untrigger " + collider.name);
            Triggered = null;
        }

        private void OnCollisionStay(Collision collision)
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
            Debug.Log(gameObject.name + ": enter " + collision.collider.name);
            Collided = collision.collider;
        }

        private void OnCollisionExit(Collision collision)
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
            Debug.Log(gameObject.name + ": exit " + collision.collider.name);
            Collided = null;
        }
    }
}