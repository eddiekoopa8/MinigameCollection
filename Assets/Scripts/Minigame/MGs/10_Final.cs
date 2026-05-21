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

    // Insufficient but it's unfortunately required to get around Unity's jank hell
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
        bool justEnded;
        public StageBase()
        {
            MGLost = false;
            MGWon = false;
            active = false;
            justEnded = false;
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
                justEnded = false;
                OnStart();
                //Debug.Log("start");
            }
        }

        public void End()
        {
            if (active)
            {
                active = false;
                justEnded = true;
                OnEnd();
            }
        }

        public bool Active()
        {
            return active;
        }

        public bool Ended()
        {
            return justEnded;
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

            if (playerCollider.Has("Stage1End") /*|| Input.GetKeyDown(KeyCode.P)*/)
            {
                End();
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
        Animator bombAnim;
        int anim;
        public override void OnStart()
        {
            bombAnim = GameObject.Find("Stage2_Bombs").GetComponent<Animator>();
            bombAnim.Play("Idle", -1, 0);
            GetPlayerBody().AllowXMovement = true;
            GetPlayerBody().AllowYMovement = false;
            skateboard = GameObject.Find("Skateboard").transform;
            player = GetPlayer().transform;
            anim = 0;

            SetText("Dodge!");
        }

        public override void OnUpdate()
        {
            skateboard.position = new Vector3(player.position.x, skateboard.position.y, skateboard.position.z);
            
            if (bombAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.3f)
            {
                Vector3 pos = bombAnim.gameObject.transform.position;
                bombAnim.gameObject.transform.position = new Vector3(player.position.x, pos.y, pos.z);
            }
            
            if (bombAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                bombAnim.Play("BombDo", -1, 0);
                anim++;
                //Debug.Log("anim++ = " + anim);
            }

            if (playerCollider.Has("Bomb"))
            {
                MGLost = true;
            }
            
            if (anim >= 5 || Input.GetKeyDown(KeyCode.P))
            {
                bombAnim.Play("BombDo", -1, 1);
                End();
            }
        }
    }

    public class Stage3 : StageBase
    {
        Transform skateboard;
        Transform player;
        TerryMovement terryObj;
        WeaponMovement weaponObj;
        int direction;
        public Stage3() : base()
        {
            terryObj = GameObject.Find("TERRY").GetComponent<TerryMovement>();
            weaponObj = GameObject.Find("Weapon").GetComponent<WeaponMovement>();
            terryObj.gameObject.SetActive(false);
            // Stupid STUPID UNITY JANK I HATE UNITY JANK I. HATE. UNKTY. JANK!!!!
            // Oh well.. at least it isn't necessary...
            //weaponObj.gameObject.SetActive(false);
            direction = 1;
        }
        public override void OnStart()
        {
            GetPlayerBody().AllowXMovement = true;
            GetPlayerBody().AllowYMovement = false;
            skateboard = GameObject.Find("Skateboard").transform;
            player = GetPlayer().transform;
            terryObj.gameObject.SetActive(true);
            //weaponObj.gameObject.SetActive(true);
            //weaponObj = GameObject.Find("Weapon").GetComponent<WeaponMovement>();

            SetText("Fight!");
        }

        public override void OnUpdate()
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                direction = -1;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                direction = 1;
            }
            skateboard.position = new Vector3(player.position.x, skateboard.position.y, skateboard.position.z);

            if (playerCollider.Has("TERRY") && terryObj.CanDamage())
            {
                MGLost = true;
            }
            
            if (Input.GetKeyDown(KeyCode.Space) && !MGLost && !MGWon)
            {
                weaponObj.ResetToPosition(player.position, direction);
            }
            
            if (terryObj == null)
            {
                MGWon = true;
            }
        }
    }

    Stage1 stage1;
    Stage2 stage2;
    Stage3 stage3;

    bool started = false;

    public override void MGStart()
    {
        playerCollider = GetPlayer().GetComponent<SimpleCollisionListener>();
        stage1 = new Stage1();
        stage2 = new Stage2();
        stage3 = new Stage3();

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
            stage3.Update();
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

        if (stage1.Ended() && !stage2.Active() && !stage2.Ended() && !stage3.Active())
        {
            stage2.Start();
        }

        if (stage1.Ended() && stage2.Ended() && !stage3.Active() && !stage3.Ended())
        {
            stage3.Start();
        }
    }
}
