using UnityEngine;
using System.Collections.Generic;
using SteinCo.Ivisa.RoulettePremium.Game;
using SteinCo.Ivisa.RoulettePremium.Sound;
using SteinCo.Ivisa.RoulettePremium.UI;
using SteinCo.Utils.Pooling;
using SteinCo.Ivisa.RoulettePremium.API.Utils;
using SteinCo.Ivisa.RoulettePremium.API;

namespace SteinCo.Ivisa.RoulettePremium.Chips
{
	public class AddWinningChips : MonoBehaviour
	{
		public ChipsController chipsController;
		public ChipGUITexts chipGUITexts;
		public BetController betController;
		public AnimateChips animateChips;
		public ChipsSoundFX chipSoundFX;
		public PrizeGlow prizeGlow;
		public WonFX wonFX;

		public Pool poolPrizePiles;
		public Transform prizePilesInitialPosition;
		public float timeBetweenChipAnimations = 0.25f;
		public float waitingTime = 2.0f;

		public Vector3 prizeInitialPosition;
		public Vector3 prizeInitialPositionOffsetp1;
		public Vector3 prizeInitialPositionOffsetp2;

		public Vector3 prizeEndingPosition;
		public Vector3 prizeEndingPositionOffsetp1;
		public Vector3 prizeEndingPositionOffsetp2;

		public float pileSeparation = 0.01f;

		private struct PileInfo
		{
			public ChipsPile prizeChipsPile; // new piles for prize
			public int creditsAmount; // amount of credits to create prize piles
			public int[] chipsAmount; // the amount and type of chips for prize piles
			public int chipsCounter; // to have control on which chips will be created for prize piles
			public BettingButton button;
		}

		private bool started = false;
		private float timeCounter = 0.0f;
		private bool remainingChips = false;
		private bool firstChip = false;

		private List<PileInfo> winningPiles = new List<PileInfo>();
		private List<PileInfo> prizePiles = new List<PileInfo>();

		private int sideSize;
		private int moneyAmount;

		public delegate void Finished();
		public event Finished OnFinished;

		void Start()
		{
			prizeGlow.HideNow();
		}

