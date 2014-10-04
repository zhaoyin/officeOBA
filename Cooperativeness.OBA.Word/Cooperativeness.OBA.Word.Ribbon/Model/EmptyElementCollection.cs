using System;
using System.Collections.Generic;

namespace Cooperativeness.OBA.Word.Ribbon.Model
{
    /// <summary>
    /// 定义空元素集合
    /// </summary>
    internal sealed class EmptyElementCollection : ElementCollection
    {
        #region 字段
        private static readonly EmptyElementCollection _Empty = new EmptyElementCollection();

        #endregion

        #region 构造函数
        private EmptyElementCollection()
        {
        }

        #endregion

        #region 方法
        /// <summary>
        /// 获取空元素集合的枚举器
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<RibbonElement> GetEnumerator()
        {
            return EmptyEnumerator<RibbonElement>.EmptyEnumeratorSingleton;
        }

        /// <summary>
        /// 根据下标获取指定元素,空元素不支持该方法
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override RibbonElement GetItem(int index)
        {
            throw new ArgumentOutOfRangeException("index");
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取空元素集合的子元素的个数
        /// </summary>
        public override int Count
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取单实例空元素集合对象
        /// </summary>
        internal static EmptyElementCollection EmptySingleton
        {
            get
            {
                return _Empty;
            }
        }

        #endregion
    }
}
