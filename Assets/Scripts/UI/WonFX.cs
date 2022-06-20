using UnityEngine;
using UnityEngine.UI;
using SteinCo.Ivisa.RoulettePremium.Game;

namespace SteinCo.Ivisa.RoulettePremium.UI
{
	public class WonFX : MonoBehaviour
	{
		public Transform rays1;
		public Transform rays2;

		public ParticleSystem[] particles;

		public float speed1 = 10.0f;
		public float speed2 = -30.0f;

		public float durationStarting = 1.0f;
		public float durationEnding = 1.0f;

		public AnimationCurve scaleCurve;

		public float sizeMultiplier = 1.0f;

		private float timeCounter = 0.0f;
		public float maxSize = 0.0f;

		public CanvasGroup panel;
		public Text moneyText;

		private enum States
		{
			Idle,
			Starting,
			Animating,
			Ending,
		}
		private States state;

		void Start()
		{
			rays1.localScale = Vector3.zero;
			rays2.localScale = Vector3.zero;
		}

		void Update()
		{
			if (state == States.Idle)
			{
				return;
			}

			timeCounter += Time.deltaTime;
			switch (state)
			{
				case States.Starting:
					float v = Mathf.Clamp01(scaleCurve.Evaluate(timeCounter / durationStarting));

					rays1.localScale = Vector3.one * maxSize * v;
					rays2.localScale = Vector3.one * maxSize * v;

					panel.alpha = v;

					if (timeCounter > durationStarting)
					{
						state = States.Animating;
					}
					break;
				case States.Animating:
					rays1.localScale = Vector3.one * maxSize;
					rays2.localScale = Vector3.one * maxSize;
					break;
				case States.Ending:
					if (timeCounter > durationStarting)
					{
						state = States.Idle;
					}

					v = Mathf.Clamp01(scaleCurve.Evaluate(1.0f - timeCounter / durationEnding));

					rays1.localScale = Vector3.one * maxSize * v;
					rays2.localScale = Vector3.one * maxSize * v;

					panel.alpha = v;
					break;
				default:
					break;
			}

			Quaternion rotation = rays1.localRotation;
			rotation.eulerAngles += Vector3.up * speed1 * Time.deltaTime;
			rays1.localRotation = rotation;

			rotation = rays2.localRotation;
			rotation.eulerAngles += Vector3.up * speed2 * Time.deltaTime;
			rays2.localRotation = rotation;
		}

		public void Begin(int size, int moneyAmount)
		{
			if (state != States.Idle)
			{
				return;
			}

			state = States.Starting;

			timeCounter = 0.0f;

			//maxSize = size * sizeMultiplier;

			for (int i = 0; i < particles.Length; i++)
			{
				particles[i].Play();
			}

			moneyText.text = GameLoop.ConvertAmountToMoney(moneyAmount);
		}

		public void Stop()
		{
			if (state == States.Idle)
			{
				return;
			}

			for (int i = 0; i < particles.Length; i++)
			{
				particles[i].Stop();
			}

			state = States.Ending;
			timeCounter = 0.0f;
		}
	}
}