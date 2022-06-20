using UnityEngine;
using System.Collections.Generic;
using SteinCo.Utils;

namespace SteinCo.Ivisa.RoulettePremium.Chips
{
	public class AnimateChips : MonoBehaviour
	{
		public bool debug = false;
		public float speed = 3.0f;

		public struct ChipAnimation
		{
			public bool active;
			public float t;
			public GameObject chip;
			public Transform chipTransform;
			public int pool;
			public ChipsPile chipsPile;
			public Vector3 p0;
			public Vector3 p1;
			public Vector3 p2;
			public Vector3 p3;
			public FinishedAnimation callback;
			public bool local;
		}

		public delegate void FinishedAnimation(ChipAnimation chipAnimation);

		private List<ChipAnimation> chipAnimations = new List<ChipAnimation>();

		private bool started = false;

		public void Add(ChipAnimation ca)
		{
			chipAnimations.Add(ca);
			started = true;
		}

		void Update()
		{
			if (!started)
			{
				return;
			}

			bool active = false;
			for (int i = 0; i < chipAnimations.Count; i++)
			{
				ChipAnimation ca = chipAnimations[i];

				if (ca.active)
				{
					active = true;
					Vector3 pos = Bezier.CalculateBezierPoint(ca.t, ca.p0, ca.p1, ca.p2, ca.p3);

					ca.t += Time.deltaTime * speed;

					if (ca.t > 1.0f)
					{
						ca.t = 1.0f;
						pos = Bezier.CalculateBezierPoint(ca.t, ca.p0, ca.p1, ca.p2, ca.p3);
						ca.active = false;
						ca.callback(ca);
					}

					if (ca.local)
					{
						ca.chipTransform.localPosition = pos;
					}
					else
					{
						ca.chipTransform.position = pos;
					}

					ca.chipTransform.localRotation = Quaternion.identity;

					chipAnimations[i] = ca;

					if (debug && ca.active)
					{
						int parts = 20;
						Vector3 previousPos = ca.p0;
						float t = 1.0f / (float)parts;
						for (int j = 0; j <= parts; j++)
						{
							Vector3 linePos = Bezier.CalculateBezierPoint(t * j, ca.p0, ca.p1, ca.p2, ca.p3);

							Debug.DrawLine(previousPos, linePos, Color.red);
							previousPos = linePos;
						}
					}
				}
			}

			if (!active)
			{
				started = false;
				chipAnimations.Clear();
			}
		}
	}
}