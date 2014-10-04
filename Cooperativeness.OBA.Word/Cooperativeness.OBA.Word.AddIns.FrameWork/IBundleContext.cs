using System;
using System.Collections.Generic;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Events;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义插件上下文
    /// </summary>
    public interface IBundleContext
    {
        /// <summary>
        /// 扩展变化事件
        /// </summary>
        event EventHandler<ExtensionEventArgs> ExtensionChanged;

        /// <summary>
        /// 扩展点变化事件
        /// </summary>
        event EventHandler<ExtensionPointEventArgs> ExtensionPointChanged;

        /// <summary>
        /// 获取当前上下文所属的插件对象
        /// </summary>
        IBundle Bundle { get; }

        /// <summary>
        /// 获取所有已安装的插件列表
        /// </summary>
        IBundle[] Bundles { get; }

        /// <summary>
        /// 获取服务注册者
        /// </summary>
        /// <param name="clazzes"></param>
        /// <param name="service"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        IServiceRegistration RegisterService(string[] clazzes,
			object service, IDictionary<string,object> properties);

	    /// <summary>
	    /// 注册服务
	    /// </summary>
	    /// <param name="clazz"></param>
	    /// <param name="service"></param>
	    /// <param name="property"></param>
	    /// <returns></returns>
	    IServiceRegistration RegisterService(string clazz, object service,
			    IDictionary<string,object> properties);

        /// <summary>
        /// 获取服务引用列表
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
	    IServiceReference[] GetServiceReferences(string clazz, string filter);

	    /// <summary>
        /// 获取服务引用列表
	    /// </summary>
	    /// <param name="clazz"></param>
	    /// <param name="filter"></param>
	    /// <returns></returns>
	    IServiceReference[] GetAllServiceReferences(string clazz, string filter);

        /// <summary>
        /// 获取服务引用
        /// </summary>
        /// <param name="clazz"></param>
        /// <returns></returns>
	    IServiceReference GetServiceReference(string clazz);

	    /// <summary>
	    /// 获取服务对象
	    /// </summary>
	    /// <param name="reference"></param>
	    /// <returns></returns>
	    object GetService(IServiceReference reference);

        /// <summary>
        /// 获取服务对象
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        T GetService<T>(IServiceReference reference);
	
        /// <summary>
        /// 注销服务
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
	    bool UngetService(IServiceReference reference);

        /// <summary>
        /// 注册解析程序集的钩子
        /// </summary>
        /// <param name="hook"></param>
        void RegisterAssemblyResolverHook(IAssemblyResolverHook hook);
    }
}
