using System;
using System.IO;
using System.Net;
using System.Threading;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer.Downloader
{
    internal class HttpDownloader : WebDownloader
    {
        private short _chunkCount=5;
        private HttpWebRequest _request = null;

        private static readonly Logger Log = new Logger(typeof(HttpDownloader));

        public HttpDownloader(TransferParameter parameter)
        {
            Parameter = parameter;
            DownloadFileState = null;
            _chunkCount = (parameter.ChunkCount > 0 ? parameter.ChunkCount : (short)5);
        }

        private void CreateDownload(TransferParameter parameter)
        {
            try
            {
                bool allowRanges = false;
                int contentLength;


                _request =DownloadRequst.GetWebRequest(parameter);
                _request.PreAuthenticate = true;

                _request.Accept = "*/*";
                _request.AllowAutoRedirect = true;
                _request.Method = "HEAD";

                Log.Debug("^ GET {0} HTTP/{1}", parameter.TransferUrl, _request.ProtocolVersion);
                Log.Debug("^ Host: {0}", _request.Address.Host);
                Log.Debug("^ Accept: {0}", _request.Accept);
                Log.Debug("^ User-Agent: {0}", _request.UserAgent);
                Log.Debug("^ Referer: {0}", _request.Referer);

                var response = (HttpWebResponse)_request.GetResponse();

                if (response.ContentLength == -1 || response.ContentLength > 0x7fffffff)
                    contentLength = 1024 * 1024 * 5;
                else
                    contentLength = (int)response.ContentLength;

                Log.Debug("v HTTP/{0} {1} OK", response.ProtocolVersion, response.StatusCode);
                for (int i = 0; i < response.Headers.Count; ++i)
                {
                    if (String.Compare(response.Headers.Keys[i], "Accept-Ranges", StringComparison.InvariantCultureIgnoreCase) == 0)
                        allowRanges = true;

                    Log.Debug("v {0}: {1}", response.Headers.Keys[i], response.Headers[i]);
                }

                response.Close();
                lock (this)
                {
                    FileUtil.CreateFile(parameter.LocalFile, contentLength);
                }
                DownloadFileState = new FileState(parameter.TransferUrl, parameter.LocalFile, contentLength, (allowRanges ? _chunkCount : (short)1));
                AssignFileStateDelegate();
                DownloadFileState.FireFileDownloadBeginingEvent();
            }
            catch (WebException e)
            {
                Log.Warn("- Got an Excetpion {0} in Create WebException", e.ToString());
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    string a = ((HttpWebResponse)e.Response).StatusCode.ToString();
                    a += ((HttpWebResponse)e.Response).StatusDescription;
                    Log.Warn(a);
                }

                throw new WebException(e.Message);
            }
            catch (Exception e)
            {
                Log.Warn("- Got an Excetpion {0}  in Create Exception", e.ToString());
                throw new Exception(e.Message);
            }
        }

        private void AssignFileStateDelegate()
        {
            DownloadFileState.FileDownProgressDelegate = new ProgressDelegate(OnProgress);
            DownloadFileState.DownFileCompletedDelegate = new FileDownCompletedDelegate(OnCompleted);
            DownloadFileState.DownErrorHandlerDelegate = new FileDownErrorHandlerDelegate(OnExcetipion);
            DownloadFileState.DownFileBeginingDelegate = new FileDownBeginingDelegate(OnBegining);
        }

        private long DownloadChunkBlock(FileState state, short blockIndex)
        {
            Stream rsm = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            int errorCount = 0;
        retry:
            try
            {
                BlockState bs = state.GetBlock(blockIndex);
                if (bs == null || bs.Size <= 0)
                    return -1;

                Log.Debug("- DownloadChunkBlock index:{0}[{1}-{2}]", blockIndex, bs.EnterPoint, bs.UnTransmittedSize);

                if (bs.IsCompleted)
                {
                    return 0;
                }

                using (var fs = File.Open(state.LocalFileUri.LocalPath,
                                         FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    Log.Debug("- Open file {0} successful!", state.LocalFileUri.LocalPath);
                    fs.Seek(bs.EnterPoint, SeekOrigin.Begin);
                    Log.Debug("- File seek({0}) successful!", bs.EnterPoint);

                    int count = bs.UnTransmittedSize;

                    request = DownloadRequst.GetWebRequest(state.RemoteFileUri, Parameter.Environment);

                    request.Accept = "*/*";
                    request.AllowAutoRedirect = true;

                    request.ProtocolVersion = HttpVersion.Version10;
                    request.Timeout = Timeout.Infinite;

                    //request.CookieContainer = CookieUtil.GetCokieContainer(state.RemoteFileInfo);

                    if (state.BlockCount > 1)
                        request.AddRange(bs.EnterPoint);

                    for (int i = 0; i < request.Headers.Count; ++i)
                        Log.Debug("^ [{2}]{0}: {1}", request.Headers.Keys[i], request.Headers[i], blockIndex);

                    Log.Debug("- [{0}]请求开始", blockIndex);
                    response = (HttpWebResponse)request.GetResponse();
                    Log.Debug("- [{0}]请求返回", blockIndex);
                    const int buffersize = 64 * 1024;
                    var buffer = new byte[buffersize];

                    rsm = response.GetResponseStream();
                    Log.Debug("- [{0}]开始接收数据!", blockIndex);
                    bs.BlockRunState = ActionStateType.Running;

                    while (count > 0)
                    {
                        if (bs.AssistThread == null)
                            Log.Debug("- AssistThread is null");

                        if (bs.BlockRunState == ActionStateType.Stop)
                        {
                            Log.Info("- [{0}] Stoped,Address:{1}", blockIndex, bs.EnterPoint);
                            break;
                        }

                        int ret = (rsm != null ? rsm.Read(buffer, 0, buffersize) : 0);

                        if (ret <= 0)
                        {
                            Log.Debug("- Receive Data Error, 5 seconds retry!");
                            Thread.Sleep(5000);
                            continue;
                        }

                        int retcopy = (count < ret ? count : ret);
                        fs.Write(buffer, 0, retcopy);
                        count -= retcopy;

                        bs.TransmittedSize += retcopy;

                        state.TransmittedLength += retcopy;

                        if (count <= 0)
                        {
                            break;
                        }
                    }
                    fs.Close();
                }

                Log.Debug("- [{0}]数据接收完成!", blockIndex);
            }
            catch (WebException e)
            {
                Log.Info(e.ToString());
                Log.Warn(e.Status.ToString());
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    //					state.SaveLogFile();
                    return -1;
                }
                if (e.Status == WebExceptionStatus.ReceiveFailure)
                {
                    errorCount++;
                    if (errorCount <= 3)
                    {
                        Thread.Sleep(1000);
                        Log.Info("goto retry {0} times by ReceiveFailure", errorCount);
                        goto retry;
                    }
                }
                if (e.Status == WebExceptionStatus.Timeout)
                {
                    errorCount++;
                    if (errorCount <= 5)
                    {
                        Thread.Sleep(1000);
                        Log.Info("goto retry {0} times by Timeout: {1}", errorCount, e.InnerException.Message);
                        goto retry;
                    }
                }
                Log.Error(e);
            }
            catch (ThreadAbortException e)
            {
                Log.Warn(e.ToString());
            }
            catch (Exception e)
            {
                Log.Info(e.ToString());
                //				state.SaveLogFile();
                return -1;
            }
            finally
            {
                if (rsm != null)
                {
                    rsm.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
                Log.Info("{0} block is finished!", blockIndex);
                //				state.SaveLogFile();
            }

            return 0;
        }

        protected override void DownloadFile()
        {
            DownloadFile(Parameter);
        }

        private void DownloadFile(TransferParameter parameter)
        {
            try
            {
                CreateDownload(parameter);
                Log.Info("Start download file job : {0}, {1}, {2}", parameter.TransferUrl, parameter.LocalFile, parameter.ChunkCount);
                DownloadFileState.Start(new ThreadProcDelegate(DownloadChunkBlock));
            }
            catch (WebException e)
            {
                Log.Warn("- Got an Excetpion {0}", e.ToString());
                throw;
            }
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
