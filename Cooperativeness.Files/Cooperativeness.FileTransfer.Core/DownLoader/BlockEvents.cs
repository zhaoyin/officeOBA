using System;
using System.IO;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer.Downloader
{
	/// <summary>
	/// BlockEvent 的摘要说明。
	/// </summary>
    internal class BlockEvents
	{
	    static public FileDownBeginingDelegate Begining;
		static public ProgressDelegate Progress;
		static public FileDownCompletedDelegate Completed;
		static public FileDownErrorHandlerDelegate ExceptionError;

	    private static long _completebytes;
        public static long CompletedBytesCount
        {
            get
            {
                return _completebytes;
            }
            set
            {
                _completebytes = value;

                OnProgress(_completebytes,TotalBytes);
            }
        }



        public static long TotalBytes { get; set; }
        private static readonly Logger Log = new Logger(typeof(BlockEvents));

        static public void OnException(Exception oException)
        {
            DownloadState.StopDownload = true;
            Log.Error(oException);
            if (ExceptionError != null)
            {
                ExceptionError(Logger.GetExceptionDetail(oException));
            }
        }

		static public void OnProgress(long iCompletedBytes,long totalBytes)
		{
            double dRate = 100;
            if (!DownloadState.HasFinished)
            {
                dRate = ((double)iCompletedBytes / totalBytes) * 100;
            }
			if(Progress != null)
			{
				var iRate = (int)dRate;
                Progress(iRate, totalBytes, iCompletedBytes);
			}
		}

        static public void OnCompleted()
		{
		    try
		    {
                if (CompletedBytesCount < DownloadState.CurrentApply.FileSize) return;

                if (!DownloadState.HasFinished)
                {
                    bool allFinished = true;

                    foreach (Blocks item in DownloadState.BlockList)
                    {
                        if (!item.IsCompleted)
                        {
                            allFinished = false;
                            break;
                        }
                        //item.Cancel();
                    }
                    if (!allFinished) return;

                    DownFinished();
                    File.SetLastWriteTime(
                        DownloadState.Parameter.LocalFile,
                        DownloadState.CurrentApply.LastModified == DateTime.MinValue ? DateTime.Now : DownloadState.CurrentApply.LastModified);
                    FileUtil.DeleteDirectory(Logger.TemporaryDirectory);
                    UpdateHistory();

                    string logstr = "文件下载完成\t" + DownloadState.Parameter.TransferUrl;
                    Log.Info(logstr);
                }

                DownloadState.IsCompleted = true;

                if (Completed != null)
                {
                    Completed(Path.GetFileName(DownloadState.Parameter.LocalFile));
                }
            }
		    catch (Exception e)
		    {
		        Log.Warn(e);
		        DownloadState.IsCompleted = true;
		    }

		}

        private static void DownFinished()
        {
            string sLocalFile = DownloadState.Parameter.LocalFile;
            var blocklist = DownloadState.BlockList;
            if (File.Exists(sLocalFile))
            {
                FileUtil.DeleteFile(sLocalFile);
            }
            using (var writer = new FileStream(sLocalFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                foreach (Blocks item in blocklist)
                {
                    if (!File.Exists(item.FilePath)) continue;
                    var buffer = new byte[3 * 1024 * 1024];
                    using (var reader = new FileStream(item.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        int readlength =0;
                        while ((readlength = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            writer.Write(buffer,0,readlength);
                            writer.Flush();
                        }
                        reader.Close();
                    }
                }
                writer.Close();
            }
        }

        private static void UpdateHistory()
        {
            var remotefile = DownloadState.Parameter.RemoteFile;
            var historyApply = new Apply();
            var historyFileItem = new FileItem();
            var parameter = new TransferParameter();
            if (DownloadHistory.Exists(remotefile, ref historyFileItem, ref historyApply, ref parameter, false))
            {
                historyFileItem.EndTime = DateTime.Now;
                historyFileItem.IsCompleted = true;
                DownloadHistory.Update(remotefile,historyFileItem);
            }
        }
	}
}
