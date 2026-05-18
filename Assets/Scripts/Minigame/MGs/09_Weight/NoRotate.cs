using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRotate : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.identity;
    }
}
