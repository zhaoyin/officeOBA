using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UFIDA.U8.IN.AddIns.Loader
{
    /// <summary>
    /// 定义类加载器
    /// </summary>
    internal class ClassLoader : IClassLoader
    {
        #region 字段
        private IBundle bundle;
        private Assembly assembly;

        #endregion

        #region 属性
        public ClassLoader(IBundle bundle)
        {
            this.assembly = this.GetType().Assembly;
            this.bundle = bundle;
        }

        #endregion

        #region 方法
        public Type LoadClass(string className)
        {
            return this.assembly.GetType(className);
        }

        public object LoadResource(string resourceName, ResourceLoadMode mode)
        {
            if(mode == ResourceLoadMode.ClassSpace)
                return this.assembly.GetManifestResourceStream(resourceName);
            else if (mode == ResourceLoadMode.Local)
            {
                string path = Path.Combine(bundle.Location, resourceName);
                return File.Open(path, FileMode.Open, FileAccess.ReadWrite);
            }
            return null;
        }

        public IBundleLoader Owner
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IBundle Bundle
        {
            get { return this.bundle; }
        }

        #endregion
    }
}
