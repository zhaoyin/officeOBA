using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Service
{
    /// <summary>
    /// 定义服务注册对象
    /// </summary>
    public class ServiceRegistrationImpl : IServiceRegistration, IComparable
    {
        #region 字段
        // 框架对象
        private Core.Framework framework;
        // 服务注册器
        private ServiceRegistry registry;
        // 注册服务的上下文
        private BundleContextImpl context;
        // 注册服务的插件
        private AbstractBundle bundle;
        // 注册服务的所有类型集合
        private string[] clazzes;
        // 注册的服务对象
        private object service;
        // 服务的引用对象
        private ServiceReferenceImpl reference;
        // 使用服务的上下文列表
        private IList contextsUsing;
        // 服务注册者的属性
        private ServiceProperties properties;
        // 服务标识
        private long serviceid;
        // 服务等级
        private int serviceranking;
        // 服务注册同步锁
        private object registrationLock = new object();
        // 服务状态
        private int state;
        private static int REGISTERED = 0x00;
        private static int UNREGISTERING = 0x01;
        private static int UNREGISTERED = 0x02;

        #endregion

        #region 构造函数
        internal ServiceRegistrationImpl(ServiceRegistry registry, BundleContextImpl context, String[] clazzes, Object service)
        {
            this.registry = registry;
            this.context = context;
            this.bundle = context.GetBundleImpl();
            this.framework = context.Framework;
            this.clazzes = clazzes; /* must be set before calling createProperties. */
            this.service = service;
            this.serviceid = registry.GetNextServiceId(); /* must be set before calling createProperties. */
            this.contextsUsing = new ArrayList(10);

            lock (registrationLock)
            {
                this.state = REGISTERED;
                /* We leak this from the constructor here, but it is ok
                 * because the ServiceReferenceImpl constructor only
                 * stores the value in a final field without
                 * otherwise using it.
                 */
                this.reference = new ServiceReferenceImpl(this);
            }
        }

        #endregion

        /// <summary>
        /// 完成注册操作
        /// </summary>
        /// <param name="props"></param>
        internal void Register(IDictionary<string, object> props)
        {
            ServiceReferenceImpl reference;
            lock (registry)
            {
                lock (registrationLock)
                {
                    reference = this.reference; /* used to publish event outside sync */
                    this.properties = CreateProperties(props); /* must be valid after unregister is called. */
                }

                registry.AddServiceRegistration(context, this);
            }
        }

        /// <summary>
        /// 更新服务相关的属性
        /// </summary>
        /// <param name="props"></param>
        public void SetProperties(IDictionary<string, object> props)
        {
            ServiceReferenceImpl reference;
            ServiceProperties previousProperties;
            lock (registry)
            {
                lock (registrationLock)
                {
                    if (state != REGISTERED)
                    { /* in the process of unregisterING */
                        throw new IllegalStateException("The service has registered.");
                    }

                    reference = this.reference; /* used to publish event outside sync */
                    previousProperties = this.properties;
                    this.properties = CreateProperties(props);
                }
                registry.ModifyServiceRegistration(context, this);
            }
        }

        /// <summary>
        /// 注销服务
        /// </summary>
        public void Unregister()
        {
            ServiceReferenceImpl reference;
            lock (registry)
            {
                lock (registrationLock)
                {
                    if (state != REGISTERED)
                    { /* in the process of unregisterING */
                        throw new IllegalStateException("The state is invalidate.");
                    }

                    registry.RemoveServiceRegistration(context, this);

                    state = UNREGISTERING; /* mark unregisterING */
                    reference = this.reference; /* used to publish event outside sync */
                }
            }

            /* must not hold the registrationLock when this event is published */
            /// registry.publishServiceEvent(new ServiceEvent(ServiceEvent.UNREGISTERING, reference));

            int size = 0;
            BundleContextImpl[] users = null;

            lock (registrationLock)
            {
                /* we have published the ServiceEvent, now mark the service fully unregistered */
                state = UNREGISTERED;

                size = contextsUsing.Count;
                if (size > 0)
                {
                    users = new BundleContextImpl[size];
                    contextsUsing.CopyTo(users, 0);
                }
            }

            /* must not hold the registrationLock while releasing services */
            for (int i = 0; i < size; i++)
            {
                ReleaseService(users[i]);
            }

            lock (registrationLock)
            {
                contextsUsing.Clear();

                reference = null; /* mark registration dead */
            }

            /* The property field must remain valid after unregister completes. */
        }

        /// <summary>
        /// 获取服务的引用
        /// </summary>
        public IServiceReference Reference
        {
            get { return GetReferenceImpl(); }
        }

        /// <summary>
        /// 获取服务的引用
        /// </summary>
        /// <returns></returns>
        internal ServiceReferenceImpl GetReferenceImpl()
        {
            /* use reference instead of unregistered so that ServiceFactorys, called
             * by releaseService after the registration is unregistered, can
             * get the ServiceReference. Note this technically may violate the spec
             * but makes more sense.
             */
            lock (registrationLock)
            {
                if (reference == null)
                {
                    throw new IllegalStateException("The reference is null.");
                }

                return reference;
            }
        }

        /// <summary>
        /// 构造服务属性
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private ServiceProperties CreateProperties(IDictionary<string, object> p)
        {
            ServiceProperties props = new ServiceProperties(p);

            props.Add(ConfigConstant.OBJECTCLASS, clazzes);
            props.Add(ConfigConstant.SERVICE_ID, serviceid);
            //props.setReadOnly();
            object ranking = props.GetProperty(ConfigConstant.SERVICE_RANKING);

            serviceranking = (ranking is int) ? (int) ranking : 0;

            return props;
        }

        /// <summary>
        /// 获取服务的属性对象
        /// </summary>
        /// <returns></returns>
        public ServiceProperties GetProperties()
        {
            lock (registrationLock)
            {
                return properties;
            }
        }

        /// <summary>
        /// 根据属性名获取服务的属性值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal object GetProperty(string key)
        {
            lock (registrationLock)
            {
                return properties.GetProperty(key);
            }
        }

        /// <summary>
        /// 获取所有的属性名集合
        /// </summary>
        internal string[] PropertyKeys
        {
            get
            {
                lock (registrationLock)
                {
                    return properties.PropertyKeys;
                }
            }
        }

        /// <summary>
        /// 获取服务的标识
        /// </summary>
        internal long Id
        {
            get { return serviceid; }
        }

        /// <summary>
        /// 获取服务的等级
        /// </summary>
        internal int Ranking
        {
            get
            {
                lock (registrationLock)
                {
                    return serviceranking;
                }
            }
        }

        /// <summary>
        /// 获取注册服务的接口契约集合
        /// </summary>
        /// <returns></returns>
        internal string[] GetClasses()
        {
            return clazzes;
        }

        /// <summary>
        /// 获取服务对象
        /// </summary>
        internal object ServiceObject
        {
            get { return service; }
        }

        /// <summary>
        /// 获取注册服务的插件对象
        /// </summary>
        /// <returns></returns>
        internal AbstractBundle GetBundle()
        {
            lock (registrationLock)
            {
                if (reference == null)
                {
                    return null;
                }

                return bundle;
            }
        }

        /// <summary>
        /// 根据用户上下文获取服务对象
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        internal object GetService(BundleContextImpl user)
        {
            lock (registrationLock)
            {
                if (state == UNREGISTERED)
                { /* service unregistered */
                    return null;
                }
            }

            Hashtable servicesInUse = user.GetServicesInUseMap();
            if (servicesInUse == null)
            { /* user is closed */
                user.CheckValid(); /* throw exception */
            }
            /* Use a while loop to support retry if a call to a ServiceFactory fails */
            while (true)
            {
                ServiceUse use;
                bool added = false;
                /* Obtain the ServiceUse object for this service by bundle user */
                lock (servicesInUse)
                {
                    user.CheckValid();
                    use = (ServiceUse)servicesInUse[this];
                    if (use == null)
                    {
                        /* if this is the first use of the service
                         * optimistically record this service is being used. */
                        use = new ServiceUse(user, this);
                        added = true;
                        lock (registrationLock)
                        {
                            if (state == UNREGISTERED)
                            { /* service unregistered */
                                return null;
                            }
                            servicesInUse.Add(this, use);
                            contextsUsing.Add(user);
                        }
                    }
                }

                /* Obtain and return the service object */
                lock (use)
                {
                    /* if another thread removed the ServiceUse, then
                     * go back to the top and Start again */
                    lock (servicesInUse)
                    {
                        user.CheckValid();
                        if (servicesInUse[this] != use)
                        {
                            continue;
                        }
                    }
                    object serviceObject = use.GetService();
                    /* if the service factory failed to return an object and
                     * we created the service use, then remove the 
                     * optimistically added ServiceUse. */
                    if ((serviceObject == null) && added)
                    {
                        lock (servicesInUse)
                        {
                            lock (registrationLock)
                            {
                                servicesInUse.Remove(this);
                                contextsUsing.Remove(user);
                            }
                        }
                    }
                    return serviceObject;
                }
            }
        }

        /// <summary>
        /// 根据用户上下文注销服务
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        internal bool UngetService(BundleContextImpl user)
        {
            lock (registrationLock)
            {
                if (state == UNREGISTERED)
                {
                    return false;
                }
            }

            Hashtable servicesInUse = user.GetServicesInUseMap();
            if (servicesInUse == null)
            {
                return false;
            }

            ServiceUse use;
            lock (servicesInUse)
            {
                use = (ServiceUse)servicesInUse[this];
                if (use == null)
                {
                    return false;
                }
            }

            lock (use)
            {
                if (use.UngetService())
                {
                    /* use count is now zero */
                    lock (servicesInUse)
                    {
                        lock (registrationLock)
                        {
                            servicesInUse.Remove(this);
                            contextsUsing.Remove(user);
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 根据用户上下文释放服务
        /// </summary>
        /// <param name="user"></param>
        internal void ReleaseService(BundleContextImpl user)
        {
            lock (registrationLock)
            {
                if (reference == null)
                { /* registration dead */
                    return;
                }
            }

            Hashtable servicesInUse = user.GetServicesInUseMap();
            if (servicesInUse == null)
            {
                return;
            }
            ServiceUse use;
            lock (servicesInUse)
            {
                lock (registrationLock)
                {
                    if (!servicesInUse.ContainsKey(this))
                        return;
                    use = (ServiceUse)servicesInUse[this];
                    contextsUsing.Remove(user);
                }
            }
            lock (use)
            {
                use.ReleaseService();
            }
        }

        /// <summary>
        /// 返回正在使用服务的插件列表
        /// </summary>
        /// <returns></returns>
        internal AbstractBundle[] GetUsingBundles()
        {
            lock (registrationLock)
            {
                if (state == UNREGISTERED) /* service unregistered */
                    return null;

                int size = contextsUsing.Count;
                if (size == 0)
                    return null;

                /* Copy list of BundleContext into an array of Bundle. */
                AbstractBundle[] bundles = new AbstractBundle[size];
                for (int i = 0; i < size; i++)
                    bundles[i] = ((BundleContextImpl)contextsUsing[i]).GetBundleImpl();

                return bundles;
            }
        }

        internal bool IsAssignableTo(IBundle client, string className)
        {
            //return framework.isServiceAssignableTo(bundle, client, className, service.getClass());
            return true;
        }

        /// <summary>
        /// 转换成字符串操作
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            int size = clazzes.Count();
            StringBuilder sb = new StringBuilder(50 * size);

            sb.Append('{');

            for (int i = 0; i < size; i++)
            {
                if (i > 0)
                {
                    sb.Append(", "); //$NON-NLS-1$
                }
                sb.Append(clazzes[i]);
            }

            sb.Append("}="); //$NON-NLS-1$
            sb.Append(GetProperties().ToString());

            return sb.ToString();
        }

        /// <summary>
        /// 比较当前服务注册对象和用户指定的服务注册对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            ServiceRegistrationImpl other = (ServiceRegistrationImpl)obj;

            int thisRanking = this.Ranking;
            int otherRanking = other.Ranking;
            if (thisRanking != otherRanking) {
                if (thisRanking < otherRanking) {
                    return 1;
                }
                return -1;
            }
            long thisId = this.Id;
            long otherId = other.Id;
            if (thisId == otherId) {
                return 0;
            }
            if (thisId < otherId) {
                return -1;
            }
            return 1;
        }

    }

}
