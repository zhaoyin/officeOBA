using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Security.Permissions;

namespace Cooperativeness.Files.Server.Install
{
    [RunInstaller(true)]
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public partial class TransferInstall : Installer
    {
        private static readonly Logger Log = new Logger(typeof(TransferInstall));

        private const string IisSubKey =
            @"SYSTEM\CurrentControlSet\Services\W3SVC\Parameters";

        private const string IisVersionName = "MajorVersion";

        public TransferInstall()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            Log.Debug("TransferInstall.OnBeforeInstall---begin");
            if (IisVersion < 7)
            {
                
            }
            base.OnBeforeInstall(savedState);
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);
        }

        protected override void OnAfterUninstall(IDictionary savedState)
        {
            base.OnAfterUninstall(savedState);
        }

        private void SetIisWebSiteByOldWay()
        {
            
        }

        private double IisVersion
        {
            get 
            {
                var reg = new RegisterHelper(IisSubKey, RegisterHelper.RegDomain.LocalMachine);
                if (reg.IsRegeditKeyExist(IisVersionName))
                {
                    double version = 0;
                    object ver = reg.ReadRegeditKey(IisVersionName);
                    if (double.TryParse(ver.ToString(), out version))
                    {
                        return version;
                    }
                }
                return 6;
            }
        }
    }
}
