using System.ComponentModel;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Events
{
    /// <summary>
    /// 定义插件扩展点事件参数对象
    /// </summary>
    public class ExtensionPointEventArgs : CancelEventArgs
    {
        #region 字段
        private CollectionChangedAction action;
        private IExtensionPoint extensionPoint;

        #endregion

        #region 构造函数
        public ExtensionPointEventArgs(IExtensionPoint extensionPoint, CollectionChangedAction action)
        {
            this.extensionPoint = extensionPoint;
            this.action = action;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取扩展点名称
        /// </summary>
        public string Point
        {
            get { return this.extensionPoint.Point; }
        }

        /// <summary>
        /// 获取扩展对象
        /// </summary>
        public IExtensionPoint ExtensionPoint
        {
            get { return this.extensionPoint; }
        }

        /// <summary>
        /// 获取执行动作
        /// </summary>
        public CollectionChangedAction Action
        {
            get { return this.action; }
        }
        #endregion
    }
}
