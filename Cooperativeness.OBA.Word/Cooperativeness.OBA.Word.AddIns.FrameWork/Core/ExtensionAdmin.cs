using System.Collections;
using System.Collections.Specialized;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义扩展管理器
    /// </summary>
    internal class ExtensionAdmin
    {
        #region 字段
        private Framework framework;
        private OrderedDictionary extensionPointsByBundle;
        private OrderedDictionary extensionPointsByName;
        private IList extensions;
        private IList extensionPoints;
        private object lockObj = new object();

        #endregion

        #region 构造函数
        public ExtensionAdmin(Framework framework)
        {
            this.framework = framework;
            extensionPointsByBundle = new OrderedDictionary();
            extensionPointsByName = new OrderedDictionary();
            extensions = new ArrayList();
            extensionPoints = new ArrayList();
        }

        #endregion

        #region 方法
        /// <summary>
        /// 添加扩展点
        /// </summary>
        /// <param name="point"></param>
        public void AddExtensionPoint(IExtensionPoint item)
        {
            lock (lockObj)
            {
                if (item == null) return;
                string key = item.Point.ToLower();
                if (extensionPointsByName.Contains(key))
                {
                    IBundle bundle = item.Owner;
                    key = bundle.ToString();
                    extensionPointsByName.Add(key, item);
                    if (extensionPointsByBundle.Contains(key))
                    {
                        ListDictionary eps = extensionPointsByBundle[key] as ListDictionary;
                        if (eps.Contains(item.Point.ToLower())) return;
                        eps.Add(item.Point, item);
                        extensionPoints.Add(item);
                    }
                    else
                    {
                        ListDictionary eps = new ListDictionary();
                        eps.Add(item.Point, item);
                        extensionPoints.Add(item);
                        extensionPointsByBundle.Add(key, eps);
                    }
                }
            }
        }

        /// <summary>
        /// 添加扩展
        /// </summary>
        /// <param name="point"></param>
        public void AddExtension(IExtension item)
        {
            lock (lockObj)
            {
                if (item == null) return;
                string key = item.Point.ToLower();
                if (extensionPointsByName.Contains(key))
                {
                    IExtensionPoint extensionPoint = extensionPointsByName[key] as IExtensionPoint;
                    extensionPoint.AddExtension(item);
                    extensions.Add(item);
                }
            }
        }

        /// <summary>
        /// 移除扩展点
        /// </summary>
        /// <param name="item"></param>
        public void RemoveExtensionPoint(IExtensionPoint item)
        {
            lock (lockObj)
            {
                if (item == null) return;
                string key = item.Point.ToLower();
                if (extensionPointsByName.Contains(key))
                {
                    // 从根据名称存贮扩展点集合中移除
                    extensionPointsByName.Remove(key);
                    // 从以插件名存贮的扩展点集合中移除
                    IBundle bundle = item.Owner;
                    key = bundle.ToString();
                    ListDictionary eps = extensionPointsByBundle[key] as ListDictionary;
                    if (!eps.Contains(item.Point)) return;
                    eps.Remove(item.Point);
                    // 从扩展点列表中移除
                    extensionPoints.Remove(item);
                    // 检索扩展点所有的扩展
                    foreach (IExtension extension in item.Extensions)
                    {
                        // 从扩展列表中删除
                        extensions.Remove(extension);
                    }
                }
            }
        }

        /// <summary>
        /// 移除扩展点
        /// </summary>
        /// <param name="point"></param>
        public void RemoveExtensionPoint(string point)
        {
            // 获取扩展点
            IExtensionPoint item = null;
            lock (lockObj)
            {
                if (string.IsNullOrEmpty(point)) return;
                string key = point.ToLower();
                if (extensionPointsByName.Contains(key))
                {
                    item = extensionPointsByName[key] as IExtensionPoint;
                }
            }

            this.RemoveExtensionPoint(item);
        }

        /// <summary>
        /// 移除扩展
        /// </summary>
        /// <param name="item"></param>
        public void RemoveExtension(IExtension item)
        {
            lock (lockObj)
            {
                if (item == null) return;
                string key = item.Point.ToLower();
                if (extensionPointsByName.Contains(key))
                {
                    // 从根据名称存贮扩展点集合中获取扩展对应的扩展点
                    IExtensionPoint extensionPoint = extensionPointsByName[key] as IExtensionPoint;
                    extensionPoint.RemoveExtension(item);
                    // 从扩展列表中删除
                    extensions.Remove(item);
                }
            }
        }

        /// <summary>
        /// 根据用户指定的扩展点名获取扩展点对象
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public IExtensionPoint GetExtensionPoint(string point)
        {
            lock (lockObj)
            {
                if (string.IsNullOrEmpty(point)) 
                    return null;

                string key = point;
                if (extensionPointsByName.Contains(key))
                    return extensionPointsByName[key] as IExtensionPoint;

                return null;
            }
        }

        /// <summary>
        /// 获取所有扩展点对象
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public IExtensionPoint[] GetExtensionPoints()
        {
            lock (lockObj)
            {
                int size = extensionPoints.Count;
                IExtensionPoint[] newPoints = new IExtensionPoint[size];
                extensionPoints.CopyTo(newPoints, 0);

                return newPoints;
            }
        }

        /// <summary>
        /// 根据用户指定的扩展点名和插件获取扩展对象
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public IExtensionPoint GetExtension(IBundle bundle, string point)
        {
            lock (lockObj)
            {
                if (bundle==null || string.IsNullOrEmpty(point))
                    return null;

                string key = bundle.ToString();
                if (extensionPointsByName.Contains(key))
                    return extensionPointsByName[key] as IExtensionPoint;

                return null;
            }
        }

        /// <summary>
        /// 获取所有扩展对象
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public IExtension[] GetExtensions()
        {
            lock (lockObj)
            {
                int size = extensions.Count;
                IExtension[] newExtensions = new IExtension[size];
                extensions.CopyTo(newExtensions, 0);

                return newExtensions;
            }
        }

        /// <summary>
        /// 检查是否存在指定的扩展点
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(IExtensionPoint item)
        {
            lock (lockObj)
            {
                return this.extensionPointsByName.Contains(item.Point.ToLower());
            }
        }
        #endregion
    }
}
