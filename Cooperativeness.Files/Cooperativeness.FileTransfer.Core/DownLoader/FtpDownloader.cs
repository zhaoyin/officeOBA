using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using Cooperativeness.FileTransfer.Core;
using Cooperativeness.FileTransfer.Core.Utility;

namespace Cooperativeness.FileTransfer.Downloader
{
    internal class FtpDownloader : WebDownloader
    {
        private static readonly Logger Log = new Logger(typeof(FtpDownloader));

        private static Uri _transferUri = null;

        protected string UserName
        {
            get
            {
                if (Parameter.Environment != null && Parameter.Environment.UseAuthentication)
                {
                    return Parameter.Environment.Username;
                }
                return "anonymous";
            }
        }

        protected string Password
        {
            get
            {
                if (Parameter.Environment != null && Parameter.Environment.UseAuthentication)
                {
                    return Parameter.Environment.Username;
                }
                return "youname@domain.com";
            }
        }

        protected int Port
        {
            get
            {
                if (_transferUri.Port > 0) return _transferUri.Port;
                return 21;
            }
        }

        protected string Host
        {
            get { return _transferUri.Host; }
        }

        protected string Path
        {
            get { return _transferUri.AbsolutePath; }
        }

        protected int GetResultCode(string line)
        {
            int ret = (line.Length >= 3 ? Int32.Parse(line.Substring(0, 3)) : 0);
            return ret;
        }

        protected string ReadLine(Socket s)
        {
            if (s == null || !s.Connected)
                return "";

            const int buffersize = 512;
            var buffer = new byte[buffersize];
            string data = "";

            while (true)
            {
                var readbytes = s.Receive(buffer, buffersize, 0);
                data += Encoding.Default.GetString(buffer, 0, readbytes);

                if (readbytes < buffersize)
                    break;
            }

            char[] seperator = { '\n' };
            string[] message = data.Split(seperator);
            data = (message.Length > 2 ? message[message.Length - 2] : message[0]);

            if (data.Length > 3)
            {
                if (!data.Substring(3, 1).Equals(" "))
                    return ReadLine(s);
            }

            Log.Debug("v {0}", data);
            return data;
        }

        protected string SendFtpCommand(Socket s, string command)
        {
            if (s == null || !s.Connected || command.Length == 0)
                return "";

            Log.Debug("^ {0}", command);

            byte[] cmds = Encoding.Default.GetBytes((command + "\r\n").ToCharArray());
            s.Send(cmds, cmds.Length, 0);

            string result = ReadLine(s);
            return result;
        }

        protected void SetMode(Socket s, bool binary)
        {
            string strResult = SendFtpCommand(s, "TYPE " + (binary ? "I" : "A"));
            if (GetResultCode(strResult) != 200)
                Log.Debug(strResult.Substring(4));
        }

        protected void CleanUp(Socket s, bool quit)
        {
            if (s != null && s.Connected)
            {
                if (quit)  Log.Debug(SendFtpCommand(s, "QUIT"));
                s.Close();
            }
        }

        protected Socket Login()
        {
            Socket s = ConnectSocket(Host, Port);
            if (s == null)
            {
                Log.Debug("- couldn't Connect to {0}:{1}!", Host, Port);
                return null;
            }

            string strResult = ReadLine(s);
            if (GetResultCode(strResult) != 220)
            {
                CleanUp(s, false);
                Log.Debug("v {0}", strResult);
                return null;
            }

            strResult = SendFtpCommand(s, "USER " + UserName);
            int code = GetResultCode(strResult);
            if (!(code == 331 || code == 230))
            {
                CleanUp(s, true);
                Log.Debug("v {0}", strResult);
                return null;
            }

            if (code != 230)
            {
                strResult = SendFtpCommand(s, "PASS " + Password);
                code = GetResultCode(strResult);
                if (!(code == 230 || code == 202))
                {
                    CleanUp(s, true);
                    Log.Debug("v {0}", strResult);
                    return null;
                }
            }

            return s;
        }

        protected bool DeleteFile(Socket s, string filepath)
        {
            if (s == null || !s.Connected || filepath.Length == 0)
                return false;

            string strResult = SendFtpCommand(s, "DELETE " + filepath);
            if (GetResultCode(strResult) == 213)
                return true;
            Log.Debug(strResult);
            return false;
        }

