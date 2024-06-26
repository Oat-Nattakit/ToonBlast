using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Symbol : MonoBehaviour
{
    [SerializeField] private Image symbolImage;
    [SerializeField] private Image symbolSpecial;
    [SerializeField] private Image freamSymbol;

    [SerializeField] private SymbolType _typeSymbol = SymbolType.Normal;
    [SerializeField] private SymbolColor _symbolColor = SymbolColor.Red;

    public SymbolColor ColorSymbol { get => this._symbolColor; }
    public SymbolType TypeSymbol { get => this._typeSymbol; }

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private BoxCollider2D _boxCollider;

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

    public void InitSymbol(SymbolType _type, SymbolColor _color)
    {
        this._SettingSymbolColor(_color);
        this._SettingSymbolType(_type);
        this._InitOffset();
        this._SetName();
    }

    private void _SetName()
    {
        string symName = this._typeSymbol == SymbolType.Normal ? this.ColorSymbol.ToString() : this._typeSymbol.ToString();
        this.name = "Symbol_" + symName;
    }

    public void MovaToTarget(Vector2 _targetPos, float duration)
    {
        this.transform.DOLocalMove(_targetPos, duration)
            .SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                this.transform.localPosition = _targetPos;
            });
    }

    public void ActionDestory(float duration, Action _onComplete)
    {
        Vector3 size = Vector3.zero;
        this.freamSymbol.transform.DOScale(size, duration)
            .SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                this.transform.localScale = size;
                this.freamSymbol.transform.localScale = Vector3.one;
                _onComplete?.Invoke();
            });
    }

    #region setSymbol
    private void _SettingSymbolType(SymbolType type)
    {
        this._typeSymbol = type;
        switch (type)
        {
            case SymbolType.Bomb:
                this.symbolSpecial.gameObject.SetActive(true);
                this.symbolImage.color = Color.black;
                this.symbolSpecial.color = new Color32(215, 83, 81, 255);
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
        this._symbolColor = color;
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

    private void _InitOffset()
    {
        float w = this._rectTransform.sizeDelta.x;
        float h = this._rectTransform.sizeDelta.y;
        this._boxCollider.size = new Vector2(w, h);
        this._boxCollider.offset = new Vector2(w / 2, h / 2);
    }
    #endregion


}
