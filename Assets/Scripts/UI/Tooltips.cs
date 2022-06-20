using UnityEngine;
using UnityEngine.UI;
using SteinCo.Utils;

namespace SteinCo.Ivisa.RoulettePremium.UI
{
	public class Tooltips : MonoBehaviour
	{
		public GameObject tooltip;
		public RectTransform rt;
		public Text content;
		public Camera mainCamera;
		public Canvas canvas;
		public Vector3 offset;

		private RectTransform canvasRT;

		private float duration = 5.0f;

		void Start()
		{
			duration = float.Parse(WorkingData.GetData("Tooltip Duration", (5.0f).ToString()));
			canvasRT = canvas.GetComponent<RectTransform>();
			tooltip.SetActive(false);
			content.text = string.Empty;
		}

		public void ShowTooltip2D(string text, Vector2 position)
		{
			rt.anchoredPosition = position;
			content.text = text;
			tooltip.SetActive(true);
			CancelInvoke();
			Invoke("Hide", duration);
		}

		public void ShowTooltip(string text, Vector3 position)
		{
			position = mainCamera.WorldToScreenPoint(position) / canvas.scaleFactor;

			Vector2 pivot = rt.pivot;
			if (position.x < canvasRT.rect.width / 2.0f)
			{
				pivot.x = 0.0f;
			}
			else
			{
				pivot.x = 1.0f;
			}

			rt.pivot = pivot;
			rt.anchoredPosition = position + offset;

			content.text = text;
			tooltip.SetActive(true);
			CancelInvoke();
			Invoke("Hide", duration);
		}

		public void Hide()
		{
			tooltip.SetActive(false);
		}
	}
}