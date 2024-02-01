// using System.Collections.Generic;
// using System.Linq;
// using Fizz6.Geometry;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// namespace Fizz6.Roguelike.World
// {
//     public class WorldMapData
//     {
//         private readonly HashSet<Vector2> points = new();
//         public ICollection<Vector2> Points => points;
//         
//         public Delaunay Delaunay { get; private set; }
//
//         public WorldMapData(int radius, int count)
//         {
//             for (var index = 0; index < count; ++index)
//             {
//                 // while (true)
//                 // {
//                 //     var x = Random.Range(-length, length);
//                 //     var y = Random.Range(-length, length);
//                 //     var point = new Vector2(x, y);
//                 //     if (point.magnitude > length) continue;
//                 //     if (points.Add(point)) break;
//                 // }
//                 var theta = Mathf.PI * 2.0f * (index / (float)count);
//                 var point = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * radius;
//                 points.Add(point);
//             }
//
//             Delaunay = new Delaunay(points.ToList());
//         }
//     }
// }