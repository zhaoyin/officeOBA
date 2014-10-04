using System;
using System.IO;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Collections;
using System.Security.AccessControl;

namespace Cooperativeness.OBA.Word.Install
{
    [RunInstaller(true)]
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public partial class InstallerOba : Installer
    {
        private static readonly Logger Log = new Logger(typeof(InstallerOba));
        public InstallerOba()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            OfficeUtils.SetWinTrust();

            base.Install(stateSaver);
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            Log.Debug("InstallerOba_BeforeInstall");
            double ver = OfficeUtils.ReadWordVersion();
            if (ver <= 11)
            {
                throw new InstallException("您未安装Office或者安装的是Office 2007以下版本的Office!");
            }
            base.OnBeforeInstall(savedState);
        }

        public override void Rollback(IDictionary savedState)
        {
            try
            {
                var dir = this.Context.Parameters["vdir"];
                Log.Debug("安装路径:dir--" + dir);
                OfficeUtils.UnInStall(dir);
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }
            base.Rollback(savedState);
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            var dir = this.Context.Parameters["vdir"];
            Log.Debug("安装路径:dir--" + dir);
            if (!OfficeUtils.Install(dir))
            {
                throw new InstallException("未成功写入注册表键值，可能是权限不够，请使用管理员权限运行该程序！");
            }
            //string pathIcon=Path.Combine(dir,@"Plugins\SceneDesigner\Icons");
            //Log.Debug("PathIcon:" + pathIcon);
            //if (Directory.Exists(pathIcon))
            //{
            //    DirectoryInfo dirIcons = new DirectoryInfo(pathIcon);
            //    DirectorySecurity security = dirIcons.GetAccessControl();
            //    security.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
            //    dirIcons.SetAccessControl(security);
            //    Log.Debug("设置权限完成!");
            //}
            base.OnAfterInstall(savedState);
        }

        protected override void OnAfterUninstall(IDictionary savedState)
        {
            var dir = this.Context.Parameters["vdir"];
            Log.Debug("安装路径:dir--" + dir);
            OfficeUtils.UnInStall(dir);
            base.OnAfterUninstall(savedState);
        }

    }
}
