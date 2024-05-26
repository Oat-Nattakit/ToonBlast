using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int BoardWidth = 8;
    public int BoardHight = 8;

    public int spacingX = 0;
    public int spacingY = 0;

    public GameObject[] symbolPrefabs;

    private Node[,] _boardGame;
    public GameObject ParentBoard;


    public ArrayList boards = new ArrayList();

   

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

    public void InitBoard()
    {
        this._boardGame = new Node[BoardWidth, BoardHight];

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
                this._boardGame[x, y] = new Node(true, symbols);       
            }
        }
       
        this._findNerber();
        this._setBoardSize();
    }

    private void _setBoardSize()
    {
        float boardSizeX = (this.BoardHight * this.spacingX);
        float boardSizeY = (this.BoardWidth * this.spacingY);
        this.ParentBoard.transform.localPosition = new Vector2(-boardSizeX / 2, -boardSizeY / 2);
        // this.boardGameGo.GetComponent<RectTransform>().sizeDelta = new Vector2(boardSizeX, boardSizeY);

    }   
    #region match
   
    #endregion

    #region swapping
    public void SelectSymbols(Symbol _symbol)
    {       
        List<Symbol> symbolMatch = new List<Symbol>();
        this._collectSymbolMatch(_symbol, symbolMatch);
        this.RemoveAndRefill(symbolMatch);

        this._findNerber();
    }
    /*
    private void SwapSymbol(Symbol _select, Symbol _target)
    {
        if (!IsAdjacent(_select, _target))
        {
            return;
        }
        this.DoSwap(_select, _target);
        this.isSymbolMove = true;

        StartCoroutine(this.ProcessMatche(_select, _target));

    }

    private void DoSwap(Symbol _currentSymbol, Symbol _targetSymbol)
    {
        GameObject temp = this._boardGame[_currentSymbol.xIndex, _currentSymbol.yIndex].symbol;
        this._boardGame[_currentSymbol.xIndex, _currentSymbol.yIndex].symbol = this._boardGame[_targetSymbol.xIndex, _targetSymbol.yIndex].symbol;
        this._boardGame[_targetSymbol.xIndex, _targetSymbol.yIndex].symbol = temp;

        int tempX = _currentSymbol.xIndex;
        int tempY = _currentSymbol.yIndex;

        _currentSymbol.xIndex = _targetSymbol.xIndex;
        _currentSymbol.yIndex = _targetSymbol.yIndex;

        _targetSymbol.xIndex = tempX;
        _targetSymbol.yIndex = tempY;

        _currentSymbol.MovaToTarget(this._boardGame[_targetSymbol.xIndex, _targetSymbol.yIndex].symbol.transform.localPosition);
        _targetSymbol.MovaToTarget(this._boardGame[_currentSymbol.xIndex, _currentSymbol.yIndex].symbol.transform.localPosition);
    }

    private IEnumerator ProcessMatche(Symbol _currentSymbol, Symbol _targetSymbol)
    {
        yield return new WaitForSeconds(0.2f);
        bool hasMatch = CheckBoard(true);




        if (!hasMatch)
        {
            this.DoSwap(_currentSymbol, _targetSymbol);
        }

        this.isSymbolMove = false;

    }

    private bool IsAdjacent(Symbol _currentSymbol, Symbol _targetSymbol)
    {
        return Mathf.Abs(_currentSymbol.xIndex - _targetSymbol.xIndex) + Mathf.Abs(_currentSymbol.yIndex - _targetSymbol.yIndex) == 1;
    }
    */
    #endregion

    #region remove
    private void RemoveAndRefill(List<Symbol> symbolRemove)
    {
        foreach (Symbol symbol in symbolRemove)
        {
            int Xindex = symbol.xIndex;
            int Yindex = symbol.yIndex;

            Destroy(symbol.gameObject);


            this._boardGame[Xindex, Yindex] = new Node(true, null);
        }

        for (int x = 0; x < this.BoardWidth; x++)
        {
            for (int y = 0; y < this.BoardHight; y++)
            {
                if (this._boardGame[x, y].symbol == null)
                {
                    this.RefillBoard(x, y);
                }
            }
        }
    }

    private void RefillBoard(int x, int y)
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
        sym.GetComponent<Symbol>().SetIndicies(x, index);
        this._boardGame[x, index] = new Node(true, newSymbol);
        Vector3 targetPos = new Vector3(newSymbol.transform.localPosition.x, (sym.yIndex * spacingY), newSymbol.transform.localPosition.z);
        newSymbol.GetComponent<Symbol>().MovaToTarget(targetPos);
    }

    private int FindIndexOfLowerNull(int x)
    {
        int lowerNull = 99;
        for (int y = (this.BoardWidth - 1); y >= 0; y--)
        {
            if (this._boardGame[x, y].symbol == null)
            {
                lowerNull = y;
            }
        }
        return lowerNull;
    }
    #endregion

    #region nerberMatch

    private void _findNerber()
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
            if (!symbolNear.isMatch && symbolNear.ColorSymbol == color)
            {
                symbol.currenMatch.Add(symbolNear);
            }
        }

    }

    private void _collectSymbolMatch(Symbol symbol, List<Symbol> symbolMatch)
    {
        if (symbol.currenMatch.Count > 0)
        {
            symbolMatch.Add(symbol);
            foreach (Symbol item in symbol.currenMatch)
            {
                var a = symbolMatch.Find((sym) => sym == item);
                if (a == null)
                {
                    symbolMatch.Add(item);
                    this._collectSymbolMatch(item, symbolMatch);
                }
            }
        }
    }
    #endregion

}

public class MatchResult
{
    public List<Symbol> connectSymbols;
    public MatchDirection direction;
}
