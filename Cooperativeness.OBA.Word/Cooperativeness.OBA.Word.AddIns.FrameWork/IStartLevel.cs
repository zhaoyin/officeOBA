
namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义插件启动级别接口契约
    /// </summary>
    public interface IStartLevel
    {
        /// <summary>
        /// 获取或设置启动级别
        /// </summary>
        int StartLevel { get; set; }

        /// <summary>
        /// 获取插件的启动级别
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        int GetBundleStartLevel(IBundle bundle);

        /// <summary>
        /// 设置插件的启动级别
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="startlevel"></param>
        void SetBundleStartLevel(IBundle bundle, int startlevel);

        /// <summary>
        /// 获取或设置查件的额初始启动级别
        /// </summary>
        int InitialBundleStartLevel { get; set; }


        /// <summary>
        /// 指定的插件是否自动设置启动级别
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        bool IsBundlePersistentlyStarted(IBundle bundle);

        /// <summary>
        /// 检查插件集合策略是否自动使用
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        bool IsBundleActivationPolicyUsed(IBundle bundle);
    }
}