		public void Add(Dictionary<BetController.BetTypes, string> apiCombinations, int totalAmount)
		{
			if (totalAmount == 0)
			{
				OnFinished.Invoke();
				return;
			}

			moneyAmount = totalAmount;

			started = true;
			remainingChips = true;
			firstChip = true;

			winningPiles.Clear();
			prizePiles.Clear();

			// Add the winning bets on the table
			foreach (BetController.BetTypes type in System.Enum.GetValues(typeof(BetController.BetTypes)))
			{
				string[] winningCombinations = apiCombinations[type].Split(new string[] { APIStrings.betDivider }, System.StringSplitOptions.RemoveEmptyEntries);

				foreach (string s in winningCombinations)
				{
					string[] betParts = s.Split(new string[] { APIStrings.betCreditsDivider }, System.StringSplitOptions.RemoveEmptyEntries);

					int id = API2ID.ConvertToId(betParts[0] + APIStrings.betNextId, type);
					BettingButton button = API2ID.GetButton(type, id);

					PileInfo pileInfo = new PileInfo();

					pileInfo.creditsAmount = int.Parse(betParts[1]);

					pileInfo.prizeChipsPile = button.chipsPile;// poolPrizePiles.GetNewObject().GetComponent<ChipsPile>();
					pileInfo.prizeChipsPile.chipsController = chipsController;
					pileInfo.prizeChipsPile.chipGUITexts = chipGUITexts;
					pileInfo.prizeChipsPile.transform.position = button.chipsPile.transform.position;
					//pileInfo.prizeChipsPile.UpdateChipPile(betController.GetPileBetAmount(button.type, id), button.uid, button.chipsPile.transform.position, true);
					//pileInfo.prizeChipsPile = button.chipsPile;
					pileInfo.button = button;
					pileInfo.creditsAmount = betController.GetPileBetAmount(button.type, id);
					winningPiles.Add(pileInfo);
				}
			}

			chipGUITexts.SetCanvasToCamera();

			//winningPiles.Sort((x, y) => x.creditsAmount.CompareTo(y.creditsAmount));

			/*
			* OPTION A
			* Create a duplicate of the piles on the table and make a square
			*/

			// Create piles, split the ones too large

			for (int i = 0; i < winningPiles.Count; i++)
			{
				List<PileInfo> pileInfoPiles = SplitLargePiles(winningPiles[i]);

				foreach (PileInfo pileInfo in pileInfoPiles)
				{
					prizePiles.Add(pileInfo);
				}
			}

			prizePiles.Sort((x, y) => x.creditsAmount.CompareTo(y.creditsAmount));

			// Set up the prize piles positions now that we now how many there are
			int totalPiles = prizePiles.Count;
			int w = Mathf.CeilToInt(Mathf.Sqrt(Mathf.CeilToInt(totalPiles)));
			float w2 = w / 2.0f * pileSeparation;
			int index = 0;

			for (int y = 0; y < w; y++)
			{
				for (int x = 0; x < w; x++)
				{
					if (index < totalPiles)
					{
						PileInfo pileInfo = prizePiles[index];

						pileInfo.prizeChipsPile.transform.localRotation = Quaternion.identity;
						pileInfo.prizeChipsPile.transform.localPosition = Vector3.right * x * pileSeparation - Vector3.right * w2 + Vector3.forward * y * pileSeparation - Vector3.forward * w2;

						prizePiles[index] = pileInfo;

						index++;
					}
				}
			}

			sideSize = w;

			/*
			* OPTION B
			* Create new piles in a cube from the total prize amount
			*/
			/*
			// Get the complete list of chips for the total prize amount
			List<int> prizeChipAmountList = chipsController.GetChipAmountsPerSlot(totalAmount);

			int w = Mathf.CeilToInt(Mathf.Pow(prizeChipAmount, 1.0f / 3.0f));
			int w2 = w * w;
			float wh = w / 2.0f * pileSeparation;
			int counter = 0;

			int totalPrizeDivided = prizeChipAmount / w2;

			for (int y = 0; y < w; y++)
			{
				for (int x = 0; x < w; x++)
				{
					counter++;
					if (counter < w2)
					{
						PileInfo pileInfo = new PileInfo();
						pileInfo.prizeChipsPile = poolPrizePiles.GetNewObject().GetComponent<ChipsPile>();
						pileInfo.prizeChipsPile.chipsController = chipsController;
						pileInfo.prizeChipsPile.transform.SetParent(prizePilesInitialPosition);
						pileInfo.prizeChipsPile.transform.localRotation = Quaternion.identity;



						pileInfo.chipsAmount = chipsController.GetChipAmountsPerSlot(100000).ToArray();

						prizeChipAmount -= totalPrizeDivided;

						if (prizeChipAmount < totalPrizeDivided)
						{
							totalPrizeDivided = prizeChipAmount;
						}


						pileInfo.chipsCounter = 0;

						prizePiles.Add(pileInfo);
					}
				}

			}
			*/

			timeCounter = 0.0f;
			animationState = AnimationStates.Drop;

			SelectChipsToAnimate();
		}

		public int arbitraryLimit = 30;

		private List<PileInfo> SplitLargePiles(PileInfo pileInfo)
		{
			List<PileInfo> res = new List<PileInfo>();

			int[] chipsAmount = chipsController.GetChipAmountsPerSlot(pileInfo.creditsAmount).ToArray();

			int totalChips = 0;
			for (int i = 0; i < chipsAmount.Length; i++)
			{
				totalChips += chipsAmount[i];
			}

			int pileAmount = Mathf.CeilToInt(totalChips / (float)arbitraryLimit);

			for (int j = 0; j < pileAmount; j++)
			{
				PileInfo newPile = new PileInfo();
				newPile.chipsAmount = new int[chipsAmount.Length];

				newPile.prizeChipsPile = poolPrizePiles.GetNewObject().GetComponent<ChipsPile>();
				newPile.prizeChipsPile.chipsController = chipsController;
				newPile.prizeChipsPile.chipGUITexts = chipGUITexts;
				newPile.prizeChipsPile.transform.SetParent(prizePilesInitialPosition);

				bool finished = false;
				int slot = 0;
				int counter = 0;

				while (!finished)
				{
					if (chipsAmount[slot] > 0)
					{
						chipsAmount[slot]--;
						newPile.chipsAmount[slot]++;

						// Hackish, but it works to have something to sort this later
						newPile.creditsAmount++;
						counter++;
					}
					else
					{
						slot++;

						if (slot >= chipsAmount.Length)
						{
							finished = true;
						}
					}

					if (counter >= arbitraryLimit)
					{
						finished = true;
					}
				}

				res.Add(newPile);
			}

			return res;
		}

