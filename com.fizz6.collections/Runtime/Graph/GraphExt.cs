using System;
using System.Collections.Generic;
using System.Linq;
using Fizz6.Core;
using UnityEngine;

namespace Fizz6.Collections.Graph
{
    public static class GraphExt
    {
        public static Graph<TVertex> Grid<TVertex>(Vector3Int dimensions, out TVertex[,,] grid, Func<Vector3Int, TVertex> constructor = null)
            where TVertex : class
        {
            var graph = new Graph<TVertex>();
            grid = graph.Grid(dimensions, constructor);
            return graph;
        }
        
        private static TVertex[,,] Grid<TVertex>(this Graph<TVertex> graph, Vector3Int dimensions, Func<Vector3Int, TVertex> constructor = null)
            where TVertex : class
        {
            var grid = new TVertex[dimensions.x, dimensions.y, dimensions.z];
            graph.Clear();

            for (var x = 0; x < dimensions.x; ++x)
            {
                for (var y = 0; y < dimensions.y; ++y)
                {
                    for (var z = 0; z < dimensions.z; ++z)
                    {
                        var cell = new Vector3Int(x, y, z);
                        var vertex = constructor?.Invoke(cell) ?? Activator.CreateInstance<TVertex>();
                        graph.Add(vertex);
                        grid[x, y, z] = vertex;
                    }
                }
            }
            
            return grid;
        }

        private static readonly IEnumerable<Vector3Int> Directions = new List<Vector3Int>
        {
            Vector3Int.left,
            Vector3Int.right,
            Vector3Int.down,
            Vector3Int.up,
            Vector3Int.back,
            Vector3Int.forward
        };

        public static Graph<TVertex> RecursiveBacktrack<TVertex>(Vector3Int dimensions, out TVertex[,,] grid, Func<Vector3Int, TVertex> constructor = null)
            where TVertex : class
        {
            var graph = new Graph<TVertex>();
            grid = graph.RecursiveBacktrack(dimensions, constructor);
            return graph;
        }

        private static TVertex[,,] RecursiveBacktrack<TVertex>(this Graph<TVertex> graph, Vector3Int dimensions, Func<Vector3Int, TVertex> constructor = null)
            where TVertex : class
        {
            var grid = graph.Grid(dimensions, constructor);
            var exploration = new bool[dimensions.x, dimensions.y, dimensions.z];

            void Explore(Vector3Int cell)
            {
                foreach (var direction in Directions.Shuffle())
                {
                    var other = cell + direction;
                    if (other.x < 0 || other.x > dimensions.x - 1 || 
                        other.y < 0 || other.y > dimensions.y - 1 ||
                        other.z < 0 || other.z > dimensions.z - 1 || 
                        exploration[other.x, other.y, other.z]) continue;
                    exploration[other.x, other.y, other.z] = true;
                    var vertex0 = grid[cell.x, cell.y, cell.z];
                    var vertex1 = grid[other.x, other.y, other.z];
                    graph.Add(vertex0, vertex1);
                    graph.Add(vertex1, vertex0);
                    Explore(other);
                }
            }

            exploration[0, 0, 0] = true;
            Explore(Vector3Int.zero);
            
            return grid;
        }
        
        public static List<TVertex> DepthFirstSearch<TVertex>(this Graph<TVertex> graph, TVertex from, TVertex to)
            where TVertex : class
        {
            var stack = new Stack<List<TVertex>>();
            return graph.Search(from, to, () => stack.Count, stack.Pop, stack.Push);
        }

        public static List<TVertex> BreadthFirstSearch<TVertex>(this Graph<TVertex> graph, TVertex from, TVertex to)
            where TVertex : class
        {
            var queue = new Queue<List<TVertex>>();
            return graph.Search(from, to, () => queue.Count, queue.Dequeue, queue.Enqueue);
        }

        private static List<TVertex> Search<TVertex>(
            this Graph<TVertex> graph, TVertex from, TVertex to, 
            Func<int> counter, Func<List<TVertex>> getter, Action<List<TVertex>> setter)
            where TVertex : class
        {
            var exploration = new HashSet<TVertex>();
            var root = new List<TVertex> { from };
            setter.Invoke(root);

            while (counter.Invoke() > 0)
            {
                var path = getter.Invoke();
                var vertex = path[^1];
                if (vertex.Equals(to)) return path;
                if (!graph.TryGetValue(vertex, out var edges)) continue;
                foreach (var edge in edges.Where(connection => !exploration.Contains(connection)))
                {
                    exploration.Add(edge);
                    setter.Invoke(new List<TVertex>(path) { edge });
                }
            }

            return null;
        }
    }
}