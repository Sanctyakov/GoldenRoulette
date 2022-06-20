using UnityEngine;
using System.Collections.Generic;
using SteinCo.Ivisa.RoulettePremium.UI;
using SteinCo.Ivisa.RoulettePremium.Chips;
using SteinCo.Ivisa.RoulettePremium.API.Classes;
using SteinCo.Ivisa.RoulettePremium.API.Utils;
using SteinCo.Ivisa.RoulettePremium.API;

namespace SteinCo.Ivisa.RoulettePremium.Game
{
	public class BetController : MonoBehaviour
	{
		public ChipsController chipsController;
		public GameLoop gameLoop;
		public InfoMessage infoMessage;
		public Tooltips tooltips;
		public Vector2 toolTipPosition;
		public Vector2 toolTipPositionPreviousPlays;

		public delegate void BetChanged(int amount);
		public event BetChanged OnBetChanged;

		private static Dictionary<BetTypes, int> typeLimits = new Dictionary<BetTypes, int>();

		public static Dictionary<BetTypes, int> Limits
		{
			get
			{
				return typeLimits;
			}
		}

		private Dictionary<BetTypes, string> payouts = new Dictionary<BetTypes, string>();

		private List<BettingButton> previousBetsBettingButtons = new List<BettingButton>();

		private struct Bet
		{
			public int pileId;
			public BetTypes betType;
			public int amount;
			public int slot;
			public int previousAmount;
			public BettingButton bettingButton;
		}

		public enum BetTypes
		{
			Pleno,
			Semipleno,
			Calle,
			Cuadrado,
			Linea,
			Color,
			ParImpar,
			MenorMayor,
			Docena,
			Columna,
			TransversalConCero,
			//Combinadas
		}

		public int credits;
		private int minBet;
		private int maxBet;
		private int previousBetAmount;
		public int apMinChance;
		public int apMinSimple;

		public void SetUp(InitAnswerWeb initAnswer)
		{
			credits = initAnswer.Credito;
			minBet = initAnswer.apuestaMinima;
			maxBet = initAnswer.apuestaMaxima;
			apMinChance = initAnswer.apMinimaChance;
			apMinSimple = initAnswer.apMinimaSimple;

			typeLimits = new Dictionary<BetTypes, int>();
			typeLimits.Add(BetTypes.Pleno, initAnswer.apMaximaSimple);
			typeLimits.Add(BetTypes.Semipleno, initAnswer.apMaximaSimple);
			typeLimits.Add(BetTypes.TransversalConCero, initAnswer.apMaximaSimple);
			typeLimits.Add(BetTypes.Calle, initAnswer.apMaximaSimple);
			typeLimits.Add(BetTypes.Cuadrado, initAnswer.apMaximaSimple);
			typeLimits.Add(BetTypes.Linea, initAnswer.apMaximaSimple);

			typeLimits.Add(BetTypes.Color, initAnswer.apMaximaChance);
			typeLimits.Add(BetTypes.ParImpar, initAnswer.apMaximaChance);
			typeLimits.Add(BetTypes.MenorMayor, initAnswer.apMaximaChance);
			typeLimits.Add(BetTypes.Docena, initAnswer.apMaximaChance);
			typeLimits.Add(BetTypes.Columna, initAnswer.apMaximaChance);

			payouts = new Dictionary<BetTypes, string>();
			payouts.Add(BetTypes.Pleno, "35:1");
			payouts.Add(BetTypes.Semipleno, "17:1");
			payouts.Add(BetTypes.TransversalConCero, "8:1");
			payouts.Add(BetTypes.Calle, "11:1");
			payouts.Add(BetTypes.Cuadrado, "8:1");
			payouts.Add(BetTypes.Linea, "5:1");
			payouts.Add(BetTypes.Color, "1:1");
			payouts.Add(BetTypes.ParImpar, "1:1");
			payouts.Add(BetTypes.MenorMayor, "1:1");
			payouts.Add(BetTypes.Docena, "2:1");
			payouts.Add(BetTypes.Columna, "2:1");

			infoMessage.ShowMessage(InfoMessage.MessageTypes.PleaseBet, false);
		}

