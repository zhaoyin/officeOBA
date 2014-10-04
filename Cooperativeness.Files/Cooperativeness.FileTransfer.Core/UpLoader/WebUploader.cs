using System;
using System.Threading;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer.Uploader
{
    public abstract class WebUploader
    {
        
        protected Thread T;

        protected TransferParameter Parameter;

        public bool HasException 
        { 
            get; internal set;
        }

        public Exception InnerException
        {
            get; internal set;
        }

        public bool IsCompleted
        {
            get;
            internal set;
        }

        protected abstract void UploadFile();

        public event BeginingEventHandler Begining;

        public event ProgressEventHandle Progress;

        public event CompletedEventHandler Completed;

        public event ExceptionEventHandle ExceptionError;

        protected virtual void OnProgress(int rate, long length, long completedsize)
        {
            if (Progress != null)
            {
                var e = new ProgressEventArgs(rate, length, completedsize);
                Progress(this, e);
            }
        }

        protected virtual void OnCompleted(string fileName)
        {
            if (Completed != null)
            {
                Completed(this, new CompletedEventArgs(fileName));
            }
        }

        protected virtual void OnExcetipion(string message)
        {
            HasException = true;
            InnerException = new ApplicationException(message);
            if (ExceptionError != null)
			{
				ExceptionError(this, new ExceptionEventArgs(message));
			}
		}

		protected virtual void OnBegining(string fileName)
		{
			if (Begining != null)
			{
				Begining(this, new BeginingEventArgs(fileName, false));
			}
		}

        public virtual void Start()
		{
			var ts = new ThreadStart(UploadFile);
			T = new Thread(ts);
			T.Start();
		}

		public virtual void Join()
		{
			T.Join();
		}

        public virtual void Cancel()
		{
			T.Abort();
		}
	}
}