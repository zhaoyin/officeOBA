using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer.Uploader
{
	/// <summary>
	/// FTPClient
	/// </summary>
	internal class FtpClient
	{
		public event BeginingEventHandler Begining;

		public event ProgressEventHandle Progress;

		public event CompletedEventHandler Completed;

		public event ExceptionEventHandle ExceptionError;

        private static readonly Logger Log = new Logger(typeof(FtpClient));

		protected void OnProgress(int rate, long length, long completedsize)
		{
			if (Progress != null)
			{
				var e = new ProgressEventArgs(rate, length, completedsize);
				Progress(this, e);
			}
		}

		protected void OnCompleted(string fileName)
		{
			if (Completed != null)
			{
				Completed(this, new CompletedEventArgs(fileName));
			}
		}

		protected void OnExcetipion(string message)
		{
			if (ExceptionError != null)
			{
				ExceptionError(this, new ExceptionEventArgs(message));
			}
		}

		protected void OnBegining(string fileName)
		{
			if (Begining != null)
			{
				Begining(this, new BeginingEventArgs(fileName, false));
			}
		}

		public string Server
		{
			get { return _server; }
            set { _server = value; }
		}

		public string Username
		{
			get { return _username; }
            set { _username = value; }
		}

		public string Password
		{
			get { return _password; }
            set { _password = value; }
		}

		public int Port
		{
            get { return _port; }
            set { _port = value; }
		}
		
		private string _server = "localhost";
		private string _username = "anonymous";
		private string _password = "anonymous@anonymous.net";
		private Socket _clientSocket = null;

		private int _port = 21;

		private static readonly int BUFFER_SIZE = 512;

		public FtpClient()
		{
		}

		public FtpClient(string host, string user, string pass)
		{
            _server = host;
            _username = user;
            _password = pass;
		}

		public void Login()
		{
			try
			{
                _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress addr = Dns.GetHostEntry(_server).AddressList[0];  // Dns.Resolve(server).AddressList[0];
                var ep = new IPEndPoint(addr, _port);
                Log.Debug("ftp login server=" + _server + ",address=" + addr+ ",port=" + _port);
                _clientSocket.Connect(ep);
			}
			catch (Exception ex)
			{
                if (_clientSocket != null && _clientSocket.Connected)
				{
                    _clientSocket.Close();
				}
                Log.Error(ex);
				throw new Exception("Couldn't connect to remote server", ex);
			}
            string str = ReadLine(_clientSocket);
			int resultCode = GetResultCode(str);

			if (resultCode != 220)
			{
				return;
			}

            str = SendCommand("USER " + _username, _clientSocket);

			resultCode = GetResultCode(str);

			if (!(resultCode == 331 || resultCode == 230))
			{
                _clientSocket.Close();
				return;
			}

			if (resultCode != 230)
			{
                str = SendCommand("PASS " + _password, _clientSocket);

				resultCode = GetResultCode(str);

				if (!(resultCode == 230 || resultCode == 202))
				{
                    Log.Debug("3" + resultCode);
                    _clientSocket.Close();
				}
			}
		}

		public void Upload(string localFileName, string remoteFileName)
		{
            string str = SendCommand("PASV", _clientSocket);
			int resultCode = GetResultCode(str);
			if (resultCode != 227)
			{
                Log.Error(str);
				throw new ApplicationException(str);
			}
            Log.Debug("pasv success: " + str);
			int pos1 = str.IndexOf('(');
			int pos2 = str.IndexOf(')');
			string data = str.Substring(pos1 + 1, pos2 - pos1 - 1);
			string[] param = data.Split(',');

			if (param.Length != 6)
				return;

			string ipAddress = param[0] + "." + param[1] + "." + param[2] + "." + param[3];
            _port = (Int32.Parse(param[4]) << 8) + Int32.Parse(param[5]);

			var s = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			var ep = new IPEndPoint(Dns.GetHostEntry(ipAddress).AddressList[0],_port);

			try
			{
				s.Connect(ep);
			}
			catch
			{
				Log.Warn("- couldn't connect server!");
			}
            Log.Debug("Connect success");
            str = SendCommand("STOR " + remoteFileName, _clientSocket);
			resultCode = GetResultCode(str);
            if (resultCode != 125 && resultCode != 150)
			{
				throw new ApplicationException("Resid:{XPlugin.FileManager.Exception.NoAccess}");
			}
            Log.Debug("STOR : remoteFileName=" + remoteFileName + ",result=" + str);
			OnBegining(localFileName);
			
			int bytes;
			var input = new FileStream(localFileName, FileMode.Open, FileAccess.Read);
			long fileLength = input.Length;
			var buffer = new byte[BUFFER_SIZE];

			int completedSize = 0;
			while ((bytes = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				completedSize += s.Send(buffer, bytes, 0);
				OnProgress((int)(completedSize*100/fileLength), fileLength, completedSize);
			}

			input.Close();
			if (s.Connected)
			{
				s.Close();
			}
            Log.Debug("finish transferring data");

            str = ReadLine(_clientSocket);
			resultCode = GetResultCode(str);

			if (resultCode != 226 && resultCode != 250)
			{
				throw new ApplicationException(str);
			}
			OnCompleted(localFileName);
		}

		public void Close()
		{
            if (_clientSocket != null)
			{
                SendCommand("QUIT", _clientSocket);
			}
            if (_clientSocket != null)
			{
                _clientSocket.Close();
                _clientSocket = null;
			}
		}

		private int GetResultCode(string resultStr)
		{
			int resultCode = 0;
			if (resultStr.Length > 3)
				resultCode = int.Parse(resultStr.Substring(0, 3));
			return resultCode;
		}

		private string SendCommand(string command, Socket clientSocket2)
		{
			Log.Debug(command);
			Byte[] cmdBytes = Encoding.Default.GetBytes((command + "\r\n").ToCharArray());
			clientSocket2.Send(cmdBytes, cmdBytes.Length, 0);

			return ReadLine(clientSocket2);
		}

		private string ReadLine(Socket clientSocket2)
		{
			string message = "";
			var buffer = new byte[BUFFER_SIZE];

			while (true)
			{
				int bytes = clientSocket2.Receive(buffer, buffer.Length, 0);
				message += Encoding.Default.GetString(buffer, 0, bytes);
				Log.Debug(message);

				if (bytes < buffer.Length)
				{
					break;
				}
			}

			string[] msg = message.Split('\n');

		    message = (msg.Length > 2 ? msg[msg.Length - 2] : msg[0]);

			if (message.Length > 4 && !message.Substring(3, 1).Equals(" "))
			{
				return ReadLine(clientSocket2);
			}

			return message;
		}
	}
}