using UnityEngine;

namespace Fizz6.Roguelike.World.Map.Tiles
{
    [CreateAssetMenu(fileName = "LinkTile", menuName = "Fizz6/Roguelike/World/Tiles/Link Tile")]
    public class LinkTile : Tile
    {
        [SerializeField] 
        private Direction direction;
        public Direction Direction => direction;
    }
}