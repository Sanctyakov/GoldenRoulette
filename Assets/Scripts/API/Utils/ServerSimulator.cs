using UnityEngine;
using UnityEngine.UI;
using SteinCo.Utils;
using SteinCo.Ivisa.RoulettePremium.Game;
using SteinCo.Ivisa.RoulettePremium.API.Classes;

namespace SteinCo.Ivisa.RoulettePremium.API.Utils
{
	public class ServerSimulator : MonoBehaviour, IServerLayer
	{
		public float delay = 1.0f;
		public int credits = 100000;
		public int time = 0;
		public bool doubleZero = false;
		public int maxBet = 500000;
		public int maxBetType = 10000;
		public int[] chipValues = new int[] { 10, 50, 100, 500, 1000, 5000 };

		public bool random = false;
		public Slider slider;

		public event Init OnInit;
		public event Credits OnCredits;
		public event Spin OnSpin;
		public event EndSession OnSessionEnd;
		public event CheckPassword OnCheckPassword;
		public event InitWeb OnInitWeb;

		void Start()
		{
			delay = float.Parse(WorkingData.GetData("delay", (0.0f).ToString()));
			credits = int.Parse(WorkingData.GetData("credits", "100000"));
			time = int.Parse(WorkingData.GetData("time", "0"));
			maxBet = int.Parse(WorkingData.GetData("maxBet", "500000"));
			maxBetType = int.Parse(WorkingData.GetData("maxBetType", "10000"));

			doubleZero = bool.Parse(WorkingData.GetData("doubleZero", (false).ToString()));
			random = bool.Parse(WorkingData.GetData("random", (false).ToString()));

			slider.gameObject.SetActive(!random);

			string chipValuesWD = WorkingData.GetData("chipValues", "10,25,50,100,200,500");

			string[] temp = chipValuesWD.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);

			int counter = 0;

			foreach (string s in temp)
			{
				chipValues[counter] = int.Parse(s);
				counter++;
			}
		}

		public void SetUp(string URI, string terminal, string sessionID)
		{

		}

		public void CallInit()
		{
			Invoke("OnCallInit", delay);
		}

		private void OnCallInit()
		{
			InitAnswer initAnswer = new InitAnswer();

			initAnswer.Credito = credits;
			initAnswer.Fichas = chipValues;
			initAnswer.cero = doubleZero ? "00" : "0";

			initAnswer.apMaximaChance = maxBetType;
			initAnswer.apMaximaSimple = maxBetType;
			initAnswer.apMinimaChance = 0;
			initAnswer.apMinimaSimple = 0;
			initAnswer.apuestaMaxima = maxBet;
			initAnswer.apuestaMinima = 0;

			initAnswer.idsession = "A";
			initAnswer.keyOut = "B";
			initAnswer.mensajeError = string.Empty;
			initAnswer.timeout = time;

			OnInit.Invoke(initAnswer);
		}

		public void CallCredits()
		{
			Invoke("OnCallCredits", delay);
		}

		public void CallEndSession()
		{
			Invoke("OnCallEndSession", delay);
		}

		private string password;
        
		public void CallCheckPassword(string password)
		{
			this.password = password;
			Invoke("OnCallCheckPassword", delay);
		}

		public void OnCallCheckPassword()
		{
			CheckPasswordAnswer checkPasswordAnswer = new CheckPasswordAnswer();

			if (password.Equals("0"))
			{
				checkPasswordAnswer.mensajeError = string.Empty;
			}
			else
			{
				checkPasswordAnswer.mensajeError = "NO";
			}

			OnCheckPassword.Invoke(checkPasswordAnswer);
		}

		public void CallPreviousPlay(int IDApuestaData, int offset)
		{
			Invoke("OnCallPreviousPlay", delay);
		}

		private int playCounter = 0;

