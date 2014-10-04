using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Cooperativeness.FileTransfer.Uploader
{
	/// <summary>
	/// 
	/// </summary>
	internal class UploadStream
	{
		private MemoryStream postStream;
		private BinaryWriter postData;
		private string cMultiPartBoundary;
		private Encoding encoding;

		public MemoryStream PostStream
		{
			get { return postStream; }
			set { postStream = value; }
		}

		public string MultiPartBoundary
		{
			get { return cMultiPartBoundary; }
		}

		public UploadStream()
		{
			postStream = new MemoryStream();
			postData = new BinaryWriter(postStream);
			cMultiPartBoundary = GetTimeStampText();
			encoding = Encoding.GetEncoding("gb2312");
		}

		public void Close()
		{
			postData.Close();
			postStream.Close();
			postData = null;
			postStream = null;
		}

		public void AddPostKey(string key, string valueStr)
		{
			Debug.Assert(postData != null);

			postData.Write(encoding.GetBytes(
			               	"--" + cMultiPartBoundary + "\r\n" +
			               	"Content-Disposition: form-data; name=\"" + key + "\"\r\n\r\n"));

			postData.Write(encoding.GetBytes(valueStr));

			postData.Write(encoding.GetBytes("\r\n"));
		}

		public void AddPostFileStream(string fileName, byte[] fileBuffer, int len)
		{
			Debug.Assert(postData != null);
			string Key = "myfile";

			postData.Write(encoding.GetBytes(
			               	"--" + cMultiPartBoundary + "\r\n" +
			               	"Content-Disposition: form-data; name=\"" + Key + "\" filename=\"" +
			               	fileName + "\"\r\n"));

			postData.Write(encoding.GetBytes("Content-Type: application/octet-stream\r\n\r\n"));

			postData.Write(fileBuffer, 0, len);

			postData.Write(encoding.GetBytes("\r\n"));

			postData.Write(encoding.GetBytes("--" + cMultiPartBoundary /*+ "--"*/+ "\r\n"));
		}

		private static string GetTimeStampText()
		{
			DateTime nowTime = DateTime.Now;
			long timeTicks = nowTime.Ticks;
			return "---------------------" + timeTicks.ToString("x");
		}
	}
}