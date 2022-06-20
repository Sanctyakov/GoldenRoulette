using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using SteinCo.Ivisa.RoulettePremium.Game;
using SteinCo.Ivisa.RoulettePremium.Sound;

namespace SteinCo.Ivisa.RoulettePremium.Menu
{
	public class HelperButtons : MonoBehaviour
	{
		public Toggle musicToggle;

		public GameObject racetrack;
		public GameObject racetrackTexts;
		public GameObject racetrackButtons;

		public GameLoop gameLoop;
		public BallSoundFX ballSoundFX;

		public AudioMixerGroup musicMixerGroup;
		public AudioMixerGroup audioMixerGroup;

		public GameObject panelMusicVolume;
		public GameObject panelAudioVolume;

		public Slider musicSlider;
		public Slider audioSlider;

		private const string musicPrefsKey = "RouletteMusicVolume";
		private const string audioPrefsKey = "RouletteAudioVolume";

		void Start()
		{
			float musicVolume = PlayerPrefs.GetFloat(musicPrefsKey, 1.0f);
			float audioVolume = PlayerPrefs.GetFloat(audioPrefsKey, 1.0f);

			if (Application.isEditor)
			{
				//musicVolume = 0.0f;
			}

			musicSlider.value = musicVolume;
			audioSlider.value = audioVolume;

			OnChangeVolumeMusic();
			OnChangeVolumeAudio();
		}

		public void OnChangeVolumeMusic()
		{
			float dB = 20f * Mathf.Log10(musicSlider.value);
			if (float.IsInfinity(dB))
			{
				dB = -80f;
			}
			musicMixerGroup.audioMixer.SetFloat("MusicVolume", dB);
		}

		public void OnChangeVolumeAudio()
		{
			float dB = 20f * Mathf.Log10(audioSlider.value);
			if (float.IsInfinity(dB))
			{
				dB = -80f;
			}
			audioMixerGroup.audioMixer.SetFloat("FXVolume", dB);
		}

		public void OnToggleMusic()
		{
			if (panelMusicVolume.activeSelf)
			{
				gameLoop.OnResumeTime();
				ballSoundFX.OnResume();
				PlayerPrefs.SetFloat(musicPrefsKey, musicSlider.value);
				panelMusicVolume.SetActive(false);
			}
			else
			{
				gameLoop.OnResumeTime();
				ballSoundFX.OnResume();

				//gameLoop.OnSuspendTime();
				gameLoop.OnCloseAfterSpecifiedTime();
				//ballSoundFX.OnPause();
				panelMusicVolume.SetActive(true);
				panelAudioVolume.SetActive(false);
				PlayerPrefs.SetFloat(audioPrefsKey, audioSlider.value);
			}

			

			//AudioListener.volume = audioToggle.isOn ? 1.0f : 0.0f;
			//music.volume = musicToggle.isOn ? maxVolumeMusic : 0.0f;
		}

		public void OnToggleAudio()
		{
			if (panelAudioVolume.activeSelf)
			{
				gameLoop.OnResumeTime();
				ballSoundFX.OnResume();
				PlayerPrefs.SetFloat(audioPrefsKey, audioSlider.value);
				panelAudioVolume.SetActive(false);
			}
			else
			{
				gameLoop.OnResumeTime();
				ballSoundFX.OnResume();

				//gameLoop.OnSuspendTime();
				gameLoop.OnCloseAfterSpecifiedTime();
				//ballSoundFX.OnPause();
				panelAudioVolume.SetActive(true);
				panelMusicVolume.SetActive(false);
				PlayerPrefs.SetFloat(musicPrefsKey, musicSlider.value);
			}
			
			//AudioListener.volume = audioToggle.isOn ? 1.0f : 0.0f;
			//music.volume = musicToggle.isOn ? maxVolumeMusic : 0.0f;
		}

		public void OnTurnOffPanels()
		{
			PlayerPrefs.SetFloat(musicPrefsKey, musicSlider.value);
			panelMusicVolume.SetActive(false);

			PlayerPrefs.SetFloat(audioPrefsKey, audioSlider.value);
			panelAudioVolume.SetActive(false);
		}

		public void OnToggleRacetrack()
		{
			if (!racetrack.activeSelf && !racetrackTexts.activeSelf && !racetrackButtons.activeSelf)
			{
				gameLoop.OnResumeTime();
				ballSoundFX.OnResume();
				racetrack.SetActive(true);
				racetrackTexts.SetActive(true);
				racetrackButtons.SetActive(true);
			}
			else
			{
				gameLoop.OnResumeTime();
				ballSoundFX.OnResume();
				racetrack.SetActive(false);
				racetrackTexts.SetActive(false);
				racetrackButtons.SetActive(false);
			}

		}
	}
}