		public void SetUp(InitAnswer initAnswer)
		{
			credits = initAnswer.Credito;
			minBet = initAnswer.apuestaMinima;
			maxBet = initAnswer.apuestaMaxima;
			apMinChance = initAnswer.apMinimaChance;
			apMinSimple = initAnswer.apMinimaSimple;

			typeLimits = new Dictionary<BetTypes, int>();
			typeLimits.Add(BetTypes.Pleno, initAnswer.apMaximaSimple);
			typeLimits.Add(BetTypes.Semipleno, initAnswer.apMaximaSimple);
			typeLimits.Add(BetTypes.TransversalConCero, initAnswer.apMaximaSimple);
			typeLimits.Add(BetTypes.Calle, initAnswer.apMaximaSimple);
			typeLimits.Add(BetTypes.Cuadrado, initAnswer.apMaximaSimple);
			typeLimits.Add(BetTypes.Linea, initAnswer.apMaximaSimple);

			typeLimits.Add(BetTypes.Color, initAnswer.apMaximaChance);
			typeLimits.Add(BetTypes.ParImpar, initAnswer.apMaximaChance);
			typeLimits.Add(BetTypes.MenorMayor, initAnswer.apMaximaChance);
			typeLimits.Add(BetTypes.Docena, initAnswer.apMaximaChance);
			typeLimits.Add(BetTypes.Columna, initAnswer.apMaximaChance);

			payouts = new Dictionary<BetTypes, string>();
			payouts.Add(BetTypes.Pleno, "35:1");
			payouts.Add(BetTypes.Semipleno, "17:1");
			payouts.Add(BetTypes.TransversalConCero, "8:1");
			payouts.Add(BetTypes.Calle, "11:1");
			payouts.Add(BetTypes.Cuadrado, "8:1");
			payouts.Add(BetTypes.Linea, "5:1");
			payouts.Add(BetTypes.Color, "1:1");
			payouts.Add(BetTypes.ParImpar, "1:1");
			payouts.Add(BetTypes.MenorMayor, "1:1");
			payouts.Add(BetTypes.Docena, "2:1");
			payouts.Add(BetTypes.Columna, "2:1");

			infoMessage.ShowMessage(InfoMessage.MessageTypes.PleaseBet, false);
		}

		private Stack<Bet> stack = new Stack<Bet>();
		private Stack<Bet> previousBetStack;

		private Stack<Bet> backupBetStack;

		private Dictionary<BetTypes, int[]> piles = new Dictionary<BetTypes, int[]>();
		private Dictionary<BetTypes, bool[]> usedPiles = new Dictionary<BetTypes, bool[]>();
		private int totalAmount;

		void Awake()
		{
			//BettingButton.OnBetMade += OnBetMade;
			RouletteType.OnRouletteTypeChanged += OnRouletteTypeChanged;
		}
			
		void OnDestroy()
		{
			//BettingButton.OnBetMade -= OnBetMade;
			RouletteType.OnRouletteTypeChanged -= OnRouletteTypeChanged;
		}

		private void OnRouletteTypeChanged(RouletteType.Types type)
		{
			CancelAllBets();
		}

		void Start()
		{
			typeLimits.Clear();
			piles.Clear();
			usedPiles.Clear();

			foreach (BetTypes type in System.Enum.GetValues(typeof(BetTypes)))
			{
				typeLimits.Add(type, 0);
				int pileAmountForType = API2ID.RegisteredPiles(type);
				piles.Add(type, new int[pileAmountForType]);
				usedPiles.Add(type, new bool[pileAmountForType]);
			}
			
			
			chanceBets = new List<ChipsPile>(gameLoop.chanceBets);
		}

