using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SteinCo.Utils;

namespace SteinCo.Ivisa.RoulettePremium.UI
{
	public class SpinButtonGlow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
	{
		public LoopColors loopColorsSpinImage;
		public Image spinImage;
		public Button spinButton;
		public Sprite[] sprites;

		void Update()
		{
			if (!spinButton.interactable)
			{
				loopColorsSpinImage.Stop();
				spinImage.color = loopColorsSpinImage.colorA;
				return;
			}

			spinImage.color = loopColorsSpinImage.CurrentColor;

			loopColorsSpinImage.Begin(false);
		}

		private bool pressed = false;
		public void OnPointerDown(PointerEventData eventData)
		{
			pressed = true;
			spinImage.sprite = sprites[1];
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			pressed = false;
			spinImage.sprite = sprites[0];
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (pressed)
			{
				spinImage.sprite = sprites[1];
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (pressed)
			{
				spinImage.sprite = sprites[0];
			}
		}
	}
}