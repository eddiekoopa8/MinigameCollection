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
    int MGCount = 4; /* Generally the last number from the scene files */
    System.Random random;
    int prevIndex = -1;

    // MG OBJECTS
    Scene MGSceneHandle;
    GameObject MGRootHandle = null;
    Camera MGCamera = null;
    int MGHandleID = -1;

    // ANIMATIONS
    Animator NextMGAnim = null;
    TMP_Text[] NextMGAnimNumber;

    // DURING MINIGAME
    const int bombMax = 6;
    List<Image> bombs = new List<Image>(bombMax);

    // MGManager REQUESTS
    public enum MG_REQ
    {
        WON,
        LOST,
        FORCE_TERMINATE
    }; public MG_REQ Request;

    // UTILS
    int GetRandom(int max)
    {
        return GetRandom(0, max);
    }
    int GetRandom(int start,int max)
    {
        return start + (random.Next() % max);
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
            Debug.Log(transform.GetChild(i).gameObject.name);
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

            GameObject found = getObj(name, rootObj.transform);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    void LoadMG()
    {
        int index = GetRandom(MGCount);
        while (index == prevIndex)
        {
            index = GetRandom(MGCount);
        }
        loadMinigame(index);
        prevIndex = index;
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

        MGHandleID = scnIndex + MINIGAME_INDEX_START;
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
        MGSceneHandle = SceneManager.GetSceneByBuildIndex(MGHandleID);

        MGRootHandle = GetMGObject("MGRoot");
        MGRootHandle.GetComponent<MGManager>().MGActive = false;
        MGRootHandle.GetComponent<MGManager>().MGWorld = this;
        MGRootHandle.GetComponent<FadeObject>().FadeAlpha = 0;

        MGCamera = GetMGObject("MGCam").GetComponent<Camera>();
        MGCamera.enabled = false;

        Debug.Log("Minigame " + (MGHandleID - MINIGAME_INDEX_START) + " (" + MGSceneHandle.name + ") loaded!");
        isLoadingMG = false;
    }

    void finaliseUnload()
    {
        Debug.Log("Minigame " + (MGHandleID - MINIGAME_INDEX_START) + " (" + MGSceneHandle.name + ") unloaded!");
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

    void appendMGScale(float scale)
    {
        Vector3 v = MGRootHandle.transform.localScale;
        v += new Vector3(scale, scale, scale);
        MGRootHandle.transform.localScale = v;
    }

    Core.Timer MainCountdown;

    void Start()
    {
        MGManager.DebuggingMGs = false;

        random = new System.Random();

        state = STT.INIT;
        prevState = state;
        isLoadingMG = false;
        MainCountdown = new Core.Timer();
        NextMGAnim = GameObject.Find("NextMG_Anim").GetComponent<Animator>();

        for (int i = 0; i < bombMax; i++)
        {
            bombs.Add(GameObject.Find("Bomb" + i).GetComponent<Image>());
        }

        foreach (Image bomb in bombs)
        {
            bomb.enabled = false;
        }

        NextMGAnimNumber = new TMP_Text[2];
        for (int i = 0; i < NextMGAnimNumber.Length; i++)
        {
            NextMGAnimNumber[i] = GameObject.Find("NextMG_Anim_MGIndex" + i).GetComponent<TMP_Text>();
        }
    }

    int MGIndex = 0;
    void AddMGIndex()
    {
        MGIndex++;
        for (int i = 0; i < NextMGAnimNumber.Length; i++)
        {
            NextMGAnimNumber[i].text = MGIndex.ToString();
        }
    }

    void SetBombFrame(int bI)
    {
        for (int i = 0; i < bombMax;i++)
        {
            if (i == bI)
            {
                bombs.ElementAt(i).enabled = true;
            }
            else
            {
                bombs.ElementAt(i).enabled = false;
            }
        }
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
                    if (!IsMGLoading() && !HasMGLoaded())
                    {
                        Debug.Log("Loading...");
                        LoadMG();
                    }

                    if (!IsMGLoading() && HasMGLoaded())
                    {

                        Debug.Log("Loaded!");
                        MainCountdown.Reset();
                        MainCountdown.SetMaximumInSeconds(1);
                        state = STT.NEXT_MINIGAME;

                        NextMGAnim.Play("_", -1, 0f);
                        AddMGIndex();
                    }
                    break;
                }
            case STT.NEXT_MINIGAME:
                {
                    MainCountdown.Tick();
                    if (MainCountdown.Reached)
                    {
                        MGRootHandle.GetComponent<FadeObject>().FadeAlpha = 255;
                        MGRootHandle.GetComponent<MGManager>().MGActive = true;

                        MainCountdown.Reset();
                        MainCountdown.SetMaximumInSeconds(8);

                        state = STT.MINIGAME;
                    }
                    break;
                }
            case STT.MINIGAME:
                {
                    MGRootHandle.GetComponent<FadeObject>().FadeAlpha = 255;
                    MGRootHandle.GetComponent<MGManager>().MGActive = true;

                    MainCountdown.Tick();
                    MainCountdown.Tick();
                    SetBombFrame(MainCountdown.GetSeconds() - 1);
                    if (MainCountdown.Reached)
                    {
                        UnloadCurrentMG();
                        state = STT.AFTER_MINIGAME;
                    }
                    break;
                }
            case STT.AFTER_MINIGAME:
                {
                    if (IsCurrentMGUnloading())
                    {
                        break;
                    }

                    if (!IsMGLoading() && !HasMGLoaded())
                    {
                        Debug.Log("Loading...");
                        LoadMG();
                    }

                    if (!IsMGLoading() && HasMGLoaded())
                    {

                        Debug.Log("Loaded!");
                        MainCountdown.Reset();
                        MainCountdown.SetMaximumInSeconds(1);
                        state = STT.NEXT_MINIGAME;

                        NextMGAnim.Play("_", -1, 0f);
                        AddMGIndex();
                    }
                    break;
                }
        }

        //appendMGScale(1);

        if (prevState != state)
        {
            Debug.Log("Switch STT: " + prevState + " -> " + state + "\n");
            prevState = state;
        }
    }
}