		public void OnBetMade(BettingButton bettingButton)
		{
			//int pileId = API2ID.ConvertToId(bettingButton.apiCombination, bettingButton.type);

			int amount = chipsController.SelectedValue;

			////DebugWithDate.Log(bettingButton.apiCombination + " (" + bettingButton.id + ") [" + amount + "]");

			Bet bet = new Bet();
			bet.pileId = bettingButton.id;
			bet.betType = bettingButton.type;
			bet.amount = amount;
			bet.slot = chipsController.SelectedSlot;
			bet.previousAmount = piles[bettingButton.type][bettingButton.id];
			bet.bettingButton = bettingButton;

			bool totalLimitReached = totalAmount + bet.amount > maxBet;
			bool pileLimitReached = piles[bet.betType][bet.pileId] + bet.amount > typeLimits[bet.betType];

			bool creditsEnough = credits - (totalAmount + bet.amount) >= 0;

			if (maxBet == 0)
			{
				totalLimitReached = false;
			}

			if (typeLimits[bet.betType] == 0)
			{
				pileLimitReached = false;
			}

			if (!creditsEnough || totalLimitReached || pileLimitReached /*|| ChipsLimitReached*/)
			{
				if (!creditsEnough)
				{
					infoMessage.ShowMessage(InfoMessage.MessageTypes.CreditsNotEnough);
				}

				if (totalLimitReached)
				{
					infoMessage.ShowMessage(InfoMessage.MessageTypes.MaxBetReached);
				}

                /*if (ChipsLimitReached)
                {
                    infoMessage.ShowMessage(InfoMessage.MessageTypes.ChipsLimitReached);
                }*/

                if (pileLimitReached)
				{
					infoMessage.ShowMessage(InfoMessage.MessageTypes.MaxBetPosition);
				}

				OnBetChanged.Invoke(0);
			}
			else
			{
				infoMessage.ShowMessage(InfoMessage.MessageTypes.PleaseBet, false);

				MakeBet(bet);

				OnBetChanged.Invoke(totalAmount);

				string tooltip = "Bet: " + GameLoop.ConvertAmountToMoney(piles[bet.betType][bet.pileId]) + " / " + GameLoop.ConvertAmountToMoney(typeLimits[bet.betType]) + "\nPays " + payouts[bettingButton.type];
				tooltips.ShowTooltip2D(tooltip, toolTipPosition);
			}
				
			//tooltips.ShowTooltip(tooltip, bettingButton.chipsPile.transform.position);
		}

		private void MakeBet(Bet bet)
		{
			piles[bet.betType][bet.pileId] += bet.amount;
			totalAmount += bet.amount;

			stack.Push(bet);

			UpdateChipPile(bet);
		}

		private void UpdateChipPile(Bet bet)
		{
			int amount = piles[bet.betType][bet.pileId];

			bet.bettingButton.chipsPile.UpdateChipPile(amount, bet.bettingButton.uid, bet.bettingButton.transform.position, true);
		}
		
		public static List<ChipsPile> chanceBets = new List<ChipsPile>();

		public bool AllSimpleBetsMinReached ()
		{
			for (int i = 0; i < stack.Count; i++)
			{
				if (!chanceBets.Contains(stack.ToArray()[i].bettingButton.chipsPile) && stack.ToArray()[i].bettingButton.chipsPile.previousAmount < apMinSimple)
				{
					return false;
				}
			}

			return true;
		}

        public bool ChipsLimitReached
        {
            get
            {
                return stack.Count >= 133;
            }
        }

        public bool AreThereBetsLeft
		{
			get
			{
				return stack.Count > 0;
			}
		}
		
		public bool FirstBet
		{
			get
			{
				return (previousBetStack == null || previousBetStack.Count == 0);
			}
		}

		public bool PreviousBetPresent
		{
			get
			{
				return (previousBetStack != null && previousBetStack.Count > 0 && previousBetAmount <= credits);
			}
		}

		public int CurrentBetAmount
		{
			get
			{
				return totalAmount;
			}
		}

		public int MinimumTableBetAmount
		{
			get
			{
				return minBet;
			}
		}