        protected int GetFileSize(Socket s, string filepath)
        {
            if (s == null || !s.Connected || filepath.Length == 0)
                return 0;

            int size = 0;
            string strResult = SendFtpCommand(s, "SIZE " + filepath);
            if (GetResultCode(strResult) == 213)
                size = (strResult.Length > 4 ? Int32.Parse(strResult.Substring(4)) : 0);
            else
                Log.Debug(strResult);
            return size;
        }

        public FtpDownloader(TransferParameter parameter)
        {
            Parameter = parameter;
            DownloadFileState = null;
            _transferUri = new Uri(parameter.TransferUrl);
        }

        protected override void DownloadFile()
        {
            DownloadFile(Parameter);
        }

        private void DownloadFile(TransferParameter parameter)
        {
            if (!CreateDownload(parameter))
            {
                Log.Debug("- 无法连接服务器,请确定远程服务器是否可用");
                throw new ApplicationException("- 无法连接服务器,请确定远程服务器是否可用");
            }

            Log.Debug("- Start Thread Download");
            DownloadFileState.Start(new ThreadProcDelegate(DownloadChunkBlock));
        }

        private string GetUnescapeString(string pathfile)
        {
            return HttpUtility.UrlDecode(pathfile);
        }

        private long DownloadChunkBlock(FileState state, short blockIndex)
        {
            try
            {
                string strResult;
                int code;
                BlockState bs = state.GetBlock(blockIndex);
                if (bs == null || bs.Size <= 0)
                    return -1;

                Socket s = Login();
                if (s == null)
                    return -1;

                Log.Debug("- DownloadChunkBlock index:{0}[{1}-{2}]",
                          blockIndex, bs.EnterPoint,
                          bs.UnTransmittedSize);

                if (bs.IsCompleted)
                    return 0;

                int count = bs.UnTransmittedSize;
                Log.Debug("- [{0}]开始接收数据!", blockIndex);

                Socket dataSocket = CreateDataSocket(s);
                if (dataSocket == null)
                {
                    Log.Debug("- Create Data socket faild!");
                    return -1;
                }

                SetMode(s, true);
                if (state.BlockCount > 1 && bs.EnterPoint > 0)
                {
                    strResult = SendFtpCommand(s, "REST " + bs.EnterPoint);

                    if (GetResultCode(strResult) != 350)
                    {
                        Log.Debug("- server may not support resuming. thread exit!");
                        CleanUp(dataSocket, false);
                        CleanUp(s, true);
                        return 0;
                    }
                }

                strResult = SendFtpCommand(s, "RETR " + GetUnescapeString(_transferUri.AbsolutePath));
                code = GetResultCode(strResult);
                if (!(code == 150 || code == 125 || code == 226))
                {
                    Log.Debug(strResult.Substring(4));
                    CleanUp(dataSocket, false);
                    CleanUp(s, true);
                    return 0;
                }
                using (var fs = File.Open(state.LocalFileUri.LocalPath,
                                          FileMode.OpenOrCreate,
                                          FileAccess.ReadWrite,
                                          FileShare.ReadWrite))
                {
                    Log.Debug("- Open file {0} successful!", state.LocalFileUri.LocalPath);
                    fs.Seek(bs.EnterPoint, SeekOrigin.Begin);
                    Log.Debug("- File seek({0}) successful!", bs.EnterPoint);

                    int buffersize = /*((Environment!=null) ? Environment.WritetoDiskBufferSize :*/ 64*1024;
                    var buffer = new byte[buffersize];

                    bs.BlockRunState = ActionStateType.Running;
                    while (count > 0)
                    {
                        if (bs.AssistThread == null)
                            Log.Debug("- AssistThread is null");

                        if (bs.BlockRunState == ActionStateType.Pause &&
                            bs.AssistThread != null &&
                            bs.AssistThread.IsAlive)
                        {
                            Log.Debug("- [{0}] Paused,Address:{1}", blockIndex, bs.EnterPoint);
                            bs.AssistThread.Suspend();
                        }

                        if (bs.BlockRunState == ActionStateType.Stop)
                        {
                            break;
                        }

                        int ret = dataSocket.Receive(buffer, buffersize, 0);
                        bool error = false;

                        if (ret <= 0)
                        {
                            Log.Debug("- Receive Data Error, 5 seconds retry!");
                            Thread.Sleep(5000);
                            continue;
                        }

                        try
                        {
                            int retcopy = (count < ret ? count : ret);
                            fs.Write(buffer, 0, retcopy);
                            count -= retcopy;

                            bs.TransmittedSize += retcopy;

                            state.TransmittedLength += retcopy;
                        }
                        catch (Exception e)
                        {
                            Log.Debug(e.ToString());
                            error = true;
                        }
                        finally
                        {
                            if (error)
                                fs.Close();
                        }

                        if (error || count <= 0)
                            break;
                    }

                    CleanUp(dataSocket, false);
                    CleanUp(s, true);

                    state.CanFireDownloadCompletedEvent();
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                Log.Debug(e);
                return -1;
            }
            return 0;
        }

        internal bool CreateDownload(TransferParameter parameter)
        {
            bool ret = false;

            try
            {
                Socket s = Login();
                if (s != null)
                {
                    int contentLength = GetFileSize(s, GetUnescapeString(_transferUri.AbsolutePath));
                    if (contentLength > 0)
                    {
                        DownloadFileState = new FileState(parameter.TransferUrl, parameter.LocalFile, contentLength, parameter.ChunkCount);
                        if (DownloadFileState != null)
                        {
                            ret = true;
                            AssignFileStateDelegate();
                            DownloadFileState.FireFileDownloadBeginingEvent();
                        }
                    }
                    CleanUp(s, true);
                }

            }
            catch (Exception e)
            {
                Log.Debug("- Got an Excetpion {0}", e.ToString());
                Log.Warn(e);
                ret = false;
            }

            return ret;
        }

        private void AssignFileStateDelegate()
        {
            DownloadFileState.FileDownProgressDelegate = new ProgressDelegate(OnProgress);
            DownloadFileState.DownFileCompletedDelegate = new FileDownCompletedDelegate(OnCompleted);
            DownloadFileState.DownErrorHandlerDelegate = new FileDownErrorHandlerDelegate(OnExcetipion);
            DownloadFileState.DownFileBeginingDelegate = new FileDownBeginingDelegate(OnBegining);
        }


        protected Socket ConnectSocket(string server, int port)
        {
            Socket s = null;
            try
            {
                var iphe = Dns.GetHostEntry(server);// Dns.Resolve(server);
                foreach (IPAddress ipad in iphe.AddressList)
                {
                    var ipe = new IPEndPoint(ipad, port);
                    var sk = new Socket(ipe.AddressFamily,SocketType.Stream,ProtocolType.Tcp);
                    sk.Connect(ipe);
                    if (sk.Connected)
                    {
                        Log.Debug("- Connected");
                        s = sk;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug("- Got an Excetpion {0}", e.ToString());
                Log.Warn(e);
            }

            return s;
        }
        protected Socket CreateDataSocket(Socket commandsocket)
        {
            string strResult = SendFtpCommand(commandsocket, "PASV");
            if (GetResultCode(strResult) != 227)
                return null;

            int pos1 = strResult.IndexOf('(');
            int pos2 = strResult.IndexOf(')');
            string data = strResult.Substring(pos1 + 1, pos2 - pos1 - 1);
            string[] param = data.Split(',');

            if (param.Length != 6)
                return null;

            string ipAddress = param[0] + "." + param[1] + "." + param[2] + "." + param[3];
            int port = (Int32.Parse(param[4]) << 8) + Int32.Parse(param[5]);

            var s = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            var ep = new IPEndPoint(Dns.GetHostEntry(ipAddress).AddressList[0],port);

            try
            {
                s.Connect(ep);
            }
            catch
            {
                Log.Debug("- couldn't connect server!");
            }
            return s;
        }

        public override void Supend()
        {
            DownloadFileState.Pause(-1);
        }

        public override void Resume()
        {
            DownloadFileState.Resume(-1);
        }

        public override void Cancel()
        {
            DownloadFileState.Stop(-1);
        }

        public override void Join()
        {
            DownloadFileState.Join();
        }

    }
}
