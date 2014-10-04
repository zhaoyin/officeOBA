using System.Threading;

namespace Cooperativeness.FileTransfer.Downloader
{

    public enum ActionStateType
    {
        Stop = 0x0,
        Running = 0x1,
        Pause = 0x2,
    }

    /// <summary> 
    /// 分块信息类,该类说明了每个文件块的起始地址
    //  每块的长度,以及改块已经传输了多少数据等
    //  信息
    /// </summary> 

    public class BlockState
    {
        public BlockState()
        //            : this(0, 0, 0, ActionStateType.STOP, null)
        {
            Start = 0;
            Size = 0;
            TransmittedSize = 0;
            BlockRunState = ActionStateType.Stop;
            AssistThread = null;
        }

        public BlockState(int start, int size, int ts)
//            : this(start, size, ts, ActionStateType.STOP, null)
        {
            Start = start;
            Size = size;
            TransmittedSize = ts;
            BlockRunState = ActionStateType.Stop;
            AssistThread = null;
        }
/*
		public BlockState(int start, int size, int ts, ActionStateType ast, Thread thread)
		{
			Start	= start;
			Size	= size;
			TransmittedSize = ts;
			BlockRunState = ast;
			_thread = thread;
		}
*/
        /// <summary>
        /// 每块开始的地址,以字节长度表示
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// 每块的长度,以字节长度来表示
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 当前块已经下载完成的字节数
        /// </summary>
        public int TransmittedSize { get; set; }

		public int UnTransmittedSize
		{
			get { return Size - TransmittedSize; }
		}

		public int EnterPoint
		{
			get { return Start + TransmittedSize; }
		}

        /// <summary>
        /// 运行时的状态
        /// </summary>
        public ActionStateType BlockRunState { get; set; }

        /// <summary>
        /// 关连的线程
        /// </summary>
        public Thread AssistThread { get; set; }

		public virtual bool IsCompleted
		{
			get { return (TransmittedSize >= Size && Size > 0); }
		}
	}
}
