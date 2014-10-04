using System;
using System.Runtime.InteropServices;

namespace Cooperativeness.FileTransfer.Core
{
	[ComVisible(false)]
	public delegate void ProgressEventHandle(Object sender, ProgressEventArgs args);
	
	[ComVisible(false)]
	public delegate void ExceptionEventHandle(Object sender, ExceptionEventArgs args);
	
	[ComVisible(false)]
	public delegate void BeginingEventHandler(Object sender, BeginingEventArgs args);

	[ComVisible(false)]
	public delegate void CompletedEventHandler(Object sender, CompletedEventArgs args);
	
	[ComVisible(false)]
	public interface IWebFileLoader
	{
	}
}
