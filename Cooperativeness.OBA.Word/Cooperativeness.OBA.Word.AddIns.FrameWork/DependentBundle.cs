using System;
using Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义依赖插件对象
    /// </summary>
    internal class DependentBundle
    {
        #region 字段
        private string bundleSymblocName;
        private Version bundleVersion;
        private string assemblyName;
        private Version assemlyVersion;
        private string culture;
        private string publicKeyToken;
        private XDependency metadata;
        private ResolutionMode resolution = ResolutionMode.Mandatory;

        #endregion

        #region 构造函数
        public DependentBundle(XDependency metadata)
        {
            this.metadata = metadata;
            this.bundleSymblocName = metadata.BundleSymbolicName;
            this.bundleVersion = string.IsNullOrEmpty(metadata.BundleVersion.ToString())
                ? new Version("1.0.0.0") : new Version(metadata.BundleVersion.ToString());
            this.assemblyName = metadata.AssemblyName;
            this.assemlyVersion = string.IsNullOrEmpty(metadata.AssemblyVersion.ToString())
                ? null : new Version(metadata.AssemblyVersion.ToString());
            this.culture = string.IsNullOrEmpty(metadata.Culture.ToString())
                ? "neutral" : metadata.Culture.ToString();
            this.publicKeyToken = string.IsNullOrEmpty(metadata.PublicKeyToken.ToString())
                ? "null" : metadata.PublicKeyToken.ToString();
            this.resolution = metadata.Resolution;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取插件的唯一标识名
        /// </summary>
        public string BundleSymbolicName
        {
            get { return this.bundleSymblocName; }
        }

        /// <summary>
        /// 获取插件的版本
        /// </summary>
        public Version BundleVersion
        {
            get { return this.bundleVersion; }
        }

        /// <summary>
        /// 获取依赖的程序集名称
        /// </summary>
        public string AssemblyName
        {
            get { return this.assemblyName; }
        }

        /// <summary>
        /// 获取依赖程序集的版本
        /// </summary>
        public Version AssemblyVersion
        {
            get { return this.assemlyVersion; }
        }

        /// <summary>
        /// 获取程序集的本地语言
        /// </summary>
        public string Culture
        {
            get{return this.culture;}
        }

        /// <summary>
        /// 获取依赖程序集的公钥令牌
        /// </summary>
        public string PublicKeyToken
        {
            get { return this.publicKeyToken; }
        }

        /// <summary>
        /// 获取解析模式
        /// </summary>
        public ResolutionMode Resolution
        {
            get { return this.resolution; }
        }
        #endregion
    }
}
