using System.Xml.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork;

namespace Cooperativeness.OBA.Word.Ribbon.Resolver
{
    /// <summary>
    /// 定义功能区解析器
    /// </summary>
    internal interface IRibbonResolver
    {
        /// <summary>
        /// 解析操作
        /// </summary>
        /// <param name="element"></param>
        /// <param name="bundle"></param>
        /// <returns></returns>
        void Resolve(XElement element, IBundle bundle);
    }
}
