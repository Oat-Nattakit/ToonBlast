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
           
            symbol.symbolImageObj.color = Color.white;  

            this._boardGame[Xindex, Yindex] = new Node(true, null);
        }
        await UniTask.Delay(500);
        foreach (Symbol symbol in symbolRemove)
        {
            int Xindex = symbol.xIndex;
            int Yindex = symbol.yIndex;

            Destroy(symbol.gameObject);

            this._boardGame[Xindex, Yindex] = new Node(true, null);
        }

        await UniTask.Delay(100);
    }

    public void Refill(Action<int> Callback)
    {
        for (int x = 0; x < this.BoardWidth; x++)
        {
            for (int y = 0; y < this.BoardHight; y++)
            {
                if (this._boardGame[x, y].symbol == null)
                {
                    this.RefillBoard(x, y, Callback);
                }
            }
        }
    }

    private void RefillBoard(int x, int y, Action<int> Callback)
    {
        int yOffset = 1;
        while ((y + yOffset < this.BoardHight) && (this._boardGame[x, y + yOffset].symbol == null))
        {
            yOffset++;
        }

        if ((y + yOffset < this.BoardHight) && (this._boardGame[x, y + yOffset].symbol != null))
        {
            Symbol symbolAbove = this._boardGame[x, y + yOffset].symbol.GetComponent<Symbol>();

            Vector3 targetPos = new Vector3((x * spacingX), (y * spacingY), 0);

            symbolAbove.MovaToTarget(targetPos);

            symbolAbove.SetIndicies(x, y);
            this._boardGame[x, y] = this._boardGame[x, y + yOffset];
            this._boardGame[x, y + yOffset] = new Node(true, null);
        }
        if (y + yOffset == this.BoardHight)
        {
            Callback.Invoke(x);           
        }
    }  
}
