using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO;

namespace SteinCo.Comms
{
	public class ServerCommsGetVersion : MonoBehaviour
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

		private bool failure = false;

		public delegate void CommsSuccess(int id, string data);
		public event CommsSuccess OnCommsSuccess;

		public delegate void CommsFailed(int id, string data);
		public event CommsFailed OnCommsFailed;

		void Awake()
		{
			status = Status.Waiting;

			ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
		}

		private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
		{
			// all Certificates are accepted
			return true;
		}

		void Update()
		{
			switch (status)
			{
				case Status.Waiting:
					timeCounter += Time.unscaledDeltaTime;

					if (failure)
					{
						if (timeCounter >= retryPeriod)
						{
							StartCommunication();
						}
					}
					else
					{
						StartCommunication();
					}
					break;
				case Status.Communicating:

					timeOutCounter += Time.unscaledDeltaTime;

					if (timeOutCounter >= timeOut)
                    //if (true)
					{
						// Fail by timeOut
						//DebugWithDate.LogWarning("Connection Timeout");
						//DebugWithDate.LogWarning("URL: " + currentWWW.url);

                        failure = true;

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
			FormContents formContents = null;
			currentID = 0;

			bool success = contentManager.GetCurrentContent(out currentID, out formContents);

			if (!success)
			{
				return;
			}
			timeCounter = 0.0f;
			timeOutCounter = 0.0f;

			failure = false;

			status = Status.Communicating;

			List<string> keys = formContents.GetKeys();
			string URI = formContents.URI + "?";

			string data = string.Empty;
			foreach (string key in keys)
			{
				////DebugWithDate.Log(formContents.GetStringData(key) + " --- " + WWW.EscapeURL(formContents.GetStringData(key)));
				data += key + "=" + formContents.GetStringData(key) + "&";
				//data += key + "=" + WWW.EscapeURL(formContents.GetStringData(key)) + "&";
			}

            ////DebugWithDate.Log(URI + data);


            /*
			coroutine = WaitForAnswer(URI + data);
			StartCoroutine(coroutine);
			*/

            //DebugWithDate.Log("Llamada al servidor [" + formContents.GetStringData("command") + "] [" + currentID + "]");

            currentWWW = new WWW(URI + data);
			coroutine = WaitForAnswer(currentWWW);
			StartCoroutine(coroutine);
		}

		IEnumerator WaitForAnswer(string url)
		{
			yield return url;

			HttpWebRequest request = null;
			HttpWebResponse response = null;

			status = Status.Waiting;
			try
			{
				request = (HttpWebRequest)WebRequest.Create(url);
				response = (HttpWebResponse)request.GetResponse();
			}
			catch (System.Exception e)
			{
				//DebugWithDate.LogError("Comms Manager error " + e.Message);
				OnCommsFailed.Invoke(currentID, e.Message);
			}

			if (response != null)
			{
				if (response.StatusCode != HttpStatusCode.OK)
				{
					//DebugWithDate.LogError("Comms Manager error " + response.StatusDescription);
					OnCommsFailed.Invoke(currentID, response.StatusDescription);
				}
				else
				{
					//DebugWithDate.Log("Comms Manager Upload success " + request.RequestUri);

					Stream dataStream = response.GetResponseStream();
					StreamReader reader = new StreamReader(dataStream);
					string responseFromServer = reader.ReadToEnd();

					OnCommsSuccess.Invoke(currentID, responseFromServer);
				}
			}
		}

		IEnumerator WaitForAnswer(WWW www)
		{
			yield return www;

			status = Status.Waiting;

			if (www.error != null)
			{
				failure = true;
				//DebugWithDate.LogError("Comms Manager WWW error " + www.error + "\n" + www.url);
				OnCommsFailed.Invoke(currentID, www.error);
			}
			else
			{
				//DebugWithDate.Log("Comms Manager Upload success " + www.url);

				OnCommsSuccess.Invoke(currentID, www.text);
			}
		}
	}
}