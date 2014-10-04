using System.Net;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer.Downloader
{
    internal class BlockInfo
    {
        protected HttpWebRequest Request = null;
        protected string RemoteFile = "";
        public string FilePath
        {
            get
            {
                return Logger.TemporaryDirectory + "\\"+RemoteFile + BlockIndex.ToString() + ".dat";
            }
        }

        public int BlockIndex { get; set; }

        public int StartPosition { get; set; }

        public int BlockSize { get; set; }

        public int EndPosition
        {
            get
            {
                return StartPosition + BlockSize;
            }
        }

        public int CompletedPosition
        {
            get { return _CompletedPosition; }
        }
        protected int _CompletedPosition = 0;

        public int UnCompeleteSize
        {
            get { return BlockSize - CompletedPosition; }
        }

		public bool IsCompleted { get; set; }
    }
}
