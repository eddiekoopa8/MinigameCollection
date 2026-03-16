using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MGManager : MonoBehaviour
{
    AsyncOperation MGLoadAsync;
    GameObject MGHandle = null;
    bool hadLoaded = true;
    int MGHandleID = -1;

    const int MINIGAME_INDEX_START = 2;

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
        MGHandle = null;

        MGHandleID = scnIndex + MINIGAME_INDEX_START;
        MGLoadAsync = SceneManager.LoadSceneAsync(MGHandleID, LoadSceneMode.Additive);
        hadLoaded = true;
    }

    void Start()
    {
        LoadMinigame(0);
    }

    void Update()
    {
        if (MGLoadAsync.isDone && hadLoaded)
        {
            MGHandle = SceneManager.GetSceneByBuildIndex(MGHandleID).GetRootGameObjects()[0]; // "MGRoot"
            hadLoaded = false;
        }

        if (MGHandle != null)
        {
            MGHandle.transform.Translate(new Vector3(1, 1, 0));
        }
    }
}
