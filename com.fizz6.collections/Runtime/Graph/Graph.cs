using System.Collections.Generic;

namespace Fizz6.Collections.Graph
{
    public interface IGraph
    {
        IReadOnlyCollection<object> Items { get; }
        IReadOnlyCollection<object> this[object value] { get; }
        public bool Add(object value);
        public bool Remove(object value);
        public bool Add(object value0, object value1);
        public bool Remove(object value0, object value1);
    }
    
    public interface IReadOnlyGraph<TVertex>
        where TVertex : class
    {
        IReadOnlyCollection<TVertex> Vertices { get; }
        IReadOnlyCollection<TVertex> this[TVertex vertex] { get; }
    }
    
    public class Graph<TVertex> : IGraph, IReadOnlyGraph<TVertex>
        where TVertex : class
    {
        private readonly HashSet<TVertex> vertices = new();
        public IReadOnlyCollection<TVertex> Vertices => vertices;

        private readonly Dictionary<TVertex, HashSet<TVertex>> edges = new();
        public IReadOnlyCollection<TVertex> this[TVertex vertex] => edges[vertex];

        public bool Add(TVertex vertex)
        {
            if (!vertices.Add(vertex)) return false;
            edges.Add(vertex, new HashSet<TVertex>());
            return true;
        }

        public bool Remove(TVertex vertex)
        {
            if (!vertices.Remove(vertex)) return false;
            edges.Remove(vertex);
            return true;
        }

        public bool Add(TVertex from, TVertex to)
        {
            if (!this.edges.TryGetValue(from, out var edges) ||
                !vertices.Contains(to)) return false;
            return edges.Add(to);
        }

        public bool Remove(TVertex from, TVertex to)
        {
            if (!this.edges.TryGetValue(from, out var edges) ||
                !vertices.Contains(to)) return false;
            return edges.Remove(to);
        }

        public void Clear()
        {
            vertices.Clear();
            edges.Clear();
        }

        public bool TryGetValue(TVertex vertex, out IReadOnlyCollection<TVertex> edges)
        {
            if (!this.edges.TryGetValue(vertex, out var vertices))
            {
                edges = null;
                return false;
            }

            edges = vertices;
            return true;
        }

        public IReadOnlyCollection<object> Items => vertices;
        public IReadOnlyCollection<object> this[object value] => value is TVertex vertex
            ? edges[vertex]
            : null;

        public bool Add(object value) =>
            value is TVertex vertex && Add(vertex);
        
        public bool Remove(object value) =>
            value is TVertex vertex && Remove(vertex);
        
        public bool Add(object value0, object value1) =>
            value0 is TVertex vertex0 && value1 is TVertex vertex1 && Add(vertex0, vertex1);

        public bool Remove(object value0, object value1) =>
            value0 is TVertex vertex0 && value1 is TVertex vertex1 && Remove(vertex0, vertex1);
    }
    
    public class Vertex
    {}
}