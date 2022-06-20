using UnityEngine;
using UnityEngine.UI;

namespace SteinCo.Ivisa.RoulettePremium.Game
{
	public class RouletteType : MonoBehaviour
	{
		public MeshRenderer[] meshRenderers;

		public Material singleZeroMaterial;
		public Material doubleZeroMaterial;

		public GameObject[] singleZeroGOs;
		public GameObject[] doubleZeroGOs;

		public RectTransform zeroButtonForSingleZeroReference;
		public RectTransform zeroButtonForDoubleZeroReference;
		public RectTransform zeroButton;

		public enum Types
		{
			SingleZero,
			DoubleZero,
		}

		private static Types type = Types.SingleZero;
		public static Types Type
		{
			get
			{
				return type;
			}
		}

		public delegate void RouletteTypeChanged(Types type);

		public static event RouletteTypeChanged OnRouletteTypeChanged;

		public void ChangeToType(string doubleZero)
		{

			if (doubleZero.Equals("00"))
			{
				type = Types.DoubleZero;

				zeroButton.SetParent(zeroButtonForDoubleZeroReference, false);

				/*foreach (MeshRenderer mr in meshRenderers)
				{
					mr.sharedMaterial = doubleZeroMaterial;
				}

				foreach (GameObject go in singleZeroGOs)
				{
					go.SetActive(false);
				}

				foreach (GameObject go in doubleZeroGOs)
				{
					go.SetActive(true);
				}*/
			}
			else
			{
				type = Types.SingleZero;

				zeroButton.SetParent(zeroButtonForSingleZeroReference, false);

				/*foreach (MeshRenderer mr in meshRenderers)
				{
					mr.sharedMaterial = singleZeroMaterial;
				}

				foreach (GameObject go in singleZeroGOs)
				{
					go.SetActive(true);
				}

				foreach (GameObject go in doubleZeroGOs)
				{
					go.SetActive(false);
				}*/
			}

			OnRouletteTypeChanged.Invoke(type);
		}

		public void ChangeType()
		{
			switch (type)
			{
				case Types.SingleZero:
					ChangeToType("00");
					break;
				case Types.DoubleZero:
					ChangeToType("0");
					break;
			}
		}
	}
}