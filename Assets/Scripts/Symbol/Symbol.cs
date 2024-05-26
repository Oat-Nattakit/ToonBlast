using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Symbol : MonoBehaviour
{
    [SerializeField] private Image symbolImage;
    [SerializeField] private Image symbolSpecial;

    public Image symbolImageObj { get => this.symbolImage; }

    [SerializeField] private SymbolType Type = SymbolType.Normal;

    [SerializeField] private SymbolColor symbolColor = SymbolColor.Red;
    public SymbolColor ColorSymbol { get => this.symbolColor; }

    public int xIndex, yIndex;
    public bool isMatch = false;
    public bool isMoving;   

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
        this._settingSymbolType(Type);
        this._settingSymbolColor(symbolColor); 
    }

    public void MovaToTarget(Vector2 _targetPos)
    {
        StartCoroutine(this.MoveCoroutine(_targetPos));
    }

    private IEnumerator MoveCoroutine(Vector2 _targetPos)
    {
        this.isMoving = true;
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
        isMatch = false;
    }



    private void _settingSymbolType(SymbolType type)
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

    private void _settingSymbolColor(SymbolColor color)
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
