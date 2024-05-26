using UnityEngine;

public class SymbolData
{
    public Vector2Int coordinates;
    public SymbolColor type;

    public SymbolData(Vector2Int coordinates, SymbolColor type)
    {
        this.coordinates = coordinates;
        this.type = type;
    }
}
