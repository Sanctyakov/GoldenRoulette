using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SteinCo.Ivisa.RoulettePremium.Game
{
	public class NumberHistory : MonoBehaviour
	{
		public int maxAmount = 20;
		public Text[] numberTexts;
		public Image[] numberBackgrounds;

		public Color green = Color.green;
		public Color red = Color.red;
		public Color black = Color.black;
		public Color invisible = new Color(1.0f, 1.0f, 1.0f, 0.0f);

		private List<int> numberHistory = new List<int>();

		void Start()
		{
			for (int i = 0; i < maxAmount; i++)
			{
				numberBackgrounds[i].color = invisible;
				numberTexts[i].text = string.Empty;
			}
		}

		public void AddNumber(int number, bool redraw = true)
		{
            //DebugWithDate.Log("Añadiendo el número " + number + " a la tabla de últimos resultados.");

            numberHistory.Insert(0, number);

			if (numberHistory.Count > maxAmount)
			{
				numberHistory.RemoveAt(numberHistory.Count - 1);
			}

			if (redraw)
			{
				for (int i = 0; i < maxAmount; i++)
				{
					int currentNumber = 0;
					string currentString = string.Empty;
					Color currentColor = invisible;

					if (i < numberHistory.Count)
					{
						currentNumber = numberHistory[i];

						currentString = currentNumber.ToString();

						if (currentNumber == 0 || currentNumber == 37)
						{
							currentColor = green;

							if (currentNumber == 37)
							{
								currentString = "00";
							}
						}
						else if (Highlighter.redNumbers.Contains(currentNumber))
						{
							currentColor = red;
						}
						else
						{
							currentColor = black;
						}
					}

					numberBackgrounds[i].color = currentColor;
					numberTexts[i].text = currentString;
				}
			}
		}
	}
}