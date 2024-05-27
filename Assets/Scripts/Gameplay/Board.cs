using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private int _boardWidth = 0;
    private int _boardHight = 0;

    private int _spacingX = 0;
    private int _spacingY = 0;

    public GameObject[] symbolPrefabs;

    private Node[,] _boardGame;
    public Node[,] BoardGame { get => this._boardGame; }

    public GameObject ParentBoard;

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
                int randomIndex = Random.Range(0, this.symbolPrefabs.Length);
                GameObject symbols = Instantiate(this.symbolPrefabs[randomIndex], this.ParentBoard.transform);
                symbols.transform.localPosition = position;
                symbols.GetComponent<Symbol>().SetIndicies(x, y);
                symbols.name = "[" + x + "," + y + "]";
                this._boardGame[x, y] = new Node(true, symbols);
            }
        }

        this._setBoardSize();
    }

    private void _setBoardSize()
    {
        float boardSizeX = (this._boardHight * this._spacingX);
        float boardSizeY = (this._boardWidth * this._spacingY);
        this.ParentBoard.transform.localPosition = new Vector2(-boardSizeX / 2, -boardSizeY / 2);
    }
}
