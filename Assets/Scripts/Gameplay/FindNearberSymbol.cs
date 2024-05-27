using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNearberSymbol
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

    public void _findNerber()
    {
        for (int x = 0; x < this.BoardWidth; x++)
        {
            for (int y = 0; y < this.BoardHight; y++)
            {
                if (this._boardGame[x, y].isUsable)
                {
                    Symbol symbols = this._boardGame[x, y].symbol.GetComponent<Symbol>();
                    this.findMatching(symbols, new Vector2Int(0, 1));
                    this.findMatching(symbols, new Vector2Int(0, -1));
                    this.findMatching(symbols, new Vector2Int(1, 0));
                    this.findMatching(symbols, new Vector2Int(-1, 0));
                }
            }
        }
    }

    private void findMatching(Symbol symbol, Vector2Int pos)
    {
        SymbolColor color = symbol.ColorSymbol;
        int xIndex = symbol.xIndex + pos.x;
        int yIndex = symbol.yIndex + pos.y;
        if (xIndex >= 0 && xIndex < BoardWidth && yIndex >= 0 && yIndex < BoardHight)
        {

            Symbol symbolNear = this._boardGame[xIndex, yIndex].symbol.GetComponent<Symbol>();
            if (symbolNear.ColorSymbol == color)
            {
                symbol.currenMatch.Add(symbolNear);
            }
        }

    }
}
