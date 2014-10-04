
using System;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;
using Cooperativeness.OBA.Word.Tools;
using Microsoft.Office.Tools;
using OfficeWord = Microsoft.Office.Interop.Word;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Framework
{
    public class CustomTaskPanesAdmin
    {
        private static readonly Logger Log = new Logger(typeof(CustomTaskPanesAdmin));
        private static CustomTaskPaneCollection _customTaskPanes;
        #region 字段
        private IServiceReference _customTaskPaneReference;
        private IBundleContext _context;
        private bool _isActive = false;
        private AbstractSceneContext _sceneContext;

        #endregion

        #region 构造函数
        public CustomTaskPanesAdmin(AbstractSceneContext sceneContext, IBundleContext context)
        {
            this._context = context;
            this._sceneContext = sceneContext;
        }

        #endregion

        public CustomTaskPaneCollection CustomTaskPanes
        {
            get
            {
                CheckValid();
                return GetPanes();
            }
        }

        #region 方法
        /// <summary>
        /// 启动Word应程序管理器
        /// </summary>
        public void Start()
        {
            if (!_isActive)
            {
                string type = "Microsoft.Office.Tools.CustomTaskPaneCollection";
                _customTaskPaneReference = _context.GetServiceReference(type);
                if (_customTaskPaneReference != null)
                {
                    _isActive = true;
                }
            }
        }

        /// <summary>
        /// 停止Word应程序管理器
        /// </summary>
        public void Stop()
        {
            if (_isActive)
            {
                if (_customTaskPaneReference != null)
                {
                    _context.UngetService(_customTaskPaneReference);
                }
                if(_customTaskPanes!=null)
                {
                    _customTaskPanes.Dispose(); //释放对象
                }
            }
        }

        /// <summary>
        /// 校验是否合法
        /// </summary>
        private void CheckValid()
        {
            if (!this._isActive)
                throw new InvalidOperationException("Not start word app admin.");
        }

        /// <summary>
        /// 获取Word应用程序对象
        /// </summary>
        /// <returns></returns>
        private CustomTaskPaneCollection GetPanes()
        {
            if (_customTaskPanes == null)
            {
                _customTaskPanes = this._context.GetService<CustomTaskPaneCollection>(_customTaskPaneReference);
            }
            return _customTaskPanes;
        }

        #endregion
    }
}
