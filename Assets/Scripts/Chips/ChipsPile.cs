using UnityEngine;
using System.Collections.Generic;
using SteinCo.Ivisa.RoulettePremium.UI;
using SteinCo.Ivisa.RoulettePremium.Game;
using SteinCo.Ivisa.RoulettePremium.API.Utils;
using SteinCo.Utils;

namespace SteinCo.Ivisa.RoulettePremium.Chips
{
	public class ChipsPile : MonoBehaviour
	{
		public ChipsController chipsController;
		public ChipGUITexts chipGUITexts;
		private Stack<Chip> chips = new Stack<Chip>();
		private GameObject previousTopChip;
		private Vector3 scale = new Vector3(1.1f, 1.0f, 1.1f);

		private int maxChipsToShow = 0;
		private int maxChipsToShowBackup = -1;
		public int previousAmount = 0;
		private static int maximumPileSize = -1;

		public int previousPlaysAmount;
		public int previousPlaysWonAmount;
		public int previousPlaysLostAmount;

		private struct Chip
		{
			public GameObject chip;
			public int pool;
			public GameObject previousTopChip;
		}

		void Start()
		{
			if (maximumPileSize == -1)
			{
				maximumPileSize = int.Parse(WorkingData.GetData("Maximum chips per pile", "15"));
			}
		}

		public void UpdateChipPile(int amount, int bettingButtonId, Vector3 basePosition, bool inTable, bool useBackup = false)
		{
			if (amount == 0)
			{
				maxChipsToShow = 0;
				previousAmount = 0;
			}

			GameObject chip;
			int pool;

			// Clear all chips
			while (chips.Count > 0)
			{
				RemoveChip(bettingButtonId, out chip, out pool);
				chipsController.ReturnChip(chip, pool);
			}

			chipGUITexts.HideText(bettingButtonId);

			// Add chips
			int slotAmount = chipsController.SlotAmount;

			//int remainder = amount;

			List<int> chipAmountPerSlot = chipsController.GetChipAmountsPerSlot(amount);

			// Fix pilas altas

			int totalChips = 0;

			for (int i = 0; i < chipAmountPerSlot.Count; i++)
			{
				totalChips += chipAmountPerSlot[i];
			}

			if (inTable)
			{
				if (useBackup)
				{
					maxChipsToShow = maxChipsToShowBackup;
				}
				else
				{
					if (amount < previousAmount)
					{
						maxChipsToShow--;

						if (maxChipsToShow < 1)
						{
							maxChipsToShow = maximumPileSize;
						}
					}
					else
					{
						if (amount > 0)
						{
							maxChipsToShow++;

							if (maxChipsToShow > maximumPileSize)
							{
								maxChipsToShow = 1;
							}
						}
					}

					if (amount != 0)
					{
						maxChipsToShowBackup = maxChipsToShow;
					}
				}

				int chipsToRemove = 0;

				if (totalChips > maxChipsToShow)
				{
					chipsToRemove = totalChips - maxChipsToShow;
				}

				totalChips = chipsToRemove;

				if (chipsToRemove > 0)
				{
					for (int i = 0; i < chipAmountPerSlot.Count; i++)
					{
						while (chipAmountPerSlot[i] > 0 && chipsToRemove > 0)
						{
							chipAmountPerSlot[i]--;
							chipsToRemove--;
						}
					}

					AddChip(chipsController.GetChip(6), 6, bettingButtonId, basePosition, inTable, false);

					int maxBetAmountQuarter = BetController.Limits[API2ID.GetButtonBetTypeByUID(bettingButtonId)] / 4;
					if (amount > maxBetAmountQuarter)
					{
						AddChip(chipsController.GetChip(6), 6, bettingButtonId, basePosition, inTable, false);
					}
					if (amount > maxBetAmountQuarter * 2)
					{
						AddChip(chipsController.GetChip(6), 6, bettingButtonId, basePosition, inTable, false);
					}
					if (amount > maxBetAmountQuarter * 3)
					{
						AddChip(chipsController.GetChip(6), 6, bettingButtonId, basePosition, inTable, false);
					}
				}

				previousAmount = amount;

				/*
				int maxBetAmount = BetController.Limits[API2ID.GetButtonBetTypeByUID(bettingButtonId)];

				int maxChipTimesInMaxBet = maxBetAmount / chipsController.HigherChip;

				int maxBetAmountDivisor = maxBetAmount / (maxChipTimesInMaxBet / 4);
				int maxBetAmountDivisor2 = maxBetAmount / (maxChipTimesInMaxBet / 2);

				int numberOfExtraChips = 0;
				int numberOfExtraChips2 = 0;

				if (amount >= maxBetAmountDivisor)
				{
					numberOfExtraChips = remainder / maxBetAmountDivisor;
					remainder = remainder % maxBetAmountDivisor;
				}

				/*
				if (remainder >= maxBetAmountDivisor2)
				{
					numberOfExtraChips2 = remainder / maxBetAmountDivisor2;
					remainder = remainder % maxBetAmountDivisor2;
				}*/

				//	AddChip(chipsController.GetChip(6), 6, bettingButtonId, basePosition, inTable, false);

				/*

				for (int i = 0; i < numberOfExtraChips; i++)
				{
					AddChip(chipsController.GetChip(6), 6, bettingButtonId, basePosition, inTable, false);
				}

				for (int i = 0; i < numberOfExtraChips2; i++)
				{
					AddChip(chipsController.GetChip(7), 7, bettingButtonId, basePosition, inTable, false);
				}*/
			}

			// End Fix

			bool last = false;

			for (int i = 0; i < slotAmount; i++)
			{
				for (int j = 0; j < chipAmountPerSlot[i]; j++)
				{
					totalChips--;

					//if (totalChips == 1)
					if (j == chipAmountPerSlot[i] - 1)
					{
						last = true;
					}
					if (chipsController == null)
					{
						////DebugWithDate.Log("chip controller not present");
					}
					AddChip(chipsController.GetChip(slotAmount - i - 1), slotAmount - i - 1, bettingButtonId, basePosition, inTable, last);
				}
			}
		}

