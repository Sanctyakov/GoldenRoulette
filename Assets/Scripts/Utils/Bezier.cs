using UnityEngine;

namespace SteinCo.Utils
{
	public static class Bezier
	{
		public static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			float tt = t * t;
			float ttt = tt * t;

			float u = 1.0f - t;
			float uu = u * u;
			float uuu = uu * u;

			Vector3 p = uuu * p0;
			p += 3 * uu * t * p1;
			p += 3 * u * tt * p2;
			p += ttt * p3;

			return p;
		}
	}
}