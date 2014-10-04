using Microsoft.Office.Interop.Word;

namespace Cooperativeness.OBA.Word.Ribbon
{
    /// <summary>
    /// 定义功能区元素对象接口契约
    /// </summary>
    public interface IRibbonElement
    {
        /// <summary>
        /// 获取Office Wrod 插件对象
        /// </summary>
        AddIn AddIn { get; }

        /// <summary>
        /// 刷新整个Ribbon UI
        /// </summary>
        void Invalidate();

        /// <summary>
        /// 按用户指定的按钮标识，刷新该按钮
        /// </summary>
        /// <param name="controlId"></param>
        void InvalidateControl(string controlId);
    }
}
