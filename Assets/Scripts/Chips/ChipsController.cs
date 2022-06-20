using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SteinCo.Utils.Pooling;
using SteinCo.Ivisa.RoulettePremium.Sound;
using SteinCo.Ivisa.RoulettePremium.Game;

namespace SteinCo.Ivisa.RoulettePremium.Chips
{
	public class ChipsController : MonoBehaviour
	{
		public BetController betController;
		public GameObject[] chips;
		public Pool[] pools;
		public ChipsSoundFX chipsSoundFX;
		public Transform chipsRoot;

		public GameObject[] buttonsGUI;
		public Camera mainCamera;
		public Canvas canvas;

		public float distance = 0.25f;
		public float scale = 3.0f;

		private List<Transform> chipTransforms = new List<Transform>();
		private List<int> values = new List<int>();
		private Vector3[] positions;
		private List<string> stringValues = new List<string>();

		private List<int> chipValues = new List<int>();


		private int selectedSlot = 0;

		public int SelectedValue
		{
			get
			{
				return values[selectedSlot];
			}
		}

		public int HigherChip
		{
			get
			{
				return values[values.Count - 1];
			}
		}

		public int SelectedSlot
		{
			get
			{
				return selectedSlot;
			}
		}

		public int SlotAmount
		{
			get
			{
				return values.Count;
			}
		}

		public int GetValue(int slot)
		{
			return values[slot];
		}

		public string GetStringValue(int slot)
		{
			return stringValues[slot];
		}

		void Start()
		{
			///
		}

		public void SetUp(int[] values)
		{
			for (int i = 0; i < buttonsGUI.Length; i++)
			{
				if (i < values.Length)
				{
					buttonsGUI[i].SetActive(true);
				}
				else
				{
					buttonsGUI[i].SetActive(false);
				}
			}

			positions = new Vector3[values.Length];

			int counter = 0;

			foreach (int value in values)
			{
				string currentInt = string.Empty;
				string currentCents = string.Empty;
				string currentValue = string.Empty;

				string valueString = value.ToString("00");

				if (valueString.Length > 2)
				{
					currentInt = valueString.Substring(0, valueString.Length - 2);
					currentCents = valueString.Substring(valueString.Length - 2, 2);

					if (!currentCents.Equals("00"))
					{
						currentValue = currentInt + "," + currentCents;
					}
					else
					{
						currentValue = currentInt;
					}
				}
				else
				{
					currentValue = value + "¢";
				}

				GameObject go = Instantiate(chips[counter]);

				go.GetComponentInChildren<Text>().text = currentValue;

				go.transform.localPosition = Vector3.up * -10.0f;
				go.transform.localScale = Vector3.one * scale;
				go.transform.SetParent(chipsRoot);
				chipTransforms.Add(go.transform);

				this.values.Add(int.Parse(valueString));
				stringValues.Add(currentValue);

				counter++;

				if (counter >= 6)
				{
					break;
				}
			}

			for (int i = 0; i < this.values.Count; i++)
			{
				chipValues.Add(0);
			}

			// We need to wait at least one frame after having the GUI buttons relocated
			Invoke("SetChipPositions", 0.1f);
		}

		private void SetChipPositions()
		{
			int counter = 0;

			foreach (Transform t in chipTransforms)
			{
				// Position chips on table according to the buttons position (which have distribution and centering done on GUI)
				Vector3 positionUI = buttonsGUI[counter].GetComponent<RectTransform>().anchoredPosition;

				// Adjust by the canvas scale
				positionUI *= canvas.scaleFactor;

				// Add distance to the camera, otherwise it returns an unusable position when transformed to world
				positionUI.z = mainCamera.transform.position.y;

				int reverseCounter = values.Count - 1 - counter;
				positions[reverseCounter] = mainCamera.ScreenToWorldPoint(positionUI);

				positions[reverseCounter].x *= -1.0f;
				t.position = positions[reverseCounter];

				counter++;
			}

			setup = true;

			SelectValue(0);
		}

		public void SelectValue(int place)
		{
			if (place >= values.Count)
			{
				return;
			}
			if (place != selectedSlot)
			{
				chipsSoundFX.OnBet();
			}

			selectedSlot = place;

			int counter = 0;

			foreach (Transform t in chipTransforms)
			{
				t.position = positions[counter++];
			}

			chipTransforms[place].position = positions[place] + Vector3.forward * distance + Vector3.up * distance * 1.5f;
		}
			

		//private int chipLimit = 10;

		public List<int> GetChipAmountsPerSlot(int amount)
		{
			/*POSIBLE FIX:

			if (!betController.repeat)
			{
				for (int i = 0; i < chipValues.Count; i++)
				{
					chipValues [i] = 0;
				}

				int slot = 5 - selectedSlot;

				while (amount > SelectedValue * chipValues [slot] && chipValues[slot] < chipLimit)
				{
					chipValues [slot]++;
				}
			}*/

			int currentValue;
			bool finished = false;
			int totalAmount = amount;
			int limit = values.Count;
			int limitMinusOne = limit - 1;
			int firstTime = 1;

			for (int i = 0; i < chipValues.Count; i++)
			{
				chipValues[i] = 0;
			}

			while (!finished)
			{
				for (int i = 0; i < limitMinusOne; i++)
				{
					chipValues[i] = 0;
				}

				for (int i = 0; i < limit; i++)
				{
					currentValue = values[limitMinusOne - i];

					if (amount >= currentValue)
					{
						int modulo = amount % currentValue;
						int howMany = amount / currentValue;
							
						amount = modulo;
						chipValues[i] += howMany;
					}
				}

				if (amount != 0)
				{
                    if (amount < values[0])
                    {
                        chipValues[limitMinusOne]++;
                        finished = true;
                    }
                    else
                    {
                        amount = totalAmount - values[0] - (firstTime * values[0] * chipValues[limitMinusOne]);
                        firstTime = 0;
                        totalAmount = amount;
                        chipValues[limitMinusOne]++;
                    }
				}
				else
				{
					finished = true;
				}
			}

			return chipValues;
		}

		public GameObject GetChip()
		{
			GameObject go = pools[selectedSlot].GetNewObject();
			go.GetComponentInChildren<Text>().text = stringValues[selectedSlot];
			return go;
		}

		public GameObject GetChip(int slot)
		{
			GameObject go = pools[slot].GetNewObject();
			//Text t = go.GetComponentInChildren<Text>();

			if (slot < SlotAmount)
			{
				go.GetComponentInChildren<Text>().text = stringValues[slot];
			}
			return go;
		}

		public void ReturnChip(GameObject chip, int pool)
		{
			pools[pool].ReturnUsedObject(chip);
		}

		private bool setup = false;
		public void Update()
		{
			if (!setup)
			{
				return;
			}

			float v = Mathf.SmoothStep(0.0f, 0.5f, Mathf.PingPong(Time.time, 0.5f));
			chipTransforms[selectedSlot].position = positions[selectedSlot] + Vector3.forward * distance + Vector3.forward * distance * v + Vector3.up * distance * 1.5f;
		}
	}
}