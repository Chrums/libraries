using UnityEngine;

namespace Fizz6.Geometry
{
    public class Plane
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }

        public Plane(Vector3 normal)
        {
            Position = Vector3.zero;
            Normal = normal;
        }

        public Plane(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }
    }
}