		void Update()
		{
			if (!started)
			{
				return;
			}

			timeCounter += Time.deltaTime;

			if (chipAddedToPile)
			{
				chipAddedToPile = false;
				chipSoundFX.OnBet();
			}

			switch (animationState)
			{
				case AnimationStates.Drop:
					if (timeCounter > timeBetweenChipAnimations)
					{
						timeCounter -= timeBetweenChipAnimations;

						SelectChipsToAnimate();
					}
					break;
				case AnimationStates.Wait:
					if (timeCounter > waitingTime)
					{
						timeCounter = 0.0f;
						animationState = AnimationStates.End;
						wonFX.Stop();
					}
					break;
				case AnimationStates.End:
					if (timeCounter > timeBetweenChipAnimations)
					{
						timeCounter -= timeBetweenChipAnimations;

						if (remainingChips)
						{
							chipSoundFX.OnBet();
							remainingChips = SelectChipsToAnimate();
						}
						else
						{
							foreach (PileInfo pile in prizePiles)
							{
								poolPrizePiles.ReturnUsedObject(pile.prizeChipsPile.gameObject);
							}
							prizeGlow.Hide();

							OnFinished.Invoke();
							started = false;
						}
					}
					break;
			}


		}

		private enum AnimationStates
		{
			Drop,
			Wait,
			End,
		}
		private AnimationStates animationState = AnimationStates.Drop;

