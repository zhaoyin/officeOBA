
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.DocumentScene.Plugin.Framework;
using Cooperativeness.OBA.Word.DocumentScene.Plugin.Model;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin
{
    /// <summary>
    /// 定义场景上下文接口
    /// </summary>
    public interface ISceneContext
    {
        /// <summary>
        /// 获取一个值，该值用来指示当前用户是否已登录
        /// </summary>
        bool IsLogin { get; }

        /// <summary>
        /// 获取登录对象
        /// </summary>
        LoginEntity LoginEntity { get; }

        /// <summary>
        /// 获取插件上下文
        /// </summary>
        IBundleContext BundleContext { get; }

        /// <summary>
        /// 获取私密数据管理器
        /// </summary>
        SecretDataAdmin SecretDataAdmin { get; }

        /// <summary>
        /// 获取Word应用程序对象管理器
        /// </summary>
        WordApplicationAdmin WordAppAdmin { get; }

        /// <summary>
        /// 获取多语值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetString(string name);

        /// <summary>
        /// 刷新整个Ribbon区域
        /// </summary>
        void RefreshRibbon();

        /// <summary>
        /// 刷新指定的Ribbon按钮
        /// </summary>
        /// <param name="controlId"></param>
        void RefreshRibbon(string controlId);

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        bool Login();

        /// <summary>
        /// 注销登录
        /// </summary>
        /// <returns></returns>
        void Logout();

        /// <summary>
        /// 关闭当前上下文
        /// </summary>
        void Close();
    }
}
