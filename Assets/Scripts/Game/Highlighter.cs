using UnityEngine;
using System.Collections.Generic;

namespace SteinCo.Ivisa.RoulettePremium.Game
{
	public class Highlighter : MonoBehaviour
	{
		public MeshRenderer[] numbers;
		public MeshRenderer[] columns;
		public MeshRenderer[] dozens;
		public MeshRenderer[] color;
		public MeshRenderer[] odd;
		public MeshRenderer[] major;

		public MeshRenderer zeroSingle;
		public MeshRenderer zeroDouble;
		public MeshRenderer doubleZeroDouble;

		public MeshRenderer zeroSingleBG;
		public MeshRenderer zeroDoubleBG;
		public MeshRenderer doubleZeroDoubleBG;

		private bool started = false;
		private List<MeshRenderer> images = new List<MeshRenderer>();

		public static List<int> redNumbers = new List<int> { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };

		void Start()
		{
			ClearHighlights();
		}

		public void ClearHighlights()
		{
			started = false;

			switch (RouletteType.Type)
			{
				case RouletteType.Types.SingleZero:
					zeroSingleBG.enabled = true;
					break;
				case RouletteType.Types.DoubleZero:
					zeroDoubleBG.enabled = true;
					doubleZeroDoubleBG.enabled = true;
					break;
			}

			zeroSingle.enabled = false;
			zeroDouble.enabled = false;
			doubleZeroDouble.enabled = false;

			foreach (MeshRenderer mr in numbers)
			{
				mr.enabled = false;
			}
			foreach (MeshRenderer mr in columns)
			{
				mr.enabled = false;
			}
			foreach (MeshRenderer mr in dozens)
			{
				mr.enabled = false;
			}
			foreach (MeshRenderer mr in color)
			{
				mr.enabled = false;
			}
			foreach (MeshRenderer mr in odd)
			{
				mr.enabled = false;
			}
			foreach (MeshRenderer mr in major)
			{
				mr.enabled = false;
			}
		}

		public void Hightlight(int number)
		{
			ClearHighlights();

			started = true;

			images.Clear();



			if (number == 0)
			{
				switch (RouletteType.Type)
				{
					case RouletteType.Types.SingleZero:
						zeroSingleBG.enabled = false;
						images.Add(zeroSingle);
						break;
					case RouletteType.Types.DoubleZero:
						zeroDoubleBG.enabled = false;
						images.Add(zeroDouble);
						break;
				}


			}
			else if (number == 37)
			{
				doubleZeroDoubleBG.enabled = false;

				images.Add(doubleZeroDouble);
			}
			else
			{
				images.Add(numbers[number]);

				// Even or odd
				if ((number % 2) == 0)
				{
					images.Add(odd[0]);
				}
				else
				{
					images.Add(odd[1]);
				}

				// Columns
				images.Add(columns[(number - 1) % 3]);

				// Color
				if (redNumbers.Contains(number))
				{
					images.Add(color[0]);
				}
				else
				{
					images.Add(color[1]);
				}

				// Dozens
				if (number < 13)
				{
					images.Add(dozens[0]);
				}
				else if (number < 25)
				{
					images.Add(dozens[1]);
				}
				else
				{
					images.Add(dozens[2]);
				}

				// Major
				if (number < 19)
				{
					images.Add(major[0]);
				}
				else
				{
					images.Add(major[1]);
				}
			}

			foreach (MeshRenderer mr in images)
			{
				mr.enabled = true;
			}
		}

		void Update()
		{
			if (!started)
			{
				return;
			}

			// TODO: FX de algun tipo
		}
	}
}