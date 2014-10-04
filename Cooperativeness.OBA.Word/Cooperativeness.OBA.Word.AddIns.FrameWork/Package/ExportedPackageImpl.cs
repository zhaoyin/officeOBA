using System;
using System.Linq;
using System.Reflection;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Package
{
    /// <summary>
    /// 定义导出包实现类
    /// </summary>
    internal class ExportedPackageImpl : IExportedPackage
    {
        private static readonly Logger Log = new Logger(typeof(ExportedPackageImpl));
        #region 字段
        private Assembly packageAssembly;
        private AssemblyName assemblyName;
        private AbstractBundle bundle;
        private string path;
        private string name;
        private Version version;
        private string culture;
        private string publicKeyToken;
        private bool hasInitialize = false;
        private object lockObj = new object();

        #endregion

        #region 构造函数
        protected ExportedPackageImpl(AbstractBundle bundle,AssemblyEntry asemblyEntry)
        {
            this.bundle = bundle;
            this.path = System.IO.Path.Combine(bundle.Location, asemblyEntry.Path);
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取包名称
        /// </summary>
        public string Name
        {
            get 
            {
                if (!hasInitialize) this.Initialize();
                return this.name;
            }
        }

        /// <summary>
        /// 获取包的版本号
        /// </summary>
        public Version Version
        {
            get
            {
                if (!hasInitialize) this.Initialize();
                return this.version;
            }
        }

        /// <summary>
        /// 获取包的语言
        /// </summary>
        public string Culture
        {
            get
            {
                if (!hasInitialize) this.Initialize();
                return this.culture;
            }
        }

        /// <summary>
        /// 获取包的公钥令牌
        /// </summary>
        public string PublicKeyToken
        {
            get
            {
                if (!hasInitialize) this.Initialize();
                return this.publicKeyToken;
            }
        }

        /// <summary>
        /// 获取导出包的插件对象
        /// </summary>
        public IBundle ExportingBundle
        {
            get { return this.bundle; }
        }

        /// <summary>
        /// 获取导出包的程序集名称
        /// </summary>
        public AssemblyName AssemblyName
        {
            get
            {
                if (!hasInitialize) this.Initialize();
                return assemblyName;
            }
        }

        /// <summary>
        /// 获取导出包的反射程序集
        /// </summary>
        public Assembly ReflectOnlyPackageAssembly
        {
            get
            {
                if (!hasInitialize) this.Initialize();
                return packageAssembly;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 创建导出包
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="asemblyEntry"></param>
        /// <returns></returns>
        internal static ExportedPackageImpl Create(AbstractBundle bundle, AssemblyEntry asemblyEntry)
        {
            try
            {
                var package = new ExportedPackageImpl(bundle, asemblyEntry);
                package.Initialize();
                return package;

            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
            return null;
        }

        /// <summary>
        /// 初始化导出包
        /// </summary>
        protected void Initialize()
        {
            lock (lockObj)
            {
                if (this.hasInitialize) return;
                try
                {
                    packageAssembly = Assembly.ReflectionOnlyLoadFrom(this.path);
                    string[] names = packageAssembly.FullName.Split(",".ToArray());
                    this.name = names[0];
                    this.version = new Version(names[1].Split("=".ToArray())[1].Trim());
                    this.culture = names[2].Split("=".ToArray())[1].Trim();
                    string[] tokens = names[3].Split("=".ToArray());
                    this.publicKeyToken = tokens.Length == 1 && tokens[0].Trim().EqualsIgnoreCase("null")
                                              ? null
                                              : tokens[1].Trim();
                    assemblyName = new AssemblyName(packageAssembly.FullName);
                    this.hasInitialize = true;
                }
                catch (Exception e)
                {
                    Log.Debug(e);
                }
            }
        }

        /// <summary>
        /// 相等比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var target = obj as ExportedPackageImpl;
            if (target == null) return false;
            // 检查是一个对象
            if (target == this) return true;
            // 名称和语言是否相等
            bool isEqual = this.Name.Equals(target.Name) && this.Culture.Equals(target.Culture);
            if(!isEqual) return false;
            // 检查版本是否相等
            if (this.Version == null && target.Version == null)
                isEqual = true;
            else if ((this.Version == null && target.Version != null)
                || (this.Version != null && target.Version == null))
                isEqual = false;
            else
                isEqual = this.Version.Equals(target.Version);
            if(!isEqual) return false;

            return true;
        }
        #endregion
    }
}
