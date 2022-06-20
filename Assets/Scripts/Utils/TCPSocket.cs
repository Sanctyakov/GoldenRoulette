using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SteinCo.Intercomms
{
	public class TCPSocket
	{
		private enum States
		{
			None,
			Server,
			Client,
		}

		private States state = States.None;

		private Thread workerThread;
		private Thread serverConnectThread;

		private int threadSleepingMilliseconds = 10;
		private int bufferSize = 32;
		private int errorRetryInterval = 10;

		private TcpListener listener;
		private TcpClient client;

		private NetworkStream stream;

		public delegate void MessageReceived(string message);
		public event MessageReceived OnMessageReceived;

		public delegate void ErrorMessage(string message);
		public event ErrorMessage OnError;

		public TCPSocket(int threadSleepingMilliseconds = 10, int bufferSize = 32, int errorRetryInterval = 10)
		{
			this.threadSleepingMilliseconds = threadSleepingMilliseconds;
			this.bufferSize = bufferSize;
			this.errorRetryInterval = errorRetryInterval;
		}

		~TCPSocket()
		{
			Close();
		}

		public void StartAsServer(string URI = "127.0.0.1", int port = 12288)
		{
			if (state == States.None)
			{
				IPAddress address = IPAddress.Parse(URI);
				listener = new TcpListener(address, port);

				listener.Start();

				serverConnectThread = new Thread(ServerConnectThread);
				serverConnectThread.Start();
			}
		}

		private void ServerConnectThread()
		{
			bool success = false;

			while (!success)
			{
				try
				{
					success = StartServer();

					if (success)
					{
						workerThread = new Thread(WorkerThread);
						workerThread.Start();
					}
					else
					{
						Thread.Sleep(errorRetryInterval);
					}
				}
				catch (Exception e)
				{
					if (OnError != null)
					{
						OnError.Invoke(e.Message);
					}
				}

				Thread.Sleep(errorRetryInterval);
			}

			serverConnectThread = null;
		}

		private bool StartServer()
		{
			bool res = false;
			if (listener.Pending())
			{
				client = listener.AcceptTcpClient();

				client.NoDelay = true;

				client.ReceiveBufferSize = bufferSize;
				client.SendBufferSize = bufferSize;

				stream = client.GetStream();

				state = States.Server;

				res = true;
			}

			return res;
		}

		public void StartAsClient(string URI = "127.0.0.1", int port = 12288)
		{
			if (state == States.None || (state == States.Client && !client.Connected))
			{
				try
				{
					StartClient(URI, port);

					if (workerThread != null)
					{
						workerThread.Abort();
						workerThread = null;
					}

					workerThread = new Thread(WorkerThread);
					workerThread.Start();

					if (OnMessageReceived != null)
					{
						OnMessageReceived.Invoke("CONNECTED#");
					}
				}
				catch (Exception e)
				{
					if (OnError != null)
					{
						OnError.Invoke(e.Message);
					}
				}
			}
		}

		private void StartClient(string URI, int port)
		{
			client = new TcpClient(URI, port);
			client.ReceiveBufferSize = bufferSize;
			client.SendBufferSize = bufferSize;

			stream = client.GetStream();

			state = States.Client;
		}

		private void WorkerThread()
		{
			byte[] buffer = new byte[client.ReceiveBufferSize];

			while (true)
			{
				try
				{
					switch (state)
					{
						case States.Server:
							if (listener.Pending())
							{
								serverConnectThread = new Thread(ServerConnectThread);
								serverConnectThread.Start();
							}
							break;
						case States.Client:
							break;
					}

					if (client.Connected && stream.CanRead && stream.DataAvailable)
					{
						int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);

						string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

						dataReceived = dataReceived.Replace("*", "");

						string[] dataCheck = dataReceived.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);

						// Remove the FOOBAR message before passing the message to the recipient
						dataReceived = string.Empty;
						foreach (string s in dataCheck)
						{
							if (!s.Equals("IGNORE"))
							{
								dataReceived += s + "#";
							}
						}

						if (dataReceived.Length > 0)
						{
							if (OnMessageReceived != null)
							{
								OnMessageReceived.Invoke(dataReceived);
							}
						}
					}

					Thread.Sleep(threadSleepingMilliseconds);
				}
				catch (Exception e)
				{
					if (OnError != null)
					{
						OnError.Invoke(e.Message);
					}

					Thread.Sleep(errorRetryInterval);
				}
			}
		}

		public void SendMessage(string message)
		{
			if (state != States.None)
			{
				try
				{
					// Sending FOOBAR message to force an error message in case the server disconnected (only works for client... go figure)
					byte[] bytesToSend = Encoding.UTF8.GetBytes("IGNORE#".PadRight(32, '*'));
					stream.Write(bytesToSend, 0, 32); // bytesToSend.Length
					stream.Flush();

					bytesToSend = Encoding.UTF8.GetBytes(message.PadRight(32, '*')); //
					stream.Write(bytesToSend, 0, 32); // bytesToSend.Length
					stream.Flush();
				}
				catch (Exception e)
				{
					OnError.Invoke(e.Message);
				}
			}
		}

		public void Close()
		{
			if (stream != null)
			{
				stream.Close();
				stream.Dispose();
			}

			if (client != null)
			{
				client.Close();
			}

			if (listener != null)
			{
				listener.Stop();
			}

			if (serverConnectThread != null)
			{
				serverConnectThread.Abort();
			}

			if (workerThread != null)
			{
				workerThread.Abort();
			}

			state = States.None;
		}
	}
}