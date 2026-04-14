using Core;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class MGManager : MonoBehaviour
{
    public bool MGActive = false;
    public MGWorldManager MGWorld = null;

    public bool WonOrLost = false;

    public static bool DebuggingMGs = true;

    void Start()
    {
        if (DebuggingMGs)
        {
            MGActive = true;
        }
        MGStart();
    }

    public virtual void MGStart()
    {
    }

    void Update()
    {
        if (!MGActive && !DebuggingMGs)
        {
            return;
        }
        MGUpdate();
    }

    public virtual void MGUpdate()
    {
        
    }

    public void Won()
    {
        if (WonOrLost)
        {
            return;
        }
        if (MGWorld != null)
        {
            MGWorld.Request = MGWorldManager.MG_REQ.WON;
        }
        else
        {
            Debug.Log("MGWorldManager received WON");
        }

        WonOrLost = true;
    }

    public void Lost()
    {
        if (WonOrLost)
        {
            return;
        }
        if (MGWorld != null)
        {
            MGWorld.Request = MGWorldManager.MG_REQ.LOST;
        }
        else
        {
            Debug.Log("MGWorldManager received LOST");
        }
    
        WonOrLost = true;
    }

    public void Exit()
    {
        Debug.Assert(WonOrLost, "MUST CALL Won() OR Lost() FIRST !");
        if (MGWorld != null)
        {
            MGWorld.Request = MGWorldManager.MG_REQ.FORCE_TERMINATE;
        }
        else
        {
            Debug.Log("MGWorldManager received TERMINATE");
        }
    }
}
