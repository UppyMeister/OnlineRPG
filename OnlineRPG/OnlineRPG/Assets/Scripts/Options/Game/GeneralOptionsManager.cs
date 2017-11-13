﻿using UnityEngine;

public class GeneralOptionsManager : MonoBehaviour
{
    #region Singleton
    public static GeneralOptionsManager singleton;
    #endregion

    void Awake()
    {
        if (singleton != null)
        {
            Debug.LogError("More than one General Options Manager on the client!");
            return;
        }

        singleton = this;
    }
}