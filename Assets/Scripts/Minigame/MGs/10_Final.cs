using Core;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

// The rest is in the exclusive script folder

// one problem that I have with this: i love GetComponent<>()

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

    // BASE CLASS FOR STAGES
    public class StageBase
    {
        bool active;
        bool justEnded;
        public StageBase()
        {
            // Public booleans
            MGLost = false;
            MGWon = false;

            // private booleans
            active = false;
            justEnded = false;
        }

        // Functions needed to override.
        public virtual void OnStart()
        {

        }

        public virtual void OnEnd()
        {

        }

        public virtual void OnUpdate()
        {

        }

        // Public update function
        public void Update()
        {
            if (active)
            {
                OnUpdate();
            }
        }

        public void Start()
        {
            // Only start if we are not active.
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
            // Only end if we are active.
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

    // STAGE 1: OBSTACLES
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
            // scroll obstacles
            // (I really should decided if I want to use either Rigidbody.velocity, SetPositionAndRotation or transform.position, not use all of them randomly)
            obstacles.transform.position += Vector3.left * (Time.deltaTime * 11);

            // If player touched square, we lose!
            if (playerCollider.HasTag("MG10_Stage1_Square"))
            {
                MGLost = true;
            }

            // If player got passed the obstacles, we move on to the next stage
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

    // STAGE 2: DODGE CRUSHERS
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
            // restrict movement
            GetPlayerBody().AllowXMovement = true;
            GetPlayerBody().AllowYMovement = false;
            skateboard = GameObject.Find("Skateboard").transform;
            player = GetPlayer().transform;
            anim = 0;

            SetText("Dodge!");
        }

        public override void OnUpdate()
        {
            // Skateboard's X position is relative to the player's
            skateboard.position = new Vector3(player.position.x, skateboard.position.y, skateboard.position.z);

            // Follow player until it starts to crush.
            if (bombAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.3f)
            {
                Vector3 pos = bombAnim.gameObject.transform.position;
                bombAnim.gameObject.transform.position = new Vector3(player.position.x, pos.y, pos.z);
            }

            // Count after animation played.
            if (bombAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                bombAnim.Play("BombDo", -1, 0);
                anim++;
                //Debug.Log("anim++ = " + anim);
            }

            // If player got crushed, we lose!
            if (playerCollider.Has("Bomb"))
            {
                MGLost = true;
            }

            // If player survived, we move on to the next stage!
            if (anim >= 5 /*|| Input.GetKeyDown(KeyCode.P)*/)
            {
                bombAnim.Play("BombDo", -1, 1);
                End();
            }
        }
    }

    // STAGE 3: B- BOSS!!!!!
    public class Stage3 : StageBase
    {
        Transform skateboard;
        Transform player;
        TerryMovement terryObj;
        WeaponMovement weaponObj;
        int direction;
        // constructors are called on minigame init.
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
            // restrict movement
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

            // Skateboard X position relative to player's
            skateboard.position = new Vector3(player.position.x, skateboard.position.y, skateboard.position.z);

            // If boss touched player, lose
            if (playerCollider.Has("TERRY") && terryObj.CanDamage())
            {
                MGLost = true;
            }

            // Shoot weapon
            if (Input.GetKeyDown(KeyCode.Space) && !MGLost && !MGWon)
            {
                weaponObj.ResetToPosition(player.position, direction);
            }

            // If terry died, WE WON!!!!
            if (terryObj == null)
            {
                MGWon = true;
            }
        }
    }

    // STAGE 4: ESCAPE!!!!
    // (scrapped :( )

    Stage1 stage1;
    Stage2 stage2;
    Stage3 stage3;

    bool started = false;

    public override void MGStart()
    {
        playerCollider = GetPlayer().GetComponent<SimpleCollisionListener>();

        //setup stages
        stage1 = new Stage1();
        stage2 = new Stage2();
        stage3 = new Stage3();

        InitTextAnm();

        started = false;
    }

    public override void MGUpdate()
    {
        // start with stage 1
        if (!started)
        {
            stage1.Start();
            started = true;
        }

        // run stages
        if (GetPlayer())
        {
            stage1.Update();
            stage2.Update();
            stage3.Update();
        }

        // if we lost, kill player
        if (MGLost)
        {
            LostEndMG();
            if (GetPlayer())
            {
                PlayMGWorldSound("PlayerDie");
                Destroy(GetPlayer());
            }
        }
        // if we won, end the minigame
        else if (MGWon)
        {
            WonEndMG();
        }

        // start stage 2
        if (stage1.Ended() && !stage2.Active() && !stage2.Ended() && !stage3.Active())
        {
            stage2.Start();
        }

        // start stage 3
        if (stage1.Ended() && stage2.Ended() && !stage3.Active() && !stage3.Ended())
        {
            stage3.Start();
        }
    }
}
