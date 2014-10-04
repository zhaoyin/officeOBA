using System.Collections;
using System.Collections.Generic;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义抽象的集元元素集合
    /// </summary>
    public abstract class MetaElementCollection : IEnumerable<MetaElement>, IEnumerable
    {
        #region 方法
        /// <summary>
        /// 获取第一个指定类型的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T First<T>() where T : MetaElement
        {
            foreach (MetaElement element in this)
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
        public abstract IEnumerator<MetaElement> GetEnumerator();

        /// <summary>
        /// 获取用户指定下标所在的元素
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract MetaElement GetItem(int index);

        /// <summary>
        /// 获取用户指定类型的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> OfType<T>() where T : MetaElement
        {
            foreach (MetaElement iterator in this)
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
        /// 当前元素的个数
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// 根据下标获取元素
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public virtual MetaElement this[int i]
        {
            get
            {
                return this.GetItem(i);
            }
        }

        #endregion
    }
}
