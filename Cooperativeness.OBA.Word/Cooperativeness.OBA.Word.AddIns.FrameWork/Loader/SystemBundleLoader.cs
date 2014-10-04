using System;
using System.IO;
using System.Reflection;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Loader
{
    /// <summary>
    /// 定义系统插件加载器
    /// </summary>
    internal class SystemBundleLoader : AbstractClassLoader, IBundleLoader
    {
        #region 字段
        private AbstractBundle bundle;
        private Assembly assembly;

        #endregion

        #region 构造函数
        public SystemBundleLoader(AbstractBundle bundle)
        {
            this.assembly = this.GetType().Assembly;
            this.bundle = bundle;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取插件框架对象
        /// </summary>
        public Framework Framework
        {
            get { return this.bundle.Framework; }
        }

        /// <summary>
        /// 获取当前加载器所属的插件对象
        /// </summary>
        public IBundle Bundle
        {
            get { return this.bundle; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 加载指定的类型
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public override Type LoadClass(string className)
        {
            return this.assembly.GetType(className);
        }

        /// <summary>
        /// 加载指定的资源
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public override object LoadResource(string resourceName, ResourceLoadMode mode)
        {
            try
            {
                if (mode == ResourceLoadMode.ClassSpace)
                    return this.assembly.GetManifestResourceStream(resourceName);
                else if (mode == ResourceLoadMode.Local)
                {
                    string path = Path.Combine(bundle.Location, resourceName);
                    return File.Open(path, FileMode.Open, FileAccess.ReadWrite);
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 根据程序名称加载插件程序集
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        protected override Assembly OnLoadBundleAssembly(string fullName)
        {
            if (this.assembly.FullName.Equals(fullName))
                return this.assembly;
            return null;
        }
        #endregion


    }
}
