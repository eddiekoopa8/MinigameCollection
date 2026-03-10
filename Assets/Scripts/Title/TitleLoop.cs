using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLoop : MonoBehaviour
{
    Core.FadeObject logoFade;

    enum STATE {
        LOGO,
        PRESS_START,

        MAIN_MENU,
        OPTIONS,
        STAGE_SELECT,
        STAGE_PREMISE,

        DIALOG,
        DIALOG_RESULT,
    }; STATE state;

    void Start()
    {
        logoFade = GetComponent<Core.FadeObject>();
    }

    void Update()
    {
        switch (state)
        {
            case STATE.LOGO:
                {
                    // fade out and wait til it done
                    state = STATE.PRESS_START;
                    break;
                }
            case STATE.PRESS_START:
                {
                    break;
                }
            default:
                {
                    Debug.Assert(false, "Invalid TitleLoop State.");
                    break;
                }
        }
    }
}
