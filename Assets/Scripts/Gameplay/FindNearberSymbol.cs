using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNearberSymbol : MonoBehaviour
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

    public void FindNerberNormalMatch()
    {
        this._ClearFindMatch();
        for (int x = 0; x < this.BoardWidth; x++)
        {
            for (int y = 0; y < this.BoardHight; y++)
            {
                if (this._boardGame[x, y].isUsable)
                {
                    Symbol symbols = this._boardGame[x, y].symbol.GetComponent<Symbol>();
                    this._FindNormalMatching(symbols, new Vector2Int(0, 1));
                    this._FindNormalMatching(symbols, new Vector2Int(0, -1));
                    this._FindNormalMatching(symbols, new Vector2Int(1, 0));
                    this._FindNormalMatching(symbols, new Vector2Int(-1, 0));
                    //this._FindMatchSymbolType(symbols);
                }
            }
        }
    }

    public void FindNerberSpaMatch(Symbol symbols)
    {
        this._ClearFindMatch();
        this._FindMatchSymbolType(symbols);       
    }

    private void _ClearFindMatch()
    {
        for (int x = 0; x < this.BoardWidth; x++)
        {
            for (int y = 0; y < this.BoardHight; y++)
            {
                if (this._boardGame[x, y].isUsable)
                {
                    Symbol symbols = this._boardGame[x, y].symbol.GetComponent<Symbol>();
                    symbols.currenMatch.Clear();
                }
            }
        }
    }

    private void _FindMatchSymbolType(Symbol symbols)
    {
        switch (symbols.TypeSymbol)
        {
            case (SymbolType.Bomb):
                this._FindSpecialBombMatching(symbols, new Vector2Int(0, 1));
                this._FindSpecialBombMatching(symbols, new Vector2Int(0, -1));
                this._FindSpecialBombMatching(symbols, new Vector2Int(1, 0));
                this._FindSpecialBombMatching(symbols, new Vector2Int(-1, 0));
                break;
            case (SymbolType.Disco):
                this._FindSpecialDiscoMatching(symbols, new Vector2Int(0, 1));
                this._FindSpecialDiscoMatching(symbols, new Vector2Int(0, -1));
                this._FindSpecialDiscoMatching(symbols, new Vector2Int(1, 0));
                this._FindSpecialDiscoMatching(symbols, new Vector2Int(-1, 0));
                break;           
        }
    }

    private void _FindNormalMatching(Symbol symbol, Vector2Int pos)
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

    public void _FindSpecialBombMatching(Symbol symbol, Vector2Int direction)
    {
        SymbolColor color = symbol.ColorSymbol;

        int x = symbol.xIndex + direction.x;
        int y = symbol.yIndex + direction.y;

        while (x >= 0 && x < BoardWidth && y >= 0 && y < BoardHight)
        {
            if (this._boardGame[x, y].isUsable)
            {
                Symbol symbolNear = this._boardGame[x, y].symbol.GetComponent<Symbol>();

                if (symbolNear.ColorSymbol == color)
                {
                    symbol.currenMatch.Add(symbolNear);
                    x += direction.x;
                    y += direction.y;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }

    public void _FindSpecialDiscoMatching(Symbol _symbol, Vector2Int direction)
    {
        SymbolColor color = _symbol.ColorSymbol;

        for (int x = 0; x < this.BoardWidth; x++)
        {
            for (int y = 0; y < this.BoardHight; y++)
            {
                if (this._boardGame[x, y].isUsable)
                {
                    Symbol symbols = this._boardGame[x, y].symbol.GetComponent<Symbol>();

                    if (symbols.ColorSymbol == color)
                    {
                        _symbol.currenMatch.Add(symbols);
                    }                   
                }
            }
        }        
    }
}