		public void OnCallPreviousPlay()
		{
			string[] data = new string[] { "{\"PerdCallearray\":\"\",\"PerdColor\":\"02__10\",\"PerdColumnaarray\":\"\",\"PerdCuadroarray\":\"\",\"PerdDocenaarray\":\"\",\"PerdLineaarray\":\"\",\"PerdParimpararray\":\"\",\"PerdPlenoarray\":\"\",\"PerdSemiplenoarray\":\"\",\"PerdTransversalCero\":\"\",\"Perdmayormenorarray\":\"\",\"PremColor\":\"01__20\",\"Premcallearray\":\"\",\"Premcolumnaarray\":\"\",\"Premcuadroarray\":\"\",\"Premdocenaarray\":\"\",\"Premlineaarray\":\"\",\"Premmayormenorarray\":\"\",\"Premparimpararray\":\"\",\"Premplenoarray\":\"\",\"Premsemiplenoarray\":\"\",\"PremtransversalCero\":\"\",\"mensajeError\":\"\",\"msgPremioMayor\":\"\",\"numero\":1,\"premiosTotalInt\":111,\"proxKeyOut\":\"1425BD0E-D017-40DF-8C42-F37F86754FCF\",\"saldoInt\":111111}",
				"{\"PerdCallearray\":\"\",\"PerdColor\":\"\",\"PerdColumnaarray\":\"\",\"PerdCuadroarray\":\"\",\"PerdDocenaarray\":\"03__100-01__100\",\"PerdLineaarray\":\"\",\"PerdParimpararray\":\"\",\"PerdPlenoarray\":\"\",\"PerdSemiplenoarray\":\"\",\"PerdTransversalCero\":\"\",\"Perdmayormenorarray\":\"\",\"PremColor\":\"\",\"Premcallearray\":\"\",\"Premcolumnaarray\":\"\",\"Premcuadroarray\":\"\",\"Premdocenaarray\":\"02__300\",\"Premlineaarray\":\"\",\"Premmayormenorarray\":\"\",\"Premparimpararray\":\"\",\"Premplenoarray\":\"\",\"Premsemiplenoarray\":\"\",\"PremtransversalCero\":\"\",\"mensajeError\":\"\",\"msgPremioMayor\":\"\",\"numero\":2,\"premiosTotalInt\":222,\"proxKeyOut\":\"6AFD9E20-747B-47BE-A24C-6EE03B85B550\",\"saldoInt\":222222}",
				"{\"PerdCallearray\":\"\",\"PerdColor\":\"\",\"PerdColumnaarray\":\"03__10-02__10\",\"PerdCuadroarray\":\"\",\"PerdDocenaarray\":\"\",\"PerdLineaarray\":\"\",\"PerdParimpararray\":\"\",\"PerdPlenoarray\":\"\",\"PerdSemiplenoarray\":\"\",\"PerdTransversalCero\":\"\",\"Perdmayormenorarray\":\"\",\"PremColor\":\"\",\"Premcallearray\":\"\",\"Premcolumnaarray\":\"01__30\",\"Premcuadroarray\":\"\",\"Premdocenaarray\":\"\",\"Premlineaarray\":\"\",\"Premmayormenorarray\":\"\",\"Premparimpararray\":\"\",\"Premplenoarray\":\"\",\"Premsemiplenoarray\":\"\",\"PremtransversalCero\":\"\",\"mensajeError\":\"\",\"msgPremioMayor\":\"\",\"numero\":3,\"premiosTotalInt\":333,\"proxKeyOut\":\"6977D780-7B44-4948-A194-99810E6712DD\",\"saldoInt\":333333}" };

			playCounter++;

			if (playCounter >= data.Length)
			{
				playCounter = 0;
			}
		}

		private void OnCallCredits()
		{
			CreditAnswer creditAnswer = new CreditAnswer();

			creditAnswer.Credito = credits;
			creditAnswer.mensajeError = string.Empty;

			OnCredits.Invoke(creditAnswer);
		}

		private BetController betController;

		public void CallSpin(BetController betController)
		{
			this.betController = betController;
			Invoke("OnCallSpin", delay);
		}

