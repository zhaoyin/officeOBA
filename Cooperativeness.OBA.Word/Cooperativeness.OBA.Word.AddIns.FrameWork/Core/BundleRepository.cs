using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义插件仓库
    /// </summary>
    internal sealed class BundleRepository
    {
        #region 私有字段
        /** 以安装顺序存储的插件集合 */
        private IList bundlesByInstallOrder;
        /** 以插件标识键存贮的插件集合 */
        private Hashtable bundlesById;
        /** 以插件标识名为键存贮的插件集合 */
        private Hashtable bundlesBySymbolicName;
        /** 以插件安装位置为键存储的插件集合 */
        private Hashtable bundlesByLocation;
        /** 插件框架对象 */
        private Framework framework;
        /** 线程同步锁,确保线程安全 */
        private object syncLock = new object();

        #endregion

        #region 构造函数
        public BundleRepository(Framework framework)
        {
            this.framework = framework;
            bundlesByInstallOrder = new ArrayList();
            bundlesById = new Hashtable();
            bundlesBySymbolicName = new Hashtable();
            bundlesByLocation = new Hashtable();
        }

        #endregion

        #region 方法
        /// <summary>
        /// 获取所有的已安装的并且按照安装顺序排序的插件列表
        /// </summary>
        /// <returns></returns>
        public IList GetBundles()
        {
            return this.bundlesByInstallOrder;
        }

        /// <summary>
        /// 根据安装位置获取插件
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public AbstractBundle GetBundle(string location)
        {
            lock (syncLock)
            {
                if (string.IsNullOrEmpty(location))
                    return null;
                string key = location;
                if (bundlesByLocation.ContainsKey(key))
                    return bundlesByLocation[key] as AbstractBundle;
                return null;
            }
        }

        /// <summary>
        /// 根据Bundle的Id获取Bundle
        /// </summary>
        /// <param name="bundleId"></param>
        /// <returns></returns>
        public AbstractBundle GetBundle(long bundleId)
        {
            lock (syncLock)
            {
                long key = bundleId;
                if (bundlesById.ContainsKey(key))
                    return bundlesById[key] as AbstractBundle;
                return null;
            }
        }

        /// <summary>
        /// 根据插件名获取插件列表
        /// </summary>
        /// <param name="symbolicName"></param>
        /// <returns></returns>
        public AbstractBundle[] GetBundles(string symbolicName)
        {
            return (AbstractBundle[])bundlesBySymbolicName[symbolicName];
        }

        /// <summary>
        /// 根据插件名和插件版本号获取指定的插件
        /// </summary>
        /// <param name="symbolicName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public AbstractBundle GetBundle(string symbolicName, Version version)
        {
            AbstractBundle[] bundles = GetBundles(symbolicName);
            if (bundles != null)
            {
                if (bundles.Count() > 0)
                {
                    for (int i = 0; i < bundles.Length; i++)
                    {
                        if (bundles[i].Version.Equals(version))
                        {
                            return bundles[i];
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 添加插件
        /// </summary>
        /// <param name="bundle"></param>
        public void Add(AbstractBundle bundle)
        {
            lock (syncLock)
            {
                if (bundle != null)
                {
                    bundlesByInstallOrder.Add(bundle);
                    bundlesById.Add(bundle.BundleId, bundle);
                    bundlesByLocation.Add(bundle.Location, bundle);
                    AddSymbolicName(bundle);
                }
            }
        }

        /// <summary>
        /// 将插件插入到以插件唯一表示名存贮的集合中
        /// </summary>
        /// <param name="bundle"></param>
        private void AddSymbolicName(AbstractBundle bundle)
        {

            string symbolName = bundle.SymbolicName;
            AbstractBundle[] bundles = GetBundles(symbolName);
            string key = symbolName;
            if (bundles == null)
            {
                bundles = new AbstractBundle[1];
                bundles[0] = bundle;
                bundlesBySymbolicName.Add(key, bundles);
                return;
            }

            List<AbstractBundle> list = new List<AbstractBundle>();
            // find place to insert the bundle
            Version newVersion = bundle.Version;
            bool added = false;
            for (int i = 0; i < bundles.Length; i++)
            {
                AbstractBundle oldBundle = bundles[i];
                Version oldVersion = oldBundle.Version;
                if (!added && newVersion.CompareTo(oldVersion) >= 0)
                {
                    added = true;
                    list.Add(bundle);
                }
                list.Add(oldBundle);
            }
            if (!added)
            {
                list.Add(bundle);
            }

            bundlesBySymbolicName[key] = list.ToArray();
        }

        /// <summary>
        /// 删除指定的插件
        /// </summary>
        /// <param name="bundle"></param>
        public void Remove(AbstractBundle bundle)
        {
            lock (syncLock)
            {
                // 移除指定Bundle标识ID的Bundle
                bundlesById.Remove(bundle.BundleId);

                // 移除指定Bundle路径的Bundle
                bundlesByLocation.Remove(bundle.Location);

                // 按安装顺序移除指定的Bundle
                bundlesByInstallOrder.Remove(bundle);

                // 通过标识名移除指定的Bundle
                String symbolicName = bundle.SymbolicName;
                if (string.IsNullOrEmpty(symbolicName))
                    return;
                RemoveSymbolicName(symbolicName, bundle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbolicName"></param>
        /// <param name="bundle"></param>
        private void RemoveSymbolicName(string symbolicName, AbstractBundle bundle)
        {
            string key = symbolicName;
            if (!bundlesBySymbolicName.ContainsKey(key)) return;
            AbstractBundle[] bundles = bundlesBySymbolicName[symbolicName] as AbstractBundle[];

            // found some bundles with the global name.
            // remove all references to the specified bundle.
            int numRemoved = 0;
            for (int i = 0; i < bundles.Length; i++)
            {
                if (bundle == bundles[i])
                {
                    numRemoved++;
                    bundles[i] = null;
                }
            }
            if (numRemoved > 0)
            {
                if (bundles.Length - numRemoved <= 0)
                {
                    // no bundles left in the array remove the array from the hash
                    bundlesBySymbolicName.Remove(key);
                }
                else
                {
                    // create a new array with the null entries removed.
                    AbstractBundle[] newBundles = new AbstractBundle[bundles.Length - numRemoved];
                    int indexCnt = 0;
                    for (int i = 0; i < bundles.Length; i++)
                    {
                        if (bundles[i] != null)
                        {
                            newBundles[indexCnt] = bundles[i];
                            indexCnt++;
                        }
                    }
                    bundlesBySymbolicName[key] = newBundles;
                }
            }
        }

        /// <summary>
        /// 移除所有的插件
        /// </summary>
        public void RemoveAllBundles()
        {
            lock (syncLock)
            {
                bundlesByInstallOrder.Clear();
                bundlesById.Clear();
                bundlesBySymbolicName.Clear();
                bundlesByLocation.Clear();
            }
        }

        #endregion
    }
}
