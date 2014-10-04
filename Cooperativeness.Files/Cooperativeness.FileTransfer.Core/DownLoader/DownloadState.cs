using System.Collections;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer.Downloader
{
    internal class DownloadState
    {
        private const int MinBufferSize = 96 * 1024;
        private const int MaxBufferSize = 196 * 1024;

        public static uint BufferSize
        {
            get 
            { 
                return _bufferSize; 
            }
            set
            {
                if (value <= MinBufferSize || value >= MaxBufferSize) return;
                _bufferSize = value;
            }
        }
        private static uint _bufferSize = MinBufferSize;

        public static bool OutputDebugLog { get; set; }

        public static bool AllowAppend { get; set; }

        public static bool IsCompleted { get; set; }

        public static bool HasFinished { get; set; }

        public static bool StopDownload { get; set; }

        public static FileItem CurrentFileItem
        {
            get { return _mFileItem; } 
            set { _mFileItem = value; }
        }
        private static FileItem _mFileItem = new FileItem();

        public static Apply CurrentApply
        {
            get { return _currentApply; }
            set { _currentApply = value; }
        }
        private static Apply _currentApply = new Apply();

        public static ArrayList BlockList
        {
            get { return _blockList; }
            set { _blockList = value; }
        }
        private static ArrayList _blockList = new ArrayList();

        public static TransferParameter Parameter { get; set; }

    }
}
