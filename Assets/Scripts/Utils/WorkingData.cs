using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace SteinCo.Utils
{
	public class WorkingData
	{
		static string FileName = "workingdata.txt";

		public static string FilePath
		{
			get
			{
				return Path.Combine(Application.dataPath, FileName);
			}
			set
			{
				FileName = value;
			}
		}

		public static string GetData(string key, string valueDefault)
		{
			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
				return valueDefault;
			}
			//Debug.Log("GetConfig()" + valueDefault);

			if (!File.Exists(FilePath))
			{
				FileStream fs = File.Create(FilePath);
				fs.Close();
			}

			string[] lines = File.ReadAllLines(FilePath);

			if (lines == null || lines.Length == 0)
			{
				SetData(key, valueDefault);
				return valueDefault;
			}

			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i] == key)
				{
					return lines[i + 1];
				}
			}

			SetData(key, valueDefault);

			return valueDefault;
		}

		public static void SetData(string key, string value = "")
		{
			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
				return;
			}
			//Debug.Log("SetConfig()");
			List<string> lineList = new List<string>();
			StreamWriter sw = null;

			if (!File.Exists(FilePath))
			{
				FileStream fs = File.Create(FilePath);
				fs.Close();

				sw = new StreamWriter(FilePath);
				sw.WriteLine(key);
				sw.WriteLine(value);
			}
			else
			{
				int pos = -1;

				string[] lines = File.ReadAllLines(FilePath);

				for (int i = 0; i < lines.Length; i++)
				{
					lineList.Add(lines[i]);
					if (lines[i] == key)
					{
						pos = i + 1;
					}
				}

				if (pos != -1)
				{
					lineList[pos] = value;
				}
				else
				{
					lineList.Add(key);
					lineList.Add(value);
				}

				File.Delete(FilePath);
				sw = new StreamWriter(FilePath);
				foreach (var item in lineList)
				{
					sw.WriteLine(item);
				}
			}

			sw.Flush();
			sw.Close();
		}
	}
}