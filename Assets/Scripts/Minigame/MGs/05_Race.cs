using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _05_Race : MGManager
{
    GameObject road;
    GameObject car;
    new SimpleCollisionListener collider;
    public float Speed = 4;
    public override void MGStart()
    {
        StartAsWon();
        road = GameObject.Find("RoadScroll");
        car = GameObject.Find("Car");
        collider = car.GetComponent<SimpleCollisionListener>();
        GameObject.Find("badLayout" + 1).SetActive(true);
    }

    // Update is called once per frame
    public override void MGUpdate()
    {
        if (WonOrLost)
        {
            return;
        }
        road.transform.SetPositionAndRotation(road.transform.position + (Vector3.left * (Speed / 10)), road.transform.rotation);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            car.transform.SetPositionAndRotation(car.transform.position + (Vector3.up * 0.45f), car.transform.rotation);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            car.transform.SetPositionAndRotation(car.transform.position + (Vector3.down * 0.45f), car.transform.rotation);
        }

        if (collider && collider.Has("bad"))
        {
            LostEndMG();
        }
    }
}
