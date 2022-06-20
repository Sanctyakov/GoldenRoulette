using UnityEngine;
using UnityEngine.UI;
using SteinCo.Utils;

namespace SteinCo.Ivisa.RoulettePremium.UI
{
	public class HistoryGlow : MonoBehaviour
	{
		public LoopColors loopColors;

		public Outline[] outlines;

		private bool started = false;
		private int target;

		void Update()
		{
			if (!started)
			{
				return;
			}

			outlines[target].effectColor = loopColors.CurrentColor;
		}

		public void Begin(int number)
		{
			started = true;
			target = number;
			loopColors.Begin();
		}

		public void Stop()
		{
			started = false;
			loopColors.Stop();
			outlines[target].effectColor = loopColors.colorA;
		}
	}
}