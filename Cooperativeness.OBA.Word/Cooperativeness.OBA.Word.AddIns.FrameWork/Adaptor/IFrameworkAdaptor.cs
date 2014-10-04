
namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Adaptor
{
    /// <summary>
    /// 定义插件框架适配器接口契约
    /// </summary>
    internal interface IFrameworkAdaptor
    {
        /// <summary>
        /// 获取插件属性对象
        /// </summary>
        BundleProperty Properties { get; }

        /// <summary>
        /// 获取安装的所有插件的插件数据
        /// </summary>
        IBundleData[] InstalledBundles { get; }

        /// <summary>
        /// 整理插件持久化存贮器
        /// </summary>
        void CompactStorage();

        /// <summary>
        /// 初始化框架适配器
        /// </summary>
        void Initialize();

        /// <summary>
        /// 初始化存贮器
        /// </summary>
        void InitializeStorage();
    }
}
