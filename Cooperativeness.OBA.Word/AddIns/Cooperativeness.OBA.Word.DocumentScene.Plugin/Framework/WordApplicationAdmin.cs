using System;
using System.IO;
using System.Windows.Forms;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;
using Cooperativeness.OBA.Word.Tools;
using OfficeWord = Microsoft.Office.Interop.Word;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Framework
{
    /// <summary>
    /// 定义Word应用对象管理器
    /// </summary>
    public class WordApplicationAdmin
    {
        private static readonly Logger Log=new Logger(typeof(WordApplicationAdmin));
        private static OfficeWord.Application _app;
        #region 字段
        private IServiceReference wordApplicationReference;
        private IBundleContext context;
        private bool isActive = false;
        private AbstractSceneContext sceneContext;

        #endregion

        #region 构造函数
        public WordApplicationAdmin(AbstractSceneContext sceneContext, IBundleContext context)
        {
            this.context = context;
            this.sceneContext = sceneContext;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取Word应用程序对象
        /// </summary>
        public OfficeWord.Application Application
        {
            get 
            {
                CheckValid();
                return GetApp(); 
            }
        }

        /// <summary>
        /// 获取当前活动的文档对象
        /// </summary>
        public OfficeWord.Document ActiveDocument
        {
            get
            {
                try
                {
                    var app = this.Application;
                    if (app != null)
                    {
                        return app.ActiveDocument;
                    }
                }
                catch (Exception e)
                {
                    Log.Debug(e);
                }
                return null;
            }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 启动Word应程序管理器
        /// </summary>
        public void Start()
        {
            if (!isActive)
            {
                string type = "Microsoft.Office.Interop.Word.Application";
                wordApplicationReference = context.GetServiceReference(type);
                if (wordApplicationReference != null)
                {
                    OfficeWord.Application app = this.GetApp();
                    if (app != null)
                    {
                        app.DocumentOpen += OnDocumentOpen;
                        app.DocumentChange += OnDocumentChange;
                        app.DocumentBeforeSave += OnDocumentBeforeSave;
                        app.DocumentBeforeClose += OnDocumentBeforeClose;
                        app.WindowActivate += OnWindowActivate;
                        app.WindowSelectionChange += OnSelectionChange;
                    }
                    isActive = true;
                }
            }
        }

        /// <summary>
        /// Word文档选择变化
        /// </summary>
        /// <param name="Sel"></param>
        private void OnSelectionChange(OfficeWord.Selection Sel)
        {
            //this.sceneContext.RefreshRibbon("SceneDesinger.LinkAdmin.LinkScene");
            //this.sceneContext.RefreshRibbon("SceneDesinger.LinkAdmin.LinkFunc");
            //this.sceneContext.RefreshRibbon("SceneDesinger.LinkAdmin.LinkDocument");
        }

        /// <summary>
        /// 文档激活
        /// </summary>
        /// <param name="Doc"></param>
        /// <param name="Wn"></param>
        private void OnWindowActivate(OfficeWord.Document Doc, OfficeWord.Window Wn)
        {
            this.sceneContext.RefreshRibbon();
        }

        /// <summary>
        /// 关闭Word之前出发的事件
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="cancel"></param>
        private void OnDocumentBeforeClose(OfficeWord.Document doc, ref bool cancel)
        {
            Log.Debug("[Document close:清理私密数据]");
            if (sceneContext.SecretDataAdmin.Get(doc) != null)
            {
                cancel = true;
                MessageBox.Show("该文件通过文档管理工具打开，请点击文档管理工具中的关闭按钮来关闭!");
            }
            //sceneContext.SecretDataAdmin.Remove(doc);
        }

        /// <summary>
        /// 在保存Word之间触发的事件
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="SaveAsUI"></param>
        /// <param name="Cancel"></param>
        private void OnDocumentBeforeSave(OfficeWord.Document doc, ref bool SaveAsUI, ref bool Cancel)
        {
            if (sceneContext.SecretDataAdmin.Get(doc) != null)
            {
                Cancel = true;
                MessageBox.Show("该文件通过文档管理工具打开，请点击文档管理工具中的保存按钮来保存!");
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 打开Word文件时触发的事件
        /// </summary>
        /// <param name="doc"></param>
        private void OnDocumentOpen(OfficeWord.Document doc)
        {
            //// 设置文档视图样式
            //Doc.ActiveWindow.View.Type = OfficeWord.WdViewType.wdPrintView;

            // 设置可见
            doc.ActiveWindow.Visible = true;
        }

        /// <summary>
        /// 文档变化触发的事件
        /// </summary>
        private void OnDocumentChange()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 停止Word应程序管理器
        /// </summary>
        public void Stop()
        {
            if (isActive)
            {
                if (wordApplicationReference != null)
                {
                    context.UngetService(wordApplicationReference);
                }
                var app = this.Application;
                if (app != null)
                {
                    app.DocumentOpen -= OnDocumentOpen;
                    app.DocumentChange -= OnDocumentChange;
                    app.DocumentBeforeSave -= OnDocumentBeforeSave;
                    app.DocumentBeforeClose -= OnDocumentBeforeClose;
                    object unKnown = Type.Missing;
                    app.Quit(ref unKnown, ref unKnown, ref unKnown);
                }
            }
        }

        /// <summary>
        /// 校验是否合法
        /// </summary>
        private void CheckValid()
        {
            if(!this.isActive)
                throw new InvalidOperationException("Not start word app admin.");
        }

        /// <summary>
        /// 获取Word应用程序对象
        /// </summary>
        /// <returns></returns>
        private OfficeWord.Application GetApp()
        {
            if (_app == null)
            {
                _app = this.context.GetService<OfficeWord.Application>(wordApplicationReference);
            }
            return _app;
        }

        #endregion
    }
}
