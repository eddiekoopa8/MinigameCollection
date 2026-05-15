using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class _10_Spin : MGManager
{
    public static bool MGLost;
    public static bool MGWon;

    // Insufficient as hell but it's unfortunately required to get around Unity's jank hell
    public static GameObject GetPlayer()
    {
        return GameObject.Find("Player");
    }
    public static PlayerCollider GetPlayerCollider()
    {
        return GetPlayer().GetComponent<PlayerCollider>();
    }
    public class StageBase
    {
        protected GameObject Player;

        public StageBase()
        {
            MGLost = false;
            MGWon = false;
        }

        public virtual void Update()
        {

        }
    }
    public class Stage1 : StageBase
    {
        GameObject obstacles;

        public Stage1() : base()
        {
            obstacles = GameObject.Find("Stage1_obstacles");
        }

        public override void Update()
        {
            obstacles.transform.position += Vector3.left * 0.2f;

            if (GetPlayerCollider().Collided && GetPlayerCollider().Collided.CompareTag("MG10_Stage1_Square"))
            {
                MGLost = true;
            }
        }
    }

    Stage1 stage1;
    public override void MGStart()
    {
        stage1 = new Stage1();
    }

    public override void MGUpdate()
    {
        if (GetPlayer())
        {
            stage1.Update();
        }

        if (MGLost)
        {
            LostEndMG();
        }
        else if (MGWon)
        {
            WonEndMG();
        }

        if (WonOrLost)
        {
            if (GetPlayer())
            {
                Destroy(GetPlayer());
            }
        }
    }
}
