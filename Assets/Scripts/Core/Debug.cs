using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger
{
    public static void FatalAssert(bool condition)
    {
        if (!condition)
        {
            Debug.Assert(condition, "FATAL OCCURED.");
            BBInternal.SCENEManager.ExitGame();
        }
    }
}
