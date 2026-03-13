using SingularityGroup.HotReload;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TitleLoop : MonoBehaviour
{
    Core.FadeObject logoFade;

    enum STATE {
        PREP_LOGO,
        LOGO,

        FADE_TO_PRESS_START,
        PRESS_START,

        MAIN_MENU,
        OPTIONS,
        STAGE_SELECT,
        STAGE_PREMISE,

        DIALOG,
        DIALOG_RESULT,
        DONE,
    }; STATE state; STATE prevState;

    Core.Timer myTimer;
    Core.FadeInOutObject logo;

    void Start()
    {
        logoFade = GetComponent<Core.FadeObject>();

        myTimer = new Core.Timer();

        state = STATE.PREP_LOGO;
        prevState = state;

        logo = GameObject.Find("Logo").GetComponent<Core.FadeInOutObject>();
    }

    void Update()
    {
        switch (state)
        {
            case STATE.PREP_LOGO:
                {
                    myTimer.SetMaximumInSeconds(4);
                    state = STATE.LOGO;
                    break;
                }
            case STATE.LOGO:
                {
                    myTimer.Tick();
                    if (myTimer.Reached)
                    {
                        myTimer.Reset();

                        logo.FadeOut(5);

                        state = STATE.FADE_TO_PRESS_START;
                    }
                    break;
                }
            case STATE.FADE_TO_PRESS_START:
                {
                    if (logo.FadedOut)
                    {
                        state = STATE.PRESS_START;
                    }
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

        if (prevState != state)
        {
            Debug.Log("Switch State: " + prevState + " -> " + state + "\n");
            prevState = state;
        }
    }
}
