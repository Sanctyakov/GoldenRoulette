using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SteinCo.Ivisa.RoulettePremium.Chips;
using SteinCo.Utils.Pooling;

namespace SteinCo.Ivisa.RoulettePremium.UI
{
	public class ChipGUITexts : MonoBehaviour
	{
		public ChipsController chipController;
		public Canvas canvas;
		public Camera mainCamera;
		public RectTransform textsRoot;
		public Pool textsPool;

		private Dictionary<int, ChipTexts> texts = new Dictionary<int, ChipTexts>();

		private struct ChipTexts
		{
			public GameObject go;
			public RectTransform rt;
			public Text text;
			public GameObject chip;
		}

		void Update()
		{
			foreach (KeyValuePair<int, ChipTexts> kvp in texts)
			{
				ChipTexts chipText = kvp.Value;
				Vector3 position = mainCamera.WorldToScreenPoint(chipText.chip.transform.position + Vector3.up * 0.005f) / canvas.scaleFactor;
				//position += new Vector3(-15.0f, -15.0f, 0.0f);
				//1.288783
				chipText.rt.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				//chipText.rt.localScale = new Vector3(1.1f, 1.1f, 1.0f);
				//chipText.rt.localRotation = Quaternion.identity;
				chipText.rt.anchoredPosition = position;
			}
			
		}

		public void SetText(int id, GameObject chip, int pool)
		{
			//canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			ChipTexts chipText;

			GameObject go;
			if (!texts.ContainsKey(id))
			{
				go = textsPool.GetNewObject();
				go.transform.SetParent(textsRoot);

				chipText = new ChipTexts();

				chipText.go = go;
				chipText.rt = go.GetComponent<RectTransform>();
				chipText.text = go.GetComponent<Text>();
				chipText.chip = chip;

				texts.Add(id, chipText);
			}
			else
			{
				chipText = texts[id];
				chipText.chip = chip;
				texts[id] = chipText;
			}

			

			texts[id].go.SetActive(true);
			chipText.text.text = chipController.GetStringValue(pool);
			Vector3 position = mainCamera.WorldToScreenPoint(chipText.chip.transform.position + Vector3.up * 0.005f) / canvas.scaleFactor;
			//position += new Vector3(-15.0f, -15.0f, 0.0f);
			//1.288783
			chipText.rt.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			//chipText.rt.localScale = new Vector3(1.1f, 1.1f, 1.0f);
			//chipText.rt.localRotation = Quaternion.identity;
			chipText.rt.anchoredPosition = position;
			//canvas.renderMode = RenderMode.ScreenSpaceCamera;
		}

		public void HideText(int id)
		{
			//canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			if (texts.ContainsKey(id))
			{
				texts[id].go.SetActive(false);
			}
			//canvas.renderMode = RenderMode.ScreenSpaceCamera;
		}

		public void HideAll()
		{
			//canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			foreach (KeyValuePair<int, ChipTexts> kvp in texts)
			{
				kvp.Value.go.SetActive(false);
			}
			//canvas.renderMode = RenderMode.ScreenSpaceCamera;
		}

		public	void SetCanvasToCamera()
		{
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
		}

		public void SetCanvasToOverlay()
		{
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		}

	}
}
