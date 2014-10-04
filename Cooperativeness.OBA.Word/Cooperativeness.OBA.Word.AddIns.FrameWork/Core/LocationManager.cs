using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义位置管理器
    /// </summary>
    internal class LocationManager
    {
        private static readonly Logger Log=new Logger(typeof(LocationManager));
        #region 字段
        /** 插件目录名称 */
        private string installLocation;
        private IList bundleLocations;
        //private static LocationManager instance;
        private string appLocation;

        #endregion

        #region 构造函数
        public LocationManager()
        {
            this.Intialize();
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取应用程序主目录
        /// </summary>
        public string AppLocation
        {
            get { return this.appLocation; }
        }

        /// <summary>
        /// 获取所有插件的安装路径
        /// </summary>
        public string InstallLocation
        {
            get { return this.installLocation; }
        }
        #endregion

        #region 字段
        ///// <summary>
        ///// 获取位置管理的单实例对象
        ///// </summary>
        //public static LocationManager Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //            instance = new LocationManager();
        //        return instance;
        //    }
        //}

        #endregion

        #region 方法
        /// <summary>
        /// 初始化目录管理器
        /// </summary>
        private void Intialize()
        {
            // 获取插件框架运行的根目录
            var assebmly = this.GetType().Assembly;
            string asmFile = new Uri(assebmly.CodeBase).LocalPath;
            string asmPath = Path.GetDirectoryName(asmFile);
            // 检查目录名是否符合规范
            if (string.IsNullOrEmpty(asmPath))
            {
                Log.Debug("不存在框架运行目录:" + asmPath);
            }
            var binDir = new DirectoryInfo(asmPath);
            if (!binDir.Name.EqualsIgnoreCase("bin"))
                throw new IOException("Not find the \"bin\" directory.");
            appLocation = binDir.FullName;
            // 获取应用程序目录
            if (binDir.Parent != null)
            {
                this.appLocation = binDir.Parent.FullName;
            }
            // 检查插件目录
            this.installLocation = Path.Combine(this.appLocation,ConfigConstant.PLUGINS_DIR_NAME);
            if(!Directory.Exists(this.installLocation))
                throw new IOException("Not find the \"plugins\" directory.");

        }

        /// <summary>
        /// 获取所有插件位置
        /// </summary>
        /// <returns></returns>
        public string[] GetAddinsLocations()
        {
            // 插件位置集合已加载，则直接返回集合列表
            if ( this.bundleLocations!=null)
            {
                if (bundleLocations.Count != 0)
                {
                    var locations = new string[bundleLocations.Count];
                    bundleLocations.CopyTo(locations, 0);
                    return locations;
                }
                return null;
            }
            // 扫描位置信息
            string[] scanLocations = Scan(ConfigConstant.BUNDLE_MF, installLocation);
            this.bundleLocations = new ArrayList();
            if (scanLocations != null)
            {
                for (int i = 0; i < scanLocations.Length; i++)
                {
                    string location = scanLocations[i];
                    string pLocation = Path.GetDirectoryName(location);
                    string standPath = Path.Combine(this.installLocation, ConfigConstant.BUNDLE_MF_HOME);
                    //if (Path.GetDirectoryName(pLocation).EqualsIgnoreCase(this.installLocation)
                    //    && pLocation.EqualsIgnoreCase(BUNDLE_MF_HOME))
                    if(pLocation.EqualsIgnoreCase(standPath))
                        bundleLocations.Add(scanLocations[i]);
                }
            }

            return scanLocations;
        }

        /// <summary>
        /// 扫描指定的目录
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        private string[] Scan(string fileName, string dirPath)
        {
            // 扫描目录，查找插件自描述文件
            var dirInfo = new DirectoryInfo(dirPath);
            var searchFiles = dirInfo.GetFiles(fileName, SearchOption.AllDirectories);
            var locations = new List<string>();
            foreach (var file in searchFiles)
            {
                locations.Add(file.FullName);
            }

            return locations.Count == 0 ? null : locations.ToArray();
        }

        /// <summary>
        /// 重新扫描操作
        /// </summary>
        internal void Rescan()
        {
            // 扫描位置信息
            string[] scanLocations = Scan(ConfigConstant.BUNDLE_MF, installLocation);
            this.bundleLocations = new ArrayList();
            if (scanLocations != null)
            {
                for (int i = 0; i < scanLocations.Length; i++)
                {
                    string location = scanLocations[i];
                    string pLocation = Path.GetDirectoryName(location);
                    string standPath = Path.Combine(this.installLocation, ConfigConstant.BUNDLE_MF_HOME);
                    //if (Path.GetDirectoryName(pLocation).EqualsIgnoreCase(this.installLocation)
                    //    && pLocation.EqualsIgnoreCase(BUNDLE_MF_HOME))
                    if(pLocation.EqualsIgnoreCase(standPath))
                        bundleLocations.Add(scanLocations[i]);
                }
            }
        }

        /// <summary>
        /// 添加插件位置
        /// </summary>
        /// <param name="location"></param>
        public void AddBundleLoaction(string location)
        {
            if (bundleLocations.Contains(location))
                return;
            string pLocation = Path.GetDirectoryName(location);
            string standPath = Path.Combine(this.installLocation, ConfigConstant.BUNDLE_MF_HOME);
            //if (Path.GetDirectoryName(pLocation).EqualsIgnoreCase(this.installLocation)
            //    && pLocation.EqualsIgnoreCase(BUNDLE_MF_HOME))
            if (pLocation.EqualsIgnoreCase(standPath))
                bundleLocations.Add(location);
        }

        /// <summary>
        /// 删除插件位置
        /// </summary>
        /// <param name="location"></param>
        public void RemoveBundleLoaction(string location)
        {
            if (!bundleLocations.Contains(location))
                return;
            bundleLocations.Remove(location);
        }

        #endregion
    }
}
