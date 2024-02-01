using UnityEngine;

namespace Fizz6.Roguelike.World.Map.Tiles
{
    [CreateAssetMenu(fileName = "WaterTile", menuName = "Fizz6/Roguelike/World/Tiles/Water Tile")]
    public class WaterTile : Tile
    {
        public override bool CanEnter(Vector3Int position, Character.Character character) => false;
    }
}