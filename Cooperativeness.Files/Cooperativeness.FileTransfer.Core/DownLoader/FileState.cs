using System;
using System.Collections;
using System.IO;
using System.Threading;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer.Downloader
{
    public delegate void ProgressDelegate(int rate, long length, long completedsize);

    public delegate void FileDownCompletedDelegate(string fileName);

    public delegate void FileDownBeginingDelegate(string fileName);

    public delegate void FileDownErrorHandlerDelegate(string errorMsg);

    /// <summary> 
    /// 每个文件的信息类,包含远程和本地信息,
    /// 该文件被分成几块在下载,还有每块的信息
    /// </summary> 
    [Serializable]
    public class FileState
    {
        private static readonly Logger Log = new Logger(typeof(FileState));
        private Uri rfinfo; // 远程服务器上文件的信息
        private Uri lfinfo; // 本地文件信息
        private int filesize; // 整个文件的总大小,如果服务器上文件
        // 使用Chunk编码的时候,此值为动态变化的值
        private int transmittedsize; // 整个文件已经下载或上传的字节总数
        private short blockCount; // 文件分成多少块
        private ArrayList blockstates; // 块信息数组
        private int minisegmentsize; // 每个分块的分块的大小
        private int completedrate; // 完成率
        private ThreadProcDelegate _callbackDelegate;
        private bool isAlreadyStop; //
        private bool isCanceled = false;

        private const int BLOCKCOUNT = 1; // 缺省只使用一块来下载信息
        private const int MINISEGMENT = 256 * 1024; // 缺省最小分块大小为256KBytes
        private Timer timer = null;

        public ProgressDelegate FileDownProgressDelegate = null;
        public FileDownCompletedDelegate DownFileCompletedDelegate = null;
        public FileDownErrorHandlerDelegate DownErrorHandlerDelegate = null;
        public FileDownBeginingDelegate DownFileBeginingDelegate = null;

        //		public bool IsCanceled
        //		{
        //			get { return this.isCanceled; }
        //			set { this.isCanceled = value; }
        //		}

        public FileState()
            : this("", "", 0, 0, BLOCKCOUNT)
        {
        }

        public FileState(Uri remoteInfo, Uri localInfo)
            :
                this(remoteInfo.ToString(), localInfo.ToString(), 0, 0, BLOCKCOUNT)
        {
        }

        public FileState(string remoteInfo, string localInfo)
            :
                this(remoteInfo, localInfo, 0, 0, BLOCKCOUNT)
        {
        }

        public FileState(string remoteInfo, string localInfo, short blocks)
            :
                this(remoteInfo, localInfo, 0, 0, blocks)
        {
        }

        public FileState(string remoteInfo, string localInfo, int fileLength, short blocks)
            :
                this(remoteInfo, localInfo, fileLength, 0, blocks)
        {
        }

        public FileState(string remoteInfo,
                         string localInfo,
                         int fileLength,
                         int transmittedLength,
                         short blocks)
        {
            rfinfo = new Uri(remoteInfo);
            lfinfo = new Uri(localInfo);

            filesize = fileLength;
            transmittedsize = transmittedLength;
            minisegmentsize = MINISEGMENT;

            completedrate = GetCompletedRate();
            isAlreadyStop = false;

            SetBlocks(blocks);
        }

        public bool IsLegalBlockIndex(short blockIndex)
        {
            return ((blockstates != null) && (blockIndex >= 0 && blockIndex < blockstates.Count));
        }

        public BlockState GetBlock(short blockIndex)
        {
            if (!IsLegalBlockIndex(blockIndex))
            {
                Log.Debug("Invalid blockIndex!");
                return null;
            }

            return (BlockState)blockstates[blockIndex];
        }

        private int GetCompletedRate()
        {
            int value;
            if (filesize == 0)
                return 0;

            lock (this)
            {
                value = (int)((long)transmittedsize * 100 / filesize);
            }

            if (IsCompleted)
            {
                DeleteLogFile();
                return 100;
            }
            return value;
        }

        public int ProgressRate
        {
            get { return GetCompletedRate(); }
        }

        public short BlockCount
        {
            get { return blockCount; }
        }


        public Uri RemoteFileUri
        {
            get { return rfinfo; }
        }

        public Uri LocalFileUri
        {
            get { return lfinfo; }
        }

        public int FileLength
        {
            get { return filesize; }
            set { filesize = value; }
        }

        public void FireFileDownloadBeginingEvent()
        {
            if (DownFileBeginingDelegate != null)
            {
                DownFileBeginingDelegate(lfinfo.LocalPath);
            }
            if (timer == null)
            {
                timer = new Timer(new TimerCallback(CheckCompleted), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
                Log.Debug("timer is Created with {0}", timer != null);
            }
            else
            {
                Log.Warn("Can't go to here!");
            }
            Log.Info("File down is begining!");
        }

        private void CheckCompleted(object state)
        {
            if (isCanceled)
            {
                if (timer != null)
                {
                    timer.Change(-1, -1);
                    timer.Dispose();
                    timer = null;
                    Log.Info("Timer disponse by canceled!");
                }
                DeleteLogFile();
            }
            else if (InternalIsCompleted())
            {
                if (timer != null)
                {
                    timer.Change(-1, -1);
                    timer.Dispose();
                    timer = null;
                    Log.Info("Timer disponse by down complete!");
                }
                CanFireDownloadCompletedEvent();
            }
        }

        private void FireFileDownloadProgressEvent(int rate, int count, int translatesized)
        {
            if (FileDownProgressDelegate != null)
                FileDownProgressDelegate(rate, count, translatesized);
            //LogUtil.Debug("Download rate: {0}, FileSize : {1}, TranslatedSize : {2}",
            //              rate, count, translatesized);
        }

        private void FireFileDownloadCompletedEvent()
        {
            if (timer != null)
            {
                timer.Change(-1, -1);
                timer.Dispose();
                timer = null;
            }
            Log.Info("File down is completed!");
            if (DownFileCompletedDelegate != null)
                DownFileCompletedDelegate(new FileInfo(rfinfo.LocalPath).Name);
        }

        public void FireDownErrorHandlerEvent(string message)
        {
            if (timer != null)
            {
                timer.Change(-1, -1);
                timer.Dispose();
                timer = null;
                Log.Info("Timer disposed by exception!");
            }
            if (DownErrorHandlerDelegate != null)
            {
                DownErrorHandlerDelegate(message);
            }
            SaveLogFile();
            Log.Info("There is exception throwing : {0}", message);
        }

        public void CanFireDownloadCompletedEvent()
        {
            try
            {
                Log.Info("CanFireDownloadCompletedEvent--IsCompleted:"+IsCompleted+"--IsAlreadyStop:"+isAlreadyStop);
                lock (this)
                {
                    //				if (isCanceled)
                    //				{
                    //					DeleteLogFile();
                    //					return;
                    //				}
                    if (IsCompleted && isAlreadyStop == false)
                    {
                        Log.Debug("开始触发完成事件");
                        DeleteLogFile();
                        FireFileDownloadCompletedEvent();
                        isAlreadyStop = true;
                        Log.Debug("完成下载！");
                    }
                    else
                    {
                        if (isAlreadyStop)
                        {
                            isAlreadyStop = true;
                        }
                        else
                        {
                            SaveLogFile();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warn("CanFireDownloadCompletedEvent----"+e.Message);
            }

        }


        public int TransmittedLength
        {
            get
            {
                lock (this)
                {
                    return transmittedsize;
                }
            }
            set
            {
                int tt;

                lock (this)
                {
                    tt = transmittedsize = value;
                    completedrate = GetCompletedRate();
                }

                FireFileDownloadProgressEvent(completedrate, FileLength, tt);
            }
        }

        public bool IsCompleted
        {
            get
            {
                //				lock(this)
                {
                    return (transmittedsize >= filesize && filesize > 0);
                }
            }
        }

        public int MiniSegmentLength
        {
            get { return minisegmentsize; }
            set
            {
                if (value < 10 * 1024)
                    minisegmentsize = 10 * 1024; // 如果少于10KBytes则使用每块最小10KBytes
                else
                    minisegmentsize = value;
            }
        }


        private bool InternalIsCompleted()
        {
            foreach (BlockState bs in blockstates)
            {
                if (bs.AssistThread != null && bs.AssistThread.IsAlive)
                {
                    return false;
                }
            }
            int total = 0;
            foreach (BlockState bs in blockstates)
            {
                total += bs.TransmittedSize;
            }
            return (total >= filesize && filesize > 0);
        }

        protected void SetBlocks(short blocks)
        {
            if (blocks <= 0 || blocks > 64)
                blocks = 1;

            short count = 0;
            int fileCountbytes = FileLength;
            int perblocksize = (FileLength + (blocks - FileLength % blocks)) / blocks;
            int actuallysize;

            if (perblocksize < MiniSegmentLength)
                perblocksize = MiniSegmentLength;

            Log.Debug("- Perblocksize:{0}", perblocksize);

            blockstates = new ArrayList();
            while (fileCountbytes > 0)
            {
                actuallysize = (fileCountbytes > perblocksize ? perblocksize : fileCountbytes);

                var bs = new BlockState(count * perblocksize, actuallysize, 0);
                blockstates.Add(bs);

                fileCountbytes -= actuallysize;
                count++;
            }
            blockCount = count;
        }

        public void Stop(short blockIndex)
        {
            for (short i = 0; i < BlockCount; i++)
            {
                BlockState bs = GetBlock(i);
                Thread t = bs.AssistThread;
                try
                {
                    bs.BlockRunState = ActionStateType.Stop;
                    if (t.ThreadState == ThreadState.Suspended ||
                        t.ThreadState == ThreadState.SuspendRequested ||
                        (int)t.ThreadState == 96)
                    {
                        t.Resume();
                    }
                    t.Abort();
                }
                catch (ThreadStateException e)
                {
                    Log.Warn(e.Message);
                }
                isCanceled = true;
            }
        }

        public void Pause(short blockIndex)
        {
            for (short i = 0; i < BlockCount; i++)
            {
                BlockState bs = GetBlock(i);
                Thread t = bs.AssistThread;
                try
                {
                    t.Suspend();
                }
                catch (ThreadStateException e)
                {
                    Log.Warn(e.Message);
                }
            }
        }

        public void Resume(short blockIndex)
        {
            for (short i = 0; i < BlockCount; i++)
            {
                BlockState bs = GetBlock(i);
                Thread t = bs.AssistThread;
                try
                {
                    t.Resume();
                }
                catch (ThreadStateException e)
                {
                    Log.Warn(e.Message);
                }
            }
        }

        public void Join()
        {
            for (short i = 0; i < BlockCount; i++)
            {
                int j = 0;
                try
                {
                retry:
                    BlockState bs = GetBlock(i);
                    Thread t = bs.AssistThread;
                    if (t != null)
                    {
                        if (t.ThreadState != ThreadState.Unstarted)
                        {
                            t.Join();
                        }
                    }
                    else
                    {
                        if (j >= 3)
                        {
                            continue;
                        }
                        Thread.Sleep(1000);
                        j++;
                        goto retry;
                    }
                }
                catch (ThreadStateException e)
                {
                    Log.Warn(e.Message);
                }
                catch( Exception e)
                {
                    Log.Warn(e.Message);
                    Log.Debug(e.StackTrace);
                    throw;
                }
            }
        }

        protected void SetBlockState(short blockIndex, ActionStateType act)
        {
            bool ok = false;

            var list = new ArrayList();

            for (short i = 0; i < BlockCount; i++)
            {
                if (blockIndex != -1 && i == blockIndex)
                {
                    i = blockIndex;
                    ok = true;
                }

                var bs = GetBlock(i);
                if (bs != null)
                {
                    bs.BlockRunState = act;

                    if (bs.AssistThread != null && bs.AssistThread.IsAlive && act == ActionStateType.Stop)
                        list.Add(bs.AssistThread);
                }

                if (ok)
                    break;
            }

            foreach (var item in list)
            {
                var t = (Thread) item;
                try
                {
                    t.Abort();
                }
                catch (ThreadStateException)
                {
                }
            }
        }

        public void StartDownloadThreadProc()
        {
            try
            {
                if (File.Exists(LocalFileUri.LocalPath) == false)
                {
                    if (!FileUtil.CreateFile(LocalFileUri.LocalPath, FileLength))
                    {
                        return;
                    }
                }

                if (!File.Exists(LocalFileUri.LocalPath))
                {
                    return;
                }

                Log.Info("- Start BlockCount:{0}", BlockCount);
                string logFile = GetLogFileName();

                if (File.Exists(logFile))
                {
                    Load(logFile);
                    TransmittedLength = transmittedsize;
                }
                else
                {
                    Save(logFile);
                }

                for (int i = 0; i < BlockCount; i++)
                {
                    var ts = new ThreadParameters(this, (short) i, _callbackDelegate);
                    var t = new Thread(new ThreadStart(ts.ThreadProc));
                    ts.AssistThread = t;
                    t.Start();
                }
            }
            catch (Exception e)
            {
                Log.Warn("Get a exception :" + e.Message);
                DeleteLogFile();
                throw;
            }
        }

        private void SaveLogFile()
        {
            if (IsCompleted)
            {
                DeleteLogFile();
            }
            else
            {
                Save(GetLogFileName());
            }
        }

        private string GetLogFileName()
        {
            return lfinfo.LocalPath + ".dnl";
        }

        private void DeleteLogFile()
        {
            try
            {
                string logFile = GetLogFileName();
                if (File.Exists(logFile))
                {
                    File.Delete(logFile);
                    Log.Info("Deleted Log File: {0}", logFile);
                }
            }
            catch (IOException e)
            {
                Log.Warn(e.Message);
            }
            catch (Exception e)
            {
                Log.Warn(e.Message);
            }

        }

        public void Start(ThreadProcDelegate callbackDelegate)
        {
            if (callbackDelegate == null)
            {
                Log.Debug("- File!Start Pass an invalid Parameter!");
                return;
            }

            _callbackDelegate = callbackDelegate;
            var t = new Thread(new ThreadStart(StartDownloadThreadProc));
            t.Start();
        }

        private void Save(string fileName)
        {
            try
            {
                lock (this)
                {
                    Log.Info("Begeining Save Log File: {0}", fileName);
                    using (StreamWriter sw = File.CreateText(fileName))
                    {
                        sw.WriteLine(BlockCount);
                        sw.WriteLine(FileLength);
                        sw.WriteLine(MiniSegmentLength);
                        foreach (BlockState bs in blockstates)
                        {
                            sw.WriteLine(bs.Start);
                            sw.WriteLine(bs.Size);
                            sw.WriteLine(bs.TransmittedSize);
                        }
                        sw.Close();
                    }
                }
            }
            catch (IOException e)
            {
                Log.Warn(e.Message);
            }
            catch (Exception e)
            {
                Log.Warn(e.Message);
            }

        }

        private void Load(string fileName)
        {
            try
            {
                lock (this)
                {
                    Log.Info("Begeining Load Log File: {0}", fileName);
                    using (StreamReader sr = File.OpenText(fileName))
                    {
                        string str = sr.ReadLine();
                        str = str ?? "";
                        blockCount = short.Parse(str);
                        str = sr.ReadLine();
                        str = str ?? "";
                        filesize = int.Parse(str);
                        str = sr.ReadLine();
                        str = str ?? "";
                        minisegmentsize = int.Parse(str);
                        transmittedsize = 0;
                        blockstates = new ArrayList();
                        for (int i = 0; i < blockCount; i++)
                        {
                            var bs = new BlockState();
                            str = sr.ReadLine();
                            str = str ?? "";
                            bs.Start = int.Parse(str);
                            str = sr.ReadLine();
                            str = str ?? "";
                            bs.Size = int.Parse(str);
                            str = sr.ReadLine();
                            str = str ?? "";
                            bs.TransmittedSize = int.Parse(str);
                            transmittedsize += bs.TransmittedSize;
                            blockstates.Add(bs);
                        }
                        sr.Close();
                    }
                }
            }
            catch (IOException e)
            {
                Log.Warn(e.Message);
            }
            catch (Exception e)
            {
                Log.Warn(e.Message);
            }

        }
    }
}