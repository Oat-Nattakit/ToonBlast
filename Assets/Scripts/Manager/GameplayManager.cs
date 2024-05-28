using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private Board BoardGame;
    [SerializeField] private FindNearberSymbol FindNearberSymbol;
    [SerializeField] private RemoveSymbol RemoveSymbol;

    private int BoardWidth = 0;
    private int BoardHight = 0;

    private int spacingX = 0;
    private int spacingY = 0;

    private bool isMove = false;

    private Action<int> _callBaclScore;    

    public void Init()
    {
        this.BoardHight = GameManager.instance.BoardHight;
        this.BoardWidth = GameManager.instance.BoardWidth;

        this.spacingX = GameManager.instance.spacingX;
        this.spacingY = GameManager.instance.spacingY;

        this.BoardGame.Init();
        this.BoardGame.InitBoard();

        this.FindNearberSymbol.Init(this.BoardGame.BoardGame);
        this.FindNearberSymbol.FindNerberNormalMatch();

        this.RemoveSymbol.Init(this.BoardGame.BoardGame);
    }

    public void ResetBoard()
    {
        this._callBaclScore = null;
        this.BoardGame.ClearBoard();
        
    }

    public void InitCallbackScore(Action<int> _callback)
    {
        this._callBaclScore = _callback;
    }

    private async void Update()
    {
        if (Input.GetMouseButtonDown(0) && this.isMove == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Symbol>())
            {
                Symbol symbol = hit.collider.gameObject.GetComponent<Symbol>();
                await this.SelectSymbols(symbol);
            }
        }
    }

    public async UniTask SelectSymbols(Symbol _symbol)
    {
        List<Symbol> symbolMatch = new List<Symbol>();
        SymbolSpecial special = new SymbolSpecial();

        switch (_symbol.TypeSymbol)
        {
            case (SymbolType.Bomb):
                this.FindNearberSymbol.FindNerberSpaMatch(_symbol);
                this._CollectSymbolSpecialMatch(_symbol, symbolMatch);
                break;
            case (SymbolType.Disco):
                this.FindNearberSymbol.FindNerberSpaMatch(_symbol);
                this._CollectSymbolSpecialMatch(_symbol, symbolMatch);
                break;
            case (SymbolType.Normal):
                this._CollectSymbolNormalMatch(_symbol, symbolMatch);
                special = this._CheckConditionCreateSpecial(_symbol, symbolMatch);
                break;
        }

        if (symbolMatch.Count > 0)
        {
            this.isMove = true;
            this._callBaclScore.Invoke(symbolMatch.Count); 
            Vector2Int refIndex = new Vector2Int(_symbol.xIndex,_symbol.yIndex);
            await this.RemoveSymbol.RemoveSymbolObject(symbolMatch);
            List<int> spawnPosx = await this.BoardGame.Refill();
            await this.BoardGame.SpawnSymbolAddPosX(refIndex, spawnPosx, special);
            await UniTask.Delay(100);
            this.FindNearberSymbol.FindNerberNormalMatch();
            
            this.isMove = false;
        }
    }

    private void _CollectSymbolNormalMatch(Symbol symbol, List<Symbol> symbolMatch)
    {
        if (symbol.currenMatch.Count > 0)
        {
            symbolMatch.Add(symbol);
            foreach (Symbol item in symbol.currenMatch)
            {
                var findSymbol = symbolMatch.Find((sym) => sym == item);
                if (findSymbol == null)
                {
                    this._CollectSymbolNormalMatch(item, symbolMatch);
                }
            }
        }
    }

    private void _CollectSymbolSpecialMatch(Symbol symbol, List<Symbol> symbolMatch)
    {
        if (symbol.currenMatch.Count > 0)
        {
            symbolMatch.Add(symbol);
            foreach (Symbol item in symbol.currenMatch)
            {
                symbolMatch.Add(item);
            }
        }
    }


    #region Special 
    private SymbolSpecial _CheckConditionCreateSpecial(Symbol _symbol, List<Symbol> destoryList)
    {
        int limitBomb = GameManager.instance.LimitBomb;
        int limitDisco = GameManager.instance.LimitDisco;
        SymbolSpecial special = new SymbolSpecial();
        if (destoryList.Count >= limitDisco)
        {
            special.SymbolType = SymbolType.Disco;
            special.SymbolColor = _symbol.ColorSymbol;


        }
        else if (destoryList.Count >= limitBomb)
        {
            special.SymbolType = SymbolType.Bomb;
            special.SymbolColor = _symbol.ColorSymbol;
        }
        return special;
    }
    #endregion
}

public class SymbolSpecial
{
    public SymbolType SymbolType;
    public SymbolColor SymbolColor;
}
