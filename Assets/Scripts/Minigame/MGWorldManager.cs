using Core;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.VisualScripting;
using Unity.Loading;
using TMPro;

public class MGWorldManager : MonoBehaviour
{
    // LOAD
    AsyncOperation MGLoadAsync;
    AsyncOperation MGUnloadAsync;
    bool isLoadingMG = false;
    bool isUnloadingMG = false;
    int MGCount = 5; /* Generally the last number from the scene files */
    System.Random random;
    int prevIndex = -1;
    bool LastMinigame = false;
    bool Completed = false;

    // MG OBJECTS
    Scene MGSceneHandle;
    GameObject MGRootHandle = null;
    Camera MGCamera = null;
    int MGHandleID = -1;

    // ANIMATIONS
    Animator NextMGAnim = null;
    Animator BombAnim = null;
    Animator WinLoseAnim = null;
    TMP_Text[] NextMGAnimNumber;
    TMP_Text[] NextMGAnimDesc;
    
    GameObject tempSounds;

    // MGManager REQUESTS
    public enum MG_REQ : uint
    {
        NONE = 0x00,
        LOADED = 0x01,
        WON = 0x02,
        LOST = 0x04,
        FORCE_TERMINATE = 0x08
    }; public MG_REQ Request; public MG_REQ PrevRequest;
    
    string[] mgNames;
    
    // HEARTS
    MGHeart[] MGHearts;
    GameObject MGHeartContainer;
    //Vector3 MGHeartContainer_oldPos;
    int MGLives;
    static int MG_LIVE_COUNT = 3;

    static string reqS(MG_REQ Req)
    {
        // Print flags
        string result = "";
        if ((Req & MG_REQ.NONE) != 0)
        {
            result += "NONE ";
        }
        if ((Req & MG_REQ.LOADED) != 0)
        {
            result += "LOADED ";
        }
        if ((Req & MG_REQ.WON) != 0)
        {
            result += "WON ";
        }
        if ((Req & MG_REQ.LOST) != 0)
        {
            result += "LOST ";
        }
        if ((Req & MG_REQ.FORCE_TERMINATE) != 0)
        {
            result += "FORCE_TERMINATE ";
        }
        return result;
    }

    const int MINIGAME_INDEX_START = 3;
    enum STT
    {
        INIT,
        INTRO,
        LOAD_FIRST_MINIGAME,
        NEXT_MINIGAME,
        MINIGAME,
        AFTER_MINIGAME,
        GAME_OVER,
        COMPLETE,
    } STT state = STT.INIT; STT prevState;

