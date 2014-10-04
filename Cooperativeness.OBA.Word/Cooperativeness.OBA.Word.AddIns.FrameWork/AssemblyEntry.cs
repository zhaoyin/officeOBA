using System;
using System.Reflection;
using Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义程序集条目对象
    /// </summary>
    internal class AssemblyEntry
    {
        private static readonly Logger Log = new Logger(typeof(AssemblyEntry));
        #region 字段
        private string path;
        private bool share;
        private Assembly assembly;
        private XAssembly metadata;

        #endregion

        #region 构造函数
        public AssemblyEntry(XAssembly metadata)
        {
            this.metadata = metadata;
            this.path = this.metadata.Path;
            this.share = this.metadata.Share;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取插件的相对路径
        /// </summary>
        public string Path
        {
            get { return this.path; }
        }

        /// <summary>
        /// 获取一个值，用来指示当前程序集是否共享
        /// </summary>
        public bool Share
        {
            get { return this.share; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public Assembly LoadAssembly(IBundle bundle)
        {
            try
            {
                if (assembly == null)
                {
                    string absolutePath = System.IO.Path.Combine(bundle.Location, path);
                    assembly = Assembly.LoadFrom(absolutePath);
                }
                return assembly;
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
            return null;
        }
        #endregion
    }
}
