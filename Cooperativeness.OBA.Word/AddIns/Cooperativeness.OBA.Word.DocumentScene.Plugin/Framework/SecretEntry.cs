
using Cooperativeness.OBA.Word.DocumentScene.Plugin.Model;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Framework
{
    /// <summary>
    /// 定义私密数据条目
    /// </summary>
    public class SecretEntry
    {
        #region 属性
        /// <summary>
        /// 获取或设置登录对象
        /// </summary>
        public LoginEntity LoginEntity { get; internal set; }

        /// <summary>
        /// 获取文件的名称
        /// </summary>
        public string BizFileName { get; internal set; }

        /// <summary>
        /// 获取文件的标识
        /// </summary>
        public string BizFileId { get; internal set; }

        /// <summary>
        /// 获取场景标识
        /// </summary>
        public string SceneId { get; internal set; }

        #endregion
    }
}
