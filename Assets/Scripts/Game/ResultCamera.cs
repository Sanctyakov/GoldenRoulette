using UnityEngine;

namespace SteinCo.Ivisa.RoulettePremium.Game
{
	public class ResultCamera : MonoBehaviour
	{
		public Transform ball;

		public float offset = 0.0f;
		public float height = 0.25f;
		public float distance = 0.2f;

		public int target = 0;

		void Update()
		{
			float angle = Vector3.Angle(ball.position, Vector3.right) * Mathf.Deg2Rad;
			transform.localPosition = new Vector3(Mathf.Cos(angle) * distance, height, Mathf.Sin(angle) * distance);
		}
	}
}