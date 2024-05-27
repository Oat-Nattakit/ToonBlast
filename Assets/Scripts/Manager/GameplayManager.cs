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

    

    private async void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
        switch (_symbol.TypeSymbol)
        {
            case (SymbolType.Bomb):
                this.FindNearberSymbol.FindNerberSpaMatch(_symbol);
                this._CollectSymbolMatch(_symbol, symbolMatch);
                break;
            case (SymbolType.Disco):
                this.FindNearberSymbol.FindNerberSpaMatch(_symbol);
                this._CollectSymbolMatch(_symbol, symbolMatch);
                break;
            case (SymbolType.Normal):
                this._CollectSymbolMatch(_symbol, symbolMatch);
                this._CheckConditionCreateSpecial(symbolMatch);
                break;
        }       

        await this.RemoveSymbol.RemoveSymbolObject(symbolMatch);
        this.RemoveSymbol.Refill((value) => this.BoardGame.SpawnSymbolAtTop(value));
        await UniTask.Delay(200);
        this.FindNearberSymbol.FindNerberNormalMatch();
    }

    private void _CollectSymbolMatch(Symbol symbol, List<Symbol> symbolMatch)
    {
        if (symbol.currenMatch.Count > 0)
        {
            symbolMatch.Add(symbol);
            foreach (Symbol item in symbol.currenMatch)
            {
                var findSymbol = symbolMatch.Find((sym) => sym == item);
                if (findSymbol == null)
                {
                    this._CollectSymbolMatch(item, symbolMatch);
                }
            }
        }
    }


    #region Special 
    private void _CheckConditionCreateSpecial(List<Symbol> destoryList)
    {
        int limitBomb = GameManager.instance.LimitBomb;
        int limitDisco = GameManager.instance.LimitDisco;

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
