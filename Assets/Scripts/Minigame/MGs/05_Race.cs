using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _05_Race : MGManager
{
    GameObject road;
    GameObject car;
    public override void MGStart()
    {
        road = GameObject.Find("RoadScroll");
        car = GameObject.Find("Car");
    }

    // Update is called once per frame
    public override void MGUpdate()
    {
        road.transform.SetPositionAndRotation(road.transform.position + (Vector3.left * 0.25f), road.transform.rotation);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            car.transform.SetPositionAndRotation(car.transform.position + (Vector3.up * 0.45f), car.transform.rotation);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            car.transform.SetPositionAndRotation(car.transform.position + (Vector3.down * 0.45f), car.transform.rotation);
        }
    }
}
