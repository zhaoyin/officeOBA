using System;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer.Uploader
{
	internal class FtpWebUploader : WebUploader
	{
        private static readonly Logger Log = new Logger(typeof(FtpWebUploader));

		public FtpWebUploader(TransferParameter parameter)
		{
		    Parameter = parameter;

            Log.Debug("Create FtpWebUploader: url=" + parameter.TransferUrl + ",remoteFile=" + parameter.RemoteFile + ",localFile=" + parameter.LocalFile + ",chunkCount=" + parameter.ChunkSize.ToString());
		}

		protected override void UploadFile()
		{
			UploadFile(Parameter);
		}

		private void UploadFile(TransferParameter parameter)
		{
            FtpClient ftp = null;
            try
            {
                ftp = new FtpClient();
                var uri = new Uri(parameter.TransferUrl);
                ftp.Server = uri.Host;
                ftp.Port = uri.Port;
                ftp.Begining += new BeginingEventHandler(FtpBegining);
                ftp.Completed += new CompletedEventHandler(FtpCompleted);
                ftp.ExceptionError += new ExceptionEventHandle(FtpExceptionError);
                ftp.Progress += new ProgressEventHandle(FtpProgress);
                var env = parameter.Environment;
                if (env != null)
                {
                    if (env.Username != "")
                    {
                        ftp.Username = env.Username;
                    }
                    if (env.Password != "")
                    {
                        ftp.Password = env.Password;
                    }
                }
                ftp.Login();
                ftp.Upload(parameter.LocalFile, parameter.RemoteFile);
            }
            catch (Exception e)
            {
                OnExcetipion(e.Message);
            }
            finally
            {
                if (ftp != null)
                {
                    ftp.Close();
                }
            }
        }

		private void FtpExceptionError(object sender, ExceptionEventArgs args)
		{
			OnExcetipion(args.Message);
		}

		private void FtpCompleted(object sender, CompletedEventArgs args)
		{
			OnCompleted(args.FileName);
		}

		private void FtpBegining(object sender, BeginingEventArgs args)
		{
			OnBegining(args.FileName);
		}

		private void FtpProgress(object sender, ProgressEventArgs args)
		{
			OnProgress(args.Rate, args.Length, args.Completedsize);
		}
	}
}