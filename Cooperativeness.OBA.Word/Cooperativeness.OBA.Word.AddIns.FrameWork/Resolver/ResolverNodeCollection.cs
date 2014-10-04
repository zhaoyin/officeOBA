using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Resolver
{
    /// <summary>
    /// 定义解析树节点集合
    /// </summary>
    internal class ResolverNodeCollection : IEnumerable<ResolverNode>, IEnumerable
    {
        #region 字段
        private IList<ResolverNode> list;
        private object lockObj = new object();
        #endregion

        #region 构造函数
        public ResolverNodeCollection()
        {
            list = new List<ResolverNode>();
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取索引对应的解析插件对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ResolverNode this[int index]
        {
            get
            {
                if (index >= list.Count || index < 0)
                    throw new IndexOutOfRangeException();
                return list[index];
            }
        }

        /// <summary>
        /// 获取当前集合的数量
        /// </summary>
        public int Count
        {
            get { return this.list.Count; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 添加用户指定解析插件
        /// </summary>
        /// <param name="resolverBundle"></param>
        public void Add(ResolverNode resolverBundle)
        {
            if (resolverBundle == null) return;
            lock (lockObj)
            {
                if (!list.Contains(resolverBundle))
                {
                    list.Add(resolverBundle);
                }
            }
        }

        /// <summary>
        /// 移除用户指定解析插件
        /// </summary>
        /// <param name="?"></param>
        public void Remove(ResolverNode resolverBundle)
        {
            if (resolverBundle == null) return;
            lock (lockObj)
            {
                if (list.Contains(resolverBundle))
                {
                    list.Remove(resolverBundle);
                }
            }
        }

        /// <summary>
        /// 移除所有的解析插件
        /// </summary>
        public void RemoveAll()
        {
            lock (lockObj)
            {
                list.Clear();
            }
        }

        /// <summary>
        /// 转换为数组
        /// </summary>
        /// <returns></returns>
        public ResolverNode[] ToArray()
        {
            lock (lockObj)
            {
                return list.ToArray();
            }
        }

        public IEnumerator<ResolverNode> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
        #endregion
    }
}
