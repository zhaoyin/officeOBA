using System.Net;
using System.Runtime.InteropServices;

namespace Cooperativeness.FileTransfer.Core
{
    /// <summary>
    /// 
    /// </summary>
    [Guid("0214E951-2EBE-43ba-AD18-751BEA9380BC"), ClassInterface(ClassInterfaceType.None)]
    public class WebEnvironment : IWebEnvironment
    {
/*
        public WebEnvironment()
        {
        }
 */

        public bool UseAuthentication
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public bool UseProxy
        {
            get;
            set;
        }

        public string ProxyUrl
        {
            get;
            set;
        }

        public string ProxyBypass
        {
            get;
            set;
        }

        public string ProxyUsername
        {
            get;
            set;
        }

        public string ProxyPassword
        {
            get;
            set;
        }

        public string ProxyDomain
        {
            get;
            set;
        }

        public ICredentials ProxyCredentials
        {
            get { return new NetworkCredential(ProxyUsername, ProxyPassword);}
        }

        public bool UseIEProxy
        {
            get;
            set;
        }

        public IWebProxy WebProxy
        {
            get { return GetProxy(); }
        }

        public ICredentials Credentials
        {
            get { return GetCredential(); }
        }

        private IWebProxy GetProxy()
        {
            if (UseIEProxy)
            {
                return WebRequest.GetSystemWebProxy();
            }
            if (UseProxy)
            {
                if (ProxyUrl != "")
                {
                    IWebProxy result = new WebProxy(ProxyUrl);
                    if (ProxyUsername != "" && ProxyPassword != "" && UseAuthentication)
                    {
                        result.Credentials = GetWebProxyCredential();
                    }
                    return result;
                }
                return WebRequest.DefaultWebProxy;
            }
            return WebRequest.GetSystemWebProxy();
        }

        private ICredentials GetWebProxyCredential()
        {
            if (ProxyDomain != "")
            {
                return new NetworkCredential(ProxyUsername, ProxyPassword, ProxyDomain);
            }
            return new NetworkCredential(ProxyUsername, ProxyPassword);
        }

        private ICredentials GetCredential()
        {
            if (UseAuthentication)
            {
				return new NetworkCredential(Username, Password,ProxyDomain);
            }
            return null;
        }

        public override string ToString()
        {
            return string.Format("cUsername:{0},cPassword:{1},cProxyUrl:{2},cProxyBypass:{3}," +
                                 "cProxyUsername:{4},cProxyPassword:{5},cProxyDomain:{6}," +
                                 "useAuthentication:{7},useProxy:{8},useIEProxy:{9}",
                                 Username, Password, ProxyUrl,ProxyBypass, ProxyUsername,
                                 ProxyPassword,ProxyDomain,UseAuthentication, UseProxy, UseIEProxy);
        }
    }
}