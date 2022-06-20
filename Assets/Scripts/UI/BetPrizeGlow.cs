using UnityEngine;
using UnityEngine.UI;
using SteinCo.Utils;
using SteinCo.Ivisa.RoulettePremium.Chips;

namespace SteinCo.Ivisa.RoulettePremium.UI
{
	public class BetPrizeGlow : MonoBehaviour
	{
		public LoopColors loopColorsOutline;
		public LoopColors loopColorsText;

		public Outline outline;
		public Text prizeText;

		public AddWinningChips winningChips;

		private bool started = false;

		void Update()
		{
			if (!started)
			{
				return;
			}

			outline.effectColor = loopColorsOutline.CurrentColor;
			prizeText.color = loopColorsText.CurrentColor;
		}

		public void Begin()
		{
			if (started)
			{
				return;
			}
			started = true;

			loopColorsOutline.Begin();
			loopColorsText.Begin();
		}

		public void Stop()
		{
			started = false;

			outline.effectColor = loopColorsOutline.colorA;
			prizeText.color = loopColorsText.colorA;

			loopColorsOutline.Stop();
			loopColorsText.Stop();
		}
	}
}