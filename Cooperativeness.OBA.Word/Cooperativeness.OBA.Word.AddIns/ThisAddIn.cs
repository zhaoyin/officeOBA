using System;
using System.Runtime.InteropServices;
using Cooperativeness.OBA.Word.Tools;
using Office = Microsoft.Office.Core;

namespace Cooperativeness.OBA.Word.AddIns
{
    /// <summary>
    /// 插件核心程序
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("553C1FCF-1EB7-4bf7-9C04-B6D3EE4BB19F")]
    [ProgId("Cooperativeness_OBA_Word.Addins")]
    [Serializable]
    public partial class ThisAddIn : IAddinUtilities
    {
        private static readonly Logger Log = new Logger(typeof(ThisAddIn));
        private ServiceAdmin serviceAdmin;
        private AddinsAdmin addinsAdmin;

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            try
            {
                if (serviceAdmin == null)
                {
                    serviceAdmin = new ServiceAdmin(this, addinsAdmin.BundleStarter);
                }
                serviceAdmin.Start();
            }
            catch (Exception ex) 
            {
                Log.Debug(ex);
            }
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            serviceAdmin.Stop();
            addinsAdmin.Shutdown();
        }

        protected override Office.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            try
            {
                // 检查插件管理器是否初始化
                if (addinsAdmin == null)
                {
                    addinsAdmin = new AddinsAdmin();
                    addinsAdmin.Start();
                }
                // 获取插件的功能区
                Office.IRibbonExtensibility ribbon = addinsAdmin.RibbonUi;
                if (ribbon != null) return ribbon;

            }
            catch (Exception e)
            {
                Log.Debug(e);
            }
            return base.CreateRibbonExtensibilityObject();
        }

        #region IAddinUtilities 成员

        private ThisAddIn _addinUtilities;
        protected override object RequestComAddInAutomationService()
        {
            if (_addinUtilities == null)
            {
                _addinUtilities = this;
            }
            return _addinUtilities;
        }

        public bool SaveDocumentByVba()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region VSTO 生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion

    }

    [ComVisible(true)]
    [Guid("C8F2A184-8DCB-400e-BAB2-637AACA088B2")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAddinUtilities
    {
        bool SaveDocumentByVba();
    }
}
