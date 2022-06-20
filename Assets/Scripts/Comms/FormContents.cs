using UnityEngine;
using System.Collections.Generic;

namespace SteinCo.Comms
{
	public class FormContents
	{
		private class DataClass
		{
			public bool isBinary;
			public string data;
			public byte[] binaryData;
			public string path;
			public string mimeType;
		}

		public enum ImageType
		{
			PNG,
			JPEG,
		}

		[SerializeField]
		private Dictionary<string, DataClass> data;
		[SerializeField]
		public string URI
		{
			get;
			set;
		}

		public FormContents(string uri, int priority)
		{
			URI = uri;
			data = new Dictionary<string, DataClass>();
		}

		public void Add(string name, string value)
		{
			DataClass dataObject = new DataClass();
			dataObject.isBinary = false;
			dataObject.data = value;
			data.Add(name, dataObject);
		}

		public void Remove(string name)
		{
			data.Remove(name);
		}

		public void AddBinary(string name, Texture2D image, string path, ImageType type = ImageType.PNG)
		{
			DataClass dataObject = new DataClass();

			dataObject.isBinary = true;
			dataObject.path = path;

			switch (type)
			{
				case ImageType.PNG:
					dataObject.binaryData = image.EncodeToPNG();
					dataObject.mimeType = "image/png";
					break;
				case ImageType.JPEG:
					dataObject.binaryData = image.EncodeToJPG();
					dataObject.mimeType = "image/jpg";
					break;
			}

			data.Add(name, dataObject);
		}

		public void AddBinary(string name, byte[] data, string path, string mimeType)
		{
			DataClass dataObject = new DataClass();

			dataObject.isBinary = true;
			dataObject.binaryData = data;
			dataObject.path = path;
			dataObject.mimeType = mimeType;

			this.data.Add(name, dataObject);
		}

		public List<string> GetKeys()
		{
			List<string> keys = new List<string>();

			foreach (string key in data.Keys)
			{
				keys.Add(key);
			}

			return keys;
		}

		public bool IsBinary(string name)
		{
			return data[name].isBinary;
		}

		public string GetStringData(string name)
		{
			return data[name].data;
		}

		public void GetBinaryData(string name, out byte[] data, out string path, out string mimeType)
		{
			path = this.data[name].path;
			data = this.data[name].binaryData;
			mimeType = this.data[name].mimeType;
		}
	}
}