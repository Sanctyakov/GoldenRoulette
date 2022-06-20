using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

namespace SteinCo.Ivisa.RoulettePremium.Tools
{
	public class QualityController : MonoBehaviour
	{
		public Camera mainCamera;
		public Camera resultCamera;
		public Camera rouletteCamera;
		public RenderTexture resultCameraRT;
		public RenderTexture rouletteCameraRT;

		public RawImage resultCameraImage;
		public RawImage rouletteCameraImage;

		public Antialiasing cameraAAComponent;

		private int aa = 0;
		private bool lowPoly = false;
		private bool cameraAA = false;
		private int vsync = 0;


		private bool started = false;

		void Start()
		{
			/*
			resultCameraRTLow = new RenderTexture(resultCameraRT.width / 2, resultCameraRT.height / 2, resultCameraRT.depth, resultCameraRT.format, RenderTextureReadWrite.Default);
			rouletteCameraRTLow = new RenderTexture(rouletteCameraRT.width / 2, rouletteCameraRT.height / 2, rouletteCameraRT.depth, rouletteCameraRT.format, RenderTextureReadWrite.Default);

			resultCameraRTLow.Create();
			rouletteCameraRTLow.Create();
			*/

			aa = 0;
			//aa = QualitySettings.antiAliasing;
			QualitySettings.antiAliasing = aa;

			QualitySettings.SetQualityLevel(2, true);

			QualitySettings.vSyncCount = vsync;

			cameraAA = true;

			cameraAAComponent.enabled = cameraAA;
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				//started = !started;
			}

			if (!started)
			{
				return;
			}

			if (Input.GetKeyDown(KeyCode.Q))
			{
				mainCamera.orthographic = !mainCamera.orthographic;
			}

			/*
			if (Input.GetKeyDown(KeyCode.W))
			{
				lowPoly = !lowPoly;
				ChangePoly();
			}
			*/

			if (Input.GetKeyDown(KeyCode.E))
			{
				cameraAA = !cameraAA;

				cameraAAComponent.enabled = cameraAA;
			}

			/*
			if (Input.GetKeyDown(KeyCode.R))
			{
				AAMode mode = cameraAAComponent.mode;

				switch (mode)
				{
					case AAMode.FXAA2:
						mode = AAMode.FXAA3Console;
						break;
					case AAMode.FXAA3Console:
						mode = AAMode.FXAA1PresetA;
						break;
					case AAMode.FXAA1PresetA:
						mode = AAMode.FXAA1PresetB;
						break;
					case AAMode.FXAA1PresetB:
						mode = AAMode.NFAA;
						break;
					case AAMode.NFAA:
						mode = AAMode.SSAA;
						break;
					case AAMode.SSAA:
						mode = AAMode.DLAA;
						break;
					case AAMode.DLAA:
						mode = AAMode.FXAA3Console;
						break;
				}

				cameraAAComponent.mode = mode;
			}
			*/
			if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Space))
			{
				aa += 2;

				if (aa > 2)
				{
					aa = 0;
				}
				QualitySettings.antiAliasing = aa;
			}

			/*
			if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
			{
				high = !high;

				if (high)
				{
					rouletteCamera.targetTexture = rouletteCameraRT;
					rouletteCameraImage.texture = rouletteCameraRT;

					resultCamera.targetTexture = resultCameraRT;
					resultCameraImage.texture = resultCameraRT;
				}
				else
				{
					rouletteCamera.targetTexture = rouletteCameraRTLow;
					rouletteCameraImage.texture = rouletteCameraRTLow;

					resultCamera.targetTexture = resultCameraRTLow;
					resultCameraImage.texture = resultCameraRTLow;
				}
			}*/

			if (Input.GetKeyDown(KeyCode.V))
			{
				vsync++;

				if (vsync > 2)
				{
					vsync = 0;
				}

				QualitySettings.vSyncCount = vsync;
			}
		}

		void OnGUI()
		{
			if (started)
			{
				string[] names = QualitySettings.names;
				GUILayout.BeginVertical();
				int i = 0;

				Color original = GUI.contentColor;

				while (i < names.Length)
				{
					GUI.contentColor = original;
					if (i == QualitySettings.GetQualityLevel())
					{
						GUI.contentColor = Color.yellow;
					}
					if (GUILayout.Button(names[i]))
					{
						QualitySettings.SetQualityLevel(i, true);
					}

					i++;
				}


				GUI.contentColor = original;

				GUILayout.EndVertical();

				GUILayout.Label("(space bar) AA: " + aa);

				//			GUILayout.Label("(enter) RT High: " + high);

				GUILayout.Label("(Q) Perspective: " + (!mainCamera.orthographic));

				//			GUILayout.Label("(W) Low Poly: " + lowPoly);

				GUILayout.Label("(E) post AA: " + cameraAA);

				//			GUILayout.Label("(R) post AA tech: " + cameraAAComponent.mode);

				GUILayout.Label("(V) Vsync: " + vsync);
			}
		}

		public GameObject[] lowPolys;

		private void ChangePoly()
		{
			foreach (GameObject go in lowPolys)
			{
				go.SetActive(!lowPoly);
			}
		}
	}
}