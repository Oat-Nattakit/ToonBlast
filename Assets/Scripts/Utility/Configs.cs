using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Configs 
{
    public static Configs instance = null;

    void Awake()
    {
        Configs.instance = this;
    }
}
