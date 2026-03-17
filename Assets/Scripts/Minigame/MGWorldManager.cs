using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MGWorldManager : MonoBehaviour
{
    AsyncOperation MGLoadAsync;

    Scene MGHandle;
    GameObject MGRootHandle = null;
    GameObject MGCamera = null;
    int MGHandleID = -1;

    bool hadLoaded = true;

    const int MINIGAME_INDEX_START = 2;

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

    bool HasMGLoaded()
    {
        return MGHandleID != -1 && MGRootHandle != null;
    }

    GameObject GetMGObject(string name)
    {
        GameObject[] rootObjs = MGHandle.GetRootGameObjects();

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

    void LoadMinigame(int index)
    {
        loadMinigame(index);
    }
    void loadMinigame(int scnIndex)
    {
        if (MGHandleID == scnIndex)
        {
            return;
        }
        MGHandleID = -1;

        MGHandleID = scnIndex + MINIGAME_INDEX_START;
        MGLoadAsync = SceneManager.LoadSceneAsync(MGHandleID, LoadSceneMode.Additive);
        hadLoaded = true;
    }

    void Start()
    {
        LoadMinigame(0);
    }

    void appendMGScale(float scale)
    {
        Vector3 v = MGRootHandle.transform.localScale;
        v += new Vector3(scale, scale, scale);
        MGRootHandle.transform.localScale = v;
    }

    void Update()
    {
        if (MGLoadAsync.isDone && hadLoaded)
        {
            MGHandle = SceneManager.GetSceneByBuildIndex(MGHandleID);

            MGCamera = GetMGObject("MGCam");
            MGRootHandle = GetMGObject("MGRoot");

            MGCamera.SetActive(false);

            Debug.Log("Minigame " + (MGHandleID - MINIGAME_INDEX_START) + " (" + MGHandle.name + ") loaded!");
            hadLoaded = false;
        }

        if (!HasMGLoaded())
        {
            return;
        }

        appendMGScale(1);
    }
}
