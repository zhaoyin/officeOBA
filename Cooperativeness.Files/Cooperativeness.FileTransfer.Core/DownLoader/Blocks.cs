using System;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer.Downloader
{
    internal class Blocks : BlockInfo
    {
		private Thread _mThread = null;
        private static readonly Logger Log = new Logger(typeof(Blocks));

		public void Supend()
		{
            if (_mThread == null) return;
            _mThread.Suspend();
		}

		public void Resume()
		{
            if (this._mThread == null) return;
            this._mThread.Resume();
		}

		public void Cancel()
		{
            if (this._mThread == null) return;
            this._mThread.Abort();
		}

        public Blocks(HttpWebRequest oHttpWebRequest,string remotefile, int iBlockIndex, int iStartPosition, int blockSize)
        {
            Request = oHttpWebRequest;
            BlockIndex = iBlockIndex;
            StartPosition = iStartPosition;
            BlockSize = blockSize;
            RemoteFile = remotefile;
        }

        public override string ToString()
        {
            return string.Format(
                "Blocks\tBlockIndex:{0},StartPosition:{1},BlockSize:{2},CompletedPosition:{3},FilePath:{4}",
                BlockIndex.ToString(),
                StartPosition.ToString(),
                BlockSize.ToString(),
                CompletedPosition.ToString(),
                FilePath);
        }

        public void ReStore()
        {
            if (!DownloadState.AllowAppend)
            {
                if (File.Exists(FilePath))
                {
                    FileUtil.DeleteFile(FilePath);
                }
                return;
            }
            if (!File.Exists(FilePath)) return;
            var fileInfo = new FileInfo(FilePath);
            _CompletedPosition = (int)fileInfo.Length;
            BlockEvents.CompletedBytesCount += this.CompletedPosition;

            string loggerstr = string.Format("ReStore\tBlockIndex:{0},CompletedPosition:{1},CompletedBytesCount:{2}", BlockIndex.ToString(),this.CompletedPosition.ToString(),BlockEvents.CompletedBytesCount.ToString());
            Log.Debug(loggerstr);
        }
        
        public void Download()
        {
            var t = new ThreadStart(_Download);
            _mThread = new Thread(t);
            _mThread.Start();
        }
        private void _Download()
        {
            try
            {
				if(UnCompeleteSize <= 0)
				{
					IsCompleted = true;
                    BlockEvents.OnProgress(100,100);
					BlockEvents.OnCompleted();
					Request.Abort();
					return;
				}
                Log.Debug(BlockIndex.ToString() + "号文件块开始下载.");
                Request.Timeout = Timeout.Infinite;
                Request.Accept = "*/*";
                Request.AllowAutoRedirect = true;
                Request.AddRange(StartPosition + _CompletedPosition);
                Log.Debug(BlockIndex.ToString() + "号块正在获取网络回应.");
                var response = (HttpWebResponse)Request.GetResponse();
                Log.Debug(BlockIndex.ToString() + "号块正在获取流.");
                Stream stream = response.GetResponseStream();
                if (stream == null)
                {
                    IsCompleted = true;
                    BlockEvents.OnProgress(100,100);
                    BlockEvents.OnCompleted();
                    Request.Abort();
                    Log.Debug("无法获取到WebRequest请求的相应文件流~");
                    return;
                }
                Log.Debug(BlockIndex.ToString() + "号块正在创建暂存文件.");
                using (var filestream=new FileStream(FilePath,FileMode.Append,FileAccess.Write,FileShare.ReadWrite))
                {
                    var buffersize = DownloadState.BufferSize;
                    var buffer = new byte[buffersize];
                    Log.Debug(BlockIndex.ToString() + "号块初始化完毕.");
                    try
                    {
                        while (UnCompeleteSize > 0)
                        {
                            Log.Debug(BlockIndex.ToString() + "号块开始从" + _CompletedPosition.ToString() + "处读取流数据.");
                            int receiveCount = stream.Read(buffer, 0, (int)buffersize);
                            Log.Debug(BlockIndex.ToString() + "号块本次读取流数据长度为" + receiveCount.ToString() + "字节.");

                            if (receiveCount <= 0)
                            {
                                var sb = new StringBuilder();
                                sb.Append("发生下载阻塞" + "\t");
                                sb.Append("BlockIndex：" + BlockIndex.ToString() + "\t");
                                sb.Append("StartPosition：" + (StartPosition + CompletedPosition).ToString() + "\t");
                                sb.Append("BufferSize：" + BlockSize.ToString() + "\t");
                                sb.Append("ReceiveCount：" + receiveCount.ToString());
                                Log.Debug(sb.ToString());
                                Thread.Sleep(5000);
                                continue;
                            }

                            int availbleReceiveCount = (UnCompeleteSize < receiveCount ? UnCompeleteSize : receiveCount);
                            filestream.Write(buffer, 0, availbleReceiveCount);
                            filestream.Flush();
                            _CompletedPosition += availbleReceiveCount;
                            Log.Debug(BlockIndex.ToString() +
                                "号块已完成" + _CompletedPosition.ToString() + "/" + this.BlockSize.ToString() + "字节的下载.");
                            BlockEvents.CompletedBytesCount += availbleReceiveCount;
                            if (UnCompeleteSize <= 0 || DownloadState.StopDownload)
                            {
                                break;
                            }
                        }
                        buffer = null;
                        response.Close();
                        stream.Close();
                        if (DownloadState.StopDownload) return;

                        Log.Debug(BlockIndex.ToString() + "号文件块下载完成.");
                        this.IsCompleted = true;
                        BlockEvents.OnCompleted();
                        this.Request.Abort();
                        filestream.Close();
                    }
                    catch (Exception err)
                    {
                        buffer = null;
                        response.Close();
                        stream.Close();
                        filestream.Close();
                        BlockEvents.OnException(err);
                        this.Request.Abort();
                    }
                }
            }
            catch (Exception err)
            {
				BlockEvents.OnException(err);					
				this.Request.Abort();
            }
        }
    }
}
