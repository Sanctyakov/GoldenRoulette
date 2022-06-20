using UnityEngine;

namespace SteinCo.Utils
{
	public class FadeImage : MonoBehaviour
	{
		public float duration = 1.0f;

		private bool started = false;
		private bool show = false;
		private CanvasGroup cg;
		private float timeCounter = 0.0f;

		// Use this for initialization
		void Start()
		{
			cg = GetComponent<CanvasGroup>();
		}

		// Update is called once per frame
		void Update()
		{
			if (!started)
			{
				return;
			}

			timeCounter += Time.deltaTime;

			if (timeCounter > duration)
			{
				started = false;
			}

			if (show)
			{
				cg.alpha += Time.deltaTime / duration;
			}
			else
			{
				cg.alpha -= Time.deltaTime / duration;
			}
		}

		public void Show()
		{
			started = true;
			show = true;
			timeCounter = 0.0f;
		}

		public void Hide()
		{
			started = true;
			show = false;
			timeCounter = 0.0f;
		}

		public void HideNow()
		{
			cg.alpha = 0.0f;
		}

		public void ShowNow()
		{
			cg.alpha = 1.0f;
		}
	}
}