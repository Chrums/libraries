using System.Collections.Generic;
using UnityEngine;

namespace Fizz6.Geometry
{
    public class Delaunay
    {
	    private const int Safety = 1000000;

	    private readonly List<Triangle2> triangles;
	    public IReadOnlyList<Triangle2> Triangles => triangles;
	    
	    private List<HalfEdge2> halfEdges;
	    private Dictionary<HalfEdge2, HalfEdge2> nextHalfEdge = new();
	    private Dictionary<HalfEdge2, HalfEdge2> previousHalfEdge = new();
	    private Dictionary<HalfEdge2, HalfEdge2> oppositeHalfEdge = new();
	    private Dictionary<Vector2, HalfEdge2> vertexHalfEdge = new();
	    private Dictionary<Triangle2, HalfEdge2> triangleHalfEdge = new();
	    private Dictionary<HalfEdge2, Triangle2> halfEdgeTriangle = new();

	    public Delaunay(List<Vector2> points) //  : this(Geometry.Triangulate(points))
	    {}

        public Delaunay(List<Triangle2> triangles)
        {
	        this.triangles = triangles;
            halfEdges = new List<HalfEdge2>(triangles.Count * 3);
            
            foreach (var triangle in triangles)
            {
            	triangle.Orient();
            
            	var halfEdge0 = new HalfEdge2(triangle.Vertex0);
            	var halfEdge1 = new HalfEdge2(triangle.Vertex1);
            	var halfEdge2 = new HalfEdge2(triangle.Vertex2);
            
            	nextHalfEdge[halfEdge0] = halfEdge1;
            	previousHalfEdge[halfEdge0] = halfEdge2;
            	
            	nextHalfEdge[halfEdge1] = halfEdge2;
            	previousHalfEdge[halfEdge1] = halfEdge0;
            	
            	nextHalfEdge[halfEdge2] = halfEdge0;
            	previousHalfEdge[halfEdge2] = halfEdge1;
            
            	vertexHalfEdge[halfEdge0.Vertex] = halfEdge1;
            	vertexHalfEdge[halfEdge1.Vertex] = halfEdge2;
            	vertexHalfEdge[halfEdge2.Vertex] = halfEdge0;
            
            	triangleHalfEdge[triangle] = halfEdge0;
            
            	halfEdgeTriangle[halfEdge0] = triangle;
            	halfEdgeTriangle[halfEdge1] = triangle;
            	halfEdgeTriangle[halfEdge2] = triangle;
            }
            
            for (var halfEdgeIndex = 0; halfEdgeIndex < halfEdges.Count; ++halfEdgeIndex)
            {
            	var halfEdge = halfEdges[halfEdgeIndex];
            	var nextVertex = halfEdge.Vertex;
            	var previousVertex = previousHalfEdge[halfEdge].Vertex;
            	for (var compareIndex = 0; compareIndex < halfEdges.Count; ++compareIndex)
            	{
            		if (halfEdgeIndex == compareIndex) continue;
            		var compareHalfEdge = halfEdges[compareIndex];
            		if (previousVertex != compareHalfEdge.Vertex ||
            		    nextVertex != previousHalfEdge[compareHalfEdge].Vertex) continue;
            		oppositeHalfEdge[halfEdge] = compareHalfEdge;
            		break;
            	}
            }
            
            var safety = Safety;
            var flipped = 0;
            
            while (safety > 0)
            {
	            safety--;
	            foreach (var halfEdge in halfEdges)
	            {
		            if (!oppositeHalfEdge.TryGetValue(halfEdge, out var opposite)) continue;
		            var a = halfEdge.Vertex;
		            var b = nextHalfEdge[halfEdge].Vertex;
		            var c = previousHalfEdge[halfEdge].Vertex;
		            var d = nextHalfEdge[opposite].Vertex;
		            // if (Geometry.PointRelativeToCircle(a, b, c, d) < 0.0f &&
		            //     Geometry.IsConvex(a, b, c, d) &&
		            //     Geometry.PointRelativeToCircle(b, c, d, a) < 0.0f)
			           //  Flip(halfEdge);
	            }
            }
        }

        private void Flip(HalfEdge2 halfEdge)
        {
	        var next = nextHalfEdge[halfEdge];
	        var previous = previousHalfEdge[halfEdge];
	        
	        var opposite = oppositeHalfEdge[halfEdge];
	        var oppositeNext = nextHalfEdge[opposite];
	        var oppositePrevious = previousHalfEdge[opposite];
	        
	        var a = halfEdge.Vertex;
	        var b = next.Vertex;
	        var c = previous.Vertex;
	        var d = oppositeNext.Vertex;

	        vertexHalfEdge[a] = next;
	        vertexHalfEdge[c] = oppositeNext;

	        nextHalfEdge[halfEdge] = previous;
	        previousHalfEdge[halfEdge] = oppositeNext;

	        nextHalfEdge[next] = opposite;
	        previousHalfEdge[next] = oppositePrevious;

	        nextHalfEdge[previous] = oppositeNext;
	        previousHalfEdge[previous] = halfEdge;

	        nextHalfEdge[opposite] = oppositePrevious;
	        previousHalfEdge[opposite] = next;

	        nextHalfEdge[oppositeNext] = halfEdge;
	        previousHalfEdge[oppositeNext] = previous;

	        nextHalfEdge[oppositePrevious] = next;
	        previousHalfEdge[oppositePrevious] = opposite;

	        halfEdge.Vertex = b;
	        next.Vertex = b;
	        previous.Vertex = c;
	        opposite.Vertex = d;
	        oppositeNext.Vertex = d;
	        oppositePrevious.Vertex = a;

	        var t0 = halfEdgeTriangle[halfEdge];
	        var t1 = halfEdgeTriangle[opposite];

	        halfEdgeTriangle[halfEdge] = t0;
	        halfEdgeTriangle[previous] = t0;
	        halfEdgeTriangle[oppositeNext] = t0;

	        halfEdgeTriangle[next] = t1;
	        halfEdgeTriangle[opposite] = t1;
	        halfEdgeTriangle[oppositePrevious] = t1;

	        t0.Vertex0 = b;
	        t0.Vertex1 = c;
	        t0.Vertex2 = d;

	        t1.Vertex0 = b;
	        t1.Vertex1 = d;
	        t1.Vertex2 = a;

	        triangleHalfEdge[t0] = previous;
	        triangleHalfEdge[t1] = opposite;
        }
    }
}