		private void OnCallSpin()
		{
			// Simulate spin
			int maxNumber = 37;

			if (RouletteType.Type == RouletteType.Types.DoubleZero)
			{
				maxNumber = 38;
			}

			int number = Random.Range(0, maxNumber);

			if (!random)
			{
				number = Mathf.FloorToInt(slider.value);
			}

			string numberString = number.ToString("00");

			// Test for prizes
			int prizeMoney = 0;
			int currentPrize = 0;

			SpinAnswer spinAnswer = new SpinAnswer();

			spinAnswer.proxKeyOut = string.Empty;
			spinAnswer.numero = number;

			string prizeString = string.Empty;
			string loserString = string.Empty;

			EvaluatePrize(betController.GetBets(BetController.BetTypes.Pleno), 35, numberString, out currentPrize, out prizeString, out loserString);
			prizeMoney += currentPrize;
			spinAnswer.Premplenoarray = prizeString;
			spinAnswer.PerdPlenoarray = loserString;

			EvaluatePrize(betController.GetBets(BetController.BetTypes.Semipleno), 17, numberString, out currentPrize, out prizeString, out loserString);
			prizeMoney += currentPrize;
			spinAnswer.Premsemiplenoarray = prizeString;
			spinAnswer.PerdSemiplenoarray = loserString;

			EvaluatePrize(betController.GetBets(BetController.BetTypes.Calle), 11, numberString, out currentPrize, out prizeString, out loserString);
			prizeMoney += currentPrize;
			spinAnswer.Premcallearray = prizeString;
			spinAnswer.PerdCallearray = loserString;

			EvaluatePrize(betController.GetBets(BetController.BetTypes.Cuadrado), 8, numberString, out currentPrize, out prizeString, out loserString);
			prizeMoney += currentPrize;
			spinAnswer.Premcuadroarray = prizeString;
			spinAnswer.PerdCuadroarray = loserString;

			EvaluatePrize(betController.GetBets(BetController.BetTypes.Linea), 5, numberString, out currentPrize, out prizeString, out loserString);
			prizeMoney += currentPrize;
			spinAnswer.Premlineaarray = prizeString;
			spinAnswer.PerdLineaarray = loserString;

			EvaluatePrize(betController.GetBets(BetController.BetTypes.TransversalConCero), 8, numberString, out currentPrize, out prizeString, out loserString);
			prizeMoney += currentPrize;
			spinAnswer.PremtransversalCero = prizeString;
			spinAnswer.PerdTransversalCero = loserString;

			if (number % 2 == 0)
			{
				numberString = "01";
			}
			else
			{
				numberString = "02";
			}

			if (number == 0 || number == 37)
			{
				numberString = "99";
			}

			EvaluatePrize(betController.GetBets(BetController.BetTypes.ParImpar), 1, numberString, out currentPrize, out prizeString, out loserString);
			prizeMoney += currentPrize;
			spinAnswer.Premparimpararray = prizeString;
			spinAnswer.PerdParimpararray = loserString;

			if (number < 19)
			{
				numberString = "01";
			}
			else
			{
				numberString = "02";
			}

			if (number == 0 || number == 37)
			{
				numberString = "99";
			}

			EvaluatePrize(betController.GetBets(BetController.BetTypes.MenorMayor), 1, numberString, out currentPrize, out prizeString, out loserString);
			prizeMoney += currentPrize;
			spinAnswer.Premmayormenorarray = prizeString;
			spinAnswer.Perdmayormenorarray = loserString;

			if (number < 13)
			{
				numberString = "01";
			}
			else if (number < 25)
			{
				numberString = "02";
			}
			else
			{
				numberString = "03";
			}
			if (number == 0 || number == 37)
			{
				numberString = "99";
			}
			EvaluatePrize(betController.GetBets(BetController.BetTypes.Docena), 2, numberString, out currentPrize, out prizeString, out loserString);
			prizeMoney += currentPrize;
			spinAnswer.Premdocenaarray = prizeString;
			spinAnswer.PerdDocenaarray = loserString;

			numberString = (number % 3) == 0 ? "03" : "0" + (number % 3);
			if (number == 0 || number == 37)
			{
				numberString = "99";
			}
			EvaluatePrize(betController.GetBets(BetController.BetTypes.Columna), 2, numberString, out currentPrize, out prizeString, out loserString);
			prizeMoney += currentPrize;
			spinAnswer.Premcolumnaarray = prizeString;
			spinAnswer.PerdColumnaarray = loserString;

			if (!Highlighter.redNumbers.Contains(number))
			{
				numberString = "02";
			}
			else
			{
				numberString = "01";
			}
			if (number == 0 || number == 37)
			{
				numberString = "99";
			}
			EvaluatePrize(betController.GetBets(BetController.BetTypes.Color), 1, numberString, out currentPrize, out prizeString, out loserString);
			prizeMoney += currentPrize;
			spinAnswer.PremColor = prizeString;
			spinAnswer.PerdColor = loserString;

			spinAnswer.mensajeError = string.Empty;

			credits += prizeMoney;

			spinAnswer.saldoInt = credits;
			spinAnswer.premiosTotalInt = prizeMoney;

			OnSpin.Invoke(spinAnswer);
		}

		private void OnCallEndSession()
		{
			EndSessionAnswer endSessionAnswer = new EndSessionAnswer();
			endSessionAnswer.mensajeError = string.Empty;
			OnSessionEnd.Invoke(endSessionAnswer);
		}

		private void EvaluatePrize(string bet, float prizePayout, string numberString, out int totalPrize, out string prizeString, out string loserString)
		{
			prizeString = string.Empty;
			loserString = string.Empty;

			string[] allBets = bet.Split(new string[] { APIStrings.betDivider }, System.StringSplitOptions.RemoveEmptyEntries);
			int prize = 0;
			totalPrize = 0;

			foreach (string individualBets in allBets)
			{
				string[] betParts = individualBets.Split(new string[] { APIStrings.betCreditsDivider }, System.StringSplitOptions.RemoveEmptyEntries);

				if (betParts.Length > 0)
				{
					int betAmount = int.Parse(betParts[1]);
					credits -= betAmount;
					if (betParts[0].Contains(numberString))
					{
						// Winner!
						prize = Mathf.RoundToInt(betAmount + betAmount * prizePayout);
						prizeString += betParts[0] + APIStrings.betCreditsDivider + prize + APIStrings.betDivider;
						totalPrize += prize;

						////DebugWithDate.Log("WINNING BET: " + betAmount + " " + prize);
					}
					else
					{
						loserString += betParts[0] + APIStrings.betCreditsDivider + betAmount + APIStrings.betDivider;
					}
				}
			}

			if (prizeString.Length > 0)
			{
				prizeString = prizeString.Substring(0, prizeString.Length - APIStrings.betDivider.Length);
			}

			if (loserString.Length > 0)
			{
				loserString = loserString.Substring(0, loserString.Length - APIStrings.betDivider.Length);
			}
		}

		public void CallInitWeb()
		{
			
		}
	}
}