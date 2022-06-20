using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SteinCo.Comms
{
	public class ServerComms : MonoBehaviour
	{
		public CommsContentManager contentManager;
		
		public float RetryPeriod
		{
			get
			{
				return retryPeriod;
			}
			set
			{
				retryPeriod = value;
			}
		}

		public float TimeOut
		{
			get
			{
				return timeOut;
			}
			set
			{
				timeOut = value;
			}
		}

		private enum Status
		{
			Waiting,
			Communicating,
		}

		private Status status = Status.Waiting;
		private int currentID = 0;

		private IEnumerator coroutine;
		private WWW currentWWW = null;

		private float retryPeriod = 60.0f;
		private float timeCounter = 0.0f;
		private float timeOutCounter = 0.0f;
		private float timeOut = 30.0f;

		public delegate void CommsSuccess(int id, string data);
		public event CommsSuccess OnCommsSuccess;

		public delegate void CommsFailed(int id, string data);
		public event CommsFailed OnCommsFailed;

		void Awake()
		{
			RetryPeriod = 60.0f;
			TimeOut = 30.0f;
			status = Status.Waiting;
		}

		void Update()
		{
			switch (status)
			{
				case Status.Waiting:
					timeCounter += Time.deltaTime;

					if (timeCounter >= retryPeriod)
					{
						StartCommunication();
					}
					break;
				case Status.Communicating:
					timeOutCounter += Time.deltaTime;
					if (timeOutCounter >= timeOut)
					{
						// Fail by timeOut
						////DebugWithDate.LogWarning("Conection Timeout");
						StopCoroutine(coroutine);
						currentWWW.Dispose();
						currentWWW = null;

						OnCommsFailed.Invoke(currentID, "Timed out at " + timeOut + " seconds");

						status = Status.Waiting;
					}
					break;
			}
		}

		private void StartCommunication()
		{
			timeCounter = 0.0f;
			timeOutCounter = 0.0f;

			FormContents formContents = null;
			currentID = 0;

			bool success = contentManager.GetCurrentContent(out currentID, out formContents);

			if (!success)
			{
				return;
			}

			status = Status.Communicating;

			List<string> keys = formContents.GetKeys();
			WWWForm form = new WWWForm();

			foreach (string key in keys)
			{
				if (formContents.IsBinary(key))
				{
					byte[] data;
					string mimeType;
					string path;

					formContents.GetBinaryData(key, out data, out path, out mimeType);

					form.AddBinaryData(key, data, path, mimeType);
				}
				else
				{
					form.AddField(key, formContents.GetStringData(key));
				}
			}

			currentWWW = new WWW(formContents.URI, form);
			coroutine = WaitForVisitorAnswer(currentWWW);
			StartCoroutine(coroutine);
		}

		IEnumerator WaitForVisitorAnswer(WWW www)
		{
			yield return www;

			status = Status.Waiting;

			if (www.error != null)
			{
				////DebugWithDate.LogError("Comms Manager WWW error " + www.error + "\n" + www.url);
				OnCommsFailed.Invoke(currentID, www.error);
			}
			else
			{
				////DebugWithDate.Log("Comms Manager Upload success " + www.url);

				OnCommsSuccess.Invoke(currentID, www.text);
			}
		}
	}
}