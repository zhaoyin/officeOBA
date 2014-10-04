
using System.IO;
using System.Xml;
using Cooperativeness.FileTransfer;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.Tools.Language;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin
{
    /// <summary>
    /// 定义场景上下文
    /// </summary>
    internal class SceneContext : AbstractSceneContext
    {
        #region 字段
        private static SceneContext instance;
        private IMultiLanguage multilingualAdmin;
        private string culture;
        private static ClientService _clientService;
        #endregion

        #region 构造函数
        private SceneContext()
        {
            // 初始化多语管理器
            InitMulitingualAdmin();
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取场景上下文单实例对象
        /// </summary>
        public static SceneContext Instance
        {
            get
            {
                if (instance == null)
                    instance = new SceneContext();
                return instance;
            }
        }

        /// <summary>
        /// 获取插件上下文
        /// </summary>
        public override IBundleContext BundleContext
        {
            get { return Activator.Context; }
        }

        public ClientService FileServer
        {
            get
            {
                if (_clientService == null)
                {
                    _clientService = new ClientService();
                    var fileDoc = new XmlDocument();
                    var path = Path.Combine(BundleContext.Bundle.Location, "FileServer.xml");
                    fileDoc.Load(path);
                    _clientService.SetServerInfo(fileDoc.OuterXml);
                }
                return _clientService;
            }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 根据多语名称获取多语值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string GetString(string name)
        {
            if (this.multilingualAdmin != null)
                return this.multilingualAdmin.GetString(name,culture);
            return name;
        }

        /// <summary>
        /// 初始化多语资源管理器
        /// </summary>
        private void InitMulitingualAdmin()
        {
            // 获取当前Office的语言
            this.culture = "zh-cn";  //多语暂时不处理
            // 初始化当前多语管理器
            string baseDir = this.BundleContext.Bundle.Location;
            this.multilingualAdmin = new MultiLanguageDefault(baseDir);
        }
        #endregion

        
    }
}
