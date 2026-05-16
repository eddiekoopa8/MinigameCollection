using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Unityls /* shameless pum */
{
    public static GameObject FindChild(this GameObject gameObject, string name)
    {
        Transform transform = gameObject.transform;
        for (int i = 0; i < transform.childCount; i++)
        {
            // Debug.Log(transform.GetChild(i).gameObject.name);
            if (transform.GetChild(i).gameObject.name == name)
            {
                return transform.GetChild(i).gameObject;
            }
            if (transform.GetChild(i).childCount != 0)
            {
                return transform.GetChild(i).gameObject.FindChild(name);
            }
        }
        return null;
    }

    static bool rngCreate = false;
    static System.Random rand;
    static public int Rand(int n1, int n2)
    {
        int min = n1;
        int max = n2;
        if (n1 > n2)
        {
            min = n2;
            max = n1;
        }
        if (!rngCreate)
        {
            rand = new System.Random();
            rngCreate = true;
        }
        return rand.Next(min, max);
    }
}
