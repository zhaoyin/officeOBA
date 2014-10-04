using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Package
{
    /// <summary>
    /// 定义导出插件
    /// </summary>
    internal class ImportBundleImpl : IImportBundle
    {
        #region 字段
        protected AbstractBundle bundle;
        protected ResolutionMode resolution;

        #endregion

        #region 构造函数
        public ImportBundleImpl(AbstractBundle bundle,ResolutionMode resolution)
        {
            this.bundle = bundle;
            this.resolution = resolution;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取导入的插件
        /// </summary>
        public IBundle Bundle
        {
            get { return this.bundle; }
        }

        /// <summary>
        /// 获取依赖插件的解析方式
        /// </summary>
        public ResolutionMode Resolution
        {
            get { return this.resolution; }
        }
        #endregion

        #region 方法

        /// <summary>
        /// 检查两个对象是否相等
        /// </summary>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var package = obj as ImportBundleImpl;
            if (package == null) return false;
            if (this == package) return true;

            return this.bundle.Equals(package.bundle);

        }
        #endregion
    }
}
