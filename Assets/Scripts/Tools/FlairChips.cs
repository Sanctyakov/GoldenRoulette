using UnityEngine;
using System.Collections.Generic;
using SteinCo.Utils.Pooling;

namespace SteinCo.Ivisa.RoulettePremium.Tools
{
	public class FlairChips : MonoBehaviour
	{
		public Transform flairChipsRoot;
		public Pool[] pools;

		public bool started = false;

		public int pilePerValueAmount = 1;
		public int chipsPerPile = 10;
		public float separation = 0.0f;
		public float radius = 0.0f;
		public float angle = 0.0f;

		public float chipHeight = 0.005f;

		private List<GameObject>[] chips;
		void Start()
		{
			chips = new List<GameObject>[pools.Length];

			for (int i = 0; i < pools.Length; i++)
			{
				chips[i] = new List<GameObject>();
			}

			//		Create();
		}

		// Update is called once per frame
		void Update()
		{
			if (!started)
			{
				return;
			}

			started = false;

			Create();
		}

		private void Create()
		{
			for (int i = 0; i < pools.Length; i++)
			{
				foreach (GameObject go in chips[i])
				{
					go.transform.GetChild(0).gameObject.SetActive(true);
					pools[i].ReturnUsedObject(go);
				}
				chips[i].Clear();
			}

			while (flairChipsRoot.childCount > 0)
			{
				DestroyImmediate(flairChipsRoot.GetChild(0).gameObject);
			}

			for (int i = 0; i < pools.Length; i++)
			{
				GameObject rootPile = new GameObject();
				rootPile.transform.parent = flairChipsRoot;

				float x = Mathf.Cos(angle * i * Mathf.Deg2Rad) * radius;
				float z = Mathf.Sin(angle * i * Mathf.Deg2Rad) * radius;

				Vector3 direction = new Vector3(x, 0.0f, z);

				rootPile.transform.localPosition = direction;

				for (int j = 0; j < pilePerValueAmount; j++)
				{
					for (int k = 0; k < chipsPerPile; k++)
					{
						GameObject go = pools[i].GetNewObject();

						if (k < chipsPerPile - 1)
						{
							go.transform.GetChild(0).gameObject.SetActive(false);
						}

						go.transform.parent = rootPile.transform;
						go.transform.localPosition = rootPile.transform.position + direction.normalized * j * separation + Vector3.up * k * chipHeight;

						chips[i].Add(go);
					}
				}
			}
		}
	}
}