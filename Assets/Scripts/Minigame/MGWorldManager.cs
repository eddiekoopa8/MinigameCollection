using Core;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MGWorldManager : MonoBehaviour
{
    AsyncOperation MGLoadAsync;

    Scene MGSceneHandle;
    GameObject MGRootHandle = null;
    //GameObject MGCamera = null;
    int MGHandleID = -1;
    Animator NextMGAnim = null;
    Transform transitionPos = null;

    bool isLoadingMG = false;

    public FadeObject NextMinigameDialog;

    const int MINIGAME_INDEX_START = 2;
    enum STT
    {
        INIT,
        INTRO,
        LOAD_FIRST_MINIGAME,
        NEXT_MINIGAME,
        MINIGAME,
        MINIGAME_RESULT,
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

    void LoadMG(int index)
    {
        loadMinigame(index);
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
    bool loadAsyncComplete()
    {
        return MGLoadAsync != null && MGLoadAsync.isDone;
    }

    void finaliseLoad()
    {
        MGSceneHandle = SceneManager.GetSceneByBuildIndex(MGHandleID);

        //MGCamera = GetMGObject("MGCam");
        MGRootHandle = GetMGObject("MGRoot");
        MGRootHandle.GetComponent<MGManager>().MGActive = false;
        MGRootHandle.GetComponent<FadeObject>().FadeAlpha = 0;

        //MGCamera.SetActive(false);

        Debug.Log("Minigame " + (MGHandleID - MINIGAME_INDEX_START) + " (" + MGSceneHandle.name + ") loaded!");
        isLoadingMG = false;
    }

    bool HasMGLoaded()
    {
        return MGHandleID != -1 && MGRootHandle != null;
    }

    bool IsMGLoading()
    {
        return isLoadingMG;
    }

    void appendMGScale(float scale)
    {
        Vector3 v = MGRootHandle.transform.localScale;
        v += new Vector3(scale, scale, scale);
        MGRootHandle.transform.localScale = v;
    }

    Core.Timer countdown;

    void Start()
    {
        state = STT.INIT;
        prevState = state;
        isLoadingMG = false;
        countdown = new Core.Timer();
        NextMinigameDialog.FadeAlpha = 0;
        NextMGAnim = GameObject.Find("NextMG_Anim").GetComponent<Animator>();
    }

    void Update()
    {
        transitionPos = GameObject.Find("Transition").transform;
        // Minigame loading loop
        if (loadAsyncComplete())
        {
            finaliseLoad();
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
                        LoadMG(0);
                    }

                    if (!IsMGLoading() && HasMGLoaded())
                    {
                        Debug.Log("Loaded!");
                        state = STT.NEXT_MINIGAME;
                    }
                    break;
                }
            case STT.NEXT_MINIGAME:
                {
                    countdown.SetMaximumInSeconds(1);
                    countdown.Tick();
                    NextMinigameDialog.FadeAlpha = 255;
                    if (countdown.Reached)
                    {
                        NextMinigameDialog.FadeAlpha = 0;
                        MGRootHandle.GetComponent<FadeObject>().FadeAlpha = 255;
                        MGRootHandle.GetComponent<MGManager>().MGActive = true;
                        state = STT.MINIGAME;
                        MGRootHandle.transform.SetPositionAndRotation(transitionPos.localPosition, transitionPos.localRotation);
                        NextMGAnim.Play("Main");
                    }
                    break;
                }
            case STT.MINIGAME:
                {
                    MGRootHandle.transform.SetPositionAndRotation(new Vector3(transitionPos.localPosition.x + 25, transitionPos.localPosition.y + 25, transitionPos.localPosition.z + 3), transitionPos.localRotation);
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
