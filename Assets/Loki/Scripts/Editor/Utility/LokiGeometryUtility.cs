using UnityEngine;

namespace Loki.Editor.Utility
{
	public static class LokiGeometryUtility
	{
		public static Vector2 ClosestPointOnLine(Vector2 p, Vector2 a, Vector2 b)
		{
			var v0 = b - a;
			var v1 = p - a;

			var sqrMag = v0.sqrMagnitude;
			var dot = Vector2.Dot(v0, v1);

			if (dot <= 0f)
				return a;

			var t = dot / sqrMag;

			if (t >= 1f)
				return b;

			return a + t * v0;
		}

		public static float PointSqrDistanceToLineSegment(Vector2 p, Vector2 a, Vector2 b)
		{
			var c = ClosestPointOnLine(p, a, b);
			return Vector2.SqrMagnitude(p - c);
		}

		public static float PointSqrDistanceToLineSegment(Vector2 p, Vector2 a, Vector2 b, out Vector2 closest)
		{
			closest = ClosestPointOnLine(p, a, b);
			return Vector2.SqrMagnitude(p - closest);
		}
	}
}
