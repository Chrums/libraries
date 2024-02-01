using System;
using System.Collections.Generic;
using System.Linq;
using Fizz6.Roguelike.Minion.Abilities;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Fizz6.Roguelike.World.Map.Tiles
{
    [CreateAssetMenu(fileName = "Tile", menuName = "Fizz6/Roguelike/World/Tiles/Tile")]
    public class Tile : UnityEngine.Tilemaps.Tile
    {
        private const string Path = "Tiles";
        private static Dictionary<string, Tile> _tiles;
        private static IReadOnlyDictionary<string, Tile> Tiles =>
            _tiles ??= Resources
                .LoadAll<Tile>(Path)
                .ToDictionary(tile => tile.name, tile => tile);
        
        public virtual bool CanEnter(Vector3Int position, Character.Character character) => true;
        public virtual bool CanExit(Vector3Int position, Character.Character character) => true;
        
        public class Converter : JsonConverter<Tile>
        {
            public override void WriteJson(JsonWriter writer, Tile value, JsonSerializer serializer)
            {
                writer.WriteValue(value.name);
            }

            public override Tile ReadJson(JsonReader reader, System.Type objectType, Tile existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var name = reader.Value?.ToString();
                return name != null 
                    ? Tiles[name]
                    : existingValue;
            }
        }
    }
}