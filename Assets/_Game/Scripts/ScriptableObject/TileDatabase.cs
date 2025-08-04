using UnityEngine;

[CreateAssetMenu(fileName = "new Tile Database", menuName = "ScriptableObject/TileDatabase", order = 1)]
public class TileDatabase : ScriptableObject
{
    [field: SerializeField] public TileData[] TilesData { get; private set; }
}

