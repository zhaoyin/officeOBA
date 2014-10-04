using System.Collections.Generic;
using Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义扩展点对象
    /// </summary>
    internal class ExtensionPointImpl : IExtensionPoint
    {
        #region 字段
        private string point;
        private string schema;
        private IBundle owner;
        private XExtensionPoint metadata;
        private IList<IExtension> extensions;

        #endregion

        #region 构造函数
        public ExtensionPointImpl(IBundle bundle,XExtensionPoint metadata)
        {
            this.owner = bundle;
            this.metadata = metadata;
            this.point = metadata.Point;
            this.schema = metadata.Schema;
            this.extensions = new List<IExtension>();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取扩展点所属的插件
        /// </summary>
        public IBundle Owner
        {
            get { return this.owner; }
            internal set { this.owner = value; }
        }

        /// <summary>
        /// 获取当前扩展点的所有扩展列表
        /// </summary>
        public IList<IExtension> Extensions
        {
            get { return this.extensions; }
        }

        /// <summary>
        /// 获取扩展点名称
        /// </summary>
        public string Point
        {
            get { return this.point; }
        }

        /// <summary>
        /// 获取扩展信息需要遵循的架构
        /// </summary>
        public string Schema
        {
            get { return this.schema; }
        }

        /// <summary>
        /// 检查是否有效
        /// </summary>
        public bool Available
        {
            get
            {
                try
                {
                    Framework framework = ((AbstractBundle)owner).Framework;
                    return !framework.ExtensionAdmin.Contains(this);
                }
                catch
                {
                    return false;
                }
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 添加扩展
        /// </summary>
        /// <param name="extension"></param>
        public bool AddExtension(IExtension extension)
        {
            if (extension == null)
                return false;
            if (!this.extensions.Contains(extension))
            {
                this.extensions.Add(extension);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除扩展
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public bool RemoveExtension(IExtension extension)
        {
            if (extension == null)
                return false;
            if (this.extensions.Contains(extension))
            {
                this.extensions.Remove(extension);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "ExtensionPoint:" + this.point;
        }

        /// <summary>
        /// 检查是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            ExtensionPointImpl target = obj as ExtensionPointImpl;
            if (target == null) return false;
            if (this == target) return true;

            return this.point.EqualsIgnoreCase(target.point);
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        #endregion
    }
}
