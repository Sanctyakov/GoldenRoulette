using UnityEngine;

namespace SteinCo.Ivisa.RoulettePremium.Game
{
	public class PseudoBall : MonoBehaviour
	{
		public Transform pseudoBallRotationAxis;
		public float speed = 1.0f;

		private bool started = false;

		void Start()
		{
			pseudoBallRotationAxis.gameObject.SetActive(false);
		}

		void Update()
		{
			if (!started)
			{
				return;
			}

			pseudoBallRotationAxis.Rotate(Vector3.up * Time.deltaTime * speed);
		}

		public void Begin()
		{
			started = true;

			pseudoBallRotationAxis.gameObject.SetActive(true);

			pseudoBallRotationAxis.transform.localRotation = Quaternion.identity;
		}

		public void Stop()
		{
			started = false;

			pseudoBallRotationAxis.gameObject.SetActive(false);
		}
	}
}