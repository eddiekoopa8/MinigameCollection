using SingularityGroup.HotReload;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
public class TitleLoop : MonoBehaviour
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
    enum STATE {
        INIT,
        LOGO,
        LOGO_TO_TITLE,

        TITLE,
        TITLE_TO_MAIN_MENU,
        MAIN_MENU_TO_TITLE,
        MAIN_MENU,
        MAIN_MENU_TO_READY,
        READY,

        DONE,
    }; STATE state; STATE prevState;
    enum TITLE_STATE
    {
        MAIN,
        OPTION,
        EXIT
    }; TITLE_STATE titleState;

    Core.Timer myTimer;

    TitleScn logoScene;
    TitleScn titleScene;
    TitleScn mainMenuScene;
    TitleScn readyScene;

    void Start()
    {
        myTimer = new Core.Timer();

        state = STATE.INIT;
        prevState = state;

        logoScene = new TitleScn("Logo");
        titleScene = new TitleScn("Title");
        mainMenuScene = new TitleScn("MainMenu");
        readyScene = new TitleScn("GetReady");

        titleScene.FindButton("Play");
        titleScene.FindButton("Options");
        titleScene.FindButton("Exit");
        titleScene.FindButton("error_test");

        mainMenuScene.FindButton("FromMainMenuToTitle");
        mainMenuScene.FindButton("Start");
    }

    void Update()
    {
        switch (state)
        {
            case STATE.INIT:
                {
                    titleScene.instance.SetActive(false);
                    mainMenuScene.instance.SetActive(false);
                    readyScene.instance.SetActive(false);

                    // Prepare logo
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

                        titleScene.instance.SetActive(true);

                        logoScene.fader.FadeOut(5);
                        titleScene.fader.FadeIn(5);

                        state = STATE.LOGO_TO_TITLE;
                    }
                    break;
                }
            case STATE.LOGO_TO_TITLE:
                {
                    if (logoScene.fader.FadedOut && titleScene.fader.FadedIn)
                    {
                        logoScene.instance.SetActive(false);
                        titleScene.Prepare();

                        titleState = TITLE_STATE.MAIN;
                        state = STATE.TITLE;
                    }
                    break;
                }
            case STATE.TITLE:
                {
                    switch (titleState)
                    {
                        case TITLE_STATE.MAIN:
                            {
                                if (titleScene.ClickedOnButton("Play"))
                                {
                                    mainMenuScene.instance.SetActive(true);

                                    titleScene.fader.FadeOut(5);
                                    mainMenuScene.fader.FadeIn(5);

                                    state = STATE.TITLE_TO_MAIN_MENU;
                                }
                                else if (titleScene.ClickedOnButton("Options"))
                                {

                                }
                                else if (titleScene.ClickedOnButton("Exit"))
                                {
                                    Application.Quit();
                                }
                                break;
                            }
                        case TITLE_STATE.OPTION:
                            {
                                break;
                            }
                        case TITLE_STATE.EXIT:
                            {
                                break;
                            }
                    }
                    break;
                }
            case STATE.TITLE_TO_MAIN_MENU:
                {
                    if (mainMenuScene.fader.FadedIn && titleScene.fader.FadedOut)
                    {
                        titleScene.instance.SetActive(false);
                        mainMenuScene.Prepare();
                        state = STATE.MAIN_MENU;
                    }
                    break;
                }
            case STATE.MAIN_MENU_TO_TITLE:
                {
                    if (mainMenuScene.fader.FadedOut && titleScene.fader.FadedIn)
                    {
                        mainMenuScene.instance.SetActive(false);
                        titleScene.Prepare();
                        state = STATE.TITLE;
                    }
                    break;
                }
            case STATE.MAIN_MENU:
                {
                    if (mainMenuScene.ClickedOnButton("FromMainMenuToTitle"))
                    {
                        titleScene.instance.SetActive(true);

                        mainMenuScene.fader.FadeOut(5);
                        titleScene.fader.FadeIn(5);

                        state = STATE.MAIN_MENU_TO_TITLE;
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
