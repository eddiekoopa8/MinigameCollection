using Core;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class MGManager : MonoBehaviour
{
    Core.Timer countdown;
    TMP_Text countdownT;

    public bool MGActive = false;
    void Start()
    {
        countdown = new Core.Timer();
        countdown.SetMaximumInSeconds(7);
        countdownT = GameObject.Find("countdown").GetComponent<TMP_Text>();
        GetComponent<FadeObject>().FadeAlpha = 0;
    }

    void Update()
    {
        if (!MGActive)
        {
            return;
        }

        countdown.Tick();
        countdownT.text = "" + countdown.GetCountdownSeconds();
    }
}
