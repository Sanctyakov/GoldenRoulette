using UnityEngine;
using SteinCo.Ivisa.RoulettePremium.Game;

namespace SteinCo.Ivisa.RoulettePremium.Sound
{
	public class BallSoundFX : MonoBehaviour
	{
		public AudioClip ballTurning;
		public AudioClip[] bounces;

		public AudioClip ballStartTurning;
		public AudioClip ballTurningLoop;

		public AudioClip bounceEnding;
		public AudioClip turningLoop;
		public AudioSource audioSource;
		public CylinderController cylinderController;

		private bool attractLoop = false;

		void Awake()
		{
			cylinderController.OnSpinEnded += OnSpinEnded;
		}

		void OnDestroy()
		{
			cylinderController.OnSpinEnded -= OnSpinEnded;
		}

		private float timeCounter = 0.0f;
		private bool startedTurning = false;
		void Update()
		{
			if (!startedTurning)
			{
				return;
			}

			timeCounter += Time.deltaTime;

			if (timeCounter >= ballStartTurning.length)
			{
				startedTurning = false;
				audioSource.clip = ballTurningLoop;
				audioSource.loop = true;
				audioSource.Stop();
				audioSource.Play();
			}
		}

		public void PlayBounce()
		{
			if (attractLoop)
			{
				return;
			}
			audioSource.clip = bounces[Random.Range(0, bounces.Length)];
			audioSource.loop = false;
			audioSource.Stop();
			audioSource.Play();
		}

		public void PlayTurningLoop()
		{
			audioSource.clip = turningLoop;
			audioSource.loop = true;
			audioSource.Stop();
			audioSource.Play();
		}

		public void OnSpinEnded()
		{
			audioSource.clip = bounceEnding;
			audioSource.loop = false;
			audioSource.Stop();
			audioSource.Play();
		}

		public void OnSpin()
		{
			//audioSource.clip = ballTurning;
			audioSource.clip = ballStartTurning;
			startedTurning = true;
			timeCounter = 0.0f;

			audioSource.loop = false;
			audioSource.Stop();
			audioSource.Play();
		}

		public void OnPause()
		{
			audioSource.Pause();
		}

		public void OnResume()
		{
			audioSource.UnPause();
		}

		public void SetAttractLoopMode(bool mode)
		{
			attractLoop = mode;
		}
	}
}