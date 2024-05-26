using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;

    public int BoardWidth = 8;
    public int BoardHight = 8;

    public int spacingX = 0;
    public int spacingY = 0;

    public GameObject[] symbolPrefabs;

    public List<Symbol> tiles = new List<Symbol>();
    public Transform[] spawnPoints;
    private WaitForSeconds pacing = new WaitForSeconds(0.1f);

    private void Awake()
    {
        Instance = this;
    }

    public void InitBoard()
    {
        StartCoroutine(GenerateTiles());
    }

    public void Rocket(int y)
    {
        List<Symbol> rocketTiles = new List<Symbol>();
        foreach (Transform child in spawnPoints[y])
        {
            rocketTiles.Add(child.GetComponent<Symbol>());
        }
        foreach (Symbol tile in rocketTiles)
        {
            Destroy(tile.gameObject);
        }
    }

    public void Bomb(Vector2Int coordinates)
    {
        //Check tiles around coordinates
        //Add to a list
        // Destroy them
    }

    public void DiscoBall(SymbolColor tileType)
    {
        List<Symbol> discoTiles = new List<Symbol>();
        foreach (Symbol tile in tiles)
        {
            if (tile.symbolData.type == tileType) discoTiles.Add(tile);
        }

        foreach (Symbol tile in discoTiles)
        {
            Destroy(tile);
        }
    }

    internal void TileClicked(Symbol Symbol)
    {
        List<Symbol> interestingNeighbors = new List<Symbol>();
        List<Symbol> furtherExamination = new List<Symbol>();
        foreach (Symbol tile in tiles)
        {
            if (tile.validNeighbors.Contains(Symbol))
            {
                interestingNeighbors.Add(tile);
            }
            else
            {
                furtherExamination.Add(tile);
            }
        }

        while (furtherExamination.Count > 0)
        {
            Symbol checkingTile = furtherExamination[0];
            furtherExamination.Remove(checkingTile);
            bool addToInteresting = false;
            foreach (Symbol tile in interestingNeighbors)
            {
                if (checkingTile.validNeighbors.Contains(tile))
                {
                    addToInteresting = true;
                    break;
                }

            }
            if (addToInteresting) interestingNeighbors.Add(checkingTile);
        }
        StartCoroutine(CheckChain(Symbol, interestingNeighbors));
    }

    private IEnumerator CheckChain(Symbol Symbol, List<Symbol> directNeighbors)
    {
        GenerateTileAtColumn(Symbol.symbolData.coordinates);
        tiles.Remove(Symbol);
        Destroy(Symbol.gameObject);
        foreach (Symbol tile in directNeighbors)
        {
            GenerateTileAtColumn(tile.symbolData.coordinates);
            tiles.Remove(tile);
            if (tile != null) Destroy(tile.gameObject);
            //  yield return pacing;
        }
       // GameplayUIController.instance.LowerRemainingMoves();
        yield return null;
    }

    private IEnumerator ReassignCoordinates()
    {
        yield return new WaitForEndOfFrame();
        foreach (Symbol tile in tiles)
        {
            int parentIndex = 0;
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i] == tile.transform.parent)
                {
                    parentIndex = i;
                    break;
                }
            }
            tile.symbolData.coordinates = new Vector2Int(parentIndex, tile.transform.GetSiblingIndex());
            tile.SetName(tile.symbolData.coordinates);
        }
    }

    private void GenerateTileAtColumn(Vector2Int coordinates)
    {
        GameObject tile = Instantiate(symbolPrefabs[0], spawnPoints[coordinates.x]);
        int typeRandomized = UnityEngine.Random.Range(0, 4);
        SymbolData tileData = new SymbolData(coordinates, (SymbolColor)typeRandomized);
        tile.GetComponent<Symbol>().Initialize(tileData);
       // tile.transform.localPosition = new Vector3(coordinates.x, -(coordinates.y * this.spacingY), 0);
        tiles.Add(tile.GetComponent<Symbol>());
        StartCoroutine(ReassignCoordinates());
    }

    private IEnumerator GenerateTiles()
    {
        for (int i = 0; i < this.BoardWidth; i++)
        {
            for (int j = 0; j < spawnPoints.Length; j++)
            {
                GameObject tile = Instantiate(symbolPrefabs[0], spawnPoints[j]);
                Vector2Int coordinates = new Vector2Int(j, (i));
                int typeRandomized = UnityEngine.Random.Range(0, 4);
                SymbolData tileData = new SymbolData(coordinates, (SymbolColor)typeRandomized);
                //tile.transform.localPosition = new Vector3(j, -(i * this.spacingY),0);
                tile.GetComponent<Symbol>().Initialize(tileData);
                tiles.Add(tile.GetComponent<Symbol>());
                yield return pacing;
            }
        }
        yield return null;
    }

    public List<Symbol> GetDirectNeighbors(Symbol t)
    {
        List<Symbol> neighbors = new List<Symbol>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                if (Mathf.Abs(i) == 1 && Mathf.Abs(j) == 1) continue;
                int checkX = t.symbolData.coordinates.x + j;
                int checkY = t.symbolData.coordinates.y + i;
                if (checkY <= 5 &&
    checkX >= 0 &&
    checkY >= 0 &&
    checkX <= 4 &&
    FindTileByCoordinates(new Vector2Int(checkX, checkY)).symbolData.type == t.symbolData.type
    )
                {
                    neighbors.Add(FindTileByCoordinates(new Vector2Int(checkX, checkY)));
                }
            }
        }
        return neighbors;
    }

    private Symbol FindTileByCoordinates(Vector2Int coordinates)
    {
        foreach (Symbol tile in tiles)
        {
            if (tile.symbolData.coordinates == coordinates)
            {
                return tile;
            }
        }
        return null;
    }


}
