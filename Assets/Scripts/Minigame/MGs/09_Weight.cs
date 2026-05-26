using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the rest is in the exclusive script fole]der

public class _09_Weight : MGManager
{
    HeavyItemWeight heavyBowl;
    LightItemWeight lightBowl;
    Animator scaler;
    
    static int maxBowls = 2;
    
    enum BOWL
    {
        NONE = -1,
        LEFT,
        RIGHT
    } BOWL bowl;
    
    enum WEIGHT
    {
        NONE = -1,
        HEAVY,
        LIGHT
    } WEIGHT weight;

    enum STT
    {
        CHOOSE = 0,
        DECIDE_ANIM,
        ANIM,
        RESULT,
        IDLE
    } STT state;

    public override void MGStart()
    {
        int chose = Unityls.Rand(0, maxBowls);

        // choose bowl layout
        for (int i = 0; i < maxBowls; i++)
        {
            if (i != chose)
            {
                GameObject.Find("ItemHeavy_"+i).SetActive(false);
                GameObject.Find("ItemLight_"+i).SetActive(false);
            }
        }

        // find the heavy and light bowl
        heavyBowl = GameObject.Find("ItemHeavy_"+chose).GetComponent<HeavyItemWeight>();
        lightBowl = GameObject.Find("ItemLight_"+chose).GetComponent<LightItemWeight>();

        scaler = GameObject.Find("Scaler").GetComponent<Animator>();
        state = STT.CHOOSE;
        bowl = BOWL.NONE;
        weight = WEIGHT.NONE;
    }
    
    bool ClickedBowl()
    {
        return heavyBowl.HasClicked || lightBowl.HasClicked;
    }
    
    bool PassedTime(float time)
    {
        return scaler.GetCurrentAnimatorStateInfo(0).normalizedTime >= time;
    }

    public override void MGUpdate()
    {
        switch (state)
        {
            // Choosing
            case STT.CHOOSE:
            {
                if (ClickedBowl())
                {
                    // Get the user's choice
                    if (heavyBowl.HasClicked)
                    {
                        weight = WEIGHT.HEAVY;
                    }
                    else if (lightBowl.HasClicked)
                    {
                        weight = WEIGHT.LIGHT;
                    }
                    bowl = (BOWL)heavyBowl.type;
                    Destroy(GameObject.Find("Choose")); // hide text
                    state = STT.DECIDE_ANIM;
                }
                break;
            }
            // Play heavier bowl animation
            case STT.DECIDE_ANIM:
            {
                // Play animation depending where the heavy bowl is
                if (bowl == BOWL.LEFT)
                {
                    scaler.Play("Left");
                    state = STT.ANIM;
                }
                else if (bowl == BOWL.RIGHT)
                {
                    scaler.Play("Right");
                    state = STT.ANIM;
                }
                else
                {
                    // uhh PUNISHMENT TIME PROGRAMMER
                    Debug.Log("error. screwing you over.");
                    weight = WEIGHT.LIGHT;
                    state = STT.RESULT;
                }
                break;
            }
            // rWait for heavier bowl animation
            case STT.ANIM:
            {
                if (PassedTime(1.2f))
                {
                    state = STT.RESULT;
                }
                break;
            }
            // Result!!!
            case STT.RESULT:
            {
                // If we chose the heavy bowl, we win!
                if (weight == WEIGHT.HEAVY)
                {
                    scaler.Play("Correct");
                    WonEndMG();
                }
                // If we chose the light bowl, we lose :(
                else
                {
                    scaler.Play("Incorrect");
                    LostEndMG();
                }
                state = STT.IDLE;
                break;
            }
        }
    }
}
