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
    private Vector2 currPos, targetPos;

    public SymbolData symbolData;
    public List<Symbol> validNeighbors = new List<Symbol>();

    /*public Symbol(int x, int y)
    {
        this.xIndex = x;
        this.yIndex = y;
    }*/

   /* private void Start()
    {
        this._settingSymbolType(Type);
        this._settingSymbolColor(symbolColor);
        this.Initialize();
    }*/

    public void Initialize(SymbolData tile)
    {
        this.symbolData = tile;
        SetName(tile.coordinates);
       /* symbolSpriteRenderer.sprite = TileManager.instance.tileSymbolSprites[(int)tile.type];
        backgroundSpriteRenderer.sprite = TileManager.instance.tileBackgroundSprites[(int)tile.type];*/

        this._settingSymbolType(Type);
        this._settingSymbolColor(symbolColor);

        StartCoroutine(CheckNeighborsDelayed());
    }

    private IEnumerator CheckNeighborsDelayed()
    {
        yield return new WaitForSeconds(1);
        validNeighbors = Board.Instance.GetDirectNeighbors(this);
    }

    public void SetName(Vector2Int tileCoordinates)
    {
        name = string.Format("Tile: {0}, {1}", symbolData.coordinates.x.ToString(), symbolData.coordinates.y.ToString());
        StartCoroutine(CheckNeighborsDelayed());
    }

    private void OnMouseDown()
    {
        Board.Instance.TileClicked(this);
    }

    private void OnDestroy()
    {
        /*if (tile.coordinates.y <= 5)
        {
            GameplayUIController.instance.OnTileDestroyed(tile.type);
            MusicManager.instance.PlayTileSound(tile.type);
        }*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        symbolData.coordinates = new Vector2Int(symbolData.coordinates.x, transform.GetSiblingIndex());
        if (transform.GetSiblingIndex() > 5) Destroy(gameObject);
        SetName(symbolData.coordinates);
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