    static GameObject getObj(string name, Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            //Debug.Log(transform.GetChild(i).gameObject.name);
            if (transform.GetChild(i).gameObject.name == name)
            {
                return transform.GetChild(i).gameObject;
            }
            if (transform.GetChild(i).childCount != 0)
            {
                return getObj(name, transform.GetChild(i));
            }
        }
        return null;
    }


    GameObject GetMGObject(string name)
    {
        GameObject[] rootObjs = MGSceneHandle.GetRootGameObjects();

        if (rootObjs.Length == 0)
        {
            Debug.Log("There are no objects in minigame scene. Critical error?");
            return null;
        }
        foreach (GameObject rootObj in rootObjs)
        {
            if (rootObj.name == name)
            {
                return rootObj;
            }

            GameObject found = rootObj.FindChild(name);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }
    void UnloadCurrentMG()
    {
        unloadMinigame(MGHandleID);
    }
    void loadMinigame(int scnIndex)
    {
        if (MGHandleID == scnIndex)
        {
            return;
        }
        MGRootHandle = null;
        MGHandleID = -1;

        // get ID
        MGHandleID = scnIndex + MINIGAME_INDEX_START;
        // async load
        MGLoadAsync = SceneManager.LoadSceneAsync(MGHandleID, LoadSceneMode.Additive);
        isLoadingMG = true;
    }
    void unloadMinigame(int scnIndex)
    {
        MGRootHandle = null;

        MGUnloadAsync = SceneManager.UnloadSceneAsync(MGHandleID);
        isUnloadingMG = true;
    }
    bool loadAsyncComplete()
    {
        return MGLoadAsync != null && MGLoadAsync.isDone;
    }
    bool unloadAsyncComplete()
    {
        return MGUnloadAsync != null && MGUnloadAsync.isDone;
    }

    void finaliseLoad()
    {
        // Get handle (scene) by ID
        MGSceneHandle = SceneManager.GetSceneByBuildIndex(MGHandleID);

        // Get all of the reaquired objects
        MGRootHandle = GetMGObject("MGRoot");
        MGRootHandle.GetComponent<MGManager>().MGActive = false;
        MGRootHandle.GetComponent<MGManager>().MGWorld = this;
        MGRootHandle.GetComponent<FadeObject>().FadeAlpha = 0;

        // Disable debugging camera
        MGCamera = GetMGObject("MGCam").GetComponent<Camera>();
        MGCamera.enabled = false;

        //Debug.Log("Minigame " + (MGHandleID - MINIGAME_INDEX_START) + " (" + MGSceneHandle.name + ") loaded!");
        isLoadingMG = false;
        
        Request = MG_REQ.LOADED;
    }

    void finaliseUnload()
    {
        //Debug.Log("Minigame " + (MGHandleID - MINIGAME_INDEX_START) + " (" + MGSceneHandle.name + ") unloaded!");
        isUnloadingMG = false;
    }

    bool HasMGLoaded()
    {
        return MGHandleID != -1 && MGRootHandle != null;
    }

    bool IsMGLoading()
    {
        return isLoadingMG;
    }

    bool IsCurrentMGUnloading()
    {
        return isUnloadingMG;
    }

    // unused
    void appendMGScale(float scale)
    {
        Vector3 v = MGRootHandle.transform.localScale;
        v += new Vector3(scale, scale, scale);
        MGRootHandle.transform.localScale = v;
    }

    Core.Timer MainCountdown;
    
    public void PlaySound(string name)
    {
        tempSounds.FindChild(name).GetComponent<AudioSource>().Play();
    }

    void Start()
    {
        MGManager.DebuggingMGs = false;

        random = new System.Random();

        state = STT.INIT;
        prevState = state;
        isLoadingMG = false;
        MainCountdown = new Core.Timer();

        // reset anims
        WinLoseAnim = GameObject.Find("WinLose_Anim").GetComponent<Animator>();
        WinLoseAnim.Play("_", -1, 1);

        NextMGAnim = GameObject.Find("NextMG_Anim").GetComponent<Animator>();
        NextMGAnim.Play("_", -1, 1);

        BombAnim = GameObject.Find("BombAnim").GetComponent<Animator>();
        BombAnim.Play("Count", -1, 1);

        // get text
        NextMGAnimNumber = new TMP_Text[2];
        for (int i = 0; i < NextMGAnimNumber.Length; i++)
        {
            NextMGAnimNumber[i] = GameObject.Find("NextMG_Anim_MGIndex" + i).GetComponent<TMP_Text>();
        }
        NextMGAnimDesc = new TMP_Text[2];
        for (int i = 0; i < NextMGAnimDesc.Length; i++)
        {
            NextMGAnimDesc[i] = GameObject.Find("NextMG_Anim_MGDesc" + i).GetComponent<TMP_Text>();
        }

        // get hearts
        MGHearts = new MGHeart[MG_LIVE_COUNT];
        for (int i = 0; i < MGHearts.Length; i++)
        {
            MGHearts[i] = GameObject.Find("MGHeart" + i).GetComponent<MGHeart>();
        }
        
        Request = MG_REQ.NONE;
        PrevRequest = Request;
        
        MGLives = MG_LIVE_COUNT;
        
        MGHeartContainer = GameObject.Find("MGHearts");
        //MGHeartContainer_oldPos = MGHeartContainer.transform.position;

        // C# 9.0 why do you make me do this
        // set mingiame names
        mgNames = new string[10];
        mgNames[0] = "Wheel";
        mgNames[1] = "Tennis";
        mgNames[2] = "Jump";
        mgNames[3] = "Sword";
        mgNames[4] = "Drive";
        mgNames[5] = "Follow";
        mgNames[6] = "Choose";
        mgNames[7] = "Collect";
        mgNames[8] = "Hide";
        mgNames[9] = "Adventure";

        // temporary solutioin to sound
        // get sound collection
        tempSounds = GameObject.Find("ColinBBSound");
    }

    int MGIndex = 0;
    void LoadMG()
    {
        // reset anim
        BombAnim.Play("Count", -1, 1);

        // last minigame flag for 10th minigame
        if (MGIndex >= 9)
        {
            LastMinigame = true;
        }

        // load MG scene
        loadMinigame(LastMinigame ? MGIndex : MGIndex++);

        // set text
        for (int i = 0; i < NextMGAnimNumber.Length; i++)
        {
            NextMGAnimNumber[i].text = LastMinigame ? "Final" : MGIndex.ToString();
        }
        for (int i = 0; i < NextMGAnimDesc.Length; i++)
        {
            NextMGAnimDesc[i].text = mgNames[LastMinigame ? MGIndex : MGIndex-1];
        }
    }

    bool BombFinished()
    {
        return BombAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1; 
    }
    
    void LoseLife()
    {
        MGLives--;
        if (MGLives >= 0 && MGLives < MG_LIVE_COUNT)
        {
            MGHearts[MGLives].Kill();

            MGHearts[MGLives] = null;
        }
    }
    
    void VisibleLives()
    {
        //MGHeartContainer.transform.position = MGHeartContainer_oldPos;
        MGHeartContainer.SetActive(true);
    }
    
    void InvisibleLives()
    {
        //MGHeartContainer.transform.position = new Vector3(-5000, -5000, -5000);
        MGHeartContainer.SetActive(false);
    }

    void Update()
    {
        // Minigame loading loop
        if (loadAsyncComplete() && isLoadingMG)
        {
            finaliseLoad();
        }
        if (unloadAsyncComplete() && isUnloadingMG)
        {
            finaliseUnload();
        }

        switch (state)
        {
            case STT.INIT:
                {
                    /// TODO
                    state = STT.INTRO;
                    break;
                }
            case STT.INTRO:
                {
                    /// TODO
                    state = STT.LOAD_FIRST_MINIGAME;
                    break;
                }
            case STT.LOAD_FIRST_MINIGAME:
                {
                    // Load minigame if we aren't loading.
                    if (!IsMGLoading() && !HasMGLoaded())
                    {
                        //Debug.Log("Loading...");
                        LoadMG();
                    }
                    // If we loaded minigame
                    if (!IsMGLoading() && HasMGLoaded())
                    {
                        // Play next minigame animation
                        //Debug.Log("Loaded!");
                        state = STT.NEXT_MINIGAME;

                        NextMGAnim.Play("_", -1, 0f);
                        GameObject.Find("Loading").SetActive(false);
                    }
                    break;
                }
            case STT.NEXT_MINIGAME:
                {    
                    // Free cursor
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    // After animaiton
                    if (NextMGAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                    {
                        // Activate minigame
                        MGRootHandle.GetComponent<FadeObject>().FadeAlpha = 255;
                        MGRootHandle.GetComponent<MGManager>().MGActive = true;

                        if (!LastMinigame) BombAnim.Play("Count", -1, 0);

                        state = STT.MINIGAME;
                        InvisibleLives(); // hide lives
                    }
                    break;
                }
            case STT.MINIGAME:
                {
                    MGRootHandle.GetComponent<FadeObject>().FadeAlpha = 255;
                    MGRootHandle.GetComponent<MGManager>().MGActive = true;

#if UNITY_EDITOR
                    bool debugClick = Input.GetKeyDown(KeyCode.K);
#else
                    bool debugClick = false;
#endif
                    // If we exited minigame or bomb ticked
                    if ((!LastMinigame && BombFinished()) || debugClick || (Request & MG_REQ.FORCE_TERMINATE) != 0)
                    {
                        if (debugClick)
                        {
                            Request = MG_REQ.WON;
                        }
                        // Goodbye minigame
                        UnloadCurrentMG();

                        // show lives
                        VisibleLives();

                        // Won
                        if ((Request & MG_REQ.WON) != 0)
                        {
                            WinLoseAnim.Play("Won", -1, 0);
                            if (LastMinigame) Completed = true; // completed if it was the last one
                            PlaySound("MGWon");
                        }
                        else
                        // Lost
                        {
                            WinLoseAnim.Play("Lost", -1, 0);
                            LoseLife(); // lose life (uh oh)
                            PlaySound("MGLost");
                        }

                        // Load new minigame and wait for animation
                        state = STT.AFTER_MINIGAME;
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
                    break;
                }
            // derived from LOAD_FIRST_MINIGAME
            case STT.AFTER_MINIGAME:
                {
                    // Stop if currently unloading
                    if (IsCurrentMGUnloading())
                    {
                        break;
                    }

                    // Load new minigame
                    if (!IsMGLoading() && !HasMGLoaded())
                    {
                        //Debug.Log("Loading...");

                        LoadMG();
                    }

                    // If loaded minigame and played result animation
                    if (((!IsMGLoading() && HasMGLoaded()) || Completed) && WinLoseAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                    {
                        // if we completed, goto success screen
                        if (Completed)
                        {
                            BBInternal.SCENEManager.ChangeScene("Scenes/Complete");
                        }
                        // if we have no lives, goto gameover screen
                        else if (MGLives <= 0)
                        {
                            BBInternal.SCENEManager.ChangeScene("Scenes/GameOver");
                        }
                        else
                            {
                            //Debug.Log("Loaded!");
                            MainCountdown.Reset();
                            MainCountdown.SetMaximumInSeconds(1);
                            state = STT.NEXT_MINIGAME;

                            NextMGAnim.Play("_", -1, 0f);
                            WinLoseAnim.Play("_", -1, 1);
                        }
                    }
                    break;
                }
        }

        // old
        //appendMGScale(1);

        /* DEBUG LOGS*/
        /*if (prevState != state)
        {
            Debug.Log("Switch STT: " + prevState + " -> " + state + "\n");
            prevState = state;
        }*/

        /*if (PrevRequest != Request)
        {
            Debug.Log("Send Request " + reqS(Request) + " (prev: " + reqS(PrevRequest) + ")\n");
            PrevRequest = Request;
        }*/
    }
}
