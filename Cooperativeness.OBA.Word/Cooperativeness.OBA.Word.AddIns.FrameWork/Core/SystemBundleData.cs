using System;
using System.IO;
using Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义系统插件数据
    /// </summary>
    internal class SystemBundleData : BaseData
    {
        private  static readonly Logger Log=new Logger(typeof(SystemBundleData));

        #region 构造函数
        public SystemBundleData()
            : base(0, LoadSystemBundleManifest(), GetSystemBundleLocation())
        {
        }

        #endregion

        #region 方法
        /// <summary>
        /// 加载系统插件清单
        /// </summary>
        /// <returns></returns>
        private static XBundle LoadSystemBundleManifest()
        {
            try
            {
                Type type = typeof (SystemBundleData);
                string xml = BundleUtil.LoadResourceString(ConfigConstant.SYSTEM_MANIFEST, type.Assembly);

                if (string.IsNullOrEmpty(xml)) return null;
                var xManifest = XBundle.Parse(xml) as XBundle;

                return xManifest;
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }
            return null;
        }

        /// <summary>
        /// 获取系统插件安装位置
        /// </summary>
        /// <returns></returns>
        private static string GetSystemBundleLocation()
        {
            Type type = typeof(SystemBundleData);
            var url = new Uri(type.Assembly.CodeBase);
            string location = Path.GetDirectoryName(url.LocalPath);

            return location;
        }
        #endregion

    }
}
