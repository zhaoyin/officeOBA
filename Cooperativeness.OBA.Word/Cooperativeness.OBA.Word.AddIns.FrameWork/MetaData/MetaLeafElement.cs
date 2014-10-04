
namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义抽象的叶子元素
    /// </summary>
    public abstract class MetaLeafElement : MetaElement
    {
        #region 构造函数
        protected MetaLeafElement()
        {
        }

        #endregion

        #region 方法
        /// <summary>
        /// 移除所有的子元素
        /// </summary>
        public override void RemoveAllChildren()
        {
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取一个值，该值指示是否存在子元素
        /// </summary>
        public override bool HasChildren
        {
            get
            {
                return false;
            }
        }
        #endregion
    }
}
