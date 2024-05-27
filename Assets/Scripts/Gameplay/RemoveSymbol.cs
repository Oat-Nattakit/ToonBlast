using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveSymbol : MonoBehaviour
{
    private int BoardWidth = 0;
    private int BoardHight = 0;

    private int spacingX = 0;
    private int spacingY = 0;

    private Node[,] _boardGame = null;

    public void Init(Node[,] _board)
    {
        this.BoardHight = GameManager.instance.BoardHight;
        this.BoardWidth = GameManager.instance.BoardWidth;

        this.spacingX = GameManager.instance.spacingX;
        this.spacingY = GameManager.instance.spacingY;

        this._boardGame = _board;
    }

    public async UniTask RemoveSymbolObject(List<Symbol> symbolRemove)
    {  
        foreach (Symbol symbol in symbolRemove)
        {
            int Xindex = symbol.xIndex;
            int Yindex = symbol.yIndex;

            Destroy(symbol.gameObject);


            this._boardGame[Xindex, Yindex] = new Node(true, null);
        }

        await UniTask.Delay(100);
    }   
}
