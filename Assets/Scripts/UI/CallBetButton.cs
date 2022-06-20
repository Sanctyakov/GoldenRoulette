using UnityEngine;
using UnityEngine.UI;

namespace SteinCo.Ivisa.RoulettePremium.Game
{
	public class CallBetButton : MonoBehaviour
	{
		public Button[] botones;

		void Awake()
		{
			GetComponent<Button>().onClick.AddListener(OnClick);
		}

		public void OnClick()
		{
			for (int i = 0; i < botones.Length; i++)
			{
				botones[i].onClick.Invoke();
			}
		}
	}
}