using System;
using System.DirectoryServices;
using System.Collections;
using System.Text.RegularExpressions;

namespace Cooperativeness.Files.Server.Install
{
    public class Iis6WebSite
    {
        #region UserName,Password,HostName的定义
        public static string HostName { get; set; }
        public static string UserName { get; set; }
        public static string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (UserName.Length <= 1)
                {
                    throw new ArgumentException("还没有指定好用户名。请先指定用户名");
                }
                _password = value;
            }
        }
        public static void RemoteConfig(string hostName, string userName, string password)
        {
            HostName = hostName;
            UserName = userName;
            Password = password;
        }
        private static string _password = "password";
        #endregion
        #region 根据路径构造Entry的方法
        /// <summary>
        /// 根据是否有用户名来判断是否是远程服务器。
        /// 然后再构造出不同的DirectoryEntry出来
        /// </summary>
        /// <param name="entPath">DirectoryEntry的路径</param>
        /// <returns>返回的是DirectoryEntry实例</returns>
        public static DirectoryEntry GetDirectoryEntry(string entPath)
        {
            if (UserName == null)
            {
                return new DirectoryEntry(entPath);
            }
            return new DirectoryEntry(entPath, HostName + "//" + UserName, Password, AuthenticationTypes.Secure);
        }
        #endregion
        #region 添加，删除网站的方法
        public static void CreateNewWebSite(string hostIp, string portNum, string descOfWebSite, string commentOfWebSite, string webPath)
        {
            if (!EnsureNewSiteEnavaible(hostIp + portNum + descOfWebSite))
            {
                throw new ArgumentNullException("已经有了这样的网站了。" + Environment.NewLine + hostIp + portNum + descOfWebSite);
            }

            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry rootEntry = GetDirectoryEntry(entPath);//取得iis路径
            string newSiteNum = GetNewWebSiteId(); //取得新网站ID
            DirectoryEntry newSiteEntry = rootEntry.Children.Add(newSiteNum, "IIsWebServer"); //增加站点
            newSiteEntry.CommitChanges();//保存对区域的更改(这里对站点的更改)
            newSiteEntry.Properties["ServerBindings"].Value = hostIp + portNum + descOfWebSite;
            newSiteEntry.Properties["ServerComment"].Value = commentOfWebSite;
            newSiteEntry.CommitChanges();
            DirectoryEntry vdEntry = newSiteEntry.Children.Add("root", "IIsWebVirtualDir");
            vdEntry.CommitChanges();
            vdEntry.Properties["Path"].Value = webPath;
            vdEntry.CommitChanges();

        }
        /// <summary>
        /// 删除一个网站。根据网站名称删除。
        /// </summary>
        /// <param name="siteName">网站名称</param>
        public static void DeleteWebSiteByName(string siteName)
        {
            string siteNum = GetWebSiteNum(siteName);
            string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", HostName, siteNum);
            DirectoryEntry siteEntry = GetDirectoryEntry(siteEntPath);
            string rootPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry rootEntry = GetDirectoryEntry(rootPath);
            rootEntry.Children.Remove(siteEntry);
            rootEntry.CommitChanges();
        }
        #endregion
        #region Start和Stop网站的方法
        public static void StartWebSite(string siteName)
        {
            string siteNum = GetWebSiteNum(siteName);
            string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", HostName, siteNum);
            DirectoryEntry siteEntry = GetDirectoryEntry(siteEntPath);
            siteEntry.Invoke("Start", new object[] { });
        }
        public static void StopWebSite(string siteName)
        {
            string siteNum = GetWebSiteNum(siteName);
            string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", HostName, siteNum);
            DirectoryEntry siteEntry = GetDirectoryEntry(siteEntPath);
            siteEntry.Invoke("Stop", new object[] { });
        }
        #endregion
        #region 确认网站是否相同
        /// <summary>
        /// 确定一个新的网站与现有的网站没有相同的。
        /// 这样防止将非法的数据存放到IIS里面去
        /// </summary>
        /// <param name="bindStr">网站邦定信息</param>
        /// <returns>真为可以创建，假为不可以创建</returns>
        public static bool EnsureNewSiteEnavaible(string bindStr)
        {
            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry ent = GetDirectoryEntry(entPath);
            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    if (child.Properties["ServerBindings"].Value != null)
                    {
                        if (child.Properties["ServerBindings"].Value.ToString() == bindStr)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        #endregion
        #region 获取一个网站编号//一个输入参数为站点描述
        /// <summary>
        /// 输入参数为 站点的描述名 默认是站点描述为 "默认网站"
        /// </summary>
        /// <exception cref="NotFoundWebSiteException">表示没有找到网站</exception>
        public static string GetWebSiteNum(string siteName)
        {
            var regex = new Regex(siteName);
            string tmpStr;
            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry ent = GetDirectoryEntry(entPath);
            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    if (child.Properties["ServerBindings"].Value != null)
                    {
                        tmpStr = child.Properties["ServerBindings"].Value.ToString();
                        if (regex.Match(tmpStr).Success)
                        {
                            return child.Name;
                        }
                    }
                    if (child.Properties["ServerComment"].Value != null)
                    {
                        tmpStr = child.Properties["ServerComment"].Value.ToString();
                        if (regex.Match(tmpStr).Success)
                        {
                            return child.Name;
                        }
                    }
                }
            }
            throw new Exception("没有找到我们想要的站点" + siteName);
        }
        #endregion
        #region 获取新网站id的方法
        /// <summary>
        /// 获取网站系统里面可以使用的最小的ID。
        /// 这是因为每个网站都需要有一个唯一的编号，而且这个编号越小越好。
        /// 这里面的算法经过了测试是没有问题的。
        /// </summary>
        /// <returns>最小的id</returns>
        public static string GetNewWebSiteId()
        {
            var list = new ArrayList();
            string tmpStr;
            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry ent = GetDirectoryEntry(entPath);
            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    tmpStr = child.Name;
                    list.Add(Convert.ToInt32(tmpStr));
                }
            }
            list.Sort();
            int i = 1;
            foreach (int j in list)
            {
                if (i == j)
                {
                    i++;
                }
            }
            return i.ToString();
        }
        #endregion
    }
}