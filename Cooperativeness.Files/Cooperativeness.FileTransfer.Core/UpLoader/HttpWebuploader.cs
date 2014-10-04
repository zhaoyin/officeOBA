using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer.Uploader
{
    /// <summary>
    /// XPlugin平台HTTP协议的Web上传服务组件
    /// </summary>
    internal class HttpWebUploader : WebUploader
    {
        private static readonly Logger Log = new Logger(typeof(HttpWebUploader));

        private readonly string cUserAgent = "XPlugin HTTP Web C#.NET Uploader Component";

        private HttpWebResponse _oWebResponse;
        private HttpWebRequest _oWebRequest;
        private int _chunkSize = 2097152;  //默认分块大小为2M
        //private static string pageUrl = @"/UFFileManagerServices/WebUploadServices.aspx";
		private long _fileSize ;


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="parameter"></param>
        public HttpWebUploader(TransferParameter parameter)
        {
            Parameter = parameter;
            _chunkSize = (parameter.ChunkSize > 0 ? parameter.ChunkSize : 2) * 1024 * 1024;

            Log.Debug("Create HttpWebUploader: url=" + parameter.TransferUrl + ",remoteFile=" + parameter.RemoteFile + ",localFile=" + parameter.LocalFile + ",chunkCount=" + _chunkSize.ToString());
		}

        /// <summary>
        /// 传输文件块
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="buff"></param>
        /// <param name="buffLen"></param>
        /// <param name="offset"></param>
        /// <param name="keyLen"></param>
        /// <returns></returns>
		private bool PostFileChunk(string fileName, byte[] buff, int buffLen, long offset, long keyLen)
		{
			StreamReader loResponseStream = null;
			Stream loPostData = null;
			UploadStream uploadStream = null;
			try
			{
			    _oWebRequest = (HttpWebRequest) WebRequest.Create(Parameter.TransferUrl);
			    _oWebRequest.Timeout = 1000*60*60; //服务超时设为1小时

			    Log.Debug("WebRequest create url: " + Parameter.TransferUrl);

			    _oWebRequest.UserAgent = cUserAgent;

			    if (Parameter.Environment != null)
			    {
			        _oWebRequest.Proxy = Parameter.Environment.WebProxy;
			        _oWebRequest.Proxy.Credentials = Parameter.Environment.ProxyCredentials;
			    }

			    uploadStream = new UploadStream();

			    uploadStream.AddPostKey("offset", offset.ToString());
			    uploadStream.AddPostKey("length", keyLen.ToString());
			    if (!string.IsNullOrEmpty(Parameter.FileDirectory))
			    {
			        uploadStream.AddPostKey("filedirectory", Parameter.FileDirectory);
			    }

			    string cMultiPartBoundary = uploadStream.MultiPartBoundary;

			    _oWebRequest.ContentType = "multipart/form-data; boundary=" + cMultiPartBoundary;

			    _oWebRequest.Method = "POST";


			    uploadStream.AddPostFileStream(fileName, buff, buffLen);
			    _oWebRequest.ContentLength = uploadStream.PostStream.Length;

			    loPostData = _oWebRequest.GetRequestStream();

			    uploadStream.PostStream.WriteTo(loPostData);

			    Log.Debug("begin WebRequest.GetResponse");

			    _oWebResponse = (HttpWebResponse) _oWebRequest.GetResponse();

			    Encoding enc = Encoding.GetEncoding("gb2312");

			    var responseStream = _oWebResponse.GetResponseStream();
			    if (responseStream != null)
			    {
			        loResponseStream = new StreamReader(responseStream, enc);

			        string lcHtml = loResponseStream.ReadLine();

			        Log.Debug("WebResponse: " + lcHtml);
			        if (!string.IsNullOrEmpty(lcHtml))
			        {
			            string[] results = lcHtml.Split(' ');
			            if (results[0] == "OK")
			            {
			                long num = (long.Parse(results[2]));
			                var rate = (int) (num*100/_fileSize);
			                OnProgress(rate, num, _fileSize);
			                return true;
			            }
			        }

			    }

			    return false;
			}
			catch (WebException e)
			{
                Log.Error(e);
                OnExcetipion(e.Message);
			    var res = (HttpWebResponse) e.Response;
                if (res.GetResponseStream() != null)
                {
                    var sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("gb2312"));
                    Log.Error(sr.ReadToEnd());
                }
			    return false;
			}
			catch (Exception e)
			{
			    Log.Error(e);
                OnExcetipion(e.Message);
			    return false;
			}
			finally
			{
				if (_oWebResponse != null)
				{
                    _oWebResponse.Close();
                    _oWebResponse = null;
				}
				if (loResponseStream != null)
				{
					loResponseStream.Close();
				}
				if (loPostData != null)
				{
					loPostData.Close();
				}
				if (uploadStream != null)
				{
					uploadStream.Close();
				}
			}
		}

		protected override void UploadFile()
		{
			UploadFile(Parameter.RemoteFile, Parameter.LocalFile);
		}

		private void UploadFile(string remoteDestName, string localFileName)
		{
			FileStream loFile = null;
			try
			{
                byte[] lcFileBuffer;
                OnBegining(localFileName);
				loFile = new FileStream(localFileName, FileMode.Open, FileAccess.Read);

				_fileSize = loFile.Length;
                //如果文件较小则不分块
				if (_fileSize <= _chunkSize)
				{
                    lcFileBuffer = new byte[_fileSize];
                    loFile.Read(lcFileBuffer, 0, (int)_fileSize);
					loFile.Close();
				    loFile = null;

					IsCompleted = PostFileChunk(remoteDestName, lcFileBuffer, lcFileBuffer.Length, 0, 0);
				}
				else
				{
                    long chunkNum = (_fileSize + (_chunkSize - 1)) / _chunkSize;
					var flags = new bool[chunkNum];
                    lcFileBuffer = new byte[_chunkSize];
					for (int i = 0; i < chunkNum; i++)
					{
                        int readSize = loFile.Read(lcFileBuffer, 0, _chunkSize);
                        flags[i] = PostFileChunk(remoteDestName, lcFileBuffer, readSize, (long)i * _chunkSize, _fileSize);
					}
					foreach (bool flag in flags)
					{
						if (flag == false)
						{
                            IsCompleted = false;
							break;
						}
                        IsCompleted = true;
					}
				}
                if (IsCompleted)
				{
					OnCompleted(remoteDestName);
				}
			}
			catch (ThreadAbortException e)
			{
                Log.Error(e);
                OnExcetipion(e.Message);
			}
			catch (Exception e)
			{
				Log.Error(e);
				OnExcetipion(e.Message);
			}
			finally
			{
				if (loFile != null)
				{
					loFile.Close();
				}
			}
		}
	}
}