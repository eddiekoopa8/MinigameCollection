using SingularityGroup.HotReload;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

// the rest is in the Scripts/UI folder

public class TitleLoop : MonoBehaviour
{
    // Small class for title scene
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
            // Find object
            string btnName = getBtnName(name);
            GameObject temp = GameObject.Find(btnName);

            // If it has button, add it!
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
                // if clicked button matches names
                if (button.name == btnName)
                {
                    // then that button is clicked on!
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

        // setup scenes
        logoScene = new TitleScn("Logo");
        titleScene = new TitleScn("Title");
        mainMenuScene = new TitleScn("MainMenu");
        readyScene = new TitleScn("GetReady");

        // setup buttons

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
                    // set scene actives
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
                    myTimer.Tick()
                    if (myTimer.Reached)
                    {
                        myTimer.Reset();

                        titleScene.instance.SetActive(true);

                        // fade to title
                        logoScene.fader.FadeOut(5);
                        titleScene.fader.FadeIn(5);

                        state = STATE.LOGO_TO_TITLE;
                        // some nice (test) music
                        GameObject.Find("Music").GetComponent<AudioSource>().Play();
                    }
                    break;
                }
            case STATE.LOGO_TO_TITLE:
                {
                    // wait for fade in and out
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

                                    // faid to main menu
                                    titleScene.fader.FadeOut(5);
                                    mainMenuScene.fader.FadeIn(5);

                                    state = STATE.TITLE_TO_MAIN_MENU;
                                }
                                else if (titleScene.ClickedOnButton("Options"))
                                {
                                    // TODO
                                }
                                else if (titleScene.ClickedOnButton("Exit"))
                                {
                                    // not for editor
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
                    // wait for fade
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
                    // wait for fade
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

                        // fade to title
                        mainMenuScene.fader.FadeOut(5);
                        titleScene.fader.FadeIn(5);

                        state = STATE.MAIN_MENU_TO_TITLE;
                    }
                    if (mainMenuScene.ClickedOnButton("Start"))
                    {
                        readyScene.instance.SetActive(true);

                        // fade to ready
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
                    // wait for fade
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
                    // wait...
                    myTimer.Tick();
                    if (myTimer.Reached)
                    {
                        // GO!!!
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

        // debug
        if (prevState != state)
        {
            Debug.Log("Switch State: " + prevState + " -> " + state + "\n");
            prevState = state;
        }
    }
}
