using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private Board _board;

    public void Init()
    {
        this._board.InitBoard();
    }
}
