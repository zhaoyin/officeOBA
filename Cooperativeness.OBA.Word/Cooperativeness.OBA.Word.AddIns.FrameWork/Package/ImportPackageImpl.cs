using System;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Package
{
    /// <summary>
    /// 定义导入包实现类
    /// </summary>
    internal class ImportPackageImpl : ImportBundleImpl, IImportPackage
    {
        #region 字段
        private AbstractBundle importBundle;
        private DependentBundle dependentBundle;
        private object lockObj = new object();

        #endregion

        #region 构造函数
        public ImportPackageImpl(AbstractBundle bundle, AbstractBundle requiredBundle, DependentBundle dependentBundle)
            : base(requiredBundle, dependentBundle.Resolution)
        {
            this.importBundle = bundle;
            this.dependentBundle = dependentBundle;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取程序集的名称
        /// </summary>
        public string Name
        {
            get { return dependentBundle.AssemblyName; }
        }

        /// <summary>
        /// 获取程序集的版本
        /// </summary>
        public Version Version
        {
            get { return dependentBundle.AssemblyVersion; }
        }

        /// <summary>
        /// 获取程序集的本地语言
        /// </summary>
        public string Culture
        {
            get { return dependentBundle.Culture; }
        }

        /// <summary>
        /// 获取程序的公钥令牌
        /// </summary>
        public string PublicKeyToken
        {
            get { return dependentBundle.PublicKeyToken; }
        }

        /// <summary>
        /// 获取该导入包所属的插件对象
        /// </summary>
        public IBundle ImportingBundle
        {
            get { return this.importBundle; }
        }
        #endregion


        #region 方法

        /// <summary>
        /// 检查两个对象是否相等
        /// </summary>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var package = obj as ImportPackageImpl;
            if (package == null) return false;
            if (this == package) return true;

            return this.bundle.Equals(package.bundle)
                && this.Name.Equals(package.Name);

        }

        #endregion

    }
}
