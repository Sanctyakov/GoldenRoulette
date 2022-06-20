using UnityEngine;

namespace SteinCo.Ivisa.RoulettePremium.UI
{
	public class Spinner : MonoBehaviour
	{
		public float speed = 5.0f;
		private float angle = 0.0f;
		void Update()
		{
			angle += Time.unscaledDeltaTime * speed;
			transform.rotation = Quaternion.Euler(Vector3.back * angle);
		}
	}
}