using UnityEngine;

namespace SteinCo.Ivisa.RoulettePremium.Tools
{
	public class TableColor : MonoBehaviour
	{
		public Texture2D[] textures;
		public Material mat;
		private int counter = 0;

		void OnAwake()
		{
			mat.SetTexture("_MainTex", textures[0]);
		}

		void OnDestroy()
		{
			mat.SetTexture("_MainTex", textures[0]);
		}

		public void OnChangeColor()
		{
			counter++;
			if (counter >= textures.Length)
			{
				counter = 0;
			}

			mat.SetTexture("_MainTex", textures[counter]);
		}
	}
}