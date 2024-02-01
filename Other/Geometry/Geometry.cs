using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fizz6.Geometry
{
	public static class Geometry
	{
		private static List<Triangle2> Triangulate(List<Vector2> points)
		{
			if (points.Count < 3) return null;
			var triangles = new List<Triangle2>();
			
			points = points
				.OrderBy(point => point.x)
				.ThenBy(point => point.y)
				.ToList();
			
			var vertex0 = points[0];
			points.Remove(vertex0);
			
			var vertex1 = points[1];
			points.Remove(vertex1);

			var edge = new Edge2(vertex0, vertex1);

			Vector2 vertex2;
			try
			{
				vertex2 = points.First(point => edge.Relation(point) != 0);
			}
			catch (InvalidOperationException _)
			{
				return null;
			}
			
			points.Remove(vertex2);

			var root = new Triangle2(vertex0, vertex1, vertex2);
			triangles.Add(root);
			
			var hull = new HashSet<Vector2>();
			
			foreach (var triangle in triangles)
			{
				hull.Add(triangle.Vertex0);
				hull.Add(triangle.Vertex1);
				hull.Add(triangle.Vertex2);
			}

			return triangles;
		}
		
		// public static List<Triangle2> TriangulateConvex(List<Vector2> points)
		// {
		// 	var triangles = new List<Triangle2>();
		// 				
		// 	for (var index = 1; index < points.Count - 1; ++index)
		// 	{
		// 		var vertex0 = points[0];
		// 		var vertex1 = points[index];
		// 		var vertex2 = points[index + 1];
		// 		var triangle = new Triangle2(vertex0, vertex1, vertex2);
		// 		triangles.Add(triangle);
		// 	}
		// 	
		// 	return triangles;
		// }
		//
		// public static List<Triangle2> Triangulate(List<Vector2> points)
		// {
		// 	var triangles = new List<Triangle2>();
		// 	
		// 	points = points
		// 		.OrderBy(point => point.x)
		// 		.ToList();
		//
		// 	var triangle = new Triangle2(points[0], points[1], points[2]);
		// 	triangles.Add(triangle);
		// 	
		// 	var edges = new List<Edge2>();
		// 	
		// 	var edge0 = new Edge2(triangle.Vertex0, triangle.Vertex1);
		// 	edges.Add(edge0);
		// 	
		// 	var edge1 = new Edge2(triangle.Vertex1, triangle.Vertex2);
		// 	edges.Add(edge1);
		// 	
		// 	var edge2 = new Edge2(triangle.Vertex2, triangle.Vertex0);
		// 	edges.Add(edge2);
		// 	
		// 	for (var index = 3; index < points.Count; ++index)
		// 	{
		// 		var point = points[index];
		// 		var iterationEdges = new List<Edge2>();
		// 	
		// 		for (var edgeIndex = 0; edgeIndex < edges.Count; ++edgeIndex)
		// 		{
		// 			var edge = edges[edgeIndex];
		// 			var midpoint = (edge.Vertex0 + edge.Vertex1) / 2.0f;
		// 			var midpointEdge = new Edge2(point, midpoint);
		// 			var visible = edges
		// 				.Where((_, compareIndex) => edgeIndex != compareIndex)
		// 				.All(compareEdge => !Edge2.Intersect(compareEdge, midpointEdge));
		// 			
		// 			if (!visible) continue;
		// 			
		// 			var midpointEdge0 = new Edge2(edge.Vertex0, midpoint);
		// 			iterationEdges.Add(midpointEdge0);
		// 			
		// 			var midpointEdge1 = new Edge2(edge.Vertex1, midpoint);
		// 			iterationEdges.Add(midpointEdge1);
		// 	
		// 			var midpointTriangle =
		// 				new Triangle2(midpointEdge0.Vertex0, midpointEdge0.Vertex1, midpointEdge1.Vertex0);
		// 	
		// 			triangles.Add(midpointTriangle);
		// 		}
		// 		
		// 		edges.AddRange(iterationEdges);
		// 	}
		// 	
		// 	return triangles;
		// }
		//
		// public static bool IsConvex(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
		// {
		// 	var abc = new Triangle2(a, b, c).IsClockwise;
		// 	var abd = new Triangle2(a, b, d).IsClockwise;
		// 	var bcd = new Triangle2(b, c, d).IsClockwise;
		// 	var cad = new Triangle2(c, a, d).IsClockwise;
		//
		// 	return (abc && abd && bcd & !cad) ||
		// 	       (abc && abd && !bcd & cad) ||
		// 	       (abc && !abd && bcd & cad) ||
		// 	       (!abc && !abd && !bcd & cad) ||
		// 	       (!abc && !abd && bcd & !cad) ||
		// 	       (!abc && abd && !bcd & !cad);
		// }
		//
		// public static float PointRelativeToCircle(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 testPoint)
		// {
		// 	var a = pointA.x - testPoint.x;
		// 	var d = pointB.x - testPoint.x;
		// 	var g = pointC.x - testPoint.x;
		//
		// 	var b = pointA.y - testPoint.y;
		// 	var e = pointB.y - testPoint.y;
		// 	var h = pointC.y - testPoint.y;
		//
		// 	var c = a * a + b * b;
		// 	var f = d * d + e * e;
		// 	var i = g * g + h * h;
		//
		// 	return (a * e * i) + (b * f * g) + (c * d * h) - (g * e * c) - (h * f * a) - (i * d * b);
		// }
	}
}