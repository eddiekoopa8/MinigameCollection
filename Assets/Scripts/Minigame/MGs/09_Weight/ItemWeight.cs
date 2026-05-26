using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWeight : NoRotate
{
    public bool HasClicked = false;
    // types!!
    public enum TYPE
    {
        NONE = -1, // is this necessary?
        LEFT,
        RIGHT
    } public TYPE type;
    void Start()
    {
        // we havent...
        HasClicked = false;
    }
    // override mouse click event
    void OnMouseDown()
    {
        if (!HasClicked)
        {
            // we have!
            Debug.Log(gameObject.name + ": OnMouseDown();");
            HasClicked = true;
        }
    }
}
