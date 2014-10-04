using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Service
{
    /// <summary>
    /// 定义服务引用对象
    /// </summary>
    public class ServiceReferenceImpl : IServiceReference, IComparable
    {
        #region 字段
        // 注册服务对象
        private readonly ServiceRegistrationImpl registration;

        #endregion

        #region 构造函数
        public ServiceReferenceImpl(ServiceRegistrationImpl registration)
        {
            this.registration = registration;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取服务注册对象
        /// </summary>
        public ServiceRegistrationImpl Registration
        {
            get { return registration; }
        }

        /// <summary>
        /// 获取所有属性的键集合
        /// </summary>
        public string[] PropertyKeys
        {
            get { return registration.PropertyKeys; }
        }

        /// <summary>
        /// 获取注册服务的插件对象
        /// </summary>
        public IBundle Bundle
        {
            get { return registration.GetBundle(); }
        }

        /// <summary>
        /// 获取当前所有使用该服务的插件对象
        /// </summary>
        public IBundle[] UsingBundles
        {
            get { return registration.GetUsingBundles(); }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetProperty(string key)
        {
            return registration.GetProperty(key);
        }

        /// <summary>
        /// 检查是否可以分配给指定的类
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public bool IsAssignableTo(IBundle bundle, string className)
        {
            return registration.IsAssignableTo(bundle, className);
        }

        /// <summary>
        /// 比较操作
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            ServiceRegistrationImpl other = ((ServiceReferenceImpl) obj).Registration;

            int thisRanking = registration.Ranking;
            int otherRanking = other.Ranking;
            if (thisRanking != otherRanking) {
                if (thisRanking < otherRanking) {
                    return -1;
                }
                return 1;
            }
            long thisId = registration.Id;
            long otherId = other.Id;
            if (thisId == otherId) {
                return 0;
            }
            if (thisId < otherId) {
                return 1;
            }
            return -1;
        }

        /// <summary>
        /// 获取当前对象的哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return registration.GetHashCode();
        }

        /// <summary>
        /// 检查当前对象是否与指定的服务引用对象相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == this) {
                return true;
            }

            if (!(obj is ServiceReferenceImpl)) {
                return false;
            }

            ServiceReferenceImpl other = (ServiceReferenceImpl)obj;

            return registration == other.registration;
        }

        /// <summary>
        /// 转换为字符串操作
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return registration.ToString();
        }

        /// <summary>
        /// 获取所有服务注册的接口契约
        /// </summary>
        /// <returns></returns>
        internal string[] GetClasses()
        {
            return registration.GetClasses();
        }
        #endregion
    }
}
