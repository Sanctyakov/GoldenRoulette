using UnityEngine;

namespace SteinCo.Utils
{
	public class LoopColors : MonoBehaviour
	{
		public Color colorA = Color.white;
		public Color colorB = Color.white;
		public float duration = 1.0f;

		private bool started = false;
		private float timeCounter = 0.0f;

		private float t;
		private float duration2;

		public Color colorC;

		private Color currentColor;
		public Color CurrentColor
		{
			get
			{
				Color res = colorA;
				if (started)
				{
					res = currentColor;
				}
				return res;
			}
		}

		void Update()
		{
			if (!started)
			{
				return;
			}

			timeCounter += Time.deltaTime;

			if (timeCounter > duration)
			{
				timeCounter -= duration;
			}

			if (timeCounter > duration2)
			{
				t -= Time.deltaTime;
			}
			else
			{
				t += Time.deltaTime;
			}

			currentColor = InterpolateColors(colorA, colorB, t / duration2);
			colorC = currentColor;
		}

		public void Begin(bool interrupt = true)
		{
			if (started)
			{
				if (!interrupt)
				{
					return;
				}
			}

			started = true;

			duration2 = duration / 2.0f;

			timeCounter = 0.0f;
			t = 0.0f;
		}

		public void Stop()
		{
			started = false;
		}

		private Color InterpolateColors(Color A, Color B, float t)
		{
			float r, g, b, a;
			float ra, ga, ba, aa;
			float rb, gb, bb, ab;

			ra = A.r;
			ga = A.g;
			ba = A.b;
			aa = A.a;

			rb = B.r;
			gb = B.g;
			bb = B.b;
			ab = B.a;

			r = Mathf.Lerp(ra, rb, t);
			g = Mathf.Lerp(ga, gb, t);
			b = Mathf.Lerp(ba, bb, t);
			a = Mathf.Lerp(aa, ab, t);

			Color interpolated = new Color(r, g, b, a);

			return interpolated;
		}
	}
}