		public void AddChip(GameObject chip, int pool, int bettingButtonId, Vector3 basePosition, bool inTable, bool last)
		{

			Chip currentChip = new Chip();
			currentChip.chip = chip;
			currentChip.pool = pool;
			currentChip.previousTopChip = previousTopChip;
			chips.Push(currentChip);

			float chipHeight = 0.005f;
			float angle = Random.Range(-Mathf.PI, Mathf.PI);
			float distance = 0.001f;
			Vector3 shuffle = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle)) * distance;

			/*
			if (last)
			{
				shuffle = Vector3.zero;
			}
			*/
			shuffle = Vector3.zero;

			chip.transform.position = basePosition + Vector3.up * chipHeight * (chips.Count - 1) + shuffle;

			if (inTable)
			{
				chip.transform.localScale = scale;
			}
			else
			{
				chip.transform.localScale = Vector3.one;
			}


			// Turn off the shadow and the text on chips behind others, to save some performance
			if (currentChip.previousTopChip != null)
			{
				previousTopChip.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				previousTopChip.transform.GetChild(0).gameObject.SetActive(false);
			}

			// Turn off the text on this chip, to let the UI draw it (Unity's font rendering is not good at small sizes)

			if (inTable)
			{
				chip.transform.GetChild(0).gameObject.SetActive(false);

				if (pool < chipsController.SlotAmount)
				{
					chipGUITexts.SetText(bettingButtonId, chip, pool);
				}
			}
			else
			{
				chip.transform.GetChild(0).gameObject.SetActive(true);
			}

			previousTopChip = chip;
		}

		public void RemoveChip(int bettingButtonId, out GameObject chip, out int pool)
		{
			Chip currentChip = chips.Pop();
			chip = currentChip.chip;
			pool = currentChip.pool;

			// Turn on the text on the chip as it's going again to the top
			previousTopChip = currentChip.previousTopChip;
			if (previousTopChip != null)
			{
				previousTopChip.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
				previousTopChip.transform.GetChild(0).gameObject.SetActive(true);
			}

			chipGUITexts.HideText(bettingButtonId);
			chip.transform.GetChild(0).gameObject.SetActive(true);
		}

		public int RemainingChips
		{
			get
			{
				return chips.Count;
			}
		}
	}
}