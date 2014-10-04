using System.Net;

namespace Cooperativeness.FileTransfer.Core
{
	public interface IWebEnvironment
	{
		bool UseAuthentication { get; set; }

		string Username { get; set; }

		string Password { get; set; }

		bool UseProxy { get; set; }
		
		/// <summary>
		/// Address of the Proxy Server to be used.
		/// Use optional DEFAULTPROXY value to specify that you want to IE's Proxy Settings
		/// 指定的是代理的URL，其中是包括port的值的如 "192.168.10.108:8080"
		/// </summary>
		string ProxyUrl { get; set; }

		/// <summary>
		/// Semicolon separated Address list of the servers the proxy is not used for.
		/// </summary>
		string ProxyBypass { get; set; }

		/// <summary>
		/// Username for a password validating Proxy. Only used if the proxy info is set.
		/// </summary>
		string ProxyUsername { get; set; }

		/// <summary>
		/// Password for a password validating Proxy. Only used if the proxy info is set.
		/// </summary>
		string ProxyPassword { get; set; }

        ICredentials ProxyCredentials { get; }

		string ProxyDomain { get; set; }

		bool UseIEProxy { get; set; }

		IWebProxy WebProxy { get; }

		ICredentials Credentials { get; }
	}
}