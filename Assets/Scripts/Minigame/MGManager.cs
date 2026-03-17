using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class MGManager : MonoBehaviour
{
    Core.Timer countdown;
    TMP_Text countdownT;

    void Start()
    {
        countdown = new Core.Timer();
        countdown.SetMaximumInSeconds(7);
        countdownT = GameObject.Find("countdown").GetComponent<TMP_Text>();
    }

    void Update()
    {
        countdown.Tick();
        countdownT.text = "" + countdown.GetCountdownSeconds();
    }
}
