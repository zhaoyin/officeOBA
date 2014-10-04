using System;
using System.Net;
using System.Threading;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer.Downloader
{
    public delegate long ThreadProcDelegate(FileState state, short blockIndex);

    internal class ThreadParameters
	{
		protected FileState _fileState;
		protected short		_blockIndex;
		protected ThreadProcDelegate _threaddelegate;
		protected Thread	_thread;

        private static readonly Logger Log = new Logger(typeof(ThreadParameters));

		public ThreadParameters() : this(null, 0, null)
		{	
		}

		public Thread AssistThread
		{
			get { return _thread; }
			set
			{
				_thread = value;
				
				if (_fileState != null)
				{
					BlockState bs = _fileState.GetBlock(_blockIndex);
					if (bs != null)
						bs.AssistThread = _thread;
				}
			}
		}

		public ThreadParameters(FileState state, short blockIndex, ThreadProcDelegate threaddelegate)
		{
			_fileState = state;
			_blockIndex = blockIndex;
			_threaddelegate = threaddelegate;
		}

		public FileState FileStatus
		{
			get { return _fileState;  }
			set { _fileState = value; }
		}

		public short BlockIndex
		{
			get { return _blockIndex;  }
			set { _blockIndex = value; }
		}

		public void ThreadProc()
		{
			if (_threaddelegate != null)
			{
				try
				{
					int ncount = 0;
					do
					{
						ncount++;
						long ret = _threaddelegate(FileStatus, BlockIndex);
						if (ret == 0 || ncount >50)
							break;
						if (ret == -2)
						{
                            const string msgStr = "ÍøÂçÁ¬½Ó³ö´í";
							_fileState.FireDownErrorHandlerEvent(msgStr);
							return;
						}
						Log.Debug("[{0}]Rective Data Error! 5 Seconds Try again, Try{1}", BlockIndex, ncount);
						Thread.Sleep(5000);
					}while(true);
					
				}
				catch(WebException e)
				{
					_fileState.FireDownErrorHandlerEvent(e.Message);
				}
				catch(Exception e)
				{
					Log.Warn("fatel: /" + e.Message);
				}
			}
		}
	}
}
