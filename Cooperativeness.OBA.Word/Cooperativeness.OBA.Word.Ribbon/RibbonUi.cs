using System;
using Cooperativeness.OBA.Word.Tools;
using Office = Microsoft.Office.Core;
using System.Runtime.InteropServices;
using Cooperativeness.OBA.Word.Ribbon.Model;
using System.Reflection;
using System.IO;

namespace Cooperativeness.OBA.Word.Ribbon
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("50C7E87F-DCA8-4fc5-B6FE-9C98B706A22E")]
    [ProgId("Cooperativeness_OBA_Word.Ribbon")]
    [Serializable]
    public class RibbonUi : Office.IRibbonExtensibility
    {
        private static readonly Logger Log = new Logger(typeof(RibbonUi));
        #region 字段
        private XRibbon xRibbon;
        private RibbonAdminImpl ribbonAdmin;

        #endregion

        #region 构造函数
        public RibbonUi()
        {

        }

        internal RibbonUi(RibbonAdminImpl ribbonAdmin)
        {
            this.xRibbon = ribbonAdmin.xRibbon;
            this.ribbonAdmin = ribbonAdmin;
        }

        #endregion

        #region Tab回调
        public string GetTabKeytip(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonTab = ribbonAdmin.Find<XRibbonTab>(control.Id);
                if (xRibbonTab != null)
                {
                    return xRibbonTab.Keytip;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Tab:{0}]-[RibbonExtensibility->GetTabKeytip]-[Trace:{1}]-[Messge:{2}]", control.Id, ex.StackTrace, ex.Message);
            }

            return RibbonDefaultValue.Keytip;
        }

        public bool GetTabVisible(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonTab = ribbonAdmin.Find<XRibbonTab>(control.Id);
                if (xRibbonTab != null)
                {
                    return xRibbonTab.Visible;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Tab:{0}]-[RibbonExtensibility->GetTabVisible]-[Trace:{1}]-[Messge:{2}]"
                                    , control.Id
                                    , ex.StackTrace
                                    , ex.Message);
            }

            return RibbonDefaultValue.Visible;
        }

        public string GetTabLabel(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonTab = ribbonAdmin.Find<XRibbonTab>(control.Id);
                if (xRibbonTab != null)
                {
                    return xRibbonTab.Label;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Tab:{0}]-[RibbonExtensibility->GetTabLabel]-[Trace:{1}]-[Messge:{2}]"
                                    , control.Id
                                    , ex.StackTrace
                                    , ex.Message);
            }

            return RibbonDefaultValue.Label;
        }
        #endregion

        #region Group回调
        public string GetGroupKeytip(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonGroup = ribbonAdmin.Find<XRibbonGroup>(control.Id);
                if (xRibbonGroup != null)
                {
                    return xRibbonGroup.Keytip;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Group:{0}]-[RibbonExtensibility->GetGroupKeytip]-[Trace:{1}]-[Messge:{2}]"
                                    , control.Id
                                    , ex.StackTrace
                                    , ex.Message);
            }

            return RibbonDefaultValue.Keytip;
        }

        public string GetGroupScreentip(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonGroup = ribbonAdmin.Find<XRibbonGroup>(control.Id);
                if (xRibbonGroup != null)
                {
                    return xRibbonGroup.Screentip;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Group:{0}]-[RibbonExtensibility->GetGroupScreentip]-[Trace:{1}]-[Messge:{2}]"
                                    , control.Id
                                    , ex.StackTrace
                                    , ex.Message);
            }

            return RibbonDefaultValue.Screentip;
        }

        public string GetGroupSupertip(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonGroup = ribbonAdmin.Find<XRibbonGroup>(control.Id);
                if (xRibbonGroup != null)
                {
                    return xRibbonGroup.Supertip;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Group:{0}]-[RibbonExtensibility->GetGroupSupertip]-[Trace:{1}]-[Messge:{2}]"
                                    , control.Id
                                    , ex.StackTrace
                                    , ex.Message);
            }

            return RibbonDefaultValue.Supertip;
        }

        public bool GetGroupVisible(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonGroup = ribbonAdmin.Find<XRibbonGroup>(control.Id);
                if (xRibbonGroup != null)
                {
                    return xRibbonGroup.Visible;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Group:{0}]-[RibbonExtensibility->GetGroupVisible]-[Trace:{1}]-[Messge:{2}]"
                                    , control.Id
                                    , ex.StackTrace
                                    , ex.Message);
            }

            return RibbonDefaultValue.Visible;
        }

        public string GetGroupLabel(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonGroup = ribbonAdmin.Find<XRibbonGroup>(control.Id);
                if (xRibbonGroup != null)
                {
                    return xRibbonGroup.Label;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Group:{0}]-[RibbonExtensibility->GetGroupLabel]-[Trace:{1}]-[Messge:{2}]"
                                    , control.Id
                                    , ex.StackTrace
                                    , ex.Message);
            }

            return RibbonDefaultValue.Label;
        }

        #endregion

        #region Button回调
        public string GetButtonKeytip(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.Keytip;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Button:{0}]-[RibbonExtensibility->GetButtonKeytip]-[Trace:{1}]-[Messge:{2}]"
                                    , control.Id
                                    , ex.StackTrace
                                    , ex.Message);
            }

            return RibbonDefaultValue.Keytip;
        }

        public bool GetButtonVisible(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.Visible;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Button:{0}]-[RibbonExtensibility->GetButtonVisible]-[Trace:{1}]-[Messge:{2}]"
                                    , control.Id
                                    , ex.StackTrace
                                    , ex.Message);
            }

            return RibbonDefaultValue.Visible;
        }

        public string GetButtonLabel(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.Label;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Button:{0}]-[RibbonExtensibility->GetButtonLabel]-[Trace:{1}]-[Messge:{2}]"
                                    , control.Id
                                    , ex.StackTrace
                                    , ex.Message);
            }

            return RibbonDefaultValue.Label;
        }

        public bool GetButtonEnabled(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.Enabled;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Button:{0}]-[RibbonExtensibility->GetButtonEnabled]-[Trace:{1}]-[Messge:{2}]"
                                    , control.Id
                                    , ex.StackTrace
                                    , ex.Message);
            }

            return RibbonDefaultValue.Enabled;
        }

        public string GetButtonScreenTip(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.Screentip;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Button:{0}]-[RibbonExtensibility->GetButtonScreenTip]-[Trace:{1}]-[Messge:{2}]"
                                    , control.Id
                                    , ex.StackTrace
                                    , ex.Message);
            }

            return RibbonDefaultValue.Screentip;
        }

        public bool GetButtonShowImage(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.ShowImage;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Button:{0}]-[RibbonExtensibility->GetButtonShowImage]-[Trace:{1}]-[Messge:{2}]"
                                    , control.Id
                                    , ex.StackTrace
                                    , ex.Message);
            }

            return RibbonDefaultValue.ShowImage;
        }

        public bool GetButtonShowLabel(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.ShowLabel;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Button:{0}]-[RibbonExtensibility->GetButtonShowLabel]-[Trace:{1}]-[Messge:{2}]"
                                   , control.Id
                                   , ex.StackTrace
                                   , ex.Message);
            }

            return RibbonDefaultValue.ShowLabel;
        }

        public string GetButtonSuperTip(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.Supertip;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Button:{0}]-[RibbonExtensibility->GetButtonSuperTip]-[Trace:{1}]-[Messge:{2}]"
                                   , control.Id
                                   , ex.StackTrace
                                   , ex.Message);
            }

            return RibbonDefaultValue.Supertip;
        }

        public Office.RibbonControlSize GetButtonSize(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.ControlSize;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Button:{0}]-[RibbonExtensibility->GetButtonSize]-[Trace:{1}]-[Messge:{2}]"
                                   , control.Id
                                   , ex.StackTrace
                                   , ex.Message);
            }

            return RibbonDefaultValue.ControlSize;
        }

        public System.Drawing.Bitmap GetButtonImage(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.Image;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Button:{0}]-[RibbonExtensibility->GetButtonImage]-[Trace:{1}]-[Messge:{2}]"
                                   , control.Id
                                   , ex.StackTrace
                                   , ex.Message);
            }

            return RibbonDefaultValue.Image;
        }

        public void OnButtonAction(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonButton>(control.Id);
                if (xRibbonButton != null)
                {
                    xRibbonButton.OnAction(new RibbonEventArgs(control));
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Button:{0}]-[RibbonExtensibility->OnButtonAction]-[Trace:{1}]-[Messge:{2}]"
                                   , control.Id
                                   , ex.StackTrace
                                   , ex.Message);
            }
        }

        #endregion

        #region ToggleButton回调
        public string GetToggleButtonKeytip(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonToggleButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.Keytip;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[ToggleButton:{0}]-[RibbonExtensibility->GetToggleButtonKeytip]-[Trace:{1}]-[Messge:{2}]"
                                   , control.Id
                                   , ex.StackTrace
                                   , ex.Message);
            }

            return RibbonDefaultValue.Keytip;
        }

        public bool GetToggleButtonVisible(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonToggleButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.Visible;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[ToggleButton:{0}]-[RibbonExtensibility->GetToggleButtonVisible]-[Trace:{1}]-[Messge:{2}]"
                                   , control.Id
                                   , ex.StackTrace
                                   , ex.Message);
            }

            return RibbonDefaultValue.Visible;
        }

        public string GetToggleButtonLabel(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonToggleButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.Label;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[ToggleButton:{0}]-[RibbonExtensibility->GetToggleButtonLabel]-[Trace:{1}]-[Messge:{2}]"
                                   , control.Id
                                   , ex.StackTrace
                                   , ex.Message);
            }

            return RibbonDefaultValue.Label;
        }

        public bool GetToggleButtonEnabled(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonToggleButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.Enabled;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[ToggleButton:{0}]-[RibbonExtensibility->GetToggleButtonEnabled]-[Trace:{1}]-[Messge:{2}]"
                                   , control.Id
                                   , ex.StackTrace
                                   , ex.Message);
            }

            return RibbonDefaultValue.Enabled;
        }

        public string GetToggleButtonScreenTip(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonToggleButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.Screentip;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[ToggleButton:{0}]-[RibbonExtensibility->GetToggleButtonScreenTip]-[Trace:{1}]-[Messge:{2}]"
                                   , control.Id
                                   , ex.StackTrace
                                   , ex.Message);
            }

            return RibbonDefaultValue.Screentip;
        }

        public bool GetToggleButtonShowImage(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonToggleButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.ShowImage;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[ToggleButton:{0}]-[RibbonExtensibility->GetToggleButtonShowImage]-[Trace:{1}]-[Messge:{2}]"
                                  , control.Id
                                  , ex.StackTrace
                                  , ex.Message);
            }

            return RibbonDefaultValue.ShowImage;
        }

        public bool GetToggleButtonShowLabel(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonToggleButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.ShowLabel;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[ToggleButton:{0}]-[RibbonExtensibility->GetToggleButtonShowLabel]-[Trace:{1}]-[Messge:{2}]"
                                  , control.Id
                                  , ex.StackTrace
                                  , ex.Message);
            }

            return RibbonDefaultValue.ShowLabel;
        }

        public string GetToggleButtonSuperTip(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonToggleButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.Supertip;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[ToggleButton:{0}]-[RibbonExtensibility->GetToggleButtonSuperTip]-[Trace:{1}]-[Messge:{2}]"
                                  , control.Id
                                  , ex.StackTrace
                                  , ex.Message);
            }

            return RibbonDefaultValue.Supertip;
        }

        public Microsoft.Office.Core.RibbonControlSize GetToggleButtonSize(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonToggleButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.ControlSize;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[ToggleButton:{0}]-[RibbonExtensibility->GetToggleButtonSize]-[Trace:{1}]-[Messge:{2}]"
                                  , control.Id
                                  , ex.StackTrace
                                  , ex.Message);
            }

            return RibbonDefaultValue.ControlSize;
        }

        public bool GetToggleButtonPressed(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonToggleButton>(control.Id);
                if (xRibbonButton != null)
                {
                    return xRibbonButton.Pressed;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[ToggleButton:{0}]-[RibbonExtensibility->GetToggleButtonPressed]-[Trace:{1}]-[Messge:{2}]"
                                  , control.Id
                                  , ex.StackTrace
                                  , ex.Message);
            }

            return RibbonDefaultValue.Pressed;
        }

        public void OnToggleButtonAction(Office.IRibbonControl control, bool pressed)
        {
            try
            {
                var xRibbonButton = ribbonAdmin.Find<XRibbonToggleButton>(control.Id);
                if (xRibbonButton != null)
                {
                    xRibbonButton.OnAction(new RibbonToggleButtonEventArgs(control, pressed));
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[ToggleButton:{0}]-[RibbonExtensibility->OnToggleButtonAction]-[Trace:{1}]-[Messge:{2}]"
                                 , control.Id
                                 , ex.StackTrace
                                 , ex.Message);
            }
        }

        #endregion

        #region Menu回调
        public string GetMenuKeytip(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonMenu = ribbonAdmin.Find<XRibbonMenu>(control.Id);
                if (xRibbonMenu != null)
                {
                    return xRibbonMenu.Keytip;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Menu:{0}]-[RibbonExtensibility->GetMenuKeytip]-[Trace:{1}]-[Messge:{2}]"
                                 , control.Id
                                 , ex.StackTrace
                                 , ex.Message);
            }

            return RibbonDefaultValue.Keytip;
        }

        public bool GetMenuVisible(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonMenu = ribbonAdmin.Find<XRibbonMenu>(control.Id);
                if (xRibbonMenu != null)
                {
                    return xRibbonMenu.Visible;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Menu:{0}]-[RibbonExtensibility->GetMenuVisible]-[Trace:{1}]-[Messge:{2}]"
                                , control.Id
                                , ex.StackTrace
                                , ex.Message);
            }

            return RibbonDefaultValue.Visible;
        }

        public string GetMenuLabel(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonMenu = ribbonAdmin.Find<XRibbonMenu>(control.Id);
                if (xRibbonMenu != null)
                {
                    return xRibbonMenu.Label;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Menu:{0}]-[RibbonExtensibility->GetMenuLabel]-[Trace:{1}]-[Messge:{2}]"
                                , control.Id
                                , ex.StackTrace
                                , ex.Message);
            }

            return RibbonDefaultValue.Label;
        }

        public bool GetMenuEnabled(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonMenu = ribbonAdmin.Find<XRibbonMenu>(control.Id);
                if (xRibbonMenu != null)
                {
                    return xRibbonMenu.Enabled;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Menu:{0}]-[RibbonExtensibility->GetMenuEnabled]-[Trace:{1}]-[Messge:{2}]"
                                , control.Id
                                , ex.StackTrace
                                , ex.Message);
            }

            return RibbonDefaultValue.Enabled;
        }

        public string GetMenuScreenTip(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonMenu = ribbonAdmin.Find<XRibbonMenu>(control.Id);
                if (xRibbonMenu != null)
                {
                    return xRibbonMenu.Screentip;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Menu:{0}]-[RibbonExtensibility->GetMenuScreenTip]-[Trace:{1}]-[Messge:{2}]"
                               , control.Id
                               , ex.StackTrace
                               , ex.Message);
            }

            return RibbonDefaultValue.Screentip;
        }

        public bool GetMenuShowImage(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonMenu = ribbonAdmin.Find<XRibbonMenu>(control.Id);
                if (xRibbonMenu != null)
                {
                    return xRibbonMenu.ShowImage;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Menu:{0}]-[RibbonExtensibility->GetMenuShowImage]-[Trace:{1}]-[Messge:{2}]"
                              , control.Id
                              , ex.StackTrace
                              , ex.Message);
            }

            return RibbonDefaultValue.ShowImage;
        }

        public bool GetMenuShowLabel(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonMenu = ribbonAdmin.Find<XRibbonMenu>(control.Id);
                if (xRibbonMenu != null)
                {
                    return xRibbonMenu.ShowLabel;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Menu:{0}]-[RibbonExtensibility->GetMenuShowLabel]-[Trace:{1}]-[Messge:{2}]"
                              , control.Id
                              , ex.StackTrace
                              , ex.Message);
            }

            return RibbonDefaultValue.ShowLabel;
        }

        public string GetMenuSuperTip(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonMenu = ribbonAdmin.Find<XRibbonMenu>(control.Id);
                if (xRibbonMenu != null)
                {
                    return xRibbonMenu.Supertip;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Menu:{0}]-[RibbonExtensibility->GetMenuSuperTip]-[Trace:{1}]-[Messge:{2}]"
                              , control.Id
                              , ex.StackTrace
                              , ex.Message);
            }

            return RibbonDefaultValue.Supertip;
        }

        public System.Drawing.Bitmap GetMenuImage(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonMenu = ribbonAdmin.Find<XRibbonMenu>(control.Id);
                if (xRibbonMenu != null)
                {
                    return xRibbonMenu.Image;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Menu:{0}]-[RibbonExtensibility->GetMenuImage]-[Trace:{1}]-[Messge:{2}]"
                              , control.Id
                              , ex.StackTrace
                              , ex.Message);
            }

            return RibbonDefaultValue.Image;
        }

        public Microsoft.Office.Core.RibbonControlSize GetMenuSize(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonMenu = ribbonAdmin.Find<XRibbonMenu>(control.Id);
                if (xRibbonMenu != null)
                {
                    return xRibbonMenu.ControlSize;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Menu:{0}]-[RibbonExtensibility->GetMenuSize]-[Trace:{1}]-[Messge:{2}]"
                              , control.Id
                              , ex.StackTrace
                              , ex.Message);
            }

            return RibbonDefaultValue.ControlSize;
        }

        #endregion

        #region Separator回调
        public bool GetSeparatorVisible(Office.IRibbonControl control)
        {
            try
            {
                var xRibbonSeparator = ribbonAdmin.Find<XRibbonSeparator>(control.Id);
                if (xRibbonSeparator != null)
                {
                    return xRibbonSeparator.Visible;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[Separator:{0}]-[RibbonExtensibility->GetSeparatorVisible]-[Trace:{1}]-[Messge:{2}]"
                              , control.Id
                              , ex.StackTrace
                              , ex.Message);
            }

            return RibbonDefaultValue.Visible;
        }

        #endregion

        #region IRibbonExtensibility 成员
        /// <summary>
        /// 获取用户自定义的UI XML数据
        /// </summary>
        /// <param name="ribbonId"></param>
        /// <returns></returns>
        public string GetCustomUI(string ribbonId)
        {
            try
            {
                if (xRibbon != null)
                {
                    string xml = xRibbon.Serialize();
                    Log.Debug("[Ribbon:{0}]", xml);
                    return xml;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Error("[RibbonExtensibility->GetCustomUI]-[Trace:{0}]-[Messge:{1}]"
                              , ex.StackTrace
                              , ex.Message);
            }

            return string.Empty;
        }

        #endregion

        #region 功能区回调
        //在此创建回调方法。有关添加回调方法的详细信息，请在解决方案资源管理器中选择“功能区 XML”项，然后按 F1

        public void Ribbon_Load(Office.IRibbonUI ribbonUi)
        {
            if (xRibbon != null)
            {
                xRibbon.RibbonUI = ribbonUi;
            }
        }

        #endregion

        #region 帮助器

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (var resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
