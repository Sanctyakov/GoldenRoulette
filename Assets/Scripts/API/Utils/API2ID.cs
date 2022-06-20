using UnityEngine;
using System.Collections.Generic;
using SteinCo.Ivisa.RoulettePremium.Game;

namespace SteinCo.Ivisa.RoulettePremium.API.Utils
{
	public static class API2ID
	{
		private static Dictionary<BetController.BetTypes, List<string>> betButtonApiCombinations = new Dictionary<BetController.BetTypes, List<string>>();
		private static Dictionary<BetController.BetTypes, Dictionary<int, BettingButton>> betButtons = new Dictionary<BetController.BetTypes, Dictionary<int, BettingButton>>();

		private static Dictionary<int, BetController.BetTypes> betTypeByUID = new Dictionary<int, BetController.BetTypes>();

		public static int RegisteredPiles(BetController.BetTypes type)
		{
			return betButtonApiCombinations[type].Count;
		}

		public static int AddBetButton(BettingButton button)
		{
			if (!betButtonApiCombinations.ContainsKey(button.type))
			{
				betButtonApiCombinations.Add(button.type, new List<string>());
				betButtons.Add(button.type, new Dictionary<int, BettingButton>());
			}

			betButtonApiCombinations[button.type].Add(button.apiCombination);

			int id = ConvertToId(button.apiCombination, button.type);
			betButtons[button.type].Add(id, button);

			betTypeByUID.Add(button.uid, button.type);

			return id;
		}

		public static BetController.BetTypes GetButtonBetTypeByUID(int uid)
		{
			return betTypeByUID[uid];
		}

		public static BettingButton GetButton(BetController.BetTypes type, int id)
		{
			return betButtons[type][id];
		}

		public static int ConvertToId(string API, BetController.BetTypes type)
		{
			return betButtonApiCombinations[type].FindIndex(a => a.Equals(API));
		}

		public static string ConvertToAPI(int id, BetController.BetTypes type)
		{
			return betButtonApiCombinations[type][id];
		}

		public static void DebugMe()
		{
			foreach (KeyValuePair<BetController.BetTypes, List<string>> kvp in betButtonApiCombinations)
			{
				//kvp.Value.Sort();
				////DebugWithDate.Log("<color=red>" + kvp.Key + "</color>");
				foreach (string s in kvp.Value)
				{
					////DebugWithDate.Log(s);
				}
			}

			foreach (KeyValuePair<BetController.BetTypes, Dictionary<int, BettingButton>> kvp in betButtons)
			{
				////DebugWithDate.Log("<color=green>" + kvp.Key + "</color>");
				foreach (KeyValuePair<int, BettingButton> kvp2 in kvp.Value)
				{
					////DebugWithDate.Log(kvp2.Key + " -> " + kvp2.Value);
				}
			}
		}

		public static void Reset()
		{
			betButtonApiCombinations.Clear();
			betButtons.Clear();
		}
	}
}