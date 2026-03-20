using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorOnlyCamera : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }
}
