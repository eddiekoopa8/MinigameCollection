using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Unityls /* shameless pum */
{
    public static GameObject FindChild(this GameObject gameObject, string name)
    {
        // transform has all of the children info
        Transform transform = gameObject.transform;
        for (int i = 0; i < transform.childCount; i++) // go thorough all the children
        {
            // Debug.Log(transform.GetChild(i).gameObject.name);
            // same name?
            if (transform.GetChild(i).gameObject.name == name)
            {
                // found it!!
                return transform.GetChild(i).gameObject;
            }
            // oh? does the child have children too? well lets go through those!
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
        // normally, argument 1 is smaller than argument 2
        int min = n1;
        int max = n2;

        // but it doesn't matter if it's other way round
        // as we can change it.
        if (n1 > n2)
        {
            min = n2;
            max = n1;
        }

        // create random generatoer object if we havent
        if (!rngCreate)
        {
            rand = new System.Random();
            rngCreate = true;
        }

        // get random value!
        return rand.Next(min, max);
    }
}
