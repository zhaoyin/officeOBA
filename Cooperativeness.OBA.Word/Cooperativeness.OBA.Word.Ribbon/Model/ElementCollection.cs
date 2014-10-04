using System.Collections.Generic;
using System.Collections;

namespace Cooperativeness.OBA.Word.Ribbon.Model
{
    /// <summary>
    /// 定义元素集合对象
    /// </summary>
    public abstract class ElementCollection : IEnumerable<RibbonElement>, IEnumerable
    {
        #region 方法
        /// <summary>
        /// 按用户指定的类型获取第一个符合该类的元素对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T First<T>() where T : RibbonElement
        {
            foreach (RibbonElement element in this)
            {
                if (element is T)
                {
                    return (T)element;
                }
            }
            return default(T);
        }

        /// <summary>
        /// 获取元素集合枚举器
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator<RibbonElement> GetEnumerator();

        /// <summary>
        /// 按用户指定的下标获下标所在的元素对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract RibbonElement GetItem(int index);

        /// <summary>
        /// 获取用户指定类型的枚举集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> OfType<T>() where T : RibbonElement
        {
            foreach (RibbonElement iterator in this)
            {
                if (iterator is T)
                {
                    yield return (T)iterator;
                }
            }
        }

        /// <summary>
        /// 获取元素集合迭代器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取当前集合元素的个数
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// 根据下标获取元素
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public virtual RibbonElement this[int i]
        {
            get
            {
                return this.GetItem(i);
            }
        }

        #endregion
    }
}
