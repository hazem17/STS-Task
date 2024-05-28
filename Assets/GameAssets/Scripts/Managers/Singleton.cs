using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)Object.FindObjectOfType(typeof(T));
            }
            return instance;
        }
    }

    public static bool Exists
    {
        get
        {
            return instance != null;
        }
    }
}
