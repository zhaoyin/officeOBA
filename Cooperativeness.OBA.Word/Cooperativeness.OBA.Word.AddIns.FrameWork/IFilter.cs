using System.Collections.Generic;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义过滤器接口契约
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// 使用服务的属性进行匹配
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        bool Match(IServiceReference reference);

        /// <summary>
        /// 根据属性集合进行匹配
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        bool Match(IDictionary<string, object> dictionary);


        /// <summary>
        /// 根据属性集合进行大小写敏感匹配
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        bool MatchCase(IDictionary<string, object> dictionary);
    }

}
