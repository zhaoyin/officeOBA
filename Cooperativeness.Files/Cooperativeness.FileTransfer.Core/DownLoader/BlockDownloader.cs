using System;
using System.IO;
using System.Net;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer.Downloader
{
    internal class BlockDownloader : WebDownloader
    {
        private short _chunkCount = 5;
        private static readonly Logger Log = new Logger(typeof(BlockDownloader));

        public BlockDownloader(TransferParameter parameter)
        {
            Parameter = parameter;
            _chunkCount = (parameter.ChunkCount > 0 ? parameter.ChunkCount : (short) 5);
            //CreateDownload(parameter);
            AssignFileStateDelegate();
            DownloadState.Parameter = parameter;
        }

        private void AssignFileStateDelegate()
        {
            //DownloadFileState.FileDownProgressDelegate = new ProgressDelegate(OnProgress);
            //DownloadFileState.DownFileCompletedDelegate = new FileDownCompletedDelegate(OnCompleted);
            //DownloadFileState.DownErrorHandlerDelegate = new FileDownErrorHandlerDelegate(OnExcetipion);
            //DownloadFileState.DownFileBeginingDelegate = new FileDownBeginingDelegate(OnBegining);

            BlockEvents.Begining = new FileDownBeginingDelegate(OnBegining);
            BlockEvents.Progress = new ProgressDelegate(OnProgress);
            BlockEvents.Completed = new FileDownCompletedDelegate(OnCompleted);
            BlockEvents.ExceptionError = new FileDownErrorHandlerDelegate(OnExcetipion);

        }

        protected override void DownloadFile()
        {
            DownloadFile(Parameter);
        }

        private void DownloadFile(TransferParameter parameter)
        {
            try
            {
                string logstr = "下载任务开始,正在创建临时目录.";
                Log.Debug(logstr);
                string[] tempList = Directory.GetDirectories(Logger.TemporaryDirectory);
                DirectoryInfo di = null;
                if (tempList.Length > 0)
                {
                    foreach (string item in tempList)
                    {
                        di = new DirectoryInfo(item);
                        if (di.LastWriteTime <= DateTime.Now.AddMonths(-6))
                        {
                            FileUtil.DeleteDirectory(item);
                        }
                    }
                }

                logstr = "正在创建本地保存目录.";
                Log.Debug(logstr);
                string localDir = Path.GetDirectoryName(parameter.LocalFile);
                if (!string.IsNullOrEmpty(localDir))
                {
                    di = new DirectoryInfo(localDir);
                    if (!di.Exists) di.Create();
                }

                logstr = "正在初始化...";
                Log.Debug(logstr + "\r\n" + parameter.ToString());
                DownloadState.BlockList.Clear();
                var currentApply = new Apply();
                DownloadState.BlockList = DownloadRequst.GetBlocks(parameter, ref currentApply);
                Log.Debug(currentApply.ToString());

                DownloadState.AllowAppend = false;
                DownloadState.StopDownload = false;
                DownloadState.HasFinished = false;
                DownloadState.IsCompleted = false;
                DownloadState.OutputDebugLog = true;
                BlockEvents.CompletedBytesCount = 0;
                BlockEvents.TotalBytes = currentApply.FileSize;
                ServicePointManager.DefaultConnectionLimit = 100;


                var historyApply = new Apply();
                var historyFileItem = new FileItem();
                var historyparameter = new TransferParameter();
                if (DownloadHistory.Exists(parameter.RemoteFile,ref historyFileItem, ref historyApply, ref historyparameter, false)
                    && DownloadState.CurrentApply.Equals(historyApply)
                    && !File.Exists(parameter.LocalFile))
                {
                    logstr = "已恢复下载.";
                    Log.Debug(logstr);
                    DownloadState.AllowAppend = true;
                    historyFileItem.HasBreaked = true;
                    if (string.IsNullOrEmpty(historyFileItem.BreakInfo))
                    {
                        historyFileItem.BreakInfo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        historyFileItem.BreakInfo += ";" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    DownloadHistory.Update(parameter.RemoteFile,historyFileItem);
                }
                else if (DownloadHistory.Exists(parameter.RemoteFile,ref historyFileItem, ref historyApply, ref historyparameter, true)
                         && parameter.Equals(historyparameter)
                         && File.Exists(historyparameter.LocalFile))
                {
                    var fileInfo = new FileInfo(parameter.LocalFile);
                    if (fileInfo.Length == DownloadState.CurrentApply.FileSize &&
                        fileInfo.LastWriteTime == DownloadState.CurrentApply.LastModified)
                    {
                        logstr = parameter.TransferUrl + "此前已完成下载.";
                        Log.Debug(logstr);
                        DownloadState.HasFinished = true;
                        BlockEvents.CompletedBytesCount = fileInfo.Length;
                        BlockEvents.OnProgress(100,100);
                        BlockEvents.OnCompleted();
                        return;
                    }
                }

                if (!DownloadState.AllowAppend && !DownloadState.HasFinished)
                {
                    DownloadHistory.Insert(parameter.RemoteFile);

                    logstr = "已将本次下载任务追加到历史.";
                    Log.Debug(logstr);
                }

                if (BlockEvents.Begining != null)
                {
                    BlockEvents.Begining(parameter.LocalFile);
                }

                logstr = "开始处理分块";
                Log.Warn(logstr);
                foreach (Blocks item in DownloadState.BlockList)
                {
                    item.ReStore();
                    item.Download();
                    logstr += item.ToString() + "\r\n";
                    Log.Debug(logstr);
                }
                Log.Info("文件下载完毕");
            }
            catch (Exception err)
            {
                BlockEvents.OnException(err);
            }
        }

        public override void Supend()
        {
            if (DownloadState.BlockList == null || DownloadState.BlockList.Count <= 0) return;
            foreach (Blocks item in DownloadState.BlockList)
            {
                item.Supend();
            }
        }

        public override void Resume()
        {
            if (DownloadState.BlockList == null || DownloadState.BlockList.Count <= 0) return;
            foreach (Blocks item in DownloadState.BlockList)
            {
                item.Resume();
            }
        }

        public override void Cancel()
        {
            if (DownloadState.BlockList == null || DownloadState.BlockList.Count <= 0) return;
            DownloadState.StopDownload = true;
        }

        public override void Join()
        {
            return;
        }

    }
}
