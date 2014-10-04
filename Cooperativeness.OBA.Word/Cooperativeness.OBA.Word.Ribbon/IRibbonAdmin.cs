using Office = Microsoft.Office.Core;
using Cooperativeness.OBA.Word.AddIns.FrameWork;

namespace Cooperativeness.OBA.Word.Ribbon
{
    /// <summary>
    /// 定义功能区管理接口契约
    /// </summary>
    public interface IRibbonAdmin
    {
        /// <summary>
        /// 获取功能区扩展对象
        /// </summary>
        object RibbonUi { get; }

        /// <summary>
        /// 初始化功能区管理器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="extensinPoint"></param>
        void Initialize(IBundleContext context, IExtensionPoint extensinPoint);

        /// <summary>
        /// 添加指定的扩展对象
        /// </summary>
        /// <param name="extension"></param>
        void AddExtension(IExtension extension);

        /// <summary>
        /// 刷新整个功能区
        /// </summary>
        void Invalidate();

        /// <summary>
        /// 按用户指定的按钮标识，刷新该按钮
        /// </summary>
        /// <param name="controlId"></param>
        void InvalidateControl(string controlId);

        /// <summary>
        /// 关闭操作
        /// </summary>
        void Close();
    }
}
