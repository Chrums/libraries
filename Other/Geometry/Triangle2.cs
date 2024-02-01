using UnityEngine;

namespace Fizz6.Geometry
{
    public class Triangle2
    {
        public Vector2 Vertex0 { get; set; }
        public Vector2 Vertex1 { get; set; }
        public Vector2 Vertex2 { get; set; }

        public bool IsClockwise
        {
            get
            {
                var determinant = 
                    Vertex0.x * Vertex1.y + Vertex2.x * Vertex0.y + 
                    Vertex1.x * Vertex2.y - Vertex0.x * Vertex2.y - 
                    Vertex2.x * Vertex1.y - Vertex1.x * Vertex0.y;
                return determinant > 0.0f;
            }
        }

        public Triangle2(Vector2 vertex0, Vector2 vertex1, Vector2 vertex2)
        {
            Vertex0 = vertex0;
            Vertex1 = vertex1;
            Vertex2 = vertex2;
        }
        
        public void Invert()
        {
            var vertex = Vertex0;
            Vertex1 = Vertex2;
            Vertex2 = vertex;
        }

        public void Orient(bool clockwise = true)
        {
            if (!IsClockwise) Invert();
        }
    }
}