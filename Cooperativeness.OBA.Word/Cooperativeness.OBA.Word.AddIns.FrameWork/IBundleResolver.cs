using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFIDA.U8.IN.AddIns.Core;

namespace UFIDA.U8.IN.AddIns.Resolver
{
    /// <summary>
    /// 定义插件解析接口契约
    /// </summary>
    internal interface IBundleResolver
    {
        /// <summary>
        /// 数据完整性解析，对不符合规范的数据进行剔除和隔离
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        bool ResolveForValidation(IList<AbstractBundle> bundles);

        /// <summary>
        /// 解析片段插件并将所有解析正确的片段插件附加到其宿主插件中
        /// </summary>
        /// <param name="bundles"></param>
        void ResolveForAttachFragment(IList<AbstractBundle> bundles);

        /// <summary>
        /// 依赖性解析，并重新调整插件启动的顺序
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        bool ResolveForDependency(IList<AbstractBundle> bundles);

    }
}
