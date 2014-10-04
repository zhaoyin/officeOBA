using System;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Resolver
{
    /// <summary>
    /// 定义解析树节点
    /// </summary>
    internal class ResolverNode : IComparable
    {
        #region 字段
        private AbstractBundle bundle;
        private ResolverNodeCollection depencyProvidersBy;
        private ResolverNodeCollection dependencies;
        private ResolverTree resolverAdmin;

        #endregion

        #region 字段
        public ResolverNode(ResolverTree resolverAdmin, AbstractBundle bundle, ResolverNode depencyProviderBy)
        {
            this.resolverAdmin = resolverAdmin;
            this.depencyProvidersBy = new ResolverNodeCollection();
            dependencies = new ResolverNodeCollection();
            this.bundle = bundle;
            this.depencyProvidersBy.Add(depencyProviderBy);
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取当前解析插件的依赖的所有提供者
        /// </summary>
        public ResolverNodeCollection DepencyProvidersBy
        {
            get { return this.depencyProvidersBy; }
        }

        /// <summary>
        /// 获取当前插件依赖的所有解析插件
        /// </summary>
        public ResolverNodeCollection Dependencies
        {
            get { return this.dependencies; }
        }

        /// <summary>
        /// 获取当前对应的插件对象
        /// </summary>
        public AbstractBundle Bundle
        {
            get { return this.bundle; }
        }

        /// <summary>
        /// 获取解析管理器
        /// </summary>
        public ResolverTree ResolverAdmin
        {
            get { return this.resolverAdmin; }
        }

        /// <summary>
        /// 获取或设置当前解析插件所处的逆向级次
        /// </summary>
        public int DesLevel { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 比较操作
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            return this.bundle.CompareTo(((ResolverNode)obj).bundle);
        }
        #endregion
    }
}
