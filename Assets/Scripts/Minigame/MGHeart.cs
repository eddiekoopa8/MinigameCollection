using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGHeart : MonoBehaviour
{
    Animator anim;
    Core.FadeObject fade;
    void Start()
    {
        anim = GetComponent<Animator>();
        fade = GetComponent<Core.FadeObject>();
        anim.Play("Heart", -1, 0);
    }

    void Update()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 1 && info.IsName("Destroy"))
        {
            Destroy(gameObject);
        }
    }
    
    public void Kill()
    {
        anim.Play("Destroy", -1, 0);
    }
}
