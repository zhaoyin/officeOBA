using System.Collections.Generic;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Package
{
    /// <summary>
    /// 定义服务包实现类
    /// </summary>
    internal class ServicePackageImpl : IServicePackage
    {
        #region 字段
        private ServiceEntry serviceEntry;
        private AbstractBundle bundle;
        private IDictionary<string, object> property;

        #endregion

        #region 构造函数
        public ServicePackageImpl(AbstractBundle bundle, ServiceEntry serviceEntry)
        {
            this.bundle = bundle;
            this.serviceEntry = serviceEntry;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取服务接口类型
        /// </summary>
        public string InterfaceType
        {
            get { return this.serviceEntry.Type; }
        }

        /// <summary>
        /// 获取服务类型
        /// </summary>
        public string ServiceType
        {
            get { return this.serviceEntry.Service; }
        }

        /// <summary>
        /// 获取服务属性
        /// </summary>
        public IDictionary<string, object> ServiceProperty
        {
            get
            {
                if (string.IsNullOrEmpty(this.serviceEntry.Property))
                    return null;
                if (this.property == null)
                    this.property = this.serviceEntry.ParseStringProperty(this.serviceEntry.Property);
                return this.property;
            }
        }

        /// <summary>
        /// 获取发布服务的查件对象
        /// </summary>
        public IBundle Owner
        {
            get { return this.bundle; }
        }
        #endregion  
  
        #region 方法
        /// <summary>
        /// 检查两个对象是否相等
        /// </summary>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var package = obj as ServicePackageImpl;
            if (package == null) return false;
            if (this == package) return true;

            return this.InterfaceType.Equals(package.InterfaceType) 
                && this.ServiceType.Equals(package.ServiceType);

        }
        #endregion
    }
}
