using System;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Package
{
    /// <summary>
    /// 定义私有包
    /// </summary>
    internal class PrivilegedPackage : ExportedPackageImpl
    {
        private static readonly Logger Log=new Logger(typeof(PrivilegedPackage));
        #region 构造函数
        private PrivilegedPackage(AbstractBundle bundle, AssemblyEntry asemblyEntry)
            : base(bundle, asemblyEntry)
        {
        }

        #endregion



        #region 方法
        /// <summary>
        /// 创建导出包
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="asemblyEntry"></param>
        /// <returns></returns>
        internal static PrivilegedPackage Create(AbstractBundle bundle, AssemblyEntry asemblyEntry)
        {
            try
            {
                var package = new PrivilegedPackage(bundle,asemblyEntry);
                package.Initialize();
                return package;

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
