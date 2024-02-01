using UnityEngine;

namespace Fizz6.Roguelike.World
{
    public enum Direction
    {
        North,
        East,
        South,
        West,
        None,
    }

    public static class DirectionExt
    {
        public static Direction ToDirection(this Vector2Int direction) =>
            direction.y > 0
                ? Direction.North
                : direction.x > 0
                    ? Direction.East
                    : direction.y < 0
                        ? Direction.South
                        : direction.x < 0
                            ? Direction.West
                            : Direction.None;
        
        public static Direction ToDirection(this Vector3Int direction) =>
            direction.y > 0
                ? Direction.North
                : direction.x > 0
                    ? Direction.East
                    : direction.y < 0
                        ? Direction.South
                        : direction.x < 0
                            ? Direction.West
                            : Direction.None;

        public static Vector3Int ToVector3Int(this Direction direction) =>
            direction switch
            {
                Direction.North => Vector3Int.up,
                Direction.East => Vector3Int.right,
                Direction.South => Vector3Int.down,
                Direction.West => Vector3Int.left,
                _ => Vector3Int.zero
            };

        public static Direction Opposite(this Direction direction) =>
            direction switch
            {
                Direction.North => Direction.South,
                Direction.East => Direction.West,
                Direction.South => Direction.North,
                Direction.West => Direction.East,
                _ => Direction.None
            };
    }
}