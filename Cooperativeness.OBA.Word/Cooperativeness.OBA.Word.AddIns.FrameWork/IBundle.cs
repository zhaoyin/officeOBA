using System;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义插件接口契约
    /// </summary>
    public interface IBundle
    {
        /// <summary>
        /// 获取插件的标示
        /// </summary>
        long BundleId { get; }

        /// <summary>
        /// 获取插件的位置
        /// </summary>
        string Location { get; }

        /// <summary>
        /// 获取插件的标示名称
        /// </summary>
        string SymbolicName { get; }

        /// <summary>
        /// 获取插件的版本信息
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// 获取插件的上下文
        /// </summary>
        IBundleContext Context { get; }

        /// <summary>
        /// 获取插件最后修改的时间
        /// </summary>
        long LastModified { get; }

        /// <summary>
        /// 获取插件的状态
        /// </summary>
        BundleState State { get; }

        /// <summary>
        /// 加载指类名的类型
        /// </summary>
        /// <param name="className">类的完全限定名</param>
        /// <returns>对象的类型</returns>
        Type LoadClass(string className);

        /// <summary>
        /// 根据资源名加载资源
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        /// <param name="loadMode">资源加载模式</param>
        /// <returns>资源数据</returns>
        object LoadResource(string resourceName, ResourceLoadMode loadMode);

        /// <summary>
        /// 启动插件
        /// </summary>
        void Start();

        /// <summary>
        /// 启动插件
        /// </summary>
        void Start(BundleOptions option);

        /// <summary>
        /// 停止插件
        /// </summary>
        void Stop();

        /// <summary>
        /// 停止插件
        /// </summary>
        void Stop(BundleOptions option);

        /// <summary>
        /// 获取插件所有的服务引用列表
        /// </summary>
        /// <returns>返回服务引用集合</returns>
        IServiceReference[] GetRegisteredServices();

        /// <summary>
        /// 获取当前正在使用的服务的服务引用列表
        /// </summary>
        /// <returns>返回服务引用集合</returns>
        IServiceReference[] GetServicesInUse();
    }
}
