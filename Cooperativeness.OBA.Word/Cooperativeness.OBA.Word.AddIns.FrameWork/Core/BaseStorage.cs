using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义基础存储器对象
    /// </summary>
    internal class BaseStorage
    {
        private static readonly Logger Log = new Logger(typeof(BaseStorage));
        #region 字段
        private object nextIdMonitor = new object();
        private long nextId = 1;
        private LocationManager locationManager;

        #endregion

        #region 构造函数
        public BaseStorage()
        {
            locationManager = new LocationManager();
        }

        #endregion

        #region 方法
        /// <summary>
        /// 获取已安装的所有插件数据
        /// </summary>
        /// <returns></returns>
        public IBundleData[] GetInstalledBundles()
        {
            try
            {
                return ReadBundleDatas();
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }

            return null;
        }

        /// <summary>
        /// 读取插件数据
        /// </summary>
        /// <returns></returns>
        private IBundleData[] ReadBundleDatas()
        {
            // 获取所有插件清单的路径
            string[] addInsLocations = locationManager.GetAddinsLocations();
            if (addInsLocations == null || addInsLocations.Length == 0)
                return null;
            // 开始加载插件清单
            IList<BaseData> bundleDatas = new List<BaseData>();
            foreach (var location in addInsLocations)
            {
                var xManifest = XBundle.Load(location) as XBundle;
                if (xManifest == null) continue;
                string bundleLocation = Path.GetDirectoryName(Path.GetDirectoryName(location));
                BaseData bundleData = new BaseData(GetNextBundleId(), xManifest, bundleLocation);
                bundleDatas.Add(bundleData);
            }

            return bundleDatas.ToArray();
        }

        /// <summary>
        /// 获取下一个插件标识
        /// </summary>
        /// <returns></returns>
        public long GetNextBundleId()
        {
            lock (this.nextIdMonitor)
            {
                return nextId++;
            }
        }
        #endregion
    }
}
