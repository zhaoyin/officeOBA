using System;
using System.Windows.Forms;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;
using Cooperativeness.OBA.Word.DocumentScene.Plugin.Forms;
using Cooperativeness.OBA.Word.DocumentScene.Plugin.Framework;
using Cooperativeness.OBA.Word.DocumentScene.Plugin.Model;
using Cooperativeness.OBA.Word.Ribbon;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin
{
    /// <summary>
    /// 定义抽象的场景上下文
    /// </summary>
    public abstract class AbstractSceneContext : ISceneContext
    {
        private static readonly Logger Log=new Logger(typeof(AbstractSceneContext));
        #region 字段
        private SecretDataAdmin secretDataAdmin;
        private WordApplicationAdmin appAdmin;
        private IRibbonAdmin ribbonAdmin;
        private IServiceReference ribbonAdminReference;
        private CustomTaskPanesAdmin _taskPanesAdmin;
        #endregion

        private bool _isLogin = false;
        #region 属性
        /// <summary>
        /// 获取一个值，该值用来指示当前用户是否已登录
        /// </summary>
        public virtual bool IsLogin
        {
            get
            {
                return _isLogin;
                //return this.LoginEntity != null;
            }
        }

        /// <summary>
        /// 获取登录对象
        /// </summary>
        public virtual LoginEntity LoginEntity
        {
            get
            {
                // 检查内部登录
                CheckInternalLogin();
                // 获取当前文档
                var document = this.WordAppAdmin.ActiveDocument;
                if (document == null) return null;
                // 检查私密数据管理中是否存在指定文档
                var entry = this.SecretDataAdmin.Get(document) as SecretEntry;
                if (entry == null) return null;

                return entry.LoginEntity;

            }
        }

        /// <summary>
        /// 获取插件上下文
        /// </summary>
        public abstract IBundleContext BundleContext { get; }

        /// <summary>
        /// 获取私密数据管理器
        /// </summary>
        public SecretDataAdmin SecretDataAdmin
        {
            get
            {
                if (secretDataAdmin == null)
                    secretDataAdmin = new SecretDataAdmin();
                return secretDataAdmin;
            }
        }

        /// <summary>
        /// 获取功能区管理器
        /// </summary>
        public IRibbonAdmin RibbonAdmin
        {
            get
            {
                if (ribbonAdmin == null)
                {
                    if (BundleContext == null) return null;
                    string type = typeof(IRibbonAdmin).FullName;
                    ribbonAdminReference = BundleContext.GetServiceReference(type);
                    if (ribbonAdminReference != null)
                    {
                        ribbonAdmin = this.BundleContext
                                        .GetService<IRibbonAdmin>(ribbonAdminReference);
                    }
                }

                return ribbonAdmin;
            }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 获取Word应用程序对象管理器
        /// </summary>
        public virtual WordApplicationAdmin WordAppAdmin 
        {
            get
            {
                // 初始化Word应用程序管理器
                if (this.appAdmin == null)
                    InitApplicationAdmin();

                return this.appAdmin;
            }
        }

        public virtual CustomTaskPanesAdmin CustomTaskPanesAdmin
        {
            get
            {
                //初始化用户TaskPanes
                if (this._taskPanesAdmin == null)
                {
                    InitCustomTaskPanesAdmin();
                }
                return this._taskPanesAdmin;
            }
        }

        /// <summary>
        /// 获取多语值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract string GetString(string name);

        /// <summary>
        /// 关闭当前上下文
        /// </summary>
        public virtual void Close() 
        {
            if (this.appAdmin != null)
                this.appAdmin.Stop();
            if (this._taskPanesAdmin != null)
            {
                this._taskPanesAdmin.Stop();
            }
            if (this.ribbonAdminReference != null)
            {
                BundleContext.UngetService(this.ribbonAdminReference);
            }
        }

        /// <summary>
        /// 刷新整个功能区
        /// </summary>
        public void RefreshRibbon()
        {
            IRibbonAdmin admin = this.RibbonAdmin;
            if (admin != null)
            {
                admin.Invalidate();
            }
        }

        /// <summary>
        /// 刷新指定的功能区按钮
        /// </summary>
        /// <param name="controlId"></param>
        public void RefreshRibbon(string controlId)
        {
            IRibbonAdmin admin = this.RibbonAdmin;
            if (admin != null)
            {
                admin.InvalidateControl(controlId);
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public bool Login()
        {
            try
            {
                var login=new LoginForm();
                login.ShowDialog();
                _isLogin = login.IsLogin;
                //LoginAdmin loginAdmin = new LoginAdmin();
                //if (!loginAdmin.Login(null)) return false;
                //// 初始化权限管理器
                //AuthAdmin authAdmin = new AuthAdmin(loginAdmin.LoginEntity);
                //// 获取当前文档
                //var document = this.WordAppAdmin.ActiveDocument;
                //if (document == null) return false;
                //// 检查私密数据管理中是否存在指定文档
                //SecretEntry entry = this.SecretDataAdmin.Get(document) as SecretEntry;
                //if (entry == null)
                //{
                //    entry = new SecretEntry()
                //        {
                //            LoginEntity = loginAdmin.LoginEntity,
                //            AuthAdmin = authAdmin
                //        };
                //    this.SecretDataAdmin.Set(document, entry);
                //}
                //else
                //{
                //    entry.LoginEntity = loginAdmin.LoginEntity;
                //    entry.AuthAdmin = authAdmin;
                //}

                return true;
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }

            return false;
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public void Logout()
        {
            try
            {
                // 获取当前文档
                var document = this.WordAppAdmin.ActiveDocument;
                if (document == null) return;
                // 检查私密数据管理中是否存在指定文档
                //this.SecretDataAdmin.Remove(document);
                _isLogin = false;
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }
        }

        /// <summary>
        /// 初始化Word应用上下文
        /// </summary>
        private void InitApplicationAdmin()
        {
            this.appAdmin = new WordApplicationAdmin(this, this.BundleContext);
            this.appAdmin.Start();
        }

        private void InitCustomTaskPanesAdmin()
        {
            this._taskPanesAdmin=new CustomTaskPanesAdmin(this,this.BundleContext);
            this._taskPanesAdmin.Start();
        }

        /// <summary>
        /// 检查是否登录
        /// </summary>
        private void CheckInternalLogin()
        {
            try
            {
                //// 获取当前文档
                //var document = this.WordAppAdmin.ActiveDocument;
                //if (document == null) return;
                //// 检查私密数据管理中是否存在指定文档
                //var entry = this.SecretDataAdmin.Get(document) as SecretEntry;
                //if (entry == null)
                //{
                //    // 尝试从文档区中加载
                //    string xmlns = "---命名空间----";
                //    string xml = document.SelectPartXmlByNamespace(xmlns);
                //    if (!string.IsNullOrEmpty(xml))
                //    {

                //        // 定义异步工作者
                //        Async.AsyncWorker asynWorker = new Async.AsyncWorker();
                //        // 创建加载器
                //        LoadingDialog loadingDialog = new LoadingDialog();
                //        loadingDialog.Text = "登录";
                //        loadingDialog.TipText = "正在登录中.....";
                //        asynWorker.LoaderForm = loadingDialog;
                //        // 创建异步业务处理者
                //        asynWorker.AsynWork += new EventHandler<AsyncWorkerEventArgs>((sender, e) =>
                //        {
                //            try
                //            {
                //                XNamespace xNamespace = xmlns;
                //                XDocument xDoc = XDocument.Parse(xml);
                //                // 检查登录信息是否存在
                //                XElement xUserToken = xDoc.Root.Element(xNamespace + "Token");
                //                if (xUserToken != null && !string.IsNullOrEmpty(xUserToken.Value))
                //                {
                //                    // 获取有效时间,并检查时间之有效性
                //                    XElement xValidTime = xDoc.Root.Element(xNamespace + "ValidTime");
                //                    if (xValidTime != null && !string.IsNullOrEmpty(xValidTime.Value))
                //                    {
                //                        DateTime now = DateTime.Now;
                //                        DateTime validTime = DateTime.Parse(xValidTime.Value);
                //                        TimeSpan timeSpan = now - validTime;
                //                        if (timeSpan.TotalMinutes > 10)
                //                        {
                //                            // 更新
                //                            document.DeletePartByNamespace(xmlns);
                //                            document.Save();
                //                            loadingDialog.TipText = "登录失败,登录信息无效。";
                //                            Thread.Sleep(200);
                //                            e.State = WorkerState.Completed;
                //                            return;
                //                        }
                //                    }
                //                    // 登录操作
                //                    LoginAdmin loginAdmin = new LoginAdmin();
                //                    if (loginAdmin.LoginByToken(xUserToken.Value))
                //                    {
                //                        // 初始化权限管理器
                //                        AuthAdmin authAdmin = new AuthAdmin(loginAdmin.LoginEntity);
                //                        entry = new SecretEntry()
                //                        {
                //                            LoginEntity = loginAdmin.LoginEntity,
                //                            AuthAdmin = authAdmin
                //                        };
                //                        // 检查文件名是否存在
                //                        XElement xSceneId = xDoc.Root.Element(xNamespace + "SceneId");
                //                        if (xSceneId != null && !string.IsNullOrEmpty(xSceneId.Value))
                //                        {
                //                            entry.SceneId = xSceneId.Value;
                //                        }
                //                        // 检查文件标识是否存在
                //                        XElement xBizFileId = xDoc.Root.Element(xNamespace + "BizFileId");
                //                        if (xBizFileId != null && !string.IsNullOrEmpty(xBizFileId.Value))
                //                        {
                //                            entry.BizFileId = xBizFileId.Value;
                //                        }
                //                        // 检查文件名是否存在
                //                        XElement xBizFileName = xDoc.Root.Element(xNamespace + "BizFileName");
                //                        if (xBizFileName != null && !string.IsNullOrEmpty(xBizFileName.Value))
                //                        {
                //                            entry.BizFileName = xBizFileName.Value;
                //                        }
                //                        // 更新
                //                        document.DeletePartByNamespace(xmlns);
                //                        document.Save();

                //                        this.SecretDataAdmin.Set(document, entry);
                //                        e.State = WorkerState.Completed;
                //                    }
                //                }
                //            }
                //            catch (Exception ex)
                //            {
                //                loadingDialog.TipText = "登录失败。";
                //                Thread.Sleep(200);
                //                e.State = WorkerState.Completed;
                //            }
                //        });
                //        // 启动异步工作者
                //        asynWorker.Start(null);



                //    }
                //}
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }
        }

        /// <summary>
        /// 检查是否具有权限
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public bool CheckAuth(string menuId)
        {
            return true;
            //// 获取当前文档
            //var document = this.WordAppAdmin.ActiveDocument;
            //if (document == null) return true;
            //// 检查私密数据管理中是否存在指定文档
            //SecretEntry entry = this.SecretDataAdmin.Get(document) as SecretEntry;
            //if (entry == null) return true;
            //// 确认是否有权限
            //if (entry.AuthAdmin == null) return true;
            //return entry.AuthAdmin.CheckAuth(menuId);
        }

        #endregion
    }
}