		public void CancelLastBet()
		{
			Bet bet = stack.Pop();
			piles[bet.betType][bet.pileId] = bet.previousAmount;

			totalAmount -= bet.amount;

			UpdateChipPile(bet);

			OnBetChanged.Invoke(totalAmount);

			string tooltip = "Bet: " + GameLoop.ConvertAmountToMoney(piles[bet.betType][bet.pileId]) + " / " + GameLoop.ConvertAmountToMoney(typeLimits[bet.betType]) + "\nPays " + payouts[bet.bettingButton.type];
			tooltips.ShowTooltip2D(tooltip, toolTipPosition);
		}

		public void CancelAllBets()
		{
			foreach (BetTypes type in System.Enum.GetValues(typeof(BetTypes)))
			{
				for (int i = 0; i < piles[type].Length; i++)
				{
					piles[type][i] = 0;
				}
			}

			while (stack.Count > 0)
			{
				Bet bet = stack.Pop();

				/*
				GameObject chip;
				int pool;

				bet.bettingButton.RemoveChip(out chip, out pool);
				chipsController.ReturnChip(chip, pool);*/
				UpdateChipPile(bet);
			}

			totalAmount = 0;

			OnBetChanged.Invoke(totalAmount);
		}

		public bool repeat = false;

		public void RepeatBet()
		{
			repeat = true;

			CancelAllBets();

			Bet[] bets = previousBetStack.ToArray();

			for (int i = 0; i < bets.Length; i++)
			{
				MakeBet(bets[i]);
			}

			OnBetChanged.Invoke(totalAmount);

			repeat = false;
		}

		public void SetBetAsMade()
		{
			previousBetStack = new Stack<Bet>(stack);
			previousBetAmount = totalAmount;
		}

		public string GetBets(BetTypes type)
		{
			for (int i = 0; i < usedPiles[type].Length; i++)
			{
				usedPiles[type][i] = false;
			}

			string res = string.Empty;

			Bet[] bets = stack.ToArray();

			for (int i = 0; i < bets.Length; i++)
			{
				if (bets[i].betType == type)
				{
					int id = bets[i].pileId;
					if (usedPiles[type][id])
					{
						continue;
					}

					usedPiles[type][id] = true;
					res += API2ID.ConvertToAPI(id, bets[i].bettingButton.type) + APIStrings.betNextId + piles[type][id] + APIStrings.betDivider;
				}
			}

			if (res.EndsWith(APIStrings.betDivider))
			{
				res = res.Remove(res.Length - APIStrings.betDivider.Length);
			}
			return res;
		}

		public int GetPileBetAmount(BetTypes type, int id)
		{
			return piles[type][id];
		}

		public void BackupCurrentBets()
		{
			backupBetStack = new Stack<Bet>(stack);
			CancelAllBets();
			previousBetsBettingButtons.Clear();
		}

		public void RestoreCurrentBets()
		{
			ClearPreviousBetPiles();

			if (backupBetStack == null)
			{
				return;
			}

			CancelAllBets();

			Bet[] bets = backupBetStack.ToArray();

			for (int i = 0; i < bets.Length; i++)
			{
				MakeBet(bets[i]);
			}

			backupBetStack = null;

			OnBetChanged.Invoke(totalAmount);
		}

		private void ClearPreviousBetPiles()
		{
			foreach (BettingButton button in previousBetsBettingButtons)
			{
				button.chipsPile.UpdateChipPile(0, button.uid, Vector3.zero, true);
				button.chipsPile.previousPlaysAmount = 0;
				button.chipsPile.previousPlaysWonAmount = 0;
				button.chipsPile.previousPlaysLostAmount = 0;
			}
			previousBetsBettingButtons.Clear();
		}

