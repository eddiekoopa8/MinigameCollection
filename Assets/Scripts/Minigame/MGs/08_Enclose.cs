using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _08_Enclose : MGManager
{
    SimpleCollisionListener player;
    GameObject death;
    public override void MGStart()
    {
        death = GameObject.Find("Death");
        player = GameObject.Find("Player").GetComponent<SimpleCollisionListener>();
        WonMG();
    }

    public override void MGUpdate()
    {
        if (player.Has("Death"))
        {
            Destroy(player.gameObject);
            LostEndMG();
        }

        if (death.transform.position.y > 0)
            death.transform.SetPositionAndRotation(new Vector3(death.transform.position.x, death.transform.position.y - (Time.deltaTime * 4.5f), death.transform.position.z), death.transform.rotation);
        if (death.transform.position.y <= 0)
            death.transform.position = Vector3.zero;
    }
}
