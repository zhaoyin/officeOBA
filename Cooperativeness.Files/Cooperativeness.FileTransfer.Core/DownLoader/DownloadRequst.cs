using System;
using System.Net;
using System.Text;
using System.Collections;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer.Downloader
{
    internal class DownloadRequst
    {
        private static readonly Logger Log = new Logger(typeof(DownloadRequst));

        public static HttpWebRequest GetWebRequest(Uri transferUri, IWebEnvironment env)
        {
            var request = (HttpWebRequest)WebRequest.Create(transferUri);

            if (env != null)
            {
                request.Credentials = env.Credentials;
                request.PreAuthenticate = true;
                request.Proxy = env.WebProxy;
            }

            request.CookieContainer = CookieUtil.GetCokieContainer(transferUri);
            request.KeepAlive = (request.Proxy != WebRequest.DefaultWebProxy);
            request.ProtocolVersion = HttpVersion.Version10;
            var currentPoint = request.ServicePoint;
            currentPoint.ConnectionLimit = 100;

            return request;
        }

        public static HttpWebRequest GetWebRequest(TransferParameter parameter)
        {
            var uri = new Uri(parameter.TransferUrl);
            return GetWebRequest(uri, parameter.Environment);
        }

        public static bool AcceptRanges(TransferParameter parameter,out long iFileSize, out DateTime oDateTime)
        {
            Log.Debug("AcceptRanges正在获取HttpWebRequest对象.");
            HttpWebRequest request = GetWebRequest(parameter);

            request.Accept = "*/*";
            request.AllowAutoRedirect = true;
            request.Method = "HEAD";

            var sb = new StringBuilder();
            sb.Append(string.Format(" GET {0} HTTP/{1}", parameter.TransferUrl, request.ProtocolVersion) + "\r\n");
            sb.Append(string.Format(" Host: {0}", request.Address.Host) + "\r\n");
            sb.Append(string.Format(" Accept: {0}", request.Accept) + "\r\n");
            sb.Append(string.Format(" User-Agent: {0}", request.UserAgent) + "\r\n");
            sb.Append(string.Format(" Referer: {0}", request.Referer) + "\r\n");
            sb.Append("AcceptRanges正在获取网络回应.");
            Log.Debug(sb.ToString());
            var response = (HttpWebResponse)request.GetResponse();

            if (response.ContentLength == -1 || response.ContentLength > 0x7fffffff)
            {
                iFileSize = 1024 * 1024 * 5;
            }
            else
            {
                iFileSize =response.ContentLength;
            }

            bool allowRangs = false;
            oDateTime = DateTime.MinValue;
            sb = new StringBuilder();
            for (int i = 0; i < response.Headers.Count; ++i)
            {
                if (String.Compare(response.Headers.Keys[i], "Accept-Ranges", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    allowRangs = true;
                }
                if (String.Compare(response.Headers.Keys[i], "Last-Modified", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    oDateTime = response.LastModified;
                }
                sb.Append(response.Headers.Keys[i] + ":" + response.Headers[response.Headers.Keys[i]] + "\r\n");
            }
            response.Close();
			request.Abort();
            Log.Debug(sb.ToString());
            return allowRangs;
        }

        private static int GetMaxChunks(TransferParameter parameter,long iFileSize)
        {
            const int k1 = 1024;

            int maxChunks = parameter.ChunkCount;
            if (iFileSize <= 100 * k1)
            {
                //小于100K
                maxChunks = 1;
            }
            else if (iFileSize <= 500 * k1)
            {
                //小于500K
                if (maxChunks >= 5) maxChunks = 5;
            }
            return maxChunks;
        }

        public static ArrayList GetBlocks(TransferParameter parameter,ref Apply oApply)
        {
            var blockList = new ArrayList();
            long fileSize = 0;
            var lastModified = DateTime.MinValue;
            bool allowRanges = AcceptRanges(parameter,out fileSize, out lastModified);
            oApply.FileSize = fileSize;
            oApply.LastModified = lastModified;
            oApply.AllowRanges = allowRanges;
            oApply.BlockSize = (int)fileSize;
            oApply.ActuallyChunks = 1;
            int maxChunks = GetMaxChunks(parameter,fileSize);

            if (allowRanges && maxChunks > 1)
            {
                var blocksize =(int)(fileSize / maxChunks + 1);
                long sizeCount = fileSize;
                int index = 0;
                while (sizeCount > 0)
                {
                    int actuallySize = 0;
                    actuallySize = (int)(sizeCount > blocksize ? blocksize : sizeCount);

                    var blcok = new Blocks(GetWebRequest(parameter),parameter.RemoteFile, index, index * blocksize, actuallySize);
                    blockList.Add(blcok);

                    sizeCount -= actuallySize;
                    index++;
                }
                oApply.BlockSize = blocksize;
                oApply.ActuallyChunks = index;
            }
            else
            {
                var blcok = new Blocks(GetWebRequest(parameter), parameter.RemoteFile,0, 0, (int)fileSize);
                blockList.Add(blcok);
            }
            return blockList;
        }
    }
}
