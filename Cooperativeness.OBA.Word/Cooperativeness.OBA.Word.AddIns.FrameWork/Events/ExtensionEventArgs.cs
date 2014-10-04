using System.ComponentModel;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Events
{
    /// <summary>
    /// 定义插件扩展事件参数对象
    /// </summary>
    public class ExtensionEventArgs : CancelEventArgs
    {
        #region 字段
        private CollectionChangedAction action;
        private IExtension extension;

        #endregion

        #region 构造函数
        public ExtensionEventArgs(IExtension extension, CollectionChangedAction action)
        {
            this.extension = extension;
            this.action = action;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取扩展点名称
        /// </summary>
        public string Point
        {
            get { return this.extension.Point; }
        }

        /// <summary>
        /// 获取扩展对象
        /// </summary>
        public IExtension Extension
        {
            get { return this.extension; }
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
