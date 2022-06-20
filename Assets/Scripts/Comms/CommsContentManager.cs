using UnityEngine;
using System.Collections.Generic;

namespace SteinCo.Comms
{
	public class CommsContentManager : MonoBehaviour
	{
		public ServerCommsGetVersion commsManager;

		public int PendingRequests
		{
			get
			{
				return (pendingForms != null) ? pendingForms.Count : 0;
			}
		}

		private class FormDataContainer
		{
			public int id;
			public FormContents form;
		}

		private List<FormDataContainer> pendingForms;

		private int currentID;

		public delegate void RequestSuccess(int id, string data);
		public event RequestSuccess OnRequestSuccess;
		public delegate void RequestFailed(int id, string data, FormContents form);
		public event RequestFailed OnRequestFailed;

		void Awake()
		{
			commsManager.OnCommsSuccess += OnCommsSuccess;
			commsManager.OnCommsFailed += OnCommsFailed;
		}

		void OnDestroy()
		{
			commsManager.OnCommsSuccess -= OnCommsSuccess;
			commsManager.OnCommsFailed -= OnCommsFailed;
		}

		void Start()
		{
			pendingForms = new List<FormDataContainer>();
		}

		public int AddRequest(FormContents form)
		{
			currentID++;

			FormDataContainer container = new FormDataContainer();
			container.id = currentID;
			container.form = form;

			pendingForms.Add(container);

			return currentID;
		}

		public bool GetCurrentContent(out int id, out FormContents form)
		{
			bool res = false;
			form = null;
			id = 0;

			if (pendingForms != null && pendingForms.Count > 0)
			{
				res = true;
				form = pendingForms[0].form;
				id = pendingForms[0].id;
			}

			return res;
		}

		private void OnCommsSuccess(int id, string data)
		{
			FormDataContainer finishedForm = pendingForms.Find((FormDataContainer x) =>
			{
				return x.id == id;
			});

			if (finishedForm != null)
			{
				pendingForms.Remove(finishedForm);

				OnRequestSuccess.Invoke(id, data);
			}
			else
			{
				////DebugWithDate.LogError("Form with id [" + id + "] was not in the pending queue");
			}
		}

		private void OnCommsFailed(int id, string errorMessage)
		{
			FormContents form = null;

			FormDataContainer finishedForm = pendingForms.Find((FormDataContainer x) =>
			{
				return x.id == id;
			});

			if (finishedForm != null)
			{
				form = finishedForm.form;
				pendingForms.Remove(finishedForm);

				if (OnRequestFailed == null)
				{
					pendingForms.Add(finishedForm);
				}
			}

			OnRequestFailed.Invoke(id, errorMessage, form);
		}
	}
}