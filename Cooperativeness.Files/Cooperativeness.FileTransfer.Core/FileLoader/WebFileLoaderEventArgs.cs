using System;
using System.Runtime.InteropServices;

namespace Cooperativeness.FileTransfer.Core
{
	/// <summary>
	/// WebFileLoaderEventArgs 的摘要说明。
	/// </summary>
	
	[ComVisible(false)]
	public class ProgressEventArgs : EventArgs
	{
		public readonly int Rate;
		public readonly long Length;
		public readonly long Completedsize;
		
		public ProgressEventArgs(int rate, long length, long completedsize)
		{
            Rate = rate;
            Length = length;
            Completedsize = completedsize;
		}
	}
	
	[ComVisible(false)]
	public class ExceptionEventArgs : EventArgs
	{
		public readonly string Message;
		public ExceptionEventArgs(string message)
		{
            this.Message = message;
		}
	}

	[ComVisible(false)]
	public class BeginingEventArgs : EventArgs
	{
		public readonly string FileName;
		public readonly bool IsDownload;
		public BeginingEventArgs(string fileName, bool isDownload)	
		{
            this.FileName = fileName;
            this.IsDownload = isDownload;
		}
	}

	[ComVisible(false)]
	public class CompletedEventArgs : EventArgs
	{
		public readonly string FileName;
		public CompletedEventArgs(string fileName)
		{
            this.FileName = fileName;
		}
	}
}
