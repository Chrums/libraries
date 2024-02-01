using UnityEngine;

namespace Fizz6.Geometry
{
    public class Edge2
    {
        public static bool Intersect(Edge2 edge0, Edge2 edge1)
        {
            var v00 = edge0.Vertex0;
            var v01 = edge0.Vertex1;
        
            var v10 = edge1.Vertex0;
            var v11 = edge1.Vertex1;

            var denominator = (v11.y - v10.y) * (v01.x - v00.x) - (v11.x - v10.x) * (v01.y - v00.y);

            if (denominator == 0) return false;
            
            var ua = ((v11.x - v10.x) * (v00.y - v10.y) - (v11.y - v10.y) * (v00.x - v10.x)) / denominator;
            var ub = ((v01.x - v00.x) * (v00.y - v10.y) - (v01.y - v00.y) * (v00.x - v10.x)) / denominator;

            return ua is >= 0.0f and <= 1.0f && ub is >= 0.0f and <= 1.0f;
        }

        /// <summary>
        /// Calculates a value representing the relationship of a point to the edge.
        /// This number will be negative if the point lies to the left of the edge, zero if it is collinear,
        /// and positive if it lies to the right of the edge.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="point"></param>
        /// <returns>A number representing the relationship between a point and the edge.</returns>
        public static float Relation(Edge2 edge, Vector2 point)
        {
            var vector0 = new Vector2(point.x - edge.Vertex0.x, point.y - edge.Vertex0.y);
            var vector1 = new Vector2(point.x - edge.Vertex1.x, point.y - edge.Vertex1.y);
            return vector0.x * vector1.y - vector1.x * vector0.y;
        }
        
        public Vector2 Vertex0 { get; private set; }
        public Vector2 Vertex1 { get; private set; }

        public Edge2(Vector2 vertex0, Vector2 vertex1)
        {
            Vertex0 = vertex0;
            Vertex1 = vertex1;
        }

        public void Invert() =>
            (Vertex0, Vertex1) = (Vertex1, Vertex0);

        public bool Intersect(Edge2 edge) =>
            Intersect(this, edge);

        public float Relation(Vector2 point) =>
            Relation(this, point);
    }
}