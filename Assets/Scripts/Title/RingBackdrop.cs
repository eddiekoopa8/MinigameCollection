using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RingBackdrop : MonoBehaviour
{
    Animator anim;
    int boundaryX = 14;
    int boundaryY = 11;
    Core.Timer time;
    void prepAnim()
    {
        time.Reset();
        time.SetMaximumInMilliseconds(Unityls.Rand(Unityls.Rand(2000, 3000) /* sure */, 6000));
        Vector3 pos = new Vector3(Unityls.Rand(-(boundaryX), (boundaryX)), Unityls.Rand(-(boundaryY), (boundaryY)), 0);
        transform.localPosition = pos;
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        time = new Core.Timer();
        prepAnim();
        anim.Play("Ring", -1, 1);
    }

    void Update()
    {
        time.Tick();
        if (time.Reached)
        {
            prepAnim();
            anim.Play("Ring", -1, 0);
        }
    }
}
