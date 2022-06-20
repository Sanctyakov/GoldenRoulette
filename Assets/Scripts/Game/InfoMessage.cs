using UnityEngine;
using UnityEngine.UI;
using SteinCo.Utils;

namespace SteinCo.Ivisa.RoulettePremium.Game
{
	public class InfoMessage : MonoBehaviour
	{
		public Text messageText;
		public LoopColors loopColors;
		public Outline outline;
		public BetController betController;
		public FadeImage messageFrame;

		public enum MessageTypes
		{
			PleaseBet,
			OutOfCredits,
			CreditsNotEnough,
			MinBetNotReached,
			MaxBetReached,
			MaxBetPosition,
			NoBets,
			NoBetsToClear,
			NothingToRepeat,
			CantRepeat,
			Spinning,
			AllSimpleBetsMinNotReached,
			AllChanceBetsMinNotReached,
            ChipsLimitReached,
		}

		void Start()
		{
			messageFrame.HideNow();
		}

		void Update()
		{
			outline.effectColor = loopColors.CurrentColor;
		}

		public void ShowMessage(MessageTypes type, bool glow = true, bool reset = true)
		{
			switch (type)
			{
			case MessageTypes.PleaseBet:
				messageFrame.Hide ();
				//messageText.text = "Seleccione su apuesta sobre el paño y presione el botón Girar";
				break;
			case MessageTypes.OutOfCredits:
				messageFrame.Show ();
				messageText.text = "No credits left";
				break;
			case MessageTypes.CreditsNotEnough:
				messageFrame.Show ();
				messageText.text = "Not enough credit";
				break;
			case MessageTypes.MinBetNotReached:
				messageFrame.Show ();
				messageText.text = "Table minimum bet is " + GameLoop.ConvertAmountToMoney(betController.MinimumTableBetAmount);
				break;
			case MessageTypes.MaxBetReached:
				messageFrame.Show ();
				messageText.text = "Table maximum reached";
				break;
			case MessageTypes.MaxBetPosition:
				messageFrame.Show ();
				messageText.text = "Bet maximum reached";
				break;
			case MessageTypes.NoBets:
				messageFrame.Show ();
				messageText.text = "No bets";
				break;
			case MessageTypes.NoBetsToClear:
				messageFrame.Show ();
				messageText.text = "No bets to delete";
				break;
			case MessageTypes.NothingToRepeat:
				messageFrame.Show ();
				messageText.text = "No bets to repeat";
			break;
			case MessageTypes.CantRepeat:
				messageFrame.Show ();
				messageText.text = "Not enough credits to repeat previous bet";
				break;
			case MessageTypes.Spinning:
				messageFrame.Hide ();
				//messageText.text = "¡Girando!";
				break;
			case MessageTypes.AllSimpleBetsMinNotReached:
					messageFrame.Show ();
				messageText.text = "Minimum straight bet is " + GameLoop.ConvertAmountToMoney(betController.apMinSimple);
				break;
			case MessageTypes.AllChanceBetsMinNotReached:
				messageFrame.Show ();
				messageText.text = "Minimum chance bet is " + GameLoop.ConvertAmountToMoney(betController.apMinChance);
				break;
            case MessageTypes.ChipsLimitReached:
                messageFrame.Show();
                messageText.text = "Chips limit reached";
                break;
            }

			CancelInvoke();

			if (reset)
			{
				Invoke("Reset", 5.0f);
			}

			if (glow)
			{
				loopColors.Begin();
				Invoke("StopColors", 3.0f);
			}
			else
			{
				loopColors.Stop();
			}
		}

		private void Reset()
		{
			ShowMessage(MessageTypes.PleaseBet, true, false);
		}

		private void StopColors()
		{
			loopColors.Stop();
		}
	}
}