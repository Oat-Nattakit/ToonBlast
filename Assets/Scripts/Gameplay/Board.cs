using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Board : MonoBehaviour
{
    private int _boardWidth = 0;
    private int _boardHight = 0;

    private int _spacingX = 0;
    private int _spacingY = 0;

    public GameObject symbolPrefabs;

    private Node[,] _boardGame;
    public Node[,] BoardGame { get => this._boardGame; }

    public GameObject ParentBoard;

    [SerializeField]private float _durationMove = 0.2f;

    public void Init()
    {
        this._boardHight = GameManager.instance.BoardHight;
        this._boardWidth = GameManager.instance.BoardWidth;

        this._spacingX = GameManager.instance.spacingX;
        this._spacingY = GameManager.instance.spacingY;
    }


    public void InitBoard()
    {
        this._boardGame = new Node[_boardWidth, _boardHight];

        for (int y = 0; y < _boardHight; y++)
        {
            for (int x = 0; x < _boardWidth; x++)
            {
                Vector2 position = new Vector2(x * _spacingX, y * _spacingY);
                int randomIndex = this._RandomColorSymbol();
                GameObject symbols = Instantiate(this.symbolPrefabs, this.ParentBoard.transform);
                symbols.transform.localPosition = position;
                symbols.GetComponent<Symbol>().SetIndicies(x, y);
                symbols.GetComponent<Symbol>().InitSymbol(SymbolType.Normal, (SymbolColor)randomIndex);
                symbols.name = "Symbol_" + (SymbolColor)randomIndex;
                this._boardGame[x, y] = new Node(true, symbols);
            }
        }

        this._SetBoardSize();
    }

    private void _SetBoardSize()
    {
        float boardSizeX = (this._boardHight * this._spacingX);
        float boardSizeY = (this._boardWidth * this._spacingY);
        this.ParentBoard.transform.localPosition = new Vector2(-boardSizeX / 2, -boardSizeY / 2);
    }

    public async UniTask<List<int>> Refill()
    {
        List<int> spawPosX = new List<int>();
        for (int x = 0; x < this._boardWidth; x++)
        {
            for (int y = 0; y < this._boardHight; y++)
            {
                if (this._boardGame[x, y].symbol == null)
                {
                    this.RefillBoard(x, y, spawPosX);
                }
            }
        }
        await UniTask.WaitForSeconds(this._durationMove);
        return spawPosX;
    }

    private void RefillBoard(int x, int y, List<int> spawPosX)
    {
        int yOffset = 1;
        while ((y + yOffset < this._boardHight) && (this._boardGame[x, y + yOffset].symbol == null))
        {
            yOffset++;
        }

        if ((y + yOffset < this._boardHight) && (this._boardGame[x, y + yOffset].symbol != null))
        {
            Symbol symbolAbove = this._boardGame[x, y + yOffset].symbol.GetComponent<Symbol>();

            Vector3 targetPos = new Vector3((x * this._spacingX), (y * this._spacingY), 0);

            symbolAbove.MovaToTarget(targetPos, this._durationMove);

            symbolAbove.SetIndicies(x, y);
            this._boardGame[x, y] = this._boardGame[x, y + yOffset];
            this._boardGame[x, y + yOffset] = new Node(true, null);
        }
        if (y + yOffset == this._boardHight)
        {
            spawPosX.Add(x);
        }
    }

    public async UniTask SpawnSymbolAddPosX(Vector2Int refIndex, List<int> spawPosX, SymbolSpecial special)
    {
        List<Vector2Int> collectPos = new List<Vector2Int>();
        List<GameObject> collectSym = new List<GameObject>();
        for (int i = 0; i < spawPosX.Count; i++)
        {
            int index = this._FindIndexOfLowerNull(spawPosX[i]);
            collectPos.Add(new Vector2Int(spawPosX[i], index));

            int randomIndex = this._RandomColorSymbol();
            SymbolColor _color = (SymbolColor)randomIndex;
            this.SpawnSymbolAtTop(spawPosX[i], index, SymbolType.Normal, _color, collectSym);
        }
        var min = collectPos.OrderBy(v => v.sqrMagnitude).First();
        this._SetupSymboltype(min, collectSym, special);
        collectSym.ForEach((symbol) => { this.MoveSymbolToPosition(symbol); });
        await UniTask.WaitForSeconds(this._durationMove);
    }

    public void SpawnSymbolAtTop(int x, int index, SymbolType _type, SymbolColor _color, List<GameObject> collectSym)
    {
        GameObject newSymbol = Instantiate(this.symbolPrefabs, this.ParentBoard.transform);
        Symbol symbols = newSymbol.GetComponent<Symbol>();
        symbols.transform.localPosition = new Vector2((x * this._spacingX), ((this._boardHight * this._spacingY) / 2) + ((index) * this._spacingY));
        symbols.SetIndicies(x, index);
        this._boardGame[x, index] = new Node(true, newSymbol);
        collectSym.Add(newSymbol);
    }

    private void _SetupSymboltype(Vector2Int target, List<GameObject> collectSym, SymbolSpecial special)
    {
        foreach (var item in collectSym)
        {
            Symbol symbols = item.GetComponent<Symbol>();

            int randomIndex = this._RandomColorSymbol();
            SymbolColor _color = (SymbolColor)randomIndex;
            SymbolType _type = SymbolType.Normal;

            if (symbols.xIndex == target.x && symbols.yIndex == target.y)
            {
                _type = special.SymbolType;
                if (special.SymbolType == SymbolType.Disco)
                {
                    _color = special.SymbolColor;
                }
            }
            symbols.InitSymbol(_type, _color);
            string symName = _type == SymbolType.Normal ? _color.ToString() : _type.ToString();
            symbols.name = "Symbol_" + symName;
        }
    }

    private void MoveSymbolToPosition(GameObject newSymbol)
    {
        Symbol symbols = newSymbol.GetComponent<Symbol>();
        Vector3 targetPos = new Vector3(newSymbol.transform.localPosition.x, (symbols.yIndex * this._spacingY), newSymbol.transform.localPosition.z);
        newSymbol.GetComponent<Symbol>().MovaToTarget(targetPos, this._durationMove);
    }

    private int _RandomColorSymbol()
    {
        int rangeValue = Enum.GetNames(typeof(SymbolColor)).Length;
        int randomIndex = UnityEngine.Random.Range(0, rangeValue);
        return randomIndex;
    }

    private int _FindIndexOfLowerNull(int x)
    {
        int lowerNull = 99;
        for (int y = (this._boardWidth - 1); y >= 0; y--)
        {
            if (this._boardGame[x, y].symbol == null)
            {
                lowerNull = y;
            }
        }
        return lowerNull;
    }
}
