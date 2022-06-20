using UnityEngine;

namespace SteinCo.Ivisa.RoulettePremium.Sound
{
	public class ChipsSoundFX : MonoBehaviour
	{
		public AudioClip[] chips;
		public AudioClip cancelPrevious;
		public AudioClip cancelAll;
		public AudioClip failed;
		public AudioClip error;

		public AudioSource audioSource;

		public void OnBet()
		{
			audioSource.clip = chips[Random.Range(0, chips.Length)];
			audioSource.loop = false;
			audioSource.pitch = 1.0f;
			audioSource.volume = 1.0f;
			audioSource.Stop();
			audioSource.Play();
		}

		public void OnBetFailed()
		{
			audioSource.clip = failed;
			audioSource.loop = false;
			audioSource.pitch = 1.0f;
			audioSource.volume = 1.0f;
			audioSource.Stop();
			audioSource.Play();
		}

		public void OnCancelAll()
		{
			audioSource.clip = cancelAll;
			audioSource.loop = false;
			audioSource.pitch = 1.0f;
			audioSource.volume = 1.0f;
			audioSource.Stop();
			audioSource.Play();
		}

		public void OnCancelPrevious()
		{
			audioSource.clip = cancelPrevious;
			audioSource.loop = false;
			audioSource.pitch = 1.0f;
			audioSource.volume = 1.0f;
			audioSource.Stop();
			audioSource.Play();
		}

		public void OnError()
		{
			audioSource.clip = error;
			audioSource.loop = false;
			audioSource.pitch = 0.3f;
			audioSource.volume = 0.5f;
			audioSource.Stop();
			audioSource.Play();
		}
	}
}