using UnityEngine;
using UnityEngine.UI;
using SteinCo.Utils;

namespace SteinCo.Ivisa.RoulettePremium.UI
{
	public class TimeAlert : MonoBehaviour
	{
		public AudioSource timerSoundPlayer;

		public LoopColors loopColorsOutline;
		public LoopColors loopColorsText;
		public Outline outline;
		public Text text;

		private bool started = false;

		void Update()
		{
			if (!started)
			{
				return;
			}

			outline.effectColor = loopColorsOutline.CurrentColor;
			text.color = loopColorsText.CurrentColor;
		}

		public void Begin()
		{
			if (started)
			{
				return;
			}
			started = true;

			Bip();
			InvokeRepeating("Bip", 5.0f, 1.0f);
			Invoke("Glow", 5.0f);
		}

		public void Stop()
		{
			started = false;

			outline.effectColor = loopColorsOutline.colorA;
			text.color = loopColorsText.colorA;

			loopColorsOutline.Stop();
			loopColorsText.Stop();

			CancelInvoke();
		}

		private void Glow()
		{
			loopColorsOutline.Begin();
			loopColorsText.Begin();
		}

		private void Bip()
		{
			timerSoundPlayer.Stop();
			timerSoundPlayer.Play();
		}
	}
}