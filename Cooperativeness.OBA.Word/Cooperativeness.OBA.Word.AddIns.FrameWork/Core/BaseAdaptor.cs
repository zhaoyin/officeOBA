
using Cooperativeness.OBA.Word.AddIns.FrameWork.Adaptor;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义框架适配器
    /// </summary>
    internal class BaseAdaptor : IFrameworkAdaptor
    {
        #region 字段
        private BundleProperty properties;
        private BaseStorage baseStorage;

        #endregion

        #region 构造函数
        public BaseAdaptor()
        {
            baseStorage = new BaseStorage();
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取插件属性对象
        /// </summary>
        public BundleProperty Properties
        {
            get {
                if (properties == null)
                    properties = new BundleProperty();
                return properties;
            }
        }

        /// <summary>
        /// 获取已安装的插件数据列表
        /// </summary>
        public IBundleData[] InstalledBundles
        {
            get
            {
                IBundleData[] bundleDatas = baseStorage.GetInstalledBundles();
                return bundleDatas;
            }
        }

        #endregion

        #region 方法
        

        public void CompactStorage()
        {
            //throw new NotImplementedException();
        }

        public void Initialize()
        {
            //throw new NotImplementedException();
        }

        public void InitializeStorage()
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
