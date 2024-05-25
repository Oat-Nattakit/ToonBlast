using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool isUsable;
    public GameObject symbol;

    public Node(bool _isUsable, GameObject _symbol)
    {
        this.isUsable = _isUsable;
        this.symbol = _symbol;
    }
}

