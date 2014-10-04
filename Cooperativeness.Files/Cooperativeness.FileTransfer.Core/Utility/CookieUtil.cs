using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Cooperativeness.FileTransfer.Core
{
	public class CookieUtil
	{
		[DllImport("wininet.dll")]
		public static extern bool InternetGetCookie(
										string url, 
										string cookieName, 
										StringBuilder cookieData, 
										ref uint size);

		public static string GetCookies(string uri,string cookieName)
		{
			string cookie = "";
			uint   size   = 1024;
			var build = new StringBuilder((int)size);
			if (InternetGetCookie(uri,cookieName, build,ref size))
				cookie = build.ToString();
			
			return cookie;
		}

		public static CookieContainer GetCokieContainer(string uri)
		{
			if (uri == "") 
				return null;
			return GetCokieContainer(new Uri(uri));
		}

		public static CookieContainer GetCokieContainer(Uri uri)
		{
			string cookiestring = "";
			if (uri == null) 
				return null;
			
			var cct = new CookieContainer();
			cookiestring = GetCookies(uri.ToString(), "");

			string[] result = cookiestring.Split(new char[] {';'});

			foreach(string s in result)
			{
				string singleline = s.Trim();
				string[] split = singleline.Split(new char[] {'='});

				if (split.Length == 2)
				{
					cct.Add(uri, new Cookie(split[0].Trim(), split[1].Trim()));
				}
			}
			
			return cct;
		}
	}
}