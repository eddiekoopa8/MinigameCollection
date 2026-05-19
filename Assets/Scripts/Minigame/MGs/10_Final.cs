using Core;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;


public class _10_Spin : MGManager
{
    public static bool MGLost;
    public static bool MGWon;

    public static SimpleCollisionListener playerCollider;

    // Insufficient as hell but it's unfortunately required to get around Unity's jank hell
    public static GameObject GetPlayer()
    {
        return GameObject.Find("Player");
    }

    public static PlayerMovement_2 GetPlayerBody()
    {
        return GetPlayer().GetComponent<PlayerMovement_2>();
    }

    public static void SetText(string text)
    {
        Animator anim = GameObject.Find("StageAnim").GetComponent<Animator>();
        GameObject.Find("stagetext").GetComponent<TMP_Text>().text = text;
        anim.Play("_", -1, 0);
    }

    public static void InitTextAnm()
    {
        Animator anim = GameObject.Find("StageAnim").GetComponent<Animator>();
        anim.Play("_", -1, 1);
    }

    public class StageBase
    {
        bool active;
        public StageBase()
        {
            MGLost = false;
            MGWon = false;
            active = false;
        }

        public virtual void OnStart()
        {

        }

        public virtual void OnEnd()
        {

        }

        public virtual void OnUpdate()
        {

        }

        public void Update()
        {
            if (active)
            {
                OnUpdate();
            }
        }

        public void Start()
        {
            if (!active)
            {
                active = true;
                OnStart();
            }
        }

        public void End()
        {
            if (active)
            {
                active = false;
                OnEnd();
            }
        }

        public bool Active()
        {
            return active;
        }
    }
    public class Stage1 : StageBase
    {
        GameObject obstacles;

        public override void OnStart()
        {
            obstacles = GameObject.Find("Stage1_obstacles");
            GetPlayerBody().AllowXMovement = false;
            GetPlayerBody().AllowYMovement = true;

            SetText("Jump!");
        }

        public override void OnUpdate()
        {
            obstacles.transform.position += Vector3.left * (Time.deltaTime * 11);

            if (playerCollider.HasTag("MG10_Stage1_Square"))
            {
                MGLost = true;
            }
        }

        public override void OnEnd()
        {
            // yeah no going back.
            Destroy(GameObject.Find("Stage1_obstacles"));
        }
    }
    public class Stage2 : StageBase
    {
        Transform skateboard;
        Transform player;
        public override void OnStart()
        {
            GetPlayerBody().AllowXMovement = true;
            GetPlayerBody().AllowYMovement = false;
            skateboard = GameObject.Find("Skateboard").transform;
            player = GetPlayer().transform;

            SetText("Dodge!");
        }

        public override void OnUpdate()
        {
            skateboard.position = new Vector3(player.position.x, skateboard.position.y, skateboard.position.z);
        }
    }

    Stage1 stage1;
    Stage2 stage2;

    bool started = false;

    public override void MGStart()
    {
        playerCollider = GetPlayer().GetComponent<SimpleCollisionListener>();
        stage1 = new Stage1();
        stage2 = new Stage2();

        InitTextAnm();

        started = false;
    }

    public override void MGUpdate()
    {
        if (!started)
        {
            stage1.Start();
            started = true;
        }
        if (GetPlayer())
        {
            stage1.Update();
            stage2.Update();
        }

        if (MGLost)
        {
            LostEndMG();
            if (GetPlayer())
            {
                Destroy(GetPlayer());
            }
        }
        else if (MGWon)
        {
            WonEndMG();
        }

        if ((playerCollider.Has("Stage1End") && stage1.Active()) || Input.GetKeyDown(KeyCode.P))
        {
            stage1.End();
            stage2.Start();
        }
    }
}
