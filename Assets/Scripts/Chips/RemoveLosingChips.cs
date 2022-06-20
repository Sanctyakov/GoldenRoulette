using UnityEngine;
using System.Collections.Generic;
using SteinCo.Ivisa.RoulettePremium.Game;
using SteinCo.Ivisa.RoulettePremium.API.Utils;
using SteinCo.Ivisa.RoulettePremium.API;

namespace SteinCo.Ivisa.RoulettePremium.Chips
{
	public class RemoveLosingChips : MonoBehaviour
	{
		public ChipsController chipsController;
		public AnimateChips animateChips;

		public float timeBetweenChipAnimations = 0.25f;

		public Vector3 endingPosition;
		public Vector3 offsetp1;
		public Vector3 offsetp2;

		private bool started = false;
		private float timeCounter = 0.0f;
		private bool remainingChips = false;

		private List<BettingButton> loserPiles = new List<BettingButton>();

		public delegate void Finished();
		public event Finished OnFinished;

		public void Remove(Dictionary<BetController.BetTypes, string> apiCombinations)
		{
			started = true;
			remainingChips = true;

			// Convert losing piles info from API to actual pile IDs
			loserPiles.Clear();

			foreach (BetController.BetTypes type in System.Enum.GetValues(typeof(BetController.BetTypes)))
			{
				if (apiCombinations[type] == null)
				{
					apiCombinations[type] = string.Empty;
				}

				string[] loserCombinations = apiCombinations[type].Split(new string[] { APIStrings.betDivider }, System.StringSplitOptions.RemoveEmptyEntries);

				foreach (string s in loserCombinations)
				{
					string[] betParts = s.Split(new string[] { APIStrings.betCreditsDivider }, System.StringSplitOptions.RemoveEmptyEntries);

					int id = API2ID.ConvertToId(betParts[0] + APIStrings.betNextId, type);
					//int id = API2ID.ConvertToId(s, type);
					BettingButton button = API2ID.GetButton(type, id);

					loserPiles.Add(button);
				}
			}
			timeCounter = 0.0f;

			SelectChipsToAnimate();
		}

		void Update()
		{
			if (!started)
			{
				return;
			}

			timeCounter += Time.deltaTime;

			if (timeCounter > timeBetweenChipAnimations)
			{
				timeCounter -= timeBetweenChipAnimations;

				if (remainingChips)
				{
					remainingChips = SelectChipsToAnimate();
				}
				else
				{
					started = false;
					OnFinished.Invoke();
				}
			}
		}

		private bool SelectChipsToAnimate()
		{
			bool res = false;

			GameObject chip;
			int pool;

			foreach (BettingButton button in loserPiles)
			{
				if (button.chipsPile.RemainingChips > 0)
				{
					res = true;
					button.chipsPile.RemoveChip(button.uid, out chip, out pool);

					AnimateChips.ChipAnimation ca = new AnimateChips.ChipAnimation();
					ca.active = true;
					ca.chip = chip;
					ca.chipTransform = chip.transform;
					ca.pool = pool;
					ca.t = 0.0f;
					ca.p0 = chip.transform.position;
					ca.p1 = chip.transform.position + offsetp1;
					ca.p2 = chip.transform.position + offsetp2;
					ca.p3 = endingPosition;
					ca.callback = Callback;

					animateChips.Add(ca);
				}
			}

			return res;
		}

		private void Callback(AnimateChips.ChipAnimation ca)
		{
			chipsController.ReturnChip(ca.chip, ca.pool);
		}
	}
}