		public int CreateBets(Dictionary<BetTypes, string> bets, Dictionary<BetTypes, string> wonBets, Dictionary<BetTypes, string> lostBets, bool clear, bool sumOnly = false)
		{
			if (clear)
			{
				ClearPreviousBetPiles();
			}

			int totalBet = 0;
			foreach (BetTypes type in System.Enum.GetValues(typeof(BetTypes)))
			{
				string[] betCombinations = bets[type].Split(new string[] { APIStrings.betDivider }, System.StringSplitOptions.RemoveEmptyEntries);

				foreach (string s in betCombinations)
				{
					string[] betParts = s.Split(new string[] { APIStrings.betCreditsDivider }, System.StringSplitOptions.RemoveEmptyEntries);

					int betAmount = int.Parse(betParts[1]);

					if (!sumOnly)
					{
						int id = API2ID.ConvertToId(betParts[0] + APIStrings.betNextId, type);
						BettingButton button = API2ID.GetButton(type, id);
						button.chipsPile.UpdateChipPile(betAmount, button.uid, button.chipsPile.transform.position, true);
						button.chipsPile.previousPlaysAmount = betAmount;
						previousBetsBettingButtons.Add(button);
					}

					totalBet += betAmount;
				}
			}

			foreach (BetTypes type in System.Enum.GetValues(typeof(BetTypes)))
			{
				string[] winningCombinations = wonBets[type].Split(new string[] { APIStrings.betDivider }, System.StringSplitOptions.RemoveEmptyEntries);

				foreach (string s in winningCombinations)
				{
					string[] betParts = s.Split(new string[] { APIStrings.betCreditsDivider }, System.StringSplitOptions.RemoveEmptyEntries);

					int betAmount = int.Parse(betParts[1]);

					int id = API2ID.ConvertToId(betParts[0] + APIStrings.betNextId, type);
					BettingButton button = API2ID.GetButton(type, id);
					button.chipsPile.previousPlaysWonAmount = betAmount;
				}
			}

			foreach (BetTypes type in System.Enum.GetValues(typeof(BetTypes)))
			{
				string[] losingCombinations = lostBets[type].Split(new string[] { APIStrings.betDivider }, System.StringSplitOptions.RemoveEmptyEntries);

				foreach (string s in losingCombinations)
				{
					string[] betParts = s.Split(new string[] { APIStrings.betCreditsDivider }, System.StringSplitOptions.RemoveEmptyEntries);

					int betAmount = int.Parse(betParts[1]);

					int id = API2ID.ConvertToId(betParts[0] + APIStrings.betNextId, type);
					BettingButton button = API2ID.GetButton(type, id);
					button.chipsPile.previousPlaysLostAmount = betAmount;
				}
			}

			return totalBet;
		}

		/*
		public int CreateBets(Dictionary<BetTypes, string> bets, bool clear, bool sumOnly = false)
		{
			if (clear)
			{
				ClearPreviousBetPiles();
			}

			int totalBet = 0;
			foreach (BetTypes type in System.Enum.GetValues(typeof(BetTypes)))
			{
				string[] winningCombinations = bets[type].Split(new string[] { APIStrings.betDivider }, System.StringSplitOptions.RemoveEmptyEntries);

				foreach (string s in winningCombinations)
				{
					string[] betParts = s.Split(new string[] { APIStrings.betCreditsDivider }, System.StringSplitOptions.RemoveEmptyEntries);

					int betAmount = int.Parse(betParts[1]);

					if (!sumOnly)
					{
						int id = API2ID.ConvertToId(betParts[0] + APIStrings.betNextId, type);
						BettingButton button = API2ID.GetButton(type, id);
						button.chipsPile.UpdateChipPile(betAmount, button.uid, button.chipsPile.transform.position, true);
						previousBetsBettingButtons.Add(button);
					}

					totalBet += betAmount;
				}
			}

			return totalBet;
		}
		*/

		public void OnShowTooltipPreviousPlay(BettingButton bettingButton)
		{
			string tooltip = "Bet: " + GameLoop.ConvertAmountToMoney(bettingButton.chipsPile.previousPlaysAmount) + 
				"\nWon:" + GameLoop.ConvertAmountToMoney(bettingButton.chipsPile.previousPlaysWonAmount) +
				"\nLost:" + GameLoop.ConvertAmountToMoney(bettingButton.chipsPile.previousPlaysLostAmount) +
				"\nPays " + payouts[bettingButton.type];
			tooltips.ShowTooltip2D(tooltip, toolTipPositionPreviousPlays);
		}
	}
}