using UnityEngine;
using System.Collections.Generic;

namespace SteinCo.Utils.Pooling
{
	public class Pool : MonoBehaviour
	{
		public GameObject originalObject;
		public int initialSize = 10;
		public int incrementSize = 5;

		private List<GameObject> freeObjects;
		private List<GameObject> usedObjects;
		private int counter = 0;

		private Vector3 originalPosition = Vector3.zero;
		private Quaternion originalRotation = Quaternion.Euler(Vector3.zero);
		private Vector3 originalScale = Vector3.one;

		void Start()
		{
			freeObjects = new List<GameObject>();
			usedObjects = new List<GameObject>();

			originalPosition = originalObject.transform.position;
			originalRotation = originalObject.transform.rotation;
			originalScale = originalObject.transform.localScale;

			for (int i = 0; i < initialSize; i++)
			{
				GameObject go = Instantiate(originalObject);
				go.name = originalObject.name + counter;

				go.transform.SetParent(gameObject.transform);
				go.transform.localPosition = Vector3.zero;

				go.SetActive(false);

				counter++;
				freeObjects.Add(go);
			}
		}

		public GameObject GetNewObject()
		{
			GameObject go = null;

			if (!(freeObjects.Count > 0))
			{
				for (int i = 0; i < incrementSize; i++)
				{
					go = Instantiate(originalObject);
					go.name = originalObject.name + counter;

					go.transform.SetParent(gameObject.transform);
					go.SetActive(false);
					counter++;
					freeObjects.Add(go);
				}
			}

			go = freeObjects[0];
			freeObjects.RemoveAt(0);
			usedObjects.Add(go);

			go.SetActive(true);
			return go;
		}

		public void ReturnUsedObject(GameObject go)
		{
			if (usedObjects.Contains(go))
			{
				usedObjects.Remove(go);
				go.SetActive(false);

				go.transform.SetParent(gameObject.transform);
				go.transform.position = originalPosition;
				go.transform.rotation = originalRotation;
				go.transform.localScale = originalScale;

				freeObjects.Add(go);
			}
			else
			{
				//DebugWithDate.LogError("Object " + go.name + " not used");
			}
		}
	}
}