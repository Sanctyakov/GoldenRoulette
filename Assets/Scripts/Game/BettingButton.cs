using UnityEngine;
using UnityEngine.UI;
using SteinCo.Ivisa.RoulettePremium.Chips;
using SteinCo.Ivisa.RoulettePremium.API.Utils;
using SteinCo.Ivisa.RoulettePremium.UI;
using SteinCo.Ivisa.RoulettePremium.API;
using UnityEngine.EventSystems;

namespace SteinCo.Ivisa.RoulettePremium.Game
{
	public class BettingButton : MonoBehaviour//, IPointerEnterHandler
	{
		public bool customCombination = false;
		public string apiCombination = string.Empty;
		public int id; // For the API, can be repeated, distinguised by BetTypes type
		public int uid; // For the GUI texts

		public BetController.BetTypes type = BetController.BetTypes.Pleno;

		public ChipsPile chipsPile;

		public delegate void BetMade(BettingButton bettingButton);
		public static event BetMade OnBetMade;

		void Awake()
		{
			uid = GetInstanceID();
			chipsPile = GetComponent<ChipsPile>();
			GetComponent<Button>().onClick.AddListener(OnBet);

			if (!customCombination)
			{
				string[] parts = name.Split(' ');

				for (int i = 0; i < parts.Length; i++)
				{
					if (parts[i].Equals("00"))
					{
						parts[i] = "37";
					}
					if (parts[i].Length < 2)
					{
						parts[i] = "0" + parts[i];
					}
				}

				apiCombination = string.Join(APIStrings.betNextId, parts) + APIStrings.betNextId;
			}
			else
			{
				apiCombination = apiCombination.Replace("#", APIStrings.betNextId);
			}

			id = API2ID.AddBetButton(this);
		}

		public void OnBet()
		{
			OnBetMade.Invoke(this);
		}

		/*public void OnPointerEnter(PointerEventData eventData)
		{
			OnBetMade.Invoke(this);
		}*/
	}
}