		private bool SelectChipsToAnimate()
		{
			bool res = false;

			GameObject chip = null;
			int pool = 0;
			AnimateChips.ChipAnimation ca;

			float chipHeight = 0.005f;

			switch (animationState)
			{
				case AnimationStates.Drop:
					// Create prize piles
					res = false;
					for (int pileCounter = 0; pileCounter < prizePiles.Count; pileCounter++)
					{
						PileInfo pileInfo = prizePiles[pileCounter];
						bool pileChanged = false;

						////DebugWithDate.Log("<color=red>Pile counter: " + pileCounter+ "</color>");
						for (int i = 0; i < pileInfo.chipsAmount.Length; i++)
						{
							////DebugWithDate.Log("<color=green>Index: " + i + "</color>");
							if (pileInfo.chipsAmount[i] > 0)
							{
								////DebugWithDate.Log("Chip present :" + pileInfo.chipsAmount[i]);
								res = true;
								int slot = pileInfo.chipsAmount.Length - 1 - i;
								chip = chipsController.GetChip(slot);
								chip.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
								chip.transform.SetParent(pileInfo.prizeChipsPile.transform);
								pileInfo.chipsAmount[i]--;

								ca = new AnimateChips.ChipAnimation();
								ca.active = true;
								ca.chip = chip;
								ca.chipTransform = chip.transform;
								ca.pool = slot;
								ca.chipsPile = pileInfo.prizeChipsPile;
								ca.t = 0.0f;
								ca.p0 = prizeInitialPosition;
								ca.p1 = prizeInitialPosition + prizeInitialPositionOffsetp1;
								ca.p2 = prizeInitialPosition + prizeInitialPositionOffsetp2;

								ca.p0 = prizeInitialPosition - pileInfo.prizeChipsPile.transform.position;
								ca.p1 = ca.p0 + prizeInitialPositionOffsetp1;
								ca.p2 = ca.p0 + prizeInitialPositionOffsetp2;

								//ca.p3 = pileInfo.prizeChipsPile.transform.position + pileInfo.prizeChipsPile.transform.up * chipHeight * pileInfo.chipsCounter;
								ca.p3 = Vector3.up * chipHeight * pileInfo.chipsCounter;

								ca.callback = Callback;
								ca.local = true;

								pileInfo.chipsCounter++;
								pileChanged = true;

								animateChips.Add(ca);

								break;
							}
							else
							{
								////DebugWithDate.Log("<color=blue>Pile counter: " + pileCounter + "  i:" + i + "</color>");
							}
						}

						if (pileChanged)
						{
							////DebugWithDate.Log("<color=magenta>Pile changed" + pileCounter + "</color>");
							prizePiles[pileCounter] = pileInfo;
						}
					}

					if (!res)
					{
						animationState = AnimationStates.Wait;
					}

					break;
				case AnimationStates.Wait:
					break;
				case AnimationStates.End:
					// Prize piles
					res = false;
					for (int pileCounter = 0; pileCounter < prizePiles.Count; pileCounter++)
					{
						PileInfo pileInfo = prizePiles[pileCounter];
						if (pileInfo.prizeChipsPile.RemainingChips > 0)
						{
							res = true;

							pileInfo.prizeChipsPile.RemoveChip(0, out chip, out pool);

							chip.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
							ca = new AnimateChips.ChipAnimation();
							ca.active = true;
							ca.chip = chip;
							ca.chipTransform = chip.transform;
							ca.pool = pool;
							ca.t = 0.0f;
							ca.p0 = chip.transform.position;
							ca.p1 = chip.transform.position + prizeEndingPositionOffsetp1;
							ca.p2 = chip.transform.position + prizeEndingPositionOffsetp2;
							ca.p3 = prizeEndingPosition;
							ca.callback = Callback2;
							ca.local = false;

							animateChips.Add(ca);
						}
					}

					/*
					// Table piles
					for (int pileCounter = 0; pileCounter < winningPiles.Count; pileCounter++)
					{
						PileInfo pileInfo = winningPiles[pileCounter];
						if (pileInfo.prizeChipsPile.RemainingChips > 0)
						{
							res = true;

							pileInfo.prizeChipsPile.RemoveChip(out chip, out pool);

							ca = new AnimateChips.ChipAnimation();
							ca.active = true;
							ca.chip = chip;
							ca.chipTransform = chip.transform;
							ca.pool = pool;
							ca.t = 0.0f;
							ca.p0 = chip.transform.position;
							ca.p1 = chip.transform.position + prizeEndingPositionOffsetp1;
							ca.p2 = chip.transform.position + prizeEndingPositionOffsetp2;
							ca.p3 = prizeEndingPosition;
							ca.callback = Callback2;
							ca.local = true;

							animateChips.Add(ca);
						}
					}
					*/
					break;
			}

			return res;
		}

		private bool chipAddedToPile = false;
		private void Callback(AnimateChips.ChipAnimation ca)
		{
			if (firstChip)
			{
				firstChip = false;
				prizeGlow.Show();
				wonFX.Begin(sideSize, moneyAmount);
			}

			chipAddedToPile = true;
			ca.chipsPile.AddChip(ca.chip, ca.pool, 0, ca.chipsPile.transform.position, false, true);
			ca.chip.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		}

		private void Callback2(AnimateChips.ChipAnimation ca)
		{
			ca.chip.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			chipsController.ReturnChip(ca.chip, ca.pool);
		}

		public void ClearTable()
		{
			// Table piles
			for (int pileCounter = 0; pileCounter < winningPiles.Count; pileCounter++)
			{
				PileInfo pileInfo = winningPiles[pileCounter];
				pileInfo.prizeChipsPile.UpdateChipPile(0, 0, Vector3.zero, true);
				//poolPrizePiles.ReturnUsedObject(pileInfo.prizeChipsPile.gameObject);
			}
			winningPiles.Clear();
		}

		public void ResetTexts()
		{
			for (int pileCounter = 0; pileCounter < winningPiles.Count; pileCounter++)
			{
				PileInfo pileInfo = winningPiles[pileCounter];
				pileInfo.prizeChipsPile.UpdateChipPile(pileInfo.creditsAmount, pileInfo.button.uid, pileInfo.button.transform.position, true, true);
			}
		}
	}
}