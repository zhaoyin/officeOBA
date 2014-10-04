using System;
using Cooperativeness.FileTransfer.Uploader;

namespace Cooperativeness.FileTransfer.Core
{
    public class UploaderFactory
    {
        private static readonly Logger Log = new Logger(typeof(HttpWebUploader));

        public static WebUploader GetWebUploader(TransferParameter parameter)
        {
            try
            {
                WebUploader uploader = null;
                Log.Debug("WebUploader  create: url=" + parameter.TransferUrl + ",remoteFile=" + parameter.RemoteFile + ",localFile=" + parameter.LocalFile + ",chunkCount=" + parameter.ChunkSize.ToString());
                var uri = new Uri(parameter.TransferUrl);
                Log.Debug("WebUploader uri.Scheme=" + uri.Scheme);
                switch (uri.Scheme)
                {
                    case "http":
                        uploader = new HttpWebUploader(parameter);
                        break;
                    case "ftp":
                        uploader = new FtpWebUploader(parameter);
                        break;
                }
                return uploader;
            }
            catch (ArgumentNullException e)
            {
                Log.Warn(e);
                return null;
            }
            catch (UriFormatException e)
            {
                Log.Warn(e);
                return null;
            }
        }
    }
}
