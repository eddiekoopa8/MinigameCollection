using SingularityGroup.HotReload;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
public class GameOverLoop : MonoBehaviour
{
    public class TitleScn
    {
        private static string getBtnName(string name)
        {
            return "TitleButton_" + name;
        }

        public TitleScn(string name)
        {
            instance = GameObject.Find(name);
            fader = instance.GetComponent<Core.FadeInOutObject>();
            buttons = new List<ButtonForUI>();
        }
        public void Prepare()
        {
            foreach (ButtonForUI button in buttons)
            {
                button.ResetState();
            }
        }
        public void FindButton(string name)
        {
            string btnName = getBtnName(name);
            GameObject temp = GameObject.Find(btnName);
            if (temp && temp.GetComponent<ButtonForUI>())
            {
                buttons.Add(temp.GetComponent<ButtonForUI>());
            }
            else
            {
                Debug.Log("button \"" + btnName + "\" not found.");
            }
        }
        public bool ClickedOnButton(string name)
        {
            string btnName = getBtnName(name);
            foreach (ButtonForUI button in buttons)
            {
                if (button.name == btnName)
                {
                    return button.HasClicked;
                }
            }
            return false;
        }
        public Core.FadeInOutObject fader;
        public GameObject instance;
        List<ButtonForUI> buttons;
    }
    enum STATE
    {
        INIT,
        MAIN_MENU,
        MAIN_MENU_TO_READY,
        READY,

        DONE,
    }; STATE state; STATE prevState;

    Core.Timer myTimer;

    TitleScn mainMenuScene;
    TitleScn readyScene;

    void Start()
    {
        myTimer = new Core.Timer();

        state = STATE.INIT;
        prevState = state;

        mainMenuScene = new TitleScn("GameOver");
        readyScene = new TitleScn("GetReady");

        mainMenuScene.FindButton("FromMainMenuToTitle");
        mainMenuScene.FindButton("Start");
    }

    void Update()
    {
        switch (state)
        {
            case STATE.INIT:
                {
                    readyScene.instance.SetActive(false);

                    // Prepare logo
                    myTimer.SetMaximumInSeconds(4);
                    state = STATE.MAIN_MENU;
                    break;
                }
            case STATE.MAIN_MENU:
                {
                    if (mainMenuScene.ClickedOnButton("FromMainMenuToTitle"))
                    {
                        BB.ScnManager.Goto("Scenes/Title");
                    }
                    if (mainMenuScene.ClickedOnButton("Start"))
                    {
                        readyScene.instance.SetActive(true);

                        mainMenuScene.fader.FadeOut(5);
                        readyScene.fader.FadeIn(5);

                        myTimer.Reset();
                        myTimer.SetMaximumInSeconds(1);

                        state = STATE.MAIN_MENU_TO_READY;
                    }
                    break;
                }
            case STATE.MAIN_MENU_TO_READY:
                {
                    if (mainMenuScene.fader.FadedOut && readyScene.fader.FadedIn)
                    {
                        mainMenuScene.instance.SetActive(false);
                        readyScene.Prepare();
                        state = STATE.READY;
                    }
                    break;
                }
            case STATE.READY:
                {
                    myTimer.Tick();
                    if (myTimer.Reached)
                    {
                        BB.ScnManager.Goto("Scenes/MinigameWorlds/00_TestWorld");
                        myTimer.Reset();
                    }
                    break;
                }
            default:
                {
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
