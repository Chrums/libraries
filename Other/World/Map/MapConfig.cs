using System.Collections.Generic;
using System.Linq;
using Fizz6.Core;
using Fizz6.Roguelike.World.Map.Tiles;
using Newtonsoft.Json;
using UnityEngine;

namespace Fizz6.Roguelike.World.Map
{
    [CreateAssetMenu(fileName = "Map", menuName = "Fizz6/Roguelike/World/Map")]
    public class MapConfig : ScriptableObject
    {
        private const string Path = "Maps";
        private static Dictionary<string, MapConfig> _mapConfigs;
        private static IReadOnlyDictionary<string, MapConfig> MapConfigs =>
            _mapConfigs ??= Resources
                .LoadAll<MapConfig>(Path)
                .ToDictionary(config => config.name, config => config);

        public static MapConfig Random =>
            MapConfigs.Values.Random();
        
        [SerializeField]
        private Vector3Int minimumDimensions = new Vector3Int(2, 2, 1);
        public Vector3Int MinimumDimensions => minimumDimensions;
        
        [SerializeField]
        private Vector3Int maximumDimensions = new Vector3Int(4, 4, 1);
        public Vector3Int MaximumDimensions => maximumDimensions;
        
        [SerializeField]
        private List<Fragment.Fragment> fragments;
        public IReadOnlyList<Fragment.Fragment> Fragments => fragments;

        [SerializeField] 
        private Tile connect;
        public Tile Connect => connect;

        [SerializeField]
        private Tile disconnect;
        public Tile Disconnect => disconnect;
        
        public class Converter : JsonConverter<MapConfig>
        {
            public override void WriteJson(JsonWriter writer, MapConfig value, JsonSerializer serializer)
            {
                writer.WriteValue(value.name);
            }

            public override MapConfig ReadJson(JsonReader reader, System.Type objectType, MapConfig existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var name = reader.Value?.ToString();
                return name != null 
                    ? MapConfigs[name]
                    : existingValue;
            }
        }
    }
}

// using System.Collections.Generic;
// using Fizz6.Roguelike.World.Map.Tiles;
// using UnityEngine;
//
// namespace Fizz6.Roguelike.World.Map
// {
//     [CreateAssetMenu(fileName = "Map", menuName = "Fizz6/Roguelike/World/Map")]
//     public class MapConfig : ScriptableObject
//     {
//         private static Dictionary<Direction, List<Fragment.Fragment>> Sort(List<Fragment.Fragment> fragments)
//         {
//             var dictionary = new Dictionary<Direction, List<Fragment.Fragment>>
//             {
//                 [Direction.North] = new(),
//                 [Direction.East] = new(),
//                 [Direction.South] = new(),
//                 [Direction.West] = new()
//             };
//
//             foreach (var fragment in fragments)
//             {
//                 foreach (var cell in fragment.Tilemap.cellBounds.allPositionsWithin)
//                 {
//                     if (fragment.Tilemap.GetTile(cell) is not LinkTile linkTile) continue;
//                     dictionary[linkTile.Direction].Add(fragment);
//                 }
//             }
//             
//             return dictionary;
//         }
//         
//         [SerializeField]
//         private List<Fragment.Fragment> roots;
//         public IReadOnlyList<Fragment.Fragment> Roots => roots;
//         
//         private Dictionary<Direction, List<Fragment.Fragment>> rootDirections;
//         public IReadOnlyDictionary<Direction, List<Fragment.Fragment>> RootDirections
//         {
//             get
//             {
//                 if (rootDirections != null) return rootDirections;
//                 rootDirections = Sort(roots);
//                 return rootDirections;
//             }
//         }
//
//         [SerializeField] 
//         private List<Fragment.Fragment> branches;
//         public IReadOnlyList<Fragment.Fragment> Branches => branches;
//         
//         private Dictionary<Direction, List<Fragment.Fragment>> branchDirections;
//         public IReadOnlyDictionary<Direction, List<Fragment.Fragment>> BranchDirections
//         {
//             get
//             {
//                 if (branchDirections != null) return branchDirections;
//                 branchDirections = Sort(branches);
//                 return branchDirections;
//             }
//         }
//
//         [SerializeField] 
//         private List<Fragment.Fragment> leaves;
//         public IReadOnlyList<Fragment.Fragment> Leaves => leaves;
//         
//         private Dictionary<Direction, List<Fragment.Fragment>> leafDirections;
//         public IReadOnlyDictionary<Direction, List<Fragment.Fragment>> LeafDirections
//         {
//             get
//             {
//                 if (leafDirections != null) return leafDirections;
//                 leafDirections = Sort(leaves);
//                 return leafDirections;
//             }
//         }
//     }
// }