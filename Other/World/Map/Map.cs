using UnityEngine;

namespace Fizz6.Roguelike.World.Map
{
    public class Map : MonoBehaviour
    {}
}

// using System;
// using System.Linq;
// using Fizz6.Autofill;
// using Fizz6.Geometry;
// using Fizz6.Serialization;
// using UnityEditor.Experimental.GraphView;
// using UnityEngine;
// using UnityEngine.Tilemaps;
//
// namespace Fizz6.Roguelike.World.Map
// {
//     public class Map : MonoBehaviour
//     {
//         private const int Spacing = 8;
//         private static readonly Vector3Int Dimensions = new(4, 4, 1);
//
//         [SerializeField, Autofill]
//         private Tilemap tilemap;
//
//         [SerializeField] 
//         private MapConfig config;
//         
//         public int Level { get; private set; }
//
//         private WorldMapData wmd;
//         
//         private void Start()
//         {
//             var edge = new Edge2(Vector2.zero, Vector2.one);
//             Debug.LogError(edge.Relation(new Vector2(0.51f, 0.5f)));
//
//             // wmd = new WorldMapData(30, 16);
//
//             // var serializer = new Serializer();
//             // var mapData = WorldData.Generate();
//             // var json = serializer.Serialize(mapData);
//             // Debug.LogError(json);
//             // try
//             // {
//             //     var data = serializer.Deserialize<WorldData>(json);
//             // }
//             // catch (Exception e)
//             // {
//             //     Console.WriteLine(e);
//             //     throw;
//             // }
//
//
//
//             // var graph = GraphExt.RecursiveBacktrack(Dimensions, out var grid);
//             // for (var x = 0; x < Dimensions.x; ++x)
//             // {
//             //     for (var y = 0; y < Dimensions.y; ++y)
//             //     {
//             //         for (var z = 0; z < Dimensions.z; ++z)
//             //         {
//             //             var position = new Vector3Int(x, y, z);
//             //             var fragment = config.Fragments.Random();
//             //             var offset = position * (Fragment.Fragment.Dimensions + Vector3Int.one * Spacing);
//             //             fragment.Tilemap.CopyTo(tilemap, offset);
//             //             var vertex = grid[x, y, z];
//             //             var edges = graph[vertex];
//             //             
//             //             if (x > 0 && edges.Contains(grid[x - 1, y, z]))
//             //             {
//             //                 for (var index = 0; index < Spacing / 2; ++index)
//             //                 {
//             //                     var cell = offset + Vector3Int.left * (Fragment.Fragment.Dimensions.x / 2) + Vector3Int.left * (index + 1);
//             //                     tilemap.SetTile(cell, tile);
//             //                 }
//             //             }
//             //             
//             //             if (x < Dimensions.x - 1 && edges.Contains(grid[x + 1, y, z]))
//             //             {
//             //                 for (var index = 0; index < Spacing / 2; ++index)
//             //                 {
//             //                     var cell = offset + Vector3Int.right * (Fragment.Fragment.Dimensions.x / 2) + Vector3Int.right * (index);
//             //                     tilemap.SetTile(cell, tile);
//             //                 }
//             //             }
//             //             
//             //             if (y > 0 && edges.Contains(grid[x, y - 1, z]))
//             //             {
//             //                 for (var index = 0; index < Spacing / 2; ++index)
//             //                 {
//             //                     var cell = offset + Vector3Int.down * (Fragment.Fragment.Dimensions.y / 2) + Vector3Int.down * (index + 1);
//             //                     tilemap.SetTile(cell, tile);
//             //                 }
//             //             }
//             //             
//             //             if (y < Dimensions.x - 1 && edges.Contains(grid[x, y + 1, z]))
//             //             {
//             //                 for (var index = 0; index < Spacing / 2; ++index)
//             //                 {
//             //                     var cell = offset + Vector3Int.up * (Fragment.Fragment.Dimensions.y / 2) + Vector3Int.up * (index);
//             //                     tilemap.SetTile(cell, tile);
//             //                 }
//             //             }
//             //         }
//             //     }
//             // }
//         }
//
//         private void OnDrawGizmos()
//         {
//             if (wmd == null) return;
//             
//             Gizmos.color = Color.red;;
//             foreach (var point in wmd.Points)
//             {
//                 Gizmos.DrawSphere(new Vector3(point.x, point.y, 0.0f), 1.0f);
//             }
//             
//             Gizmos.color = Color.yellow;;
//             foreach (var triangle in wmd.Delaunay.Triangles)
//             {
//                 var v0 = triangle.Vertex0;
//                 var v1 = triangle.Vertex1;
//                 var v2 = triangle.Vertex2;
//                 var p0 = new Vector3(v0.x, v0.y, 0.0f);
//                 var p1 = new Vector3(v1.x, v1.y, 0.0f);
//                 var p2 = new Vector3(v2.x, v2.y, 0.0f);
//                 Gizmos.DrawSphere(p0, 1.0f);
//                 Gizmos.DrawSphere(p1, 1.0f);
//                 Gizmos.DrawSphere(p2, 1.0f);
//                 Gizmos.DrawLine(p0, p1);
//                 Gizmos.DrawLine(p1, p2);
//                 Gizmos.DrawLine(p2, p0);
//             }
//
//             // foreach (var segment in wmd.Mesh.)
//             // {
//             //     var vertex0 = wmd.Polygon.Points[segment.P0];
//             //     var vertex1 = wmd.Polygon.Points[segment.P1];
//             //     Gizmos.DrawLine(new Vector3((float)vertex0.x, (float)vertex0.y, 0.0f), new Vector3((float)vertex1.x, (float)vertex1.y, 0.0f));
//             // }
//         }
//     }
// }
//
// // using System;
// // using Fizz6.Autofill;
// // using UnityEngine;
// // using UnityEngine.Tilemaps;
// //
// // namespace Fizz6.Roguelike.World.Map
// // {
// //     public class Map : MonoBehaviour
// //     {
// //         private readonly Vector2Int Size = new(4, 4);
// //
// //         [SerializeField, Autofill]
// //         private Tilemap tilemap;
// //
// //         [SerializeField]
// //         private MapConfig config;
// //         
// //         private void Awake()
// //         {
// //             var graph = new MapGraph(config, Size);
// //             Debug.LogError($"Entrance: {graph.entrance}");
// //             Debug.LogError($"Exit: {graph.exit}");
// //             foreach (var cell in graph.path)
// //             {
// //                 var node = graph.Nodes[cell.x, cell.y];
// //                 Debug.LogError($"{cell} : {node.Connections}");
// //             }
// //         }
// //     }
// // }
//
// // using System.Linq;
// // using Fizz6.Autofill;
// // using Fizz6.Core;
// // using Fizz6.Roguelike.World;
// // using Fizz6.Roguelike.World.Map;
// // using Fizz6.Roguelike.World.Map.Fragment;
// // using Fizz6.Roguelike.World.Map.Tiles;
// // using Fizz6.Roguelike.World.Tiles;
// // using UnityEngine;
// // using UnityEngine.Tilemaps;
// // using Tile = UnityEngine.Tilemaps.Tile;
// //
// // namespace Fizz6.Roguelike.Map
// // {
// //     public class Map : MonoBehaviour
// //     {
// //         [SerializeField, Autofill] 
// //         private Tilemap tilemap;
// //
// //         [SerializeField] 
// //         private MapConfig config;
// //
// //         [SerializeField] 
// //         private Character.Character character;
// //
// //         private void Awake()
// //         {
// //             tilemap.ClearAllTiles();
// //
// //             var root = config.Roots.Random();
// //             Stamp(root, Vector3Int.zero);
// //
// //             var spawn = Vector3Int.zero;
// //             foreach (var cell in root.Tilemap.cellBounds.allPositionsWithin)
// //             {
// //                 if (!root.Tilemap.HasTile(cell)) continue;
// //                 var tile = root.Tilemap.GetTile<Tile>(cell);
// //                 if (tile is SpawnTile)
// //                 {
// //                     spawn = cell;
// //                 }
// //             }
// //
// //             foreach (var linkTileCell in root.LinkTileCells)
// //             {
// //                 var linkTile = root.Tilemap.GetTile<LinkTile>(linkTileCell);
// //                 var fragment = linkTile.Direction switch
// //                 {
// //                     Direction.North => config.LeafDirections[Direction.South].Random(),
// //                     Direction.East => config.LeafDirections[Direction.West].Random(),
// //                     Direction.South => config.LeafDirections[Direction.North].Random(),
// //                     Direction.West => config.LeafDirections[Direction.East].Random(),
// //                     _ => null
// //                 };
// //                 
// //                 if (fragment == null) continue;
// //                 
// //                 var fragmentLinkTileCell = fragment.LinkTileCells.FirstOrDefault(cell => fragment.Tilemap.GetTile<LinkTile>(cell).Direction == linkTile.Direction.Opposite());
// //                 var offset = linkTileCell - fragmentLinkTileCell - linkTile.Direction.ToVector3Int();
// //                 Stamp(fragment, offset);
// //             }
// //
// //             character.Cell = spawn;
// //         }
// //
// //         private void Stamp(Fragment fragment, Vector3Int offset)
// //         {
// //             foreach (var cell in fragment.Tilemap.cellBounds.allPositionsWithin)
// //             {
// //                 if (!fragment.Tilemap.HasTile(cell)) continue;
// //                 var tile = fragment.Tilemap.GetTile(cell);
// //                 tilemap.SetTile(cell + offset, tile);
// //             }
// //         }
// //     }
// // }