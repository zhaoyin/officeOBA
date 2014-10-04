using System.Collections.Generic;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Resolver
{
    /// <summary>
    /// 定义解析器契约
    /// </summary>
    internal interface IResolver
    {
        /// <summary>
        /// 解析操作
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        bool Resolve(IList<AbstractBundle> bundles);
    }
}
