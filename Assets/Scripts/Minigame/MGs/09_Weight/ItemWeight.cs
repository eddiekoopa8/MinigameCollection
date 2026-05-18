using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWeight : NoRotate
{
    public bool HasClicked = false;
    public enum TYPE
    {
        NONE = -1,
        LEFT,
        RIGHT
    } public TYPE type;
    void Start()
    {
        HasClicked = false;
    }
    void OnMouseDown()
    {
        if (!HasClicked)
        {
            Debug.Log(gameObject.name + ": OnMouseDown();");
            HasClicked = true;
        }
    }
}
