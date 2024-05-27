using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Symbol : MonoBehaviour
{
    [SerializeField] private Image symbolImage;
    [SerializeField] private Image symbolSpecial;    

    [SerializeField] private SymbolType _typeSymbol = SymbolType.Normal;
    [SerializeField] private SymbolColor _symbolColor = SymbolColor.Red;

    public Image symbolImageObj { get => this.symbolImage; }

    public SymbolColor ColorSymbol { get => this._symbolColor; }
    public SymbolType TypeSymbol { get => this._typeSymbol; }

    public int xIndex, yIndex; 

    public List<Symbol> currenMatch = new List<Symbol>();

    public Symbol(int x, int y)
    {
        this.xIndex = x;
        this.yIndex = y;
    }

    public void SetIndicies(int x, int y)
    {
        this.xIndex = x;
        this.yIndex = y;
    }  

    private void Start()
    {
        this._SettingSymbolType(_typeSymbol);
        this._SettingSymbolColor(_symbolColor); 
    }

    public void MovaToTarget(Vector2 _targetPos)
    {
        StartCoroutine(this.MoveCoroutine(_targetPos));
    }

    private IEnumerator MoveCoroutine(Vector2 _targetPos)
    {        
        float duration = 0.2f;

        Vector2 startPos = this.transform.localPosition;
        float elaspedTime = 0f;

        while (elaspedTime < duration)
        {
            float t = elaspedTime / duration;
            this.transform.localPosition = Vector2.Lerp(startPos, _targetPos, t);
            elaspedTime += Time.fixedDeltaTime;

            yield return null;
        }
        this.transform.localPosition = _targetPos;
    }

    private void _SettingSymbolType(SymbolType type)
    {
        switch (type)
        {
            case SymbolType.Bomb:
                this.symbolSpecial.gameObject.SetActive(true);
                break;
            case SymbolType.Disco:
                this.symbolSpecial.gameObject.SetActive(true);
                break;
            case SymbolType.Normal:
                this.symbolSpecial.gameObject.SetActive(false);
                break;

        }
    }

    private void _SettingSymbolColor(SymbolColor color)
    {
        switch (color)
        {
            case SymbolColor.Red:
                this.symbolImage.color = Color.red;
                break;
            case SymbolColor.Blue:
                this.symbolImage.color = Color.blue;
                break;
            case SymbolColor.Yellow:
                this.symbolImage.color = Color.yellow;
                break;
            case SymbolColor.Green:
                this.symbolImage.color = Color.green;
                break;

        }
    }


}
