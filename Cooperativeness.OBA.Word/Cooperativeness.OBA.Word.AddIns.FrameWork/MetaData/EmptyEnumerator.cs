using System;
using System.Collections.Generic;
using System.Collections;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义空枚举器对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class EmptyEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
    {
        #region 字段
        private static readonly IEnumerator<T> _EmptyEnumerator;

        #endregion

        #region 构造函数
        static EmptyEnumerator()
        {
            EmptyEnumerator<T>._EmptyEnumerator = new EmptyEnumerator<T>();
        }

        private EmptyEnumerator()
        {
        }

        #endregion

        #region 实现IDisposable接口
        public void Dispose()
        {
        }

        #endregion

        #region 方法
        /// <summary>
        /// 将枚举游标移至下一个，如果没有下一个元素则返回false，
        /// 否则返回true，对于空枚举器而言，直接返回空
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            return false;
        }

        /// <summary>
        /// 重置操作
        /// </summary>
        public void Reset()
        {
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取枚举器的当前元素
        /// </summary>
        public T Current
        {
            get
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 获取单实例的空枚举器
        /// </summary>
        internal static IEnumerator<T> EmptyEnumeratorSingleton
        {
            get
            {
                return EmptyEnumerator<T>._EmptyEnumerator;
            }
        }

        /// <summary>
        /// 获取枚举器的当前元素
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }
        #endregion
    }
}
