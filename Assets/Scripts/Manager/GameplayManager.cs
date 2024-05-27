using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private Board _board;
    private FindNearberSymbol FindNearberSymbol = new FindNearberSymbol();

    private int BoardWidth = 0;
    private int BoardHight = 0;

    private int spacingX = 0;
    private int spacingY = 0;

    public GameObject[] symbolPrefabs;

    //private Node[,] _board.BoardGame;
    public GameObject ParentBoard;

    public void Init()
    {
        this.BoardHight = GameManager.instance.BoardHight;
        this.BoardWidth = GameManager.instance.BoardWidth;

        this.spacingX = GameManager.instance.spacingX;
        this.spacingY = GameManager.instance.spacingY;

        //this.InitBoard();
        this._board.Init();
        this._board.InitBoard();

        this.FindNearberSymbol.Init(this._board.BoardGame);
        this.FindNearberSymbol._findNerber();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Symbol>())
            {
                Symbol symbol = hit.collider.gameObject.GetComponent<Symbol>();
                this.SelectSymbols(symbol);
            }
        }
    }

    /*public void InitBoard()
    {
        this._board.BoardGame = new Node[BoardWidth, BoardHight];

        for (int y = 0; y < BoardHight; y++)
        {
            for (int x = 0; x < BoardWidth; x++)
            {
                Vector2 position = new Vector2(x * spacingX, y * spacingY);
                int randomIndex = Random.Range(0, this.symbolPrefabs.Length);
                GameObject symbols = Instantiate(this.symbolPrefabs[randomIndex], this.ParentBoard.transform);
                symbols.transform.localPosition = position;
                symbols.GetComponent<Symbol>().SetIndicies(x, y);
                symbols.name = "[" + x + "," + y + "]";
                this._board.BoardGame[x, y] = new Node(true, symbols);
            }
        }

        this._findNerber();
        this._setBoardSize();
    }*/

    /*private void _setBoardSize()
    {
        float boardSizeX = (this.BoardHight * this.spacingX);
        float boardSizeY = (this.BoardWidth * this.spacingY);
        this.ParentBoard.transform.localPosition = new Vector2(-boardSizeX / 2, -boardSizeY / 2);
    }*/

    #region match
    public void SelectSymbols(Symbol _symbol)
    {
        List<Symbol> symbolMatch = new List<Symbol>();
        this._collectSymbolMatch(_symbol, symbolMatch);
        this._checkConditionCreateSpecial(symbolMatch);
        this.RemoveAndRefill(symbolMatch);

        this.FindNearberSymbol._findNerber();
    }

    /*private void _findNerber()
    {
        for (int x = 0; x < this.BoardWidth; x++)
        {
            for (int y = 0; y < this.BoardHight; y++)
            {
                if (this._board.BoardGame[x, y].isUsable)
                {
                    Symbol symbols = this._board.BoardGame[x, y].symbol.GetComponent<Symbol>();
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

            Symbol symbolNear = this._board.BoardGame[xIndex, yIndex].symbol.GetComponent<Symbol>();
            if (symbolNear.ColorSymbol == color)
            {
                symbol.currenMatch.Add(symbolNear);
            }
        }

    }*/

    private void _collectSymbolMatch(Symbol symbol, List<Symbol> symbolMatch)
    {
        if (symbol.currenMatch.Count > 0)
        {
            symbolMatch.Add(symbol);
            foreach (Symbol item in symbol.currenMatch)
            {
                var findSymbol = symbolMatch.Find((sym) => sym == item);                
                if (findSymbol == null)
                {                   
                    this._collectSymbolMatch(item, symbolMatch);
                }
            }
        }
    }
    #endregion

    #region remove and refill
    private void RemoveAndRefill(List<Symbol> symbolRemove)
    {
        foreach (Symbol symbol in symbolRemove)
        {
            int Xindex = symbol.xIndex;
            int Yindex = symbol.yIndex;

            Destroy(symbol.gameObject);


            this._board.BoardGame[Xindex, Yindex] = new Node(true, null);
        }

        for (int x = 0; x < this.BoardWidth; x++)
        {
            for (int y = 0; y < this.BoardHight; y++)
            {
                if (this._board.BoardGame[x, y].symbol == null)
                {
                    this.RefillBoard(x, y);
                }
            }
        }
    }

    private void RefillBoard(int x, int y)
    {
        int yOffset = 1;
        while ((y + yOffset < this.BoardHight) && (this._board.BoardGame[x, y + yOffset].symbol == null))
        {
            yOffset++;
        }

        if ((y + yOffset < this.BoardHight) && (this._board.BoardGame[x, y + yOffset].symbol != null))
        {
            Symbol symbolAbove = this._board.BoardGame[x, y + yOffset].symbol.GetComponent<Symbol>();

            Vector3 targetPos = new Vector3((x * spacingX), (y * spacingY), 0);

            symbolAbove.MovaToTarget(targetPos);

            symbolAbove.SetIndicies(x, y);
            this._board.BoardGame[x, y] = this._board.BoardGame[x, y + yOffset];
            this._board.BoardGame[x, y + yOffset] = new Node(true, null);
        }
        if (y + yOffset == this.BoardHight)
        {
            this.SpawnSymbolAtTop(x);
        }
    }

    private void SpawnSymbolAtTop(int x)
    {
        int index = this.FindIndexOfLowerNull(x);
        int locationToMove = this.BoardHight - index;
        int randomValue = Random.Range(0, this.symbolPrefabs.Length);
        GameObject newSymbol = Instantiate(this.symbolPrefabs[randomValue], this.ParentBoard.transform);
        Symbol sym = newSymbol.GetComponent<Symbol>();
        sym.transform.localPosition = new Vector2((x * spacingX), ((this.BoardHight * spacingY) / 2) + ((index) * spacingY));
        sym.SetIndicies(x, index);
        this._board.BoardGame[x, index] = new Node(true, newSymbol);
        Vector3 targetPos = new Vector3(newSymbol.transform.localPosition.x, (sym.yIndex * spacingY), newSymbol.transform.localPosition.z);
        newSymbol.GetComponent<Symbol>().MovaToTarget(targetPos);
    }

    private int FindIndexOfLowerNull(int x)
    {
        int lowerNull = 99;
        for (int y = (this.BoardWidth - 1); y >= 0; y--)
        {
            if (this._board.BoardGame[x, y].symbol == null)
            {
                lowerNull = y;
            }
        }
        return lowerNull;
    }
    #endregion

    #region Special 
    private void _checkConditionCreateSpecial(List<Symbol> destoryList)
    {
        int limitBomb = 6;
        int limitDisco = 10;
        
        if (destoryList.Count >= limitDisco)
        {
            Debug.LogWarning("get Dicgo");
        }
        else if (destoryList.Count >= limitBomb)
        {
            Debug.LogWarning("get Bomb");
        }
    }    
    #endregion
}
