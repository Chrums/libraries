// using System.Linq;
// using Fizz6.Collections.Graph;
// using Fizz6.Core;
// using Fizz6.Roguelike.Serialization;
// using Newtonsoft.Json;
// using UnityEngine;
//
// namespace Fizz6.Roguelike.World.Map
// {
//     public class MapData
//     {
//         private const int Spacing = 8;
//         
//         public static MapData Generate(Map map)
//         {
//             var mapData = new MapData
//             {
//                 Config = map.Config,
//                 Level = map.Level,
//                 Dimensions = map.Dimensions,
//                 Tilemap = map.Tilemap,
//             };
//             
//             var graph = new Graph();
//             var grid = graph.RecursiveBacktrack(dimensions);
//             
//             mapData.GenerateTilemap(graph, grid);
//             
//             return mapData;
//         }
//
//         private void GenerateTilemap(Graph graph, Graph.Vertex[,,] grid)
//         {
//             for (var x = 0; x < Dimensions.x; ++x)
//             {
//                 for (var y = 0; y < Dimensions.y; ++y)
//                 {
//                     for (var z = 0; z < Dimensions.z; ++z)
//                     {
//                         var position = new Vector3Int(x, y, z);
//                         var fragment = Config.Fragments.Random();
//                         var offset = position * (Fragment.Fragment.Dimensions + Vector3Int.one * Spacing);
//                         fragment.Tilemap.CopyTo(Tilemap, offset);
//                         var vertex = grid[x, y, z];
//                         var edges = graph[vertex];
//                         
//                         if (x > 0 && edges.Contains(grid[x - 1, y, z]))
//                         {
//                             for (var index = 0; index < Spacing / 2; ++index)
//                             {
//                                 var cell = offset + Vector3Int.left * (Fragment.Fragment.Dimensions.x / 2) + Vector3Int.left * (index + 1);
//                                 Tilemap.SetTile(cell, Config.Path);
//                             }
//                         }
//                         
//                         if (x < Dimensions.x - 1 && edges.Contains(grid[x + 1, y, z]))
//                         {
//                             for (var index = 0; index < Spacing / 2; ++index)
//                             {
//                                 var cell = offset + Vector3Int.right * (Fragment.Fragment.Dimensions.x / 2) + Vector3Int.right * (index);
//                                 Tilemap.SetTile(cell, Config.Path);
//                             }
//                         }
//                         
//                         if (y > 0 && edges.Contains(grid[x, y - 1, z]))
//                         {
//                             for (var index = 0; index < Spacing / 2; ++index)
//                             {
//                                 var cell = offset + Vector3Int.down * (Fragment.Fragment.Dimensions.y / 2) + Vector3Int.down * (index + 1);
//                                 Tilemap.SetTile(cell, Config.Path);
//                             }
//                         }
//                         
//                         if (y < Dimensions.x - 1 && edges.Contains(grid[x, y + 1, z]))
//                         {
//                             for (var index = 0; index < Spacing / 2; ++index)
//                             {
//                                 var cell = offset + Vector3Int.up * (Fragment.Fragment.Dimensions.y / 2) + Vector3Int.up * (index);
//                                 Tilemap.SetTile(cell, Config.Path);
//                             }
//                         }
//                     }
//                 }
//             }
//         }
//         
//         [JsonIgnore]
//         public MapConfig Config { get; set; }
//         
//         [JsonProperty]
//         public int Level { get; set; }
//         
//         [JsonProperty]
//         public Vector3Int Dimensions { get; set; }
//
//         [JsonProperty]
//         public TilemapData Tilemap { get; set; }
//     }
// }