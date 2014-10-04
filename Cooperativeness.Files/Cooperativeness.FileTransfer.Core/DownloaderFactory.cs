using System;
using Cooperativeness.FileTransfer.Downloader;

namespace Cooperativeness.FileTransfer.Core
{
	public class DownloaderFactory
	{
		public static WebDownloader GetDownloader(TransferParameter parameter)
		{
            var uri = new Uri(parameter.TransferUrl);
		    switch (uri.Scheme)
		    {
		        case "http":
                    if(parameter.SupportBrokenResume)
                        return new BlockDownloader(parameter);
                    return new HttpDownloader(parameter);
                case "ftp":
                    return new FtpDownloader(parameter);
                default:
		            return null;
		    }
		}
	}
}
