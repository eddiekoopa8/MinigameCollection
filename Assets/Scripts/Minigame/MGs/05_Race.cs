using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _05_Race : MGManager
{
    GameObject road;
    GameObject car;
    new SimpleCollisionListener collider;
    public float RoadSpeed = 2;
    public float CarSpeed = 1.75f;
    public override void MGStart()
    {
        road = GameObject.Find("RoadScroll");
        car = GameObject.Find("Car");
        collider = car.GetComponent<SimpleCollisionListener>();

        // scrapped :(
        //GameObject.Find("badLayout" + 1).SetActive(true);
    }

    // Update is called once per frame
    public override void MGUpdate()
    {
        // calculate speed
        float roadSpeed = RoadSpeed * (Time.deltaTime * 10);
        float carSpeed = CarSpeed * (Time.deltaTime * 10);
        if (WonOrLost)
        {
            return;
        }
        // TODO: replace these SetPositionAndRotation's with changing rigidbody velocity
        road.transform.SetPositionAndRotation(road.transform.position + (Vector3.left * roadSpeed), road.transform.rotation);

        // Movement depending on arrow keys
        if (Input.GetKey(KeyCode.UpArrow))
        {
            car.transform.SetPositionAndRotation(car.transform.position + (Vector3.up * carSpeed), car.transform.rotation);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            car.transform.SetPositionAndRotation(car.transform.position + (Vector3.down * carSpeed), car.transform.rotation);
        }

        // lose if we touch bad
        if (collider && collider.Has("bad"))
        {
            PlayMGWorldSound("CrateHit");
            LostEndMG();
        }
    }
}
