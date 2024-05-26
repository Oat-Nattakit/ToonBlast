using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance; 
    [SerializeField] private Board _board;
    private SymbolColor goal1Type, goal2Type;

    private void Awake()
    {
        instance = this;
    }
    public void Init()
    {
        this._board.InitBoard();
    }    
}
