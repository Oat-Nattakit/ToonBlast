using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class Board : MonoBehaviour
{

    public static Board Instance;

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

    private void Awake()
    {
        Instance = this;
    }

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
                GameObject symbols = Instantiate(this.symbolPrefabs[randomIndex], this.boardGameGo.transform);
                symbols.transform.localPosition = position;
                symbols.GetComponent<Symbol>().SetIndicies(x, y);
                symbols.name = "[" + x + "," + y + "]";
                this._boardGame[x, y] = new Node(true, symbols);
                this.symbolsDestory.Add(symbols);
            }
        }

        //CheckBoard(false);
        this._findNerber();
    }

    public bool CheckBoard(bool _takeAction)
    {

        bool hasMatch = false;

        List<Symbol> removeSymbol = new List<Symbol>();

        foreach (Node node in this._boardGame)
        {
            if (node.symbol != null)
            {
                node.symbol.GetComponent<Symbol>().isMatch = false;
            }
        }

        for (int x = 0; x < this.BoardWidth; x++)
        {
            for (int y = 0; y < this.BoardHight; y++)
            {
                if (this._boardGame[x, y].isUsable)
                {
                    Symbol symbols = this._boardGame[x, y].symbol.GetComponent<Symbol>();

                    if (symbols.isMatch == false)
                    {
                        MatchResult matchResult = this.IsConnect(symbols);

                        if (matchResult.connectSymbols.Count >= 2)
                        {
                            MatchResult superMatch = this.superMatch(matchResult);

                            removeSymbol.AddRange(superMatch.connectSymbols);
                            foreach (Symbol symbol in superMatch.connectSymbols)
                            {
                                symbol.isMatch = true;
                            }
                            hasMatch = true;
                        }
                    }
                }
            }
        }
        if (_takeAction)
        {
            foreach (Symbol symbol in removeSymbol)
            {
                symbol.isMatch = false;
            }
            this.RemoveAndRefill(removeSymbol);

            if (this.CheckBoard(false))
            {
                CheckBoard(true);
            }
        }

        return hasMatch;
    }


    public void checkConnect(bool _takeAction)
    {
        Symbol symbol = this.selectSymbol;
        MatchResult matchResult = this.IsConnect(symbol);

        if (matchResult.connectSymbols.Count >= 2)
        {
            MatchResult superMatch = this.superMatch(matchResult);

            foreach (Symbol sym in superMatch.connectSymbols)
            {
                sym.symbolImageObj.color = Color.cyan;
            }
        }

    }


    #region match
    private MatchResult superMatch(MatchResult matchSymbol)
    {
        if (matchSymbol.direction == MatchDirection.Horizontal || matchSymbol.direction == MatchDirection.LongHorozontal)
        {
            foreach (Symbol symbol in matchSymbol.connectSymbols)
            {
                List<Symbol> exConnect = new List<Symbol>();

                CheckDirection(symbol, new Vector2Int(0, 1), exConnect);
                CheckDirection(symbol, new Vector2Int(0, -1), exConnect);

                if (exConnect.Count >= 2)
                {
                    Debug.LogWarning("super Hori");
                    exConnect.AddRange(matchSymbol.connectSymbols);
                    return new MatchResult
                    {
                        connectSymbols = exConnect,
                        direction = MatchDirection.Super,
                    };
                }
            }
            return new MatchResult
            {
                connectSymbols = matchSymbol.connectSymbols,
                direction = matchSymbol.direction,
            };
        }
        else if (matchSymbol.direction == MatchDirection.Vertical || matchSymbol.direction == MatchDirection.LongVertical)
        {
            foreach (Symbol symbol in matchSymbol.connectSymbols)
            {
                List<Symbol> exConnect = new List<Symbol>();

                CheckDirection(symbol, new Vector2Int(1, 0), exConnect);
                CheckDirection(symbol, new Vector2Int(-1, 0), exConnect);

                if (exConnect.Count >= 2)
                {
                    Debug.LogWarning("super Verti");
                    exConnect.AddRange(matchSymbol.connectSymbols);
                    return new MatchResult
                    {
                        connectSymbols = exConnect,
                        direction = MatchDirection.Super,
                    };
                }
            }
            return new MatchResult
            {
                connectSymbols = matchSymbol.connectSymbols,
                direction = matchSymbol.direction,
            };
        }
        return null;
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

        /* connectSymbol.Clear();
         connectSymbol.Add(symbols);*/

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
    #endregion

    #region swapping
    public void SelectSymbols(Symbol _symbol)
    {
        /*if (this.selectSymbol == null)
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
        }*/
        List<Symbol> symbolMatch = new List<Symbol>();
        this.selecttoRemove(_symbol, symbolMatch);
        this.RemoveAndRefill(symbolMatch);
        this._findNerber();
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
        GameObject newSymbol = Instantiate(this.symbolPrefabs[randomValue], this.boardGameGo.transform);     
        newSymbol.transform.localPosition = new Vector2((x * spacingX), this.BoardHight * spacingY);
        newSymbol.GetComponent<Symbol>().SetIndicies(x, index);
        this._boardGame[x, index] = new Node(true, newSymbol);
        Vector3 targetPos = new Vector3(newSymbol.transform.localPosition.x, newSymbol.transform.localPosition.y - (locationToMove* spacingY), newSymbol.transform.localPosition.z);
        newSymbol.GetComponent<Symbol>().MovaToTarget(targetPos);
    }

    private int FindIndexOfLowerNull(int x)
    {
        int lowerNull = 99;
        for (int y = (this.BoardWidth - 1); y >= 0; y--)

        {
            //Debug.LogWarning(x + " " + y);
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

    private void selecttoRemove(Symbol symbol, List<Symbol> symbolMatch)
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
                    this.selecttoRemove(item, symbolMatch);
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
