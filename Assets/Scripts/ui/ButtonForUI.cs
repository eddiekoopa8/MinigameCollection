using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonForUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    Animator anim;
    static float clickEnd = 1f;
    bool clicking = false;
    bool clicked = false;
    AudioSource hoverSnd;
    AudioSource clickSnd;

    public bool HasClicked {  get { return clicked;  } }
    void Start()
    {
        anim = GetComponent<Animator>();
        hoverSnd = gameObject.FindChild("HoverSound").GetComponent<AudioSource>();
        clickSnd = gameObject.FindChild("ClickSound").GetComponent<AudioSource>();
    }

    void Update()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (clicking && info.normalizedTime >= clickEnd)
        {
            Debug.Log("Click request!");
            clicked = true;
        }
        clicking = info.IsName("Click") && info.normalizedTime < clickEnd;
    }
    public void ResetState()
    {
        clicked = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clicking) return;
        //Debug.Log("Click button!");
        clickSnd.Play();
        anim.Play("Click", -1, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (clicking) return;
        //Debug.Log("enter!");
        hoverSnd.Play();
        anim.Play("In", -1, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (clicking) return;
        //Debug.Log("exit!");
        anim.Play("Out", -1, 0);
    }
}
