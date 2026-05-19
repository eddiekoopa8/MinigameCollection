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

    void Start()
    {
        if (DebuggingMGs)
        {
            MGActive = true;
        }
        MGStart();
        exitTimer = new Core.Timer();
        exitTimer.SetMaximumInMilliseconds(1500);
        startExitTicking = false;
    }

    public virtual void MGStart()
    {
    }

    void Update()
    {
        MGBeforeUpdate();
        if (!MGActive)
        {
            return;
        }
        if (startExitTicking)
        {
            exitTimer.Tick();
            if (exitTimer.Reached)
            {
                MGWorld.Request |= MGWorldManager.MG_REQ.FORCE_TERMINATE;
            }
        }
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
        }
    }

    public void LostMG()
    {
        if (MGWorld != null)
        {
            MGWorld.Request |= MGWorldManager.MG_REQ.LOST;
        }
    }

    public void WonEndMG()
    {
        if (WonOrLost)
        {
            return;
        }
        if (MGWorld != null)
        {
            MGWorld.Request |= MGWorldManager.MG_REQ.WON;
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
        if (WonOrLost)
        {
            return;
        }
        if (MGWorld != null)
        {
            MGWorld.Request |= MGWorldManager.MG_REQ.LOST;
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
            Debug.Log("MGWorldManager received TERMINATE");
        }
    }
}
