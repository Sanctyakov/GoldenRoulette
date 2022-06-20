using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SteinCo.Ivisa.RoulettePremium.Tools
{
	public class RouletteViewTool : MonoBehaviour
	{
		public Transform cameraTransform;
		public Transform cameraPivot;
		public Transform roulette;
		public Text debugText;

		public float speedFactor = 1.0f;

		private float distance = 2.0f;
		private float rotation = 30.0f;
		private float height = 0.0f;
		private float rotationMultiplier = 10.0f;
		void Update()
		{
			string enter = "\n";

			float speed = speedFactor;

			if (Input.GetKey(KeyCode.LeftShift))
			{
				speed = speedFactor * 2.0f;
			}

			if (Input.GetKey(KeyCode.LeftControl))
			{
				speed = speedFactor / 2.0f;
			}

			if (Input.GetKey(KeyCode.Q))
			{
				distance -= speed * Time.deltaTime;
			}

			if (Input.GetKey(KeyCode.A))
			{
				distance += speed * Time.deltaTime;
			}

			if (Input.GetKey(KeyCode.Z))
			{
				distance = 2.0f;
			}


			if (Input.GetKey(KeyCode.W))
			{
				rotation += speed * Time.deltaTime * rotationMultiplier;
			}

			if (Input.GetKey(KeyCode.S))
			{
				rotation -= speed * Time.deltaTime * rotationMultiplier;
			}

			if (Input.GetKey(KeyCode.X))
			{
				rotation = 30.0f;
			}


			if (Input.GetKey(KeyCode.E))
			{
				height += speed * Time.deltaTime;
			}

			if (Input.GetKey(KeyCode.D))
			{
				height -= speed * Time.deltaTime;
			}

			if (Input.GetKey(KeyCode.C))
			{
				height = 0.0f;
			}

			distance = Mathf.Clamp(distance, 0.1f, 5.0f);
			rotation = Mathf.Clamp(rotation, 0.0f, 90.0f);
			height = Mathf.Clamp(height, -2.5f, 2.5f);

			cameraTransform.localPosition = Vector3.forward * -distance;
			cameraPivot.rotation = Quaternion.Euler(rotation, 0.0f, 0.0f);
			cameraPivot.localPosition = Vector3.up * height;
			
			string debugString = "(Q- A+ Z0) Distancia: " + distance.ToString("0.000") + enter;
			debugString += "(W+ S- X30) Rotación: " + rotation.ToString("0.000") + enter;
			debugString += "(E+ D- C0) Altura: " + height.ToString("0.000") + enter;
			debugString += enter;
			debugString += enter;
			debugString += enter;
			debugString += "(Shift * 2)" + enter;
			debugString += "(Ctrl / 2)" + enter;
			debugText.text = debugString;
		}
	}
}