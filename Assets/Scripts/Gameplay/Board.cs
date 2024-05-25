using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    public int BoardWidth = 8;
    public int BoardHight = 8;

    public float spacingX = 0;
    public float spacingY = 0;

    public GameObject[] symbolPrefabs;

    private Node[,] _boardGame;
    public GameObject boardGameGo;

    public List<GameObject> symbolsDestory = new List<GameObject>();

    [SerializeField] private Symbol selectSymbol;
    [SerializeField] private bool isSymbolMove;

    public ArrayList boards = new ArrayList();

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Symbol>())
            {
                if (this.isSymbolMove)
                {
                    return;
                }
                Symbol symbol = hit.collider.gameObject.GetComponent<Symbol>();
                 Debug.LogWarning("Hit  :"+ symbol.gameObject.name);
                this.SelectSymbols(symbol);
            }
        }
    }

    public void InitBoard()
    {
        this.DestorySymbole();
        this._boardGame = new Node[BoardWidth, BoardHight];

        for (int y = 0; y < BoardHight; y++)
        {
            for (int x = 0; x < BoardWidth; x++)
            {
                Vector2 position = new Vector2(x * spacingX, y * spacingY);
                int randomIndex = Random.Range(0, this.symbolPrefabs.Length);
                GameObject symbols = Instantiate(this.symbolPrefabs[randomIndex], this.boardGameGo.transform);
                symbols.transform.localPosition = position;
                symbols.GetComponent<Symbol>().SetIndicies(x, y);
                symbols.name = "[" + x + "," + y + "]";
                this._boardGame[x, y] = new Node(true, symbols);
                this.symbolsDestory.Add(symbols);
            }
        }


        CheckBoard();
    }

    private void DestorySymbole()
    {
        if (this.symbolsDestory != null)
        {
            foreach (GameObject symbol in this.symbolsDestory)
            {
                Destroy(symbol);
            }
            this.symbolsDestory.Clear();
        }
    }

    public bool CheckBoard()
    {
        // Debug.LogWarning("Check Board");
        bool hasMatch = false;

        List<Symbol> removeSymbol = new List<Symbol>();

        for (int x = 0; x < this.BoardWidth; x++)
        {
            for (int y = 0; y < this.BoardHight; y++)
            {
                if (this._boardGame[x, y].isUsable)
                {
                    Symbol symbols = this._boardGame[x, y].symbol.GetComponent<Symbol>();

                    if (!symbols.isMatch)
                    {
                        //Debug.LogWarning(symbols.name);
                        MatchResult matchResult = this.IsConnect(symbols);
                        if (matchResult.connectSymbols.Count >= 2)
                        {
                            removeSymbol.AddRange(matchResult.connectSymbols);
                            foreach (Symbol symbol in matchResult.connectSymbols)
                            {
                                symbol.isMatch = true;
                            }
                            hasMatch = true;
                        }
                    }
                }
            }
        }
        return hasMatch;
    }

    private MatchResult IsConnect(Symbol symbols)
    {
        List<Symbol> connectSymbol = new List<Symbol>();
        SymbolColor type = symbols.ColorSymbol;

        connectSymbol.Add(symbols);

        this.CheckDirection(symbols, new Vector2Int(1, 0), connectSymbol);
        this.CheckDirection(symbols, new Vector2Int(-1, 0), connectSymbol);

        if (connectSymbol.Count == 2)
        {
            //Debug.LogWarning("Hori Map" + connectSymbol[0].ColorSymbol);
            return new MatchResult
            {
                connectSymbols = connectSymbol,
                direction = MatchDirection.Horizontal,
            };
        }
        else if (connectSymbol.Count > 2)
        {
            //Debug.LogWarning("Hori Map Long" + connectSymbol[0].ColorSymbol);
            return new MatchResult
            {
                connectSymbols = connectSymbol,
                direction = MatchDirection.LongHorozontal,
            };
        }

        connectSymbol.Clear();
        connectSymbol.Add(symbols);



        this.CheckDirection(symbols, new Vector2Int(0, 1), connectSymbol);
        this.CheckDirection(symbols, new Vector2Int(0, -1), connectSymbol);

        if (connectSymbol.Count == 2)
        {
            // Debug.LogWarning("Verti Map" + connectSymbol[0].ColorSymbol);
            return new MatchResult
            {
                connectSymbols = connectSymbol,
                direction = MatchDirection.Vertical,
            };
        }
        else if (connectSymbol.Count > 2)
        {
            // Debug.LogWarning("Verti Map Long" + connectSymbol[0].ColorSymbol);
            return new MatchResult
            {
                connectSymbols = connectSymbol,
                direction = MatchDirection.LongVertical,
            };
        }
        else
        {
            return new MatchResult
            {
                connectSymbols = connectSymbol,
                direction = MatchDirection.None,
            };
        }
    }

    private void CheckDirection(Symbol symbol, Vector2Int direction, List<Symbol> connectSymbol)
    {
        SymbolColor color = symbol.ColorSymbol;
        int x = symbol.xIndex + direction.x;
        int y = symbol.yIndex + direction.y;

        while (x >= 0 && x < BoardWidth && y >= 0 && y < BoardHight)
        {
            if (this._boardGame[x, y].isUsable)
            {
                Symbol symbolNear = this._boardGame[x, y].symbol.GetComponent<Symbol>();

                if (!symbolNear.isMatch && symbolNear.ColorSymbol == color)
                {
                    connectSymbol.Add(symbolNear);
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

    #region swapping
    public void SelectSymbols(Symbol _symbol)
    {
        if (this.selectSymbol == null)
        {
            selectSymbol = _symbol;
        }
        else if (this.selectSymbol == _symbol)
        {
            this.selectSymbol = null;
        }
        else if (this.selectSymbol != _symbol)
        {
            this.SwapSymbol(this.selectSymbol, _symbol);
            this.selectSymbol = null;
        }
    }

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
        bool hasMatch = CheckBoard();
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
    #endregion

}

public class MatchResult
{
    public List<Symbol> connectSymbols;
    public MatchDirection direction;
}
