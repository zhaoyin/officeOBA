using System;
using System.ComponentModel;
using F=System.Windows.Forms;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Async
{
    /// <summary>
    /// 定义异步工作器
    /// </summary>
    public class AsyncWorker
    {
        private static readonly Logger Log=new Logger(typeof(AsyncWorker));
        #region 代理
        public delegate void WorkHandler(AsyncWorkerEventArgs e);

        #endregion

        #region 字段
        private BackgroundWorker worker = new BackgroundWorker();
        private F.Control _Owner;
        private F.Form _LoaderForm;
        private EventHandler<AsyncWorkerEventArgs> _AsynWork;
        private EventHandler<AsyncWorkerEventArgs> _AsynComplete;
        private EventHandler<AsyncWorkerEventArgs> _AsynReport;

        #endregion

        #region 构造函数
        public AsyncWorker(F.Control owner,F.Form loaderForm)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");
            if (loaderForm == null)
                throw new ArgumentNullException("loaderForm");
            this._Owner = owner;
            this._LoaderForm = loaderForm;
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += RunWorkerCompleted;
            worker.ProgressChanged += new ProgressChangedEventHandler(RunProgressChanged);
        }

        void OnLoaderFormShown(object sender, EventArgs e)
        {
            var args = this._LoaderForm.Tag as AsyncWorkerEventArgs;
            if (args==null) return;
            args.State = WorkerState.Canceled;
        }

        public AsyncWorker(F.Control owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");
            this._Owner = owner;
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += RunWorkerCompleted;
            worker.ProgressChanged += new ProgressChangedEventHandler(RunProgressChanged);
        }

        public AsyncWorker()
        {
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += RunWorkerCompleted;
            worker.ProgressChanged += new ProgressChangedEventHandler(RunProgressChanged);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置Loader窗体
        /// </summary>
        public F.Form LoaderForm
        {
            get
            {
                return this._LoaderForm;
            }
            set
            {
                this._LoaderForm = value;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 启动异步工作器
        /// </summary>
        public void Start(object argument)
        {
            worker.RunWorkerAsync(argument);
            // 显示加载窗体
            if (this._LoaderForm != null)
            {
                this._LoaderForm.Shown += OnLoaderFormShown;
                if (this._Owner != null) this._LoaderForm.ShowDialog(this._Owner);
                else this._LoaderForm.ShowDialog();
            }
        }

        public void Report(AsyncWorkerEventArgs e)
        {
            var args = new AsyncWorkerEventArgs(e.Argument)
            {
                Count = e.Count,
                Num = e.Num,
                State = e.State,
                Tag = e.Tag,
                Tip = e.Tip
            };
            worker.ReportProgress(0, args);
        }

        /// <summary>
        /// 停止异步工作器
        /// </summary>
        public void Cancel()
        {
            worker.CancelAsync();
        }
        #endregion

        #region 事件处理
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            var args = new AsyncWorkerEventArgs(e.Argument);
            args.State = WorkerState.None;
            this.OnWork(args);
            //AsyncWorkerEventArgs arg = new AsyncWorkerEventArgs(e.Argument);
            //arg.State = WorkerState.None;
            //WorkHandler handler = new WorkHandler(this.OnWork);
            //handler.BeginInvoke(arg, null, null);
            //e.Result = arg;
            //while (true)
            //{
            //    if (worker.CancellationPending)
            //    {
            //        e.Cancel = true;
            //        arg.State = WorkerState.Canceled;
            //        break;
            //    }
            //    if (arg.State != WorkerState.None)
            //        break;
            //    // 睡眠5毫秒，让出CUP
            //    System.Threading.Thread.Sleep(5);
            //}
        }

        private void RunProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 触发通知事件
            var arg = e.UserState as AsyncWorkerEventArgs;
            if (_AsynReport != null) _AsynReport.Invoke(this, arg);
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                // 触发完毕事件
                var arg = e.Result as AsyncWorkerEventArgs;
                if (_AsynComplete != null) _AsynComplete.Invoke(this, arg);
                // 关闭加载窗体
                if (this._LoaderForm != null)
                {
                    this._LoaderForm.Shown -= OnLoaderFormShown;
                    this._LoaderForm.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Debug("[DocumentScene]-[AsyncWorker->RunWorkerCompleted]-[StackTrace:{0}]-[Message:{1}]"
                          , ex.StackTrace, ex.Message);
            }
        }

        protected virtual void OnWork(AsyncWorkerEventArgs e)
        {
            try
            {
                if (this._LoaderForm != null)
                {
                    this._LoaderForm.Tag = e;
                }
                if (_AsynWork != null) _AsynWork.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Debug("[DocumentScene]-[AsyncWorker->OnWork]-[StackTrace:{0}]-[Message:{1}]"
                          , ex.StackTrace, ex.Message);
            }
        }

        #endregion

        #region 事件
        /// <summary>
        /// 定义异步处理工作事件
        /// </summary>
        public event EventHandler<AsyncWorkerEventArgs> AsynWork
        {
            add { this._AsynWork += value; }
            remove { this._AsynWork -= value; }
        }

        /// <summary>
        /// 定义异步工作完成事件
        /// </summary>
        public event EventHandler<AsyncWorkerEventArgs> AsynComplete
        {
            add { this._AsynComplete += value; }
            remove { this._AsynComplete -= value; }
        }

        /// <summary>
        /// 定义异步通知工作事件
        /// </summary>
        public event EventHandler<AsyncWorkerEventArgs> AsynReport
        {
            add { this._AsynReport += value; }
            remove { this._AsynReport -= value; }
        }


        #endregion
    }
}
