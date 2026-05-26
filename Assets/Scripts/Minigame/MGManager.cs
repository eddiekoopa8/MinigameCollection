using Core;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MGManager : MonoBehaviour
{
    public bool MGActive = false;
    public MGWorldManager MGWorld = null;

    public bool WonOrLost = false;

    public static bool DebuggingMGs = true;

    Core.Timer exitTimer;
    bool startExitTicking;
    bool loadedMG;
    
    public MGWorldManager.MG_REQ StartRequest = MGWorldManager.MG_REQ.NONE;

    void Start()
    {
        if (DebuggingMGs)
        {
            MGActive = true;
        }
        // Init minigame
        MGStart();

        // setup timers
        exitTimer = new Core.Timer();
        exitTimer.SetMaximumInMilliseconds(1500);
        startExitTicking = false;

        loadedMG = false;
    }

    public virtual void MGStart()
    {
    }

    void Update()
    {
        // Set start request when minigame is activated
        // (i hate world code)
        if (MGWorld != null)
        {
            if (!loadedMG)
            {
                MGWorld.Request = StartRequest;
                Debug.Log("start request: " + MGWorld.Request);
                loadedMG = true;
            }
        }
        // Global update
        MGBeforeUpdate();
        if (!MGActive)
        {
            return;
        }
        // If we exited, add a delay
        if (startExitTicking)
        {
            exitTimer.Tick();
            // after the delay, exit.
            if (exitTimer.Reached)
            {
                MGWorld.Request |= MGWorldManager.MG_REQ.FORCE_TERMINATE;
            }
        }
        // Active update
        MGUpdate();
    }

    public virtual void MGUpdate()
    {

    }

    public virtual void MGBeforeUpdate()
    {

    }

    public void WonMG()
    {
        if (MGWorld != null)
        {
            MGWorld.Request |= MGWorldManager.MG_REQ.WON;
            //MGWorld.Request &= ~MGWorldManager.MG_REQ.LOST;
        }
    }

    public void LostMG()
    {
        if (MGWorld != null)
        {
            //MGWorld.Request |= MGWorldManager.MG_REQ.LOST;
            MGWorld.Request &= ~MGWorldManager.MG_REQ.WON;
        }
    }

    public void WonEndMG()
    {
        // only once
        if (WonOrLost)
        {
            return;
        }
        if (MGWorld != null)
        {
            WonMG();
        }
        else
        {
            Debug.Log("MGWorldManager received WON");
        }

        WonOrLost = true;
        Exit();
    }

    public void LostEndMG()
    {
        // only once
        if (WonOrLost)
        {
            return;
        }
        if (MGWorld != null)
        {
            LostMG();
        }
        else
        {
            Debug.Log("MGWorldManager received LOST");
        }
    
        WonOrLost = true;
        Exit();
    }

    public void Exit()
    {
        Debug.Assert(WonOrLost, "MUST CALL WonEndMG() OR LostEndMG() FIRST !");
        if (MGWorld != null)
        {
            startExitTicking = true;
        }
        else
        {
            //Debug.Log("MGWorldManager received TERMINATE");
        }
    }

    public void PlayMGWorldSound(string name)
    {
        if (MGWorld != null)
        {
            MGWorld.PlaySound(name);
        }
    }
}
