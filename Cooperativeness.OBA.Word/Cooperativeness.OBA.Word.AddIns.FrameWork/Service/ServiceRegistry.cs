using System;
using System.Collections;
using System.Collections.Generic;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Service
{
    /// <summary>
    /// 定义服务注册表
    /// </summary>
    internal class ServiceRegistry
    {
        private static readonly Logger Log = new Logger(typeof(ServiceRegistry));
        #region 字段
        public const int SERVICEEVENT = 3;
        private Hashtable publishedServicesByClass;
        private ArrayList allPublishedServices;
        private Hashtable publishedServicesByContext;
        private long serviceid;
        private Hashtable serviceEventListeners;

        private const int initialCapacity = 50;
        private const int initialSubCapacity = 10;
        private Core.Framework framework;
        private object serviceidlock = new object();

        #endregion

        #region 构造函数
        public ServiceRegistry(Core.Framework framework)
        {
            this.framework = framework;
            serviceid = 1;
            publishedServicesByClass = new Hashtable(initialCapacity);
            publishedServicesByContext = new Hashtable(initialCapacity);
            allPublishedServices = new ArrayList(initialCapacity);
            serviceEventListeners = new Hashtable(initialCapacity);
        }

        #endregion

        #region 方法
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="clazzes"></param>
        /// <param name="service"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public ServiceRegistrationImpl RegisterService(BundleContextImpl context, string[] clazzes, object service, IDictionary<string, object> properties)
        {
            if (service == null)
            {
                throw new ArgumentException("service");
            }

            int size = clazzes.Length;

            if (size == 0)
            {

                throw new ArgumentException("size");
            }

            /* copy the array so that changes to the original will not affect us. */
            IList copy = new ArrayList(size);
            // intern the strings and remove duplicates
            for (int i = 0; i < size; i++)
            {
                string clazz = string.Intern(clazzes[i]);
                if (!copy.Contains(clazz))
                {
                    copy.Add(clazz);
                }
            }

            clazzes = new string[copy.Count];
            copy.CopyTo(clazzes, 0);


            if (!(service is IServiceFactory))
            {
                //string invalidService = checkServiceClass(clazzes, service);
                //if (string.IsNullOrEmpty(invalidService))
                //{
                //    throw new ArgumentException("invalidService");
                //}
            }

            var registration = new ServiceRegistrationImpl(this, context, clazzes, service);
            registration.Register(properties);

            return registration;
        }

        /// <summary>
        /// 根据过滤条件获取服务引用
        /// </summary>
        /// <param name="context"></param>
        /// <param name="clazz"></param>
        /// <param name="filterstring"></param>
        /// <param name="allservices"></param>
        /// <returns></returns>
        public ServiceReferenceImpl[] GetServiceReferences(BundleContextImpl context, string clazz, string filterstring, bool allservices)
        {
            IFilter filter = (filterstring == null) ? null : context.createFilter(filterstring);
            ArrayList references = ChangeRegistrationsToReferences(LookupServiceRegistrations(clazz, filter)) as ArrayList;
            if (references == null) return null;

            IList listWaitForDeleting = new ArrayList();
            var iter = references.GetEnumerator();
            while (iter.MoveNext())
            {
                ServiceReferenceImpl reference = (ServiceReferenceImpl)iter.Current;
                if (!allservices && !IsAssignableTo(context, reference))
                    listWaitForDeleting.Add(iter.Current);
            }
            foreach (var item in listWaitForDeleting)
                references.Remove(item);

            int size = references.Count;
            if (size == 0)
            {
                return null;
            }

            var newReferences = new ServiceReferenceImpl[size];
            references.CopyTo(newReferences);

            return newReferences;
        }


        /// <summary>
        /// 根据服务契约获取服务引用
        /// </summary>
        /// <param name="context"></param>
        /// <param name="clazz"></param>
        /// <returns></returns>
        public ServiceReferenceImpl GetServiceReference(BundleContextImpl context, string clazz)
        {
            try
            {
                ServiceReferenceImpl[] references = GetServiceReferences(context, clazz, null, false);

                if (references != null)
                {
                    // Since we maintain the registrations in a sorted List, the first element is always the
                    // correct one to return.
                    return references[0];
                }
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }

            return null;
        }

        /// <summary>
        /// 根据服务引用获取服务对象
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public object GetService(BundleContextImpl context, ServiceReferenceImpl reference)
        {
            return reference.Registration.GetService(context);
        }

        /// <summary>
        /// 根据服务引用释放服务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public bool UngetService(BundleContextImpl context, ServiceReferenceImpl reference)
        {
            ServiceRegistrationImpl registration = reference.Registration;

            return registration.UngetService(context);
        }

        /// <summary>
        /// 获取所有的服务引用
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ServiceReferenceImpl[] GetRegisteredServices(BundleContextImpl context)
        {
            IList references = ChangeRegistrationsToReferences(LookupServiceRegistrations(context));
            if (references == null) return null;
            int size = references.Count;
            if (size == 0)
            {
                return null;
            }

            var newReferences = new ServiceReferenceImpl[size];
            references.CopyTo(newReferences, 0);

            return newReferences;
        }

        /// <summary>
        /// 获取已被使用的所有的服务引用
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ServiceReferenceImpl[] GetServicesInUse(BundleContextImpl context)
        {
            Hashtable servicesInUse = context.GetServicesInUseMap();
            if (servicesInUse == null)
            {
                return null;
            }

            IList references;
            lock (servicesInUse)
            {
                if (servicesInUse.Count == 0)
                {
                    return null;
                }
                references = ChangeRegistrationsToReferences(new ArrayList(servicesInUse.Keys));
            }

            if (references != null)
            {
                int size = references.Count;
                if (size == 0)
                {
                    return null;
                }

                ServiceReferenceImpl[] newArray = new ServiceReferenceImpl[size];
                references.CopyTo(newArray, 0);

                return newArray;
            }

            return null;
        }

        /// <summary>
        /// 注销服务
        /// </summary>
        /// <param name="context"></param>
        public void UnregisterServices(BundleContextImpl context)
        {
            IList registrations = LookupServiceRegistrations(context);
            if (registrations == null) return;
            var iter = registrations.GetEnumerator();
            while (iter.MoveNext())
            {
                ServiceRegistrationImpl registration = (ServiceRegistrationImpl)iter.Current;
                try
                {
                    registration.Unregister();
                }
                catch (Exception e)
                {
                    Log.Debug(e);
                    /* already unregistered */
                }
            }
            RemoveServiceRegistrations(context); // remove empty list
        }

        /// <summary>
        /// 释放使用中的服务
        /// </summary>
        /// <param name="context"></param>
        public void ReleaseServicesInUse(BundleContextImpl context)
        {
            Hashtable servicesInUse = context.GetServicesInUseMap();
            if (servicesInUse == null)
            {
                return;
            }
            IList registrations;
            lock (servicesInUse)
            {
                if (servicesInUse.Count == 0)
                {
                    return;
                }
                registrations = new ArrayList(servicesInUse.Keys);
            }

            var iter = registrations.GetEnumerator();
            while (iter.MoveNext())
            {
                var registration = (ServiceRegistrationImpl)iter.Current;
                try
                {
                    if (registration != null)
                    {
                        registration.ReleaseService(context);
                    }
                }
                catch (IllegalStateException e)
                {
                    Log.Debug(e);
                    /* already unregistered */
                }
            }
        }


        /// <summary>
        /// 获取下一个服务标识
        /// </summary>
        /// <returns></returns>
        internal long GetNextServiceId()
        {
            lock (serviceidlock)
            {
                long id = serviceid;
                serviceid++;
                return id;
            }
        }

        /// <summary>
        /// 添加服务注册
        /// </summary>
        /// <param name="context"></param>
        /// <param name="registration"></param>
        internal void AddServiceRegistration(BundleContextImpl context, ServiceRegistrationImpl registration)
        {
            // Add the ServiceRegistrationImpl to the list of Services published by BundleContextImpl.
            ArrayList contextServices = (ArrayList)publishedServicesByContext[context];
            if (contextServices == null)
            {
                contextServices = new ArrayList(initialSubCapacity);
                publishedServicesByContext.Add(context, contextServices);
            }
            // The list is NOT sorted, so we just add
            contextServices.Add(registration);

            // Add the ServiceRegistrationImpl to the list of Services published by Class Name.
            String[] clazzes = registration.GetClasses();
            int insertIndex;
            for (int i = 0, size = clazzes.Length; i < size; i++)
            {
                String clazz = clazzes[i];

                ArrayList services = (ArrayList)publishedServicesByClass[clazz];

                if (services == null)
                {
                    services = new ArrayList(initialSubCapacity);
                    publishedServicesByClass.Add(clazz, services);
                }

                // The list is sorted, so we must find the proper location to insert
                insertIndex = -services.BinarySearch(registration) - 1;
                services.Insert(insertIndex, registration);
            }

            // Add the ServiceRegistrationImpl to the list of all published Services.
            // The list is sorted, so we must find the proper location to insert
            insertIndex = -allPublishedServices.BinarySearch(registration) - 1;
            allPublishedServices.Insert(insertIndex, registration);
        }

        /// <summary>
        /// 修改服务注册
        /// </summary>
        /// <param name="context"></param>
        /// <param name="registration"></param>
        internal void ModifyServiceRegistration(BundleContextImpl context, ServiceRegistrationImpl registration)
        {
            // The list of Services published by BundleContextImpl is not sorted, so
            // we do not need to modify it.

            // Remove the ServiceRegistrationImpl from the list of Services published by Class Name
            // and then add at the correct index.
            String[] clazzes = registration.GetClasses();
            int insertIndex;
            for (int i = 0, size = clazzes.Length; i < size; i++)
            {
                String clazz = clazzes[i];
                ArrayList services = (ArrayList)publishedServicesByClass[clazz];
                services.Remove(registration);
                // The list is sorted, so we must find the proper location to insert
                insertIndex = -services.BinarySearch(registration) - 1;
                services.Insert(insertIndex, registration);
            }

            // Remove the ServiceRegistrationImpl from the list of all published Services
            // and then add at the correct index.
            allPublishedServices.Remove(registration);
            // The list is sorted, so we must find the proper location to insert
            insertIndex = -allPublishedServices.BinarySearch(registration) - 1;
            allPublishedServices.Insert(insertIndex, registration);
        }

        /// <summary>
        /// 删除服务注册
        /// </summary>
        /// <param name="context"></param>
        /// <param name="registration"></param>
        internal void RemoveServiceRegistration(BundleContextImpl context, ServiceRegistrationImpl registration)
        {
            // Remove the ServiceRegistrationImpl from the list of Services published by BundleContextImpl.
            IList contextServices = (IList)publishedServicesByContext[context];
            if (contextServices != null)
            {
                contextServices.Remove(registration);
            }

            // Remove the ServiceRegistrationImpl from the list of Services published by Class Name.
            String[] clazzes = registration.GetClasses();
            for (int i = 0, size = clazzes.Length; i < size; i++)
            {
                String clazz = clazzes[i];
                IList services = (IList)publishedServicesByClass[clazz];
                services.Remove(registration);
                if (services.Count == 0)
                { // remove empty list
                    publishedServicesByClass.Remove(clazz);
                }
            }

            // Remove the ServiceRegistrationImpl from the list of all published Services.
            allPublishedServices.Remove(registration);
        }

        /// <summary>
        /// 根据过滤条件和接口契约查找服务注册者
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IList LookupServiceRegistrations(string clazz, IFilter filter)
        {
            IList result;
            lock (this)
            {
                if (clazz == null)
                { /* all services */
                    result = allPublishedServices;
                }
                else
                {
                    /* services registered under the class name */
                    result = (IList)publishedServicesByClass[clazz];
                }

                if ((result == null) || (result.Count == 0))
                {
                    return null;
                }

                result = new ArrayList(result); /* make a new list since we don't want to change the real list */
            }

            if (filter == null)
            {
                return result;
            }

            IList listWaitForDeleting = new ArrayList();
            var iter = result.GetEnumerator();
            while (iter.MoveNext())
            {
                ServiceRegistrationImpl registration = (ServiceRegistrationImpl)iter.Current;
                ServiceReferenceImpl reference;
                try
                {
                    reference = registration.GetReferenceImpl();
                }
                catch (Exception e)
                {
                    Log.Warn(e);
                    listWaitForDeleting.Add(iter.Current); /* service was unregistered after we left the synchronized block above */
                    continue;
                }
                if (!filter.Match(reference))
                {
                    listWaitForDeleting.Add(iter.Current);
                }
            }

            foreach (var item in listWaitForDeleting)
                result.Remove(item);

            return result;
        }

        /// <summary>
        /// 查找服务注册者集合
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IList LookupServiceRegistrations(BundleContextImpl context)
        {
            IList result = (IList)publishedServicesByContext[context];

            if ((result == null) || (result.Count == 0))
            {
                return null;
            }

            return new ArrayList(result); /* make a new list since we don't want to change the real list */
        }

        /// <summary>
        /// 移除服务的注册者
        /// </summary>
        /// <param name="context"></param>
        private void RemoveServiceRegistrations(BundleContextImpl context)
        {
            publishedServicesByContext.Remove(context);
        }

        /// <summary>
        /// 将服务注册器变更为服务引用
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static IList ChangeRegistrationsToReferences(IList result)
        {
            if (result == null) return null;
            ArrayList listWaitForDeleting = new ArrayList();
            for (int i = 0; i < result.Count; i++)
            {
                var registration = (ServiceRegistrationImpl)result[i];
                ServiceReferenceImpl reference;
                try
                {
                    reference = registration.GetReferenceImpl();
                }
                catch (Exception ex)
                {
                    Log.Warn(ex);
                    listWaitForDeleting.Add(result[i]); /* service was unregistered after we were called */
                    continue;
                }
                result[i] = reference;
            }
            foreach (var item in listWaitForDeleting)
                result.Remove(item);

            return result;
        }

        /// <summary>
        /// 检查是否可以分配给指定的服务引用
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        internal static bool IsAssignableTo(BundleContextImpl context, ServiceReferenceImpl reference)
        {
            AbstractBundle bundle = context.GetBundleImpl();
            string[] clazzes = reference.GetClasses();
            for (int i = 0, len = clazzes.Length; i < len; i++)
                if (!reference.IsAssignableTo(bundle, clazzes[i]))
                    return false;
            return true;
        }

        #endregion
    }
}
