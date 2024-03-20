using UnityEngine;
using UnityEngine.Tilemaps;

namespace Fizz6.Core
{
    public static class TilemapExt
    {
        public static void CopyTo(this Tilemap tilemap, Tilemap target, Vector3Int offset)
        {
             foreach (var cell in tilemap.cellBounds.allPositionsWithin)
             {
                 if (!tilemap.HasTile(cell)) continue;
                 var tile = tilemap.GetTile(cell);
                 target.SetTile(cell + offset, tile);
             }
        }
    }
}