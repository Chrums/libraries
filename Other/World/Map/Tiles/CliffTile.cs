using System.Collections.Generic;
using UnityEngine;

namespace Fizz6.Roguelike.World.Map.Tiles
{
    [CreateAssetMenu(fileName = "CliffTile", menuName = "Fizz6/Roguelike/World/Tiles/Cliff Tile")]
    public class CliffTile : Tile
    {
        [SerializeField]
        private List<Direction> directions;
        public List<Direction> Directions => directions;

        public override bool CanEnter(Vector3Int position, Character.Character character)
        {
            var enterDirection = (position - character.Cell).ToDirection();
            return (enterDirection == Direction.North && !directions.Contains(Direction.South)) ||
                   (enterDirection == Direction.East && !directions.Contains(Direction.West)) ||
                   (enterDirection == Direction.South && !directions.Contains(Direction.North)) ||
                   (enterDirection == Direction.West && !directions.Contains(Direction.East));
        }
    }
}