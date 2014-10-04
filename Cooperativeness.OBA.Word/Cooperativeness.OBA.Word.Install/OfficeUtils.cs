using System;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;

namespace Cooperativeness.OBA.Word.Install
{
    /// <summary>
    /// 定义Office助手类
    /// </summary>
    public class OfficeUtils
    {
        private readonly static Logger Log=new Logger(typeof(OfficeUtils));
        #region 常量
        private const string RegCooperativenessAddins = @"SOFTWARE\Microsoft\Office\Word\Addins\Cooperativeness.OBA.Word.Addins";
        private const string RegO2007Pia = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{50120000-1105-0000-0000-0000000FF1CE}";
        private const string RegO2010Pia = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{90140000-1105-0000-0000-0000000FF1CE}";
        private const string RegAddinsRsakey = @"Software\Microsoft\VSTO\Security\Inclusion\0a2826fc-8842-4602-83f6-8830c9b4baea";
        private const string OfficeVersionKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\Winword.exe";
        private const string WinTrustRegistryKey =@"Software\Microsoft\Windows\CurrentVersion\WinTrust\Trust Providers\Software Publishing";
        private const string StateSubKey = "State";
        #endregion

        #region 方法
        /// <summary>
        /// 读取Office Word的版本号
        /// </summary>
        /// <returns></returns>
        public static double ReadWordVersion()
        {
            try
            {
                var reg = new RegisterHelper();
                string name = "Path";
                string wordPath = "";
                if (reg.IsRegeditKeyExist(name, OfficeVersionKey, RegisterHelper.RegDomain.LocalMachine))
                {
                    wordPath = reg.ReadRegeditKey(name, OfficeVersionKey, RegisterHelper.RegDomain.LocalMachine).ToString();
                }
                else
                {
                    wordPath = reg.ReadRegeditKey(name, OfficeVersionKey, RegisterHelper.RegDomain.CurrentUser).ToString();
                }
                wordPath = wordPath.Substring(0, wordPath.LastIndexOf('\\'));
                string officeVersion = wordPath.Substring(wordPath.LastIndexOf('\\') + 1).ToUpper();
                const string pattern = @"^OFFICE(?'number'\d+)";
                var regex = new Regex(pattern);
                double wordVersion = double.Parse(regex.Match(officeVersion).Groups["number"].Value);
                return wordVersion;
            }
            catch(Exception e)
            {
                Log.Debug(e);
            }

            return 0;
        }

        /// <summary>
        ///  检测是否安装office
        /// </summary>
        /// <returns></returns>
        public static bool IsOfficeInstalled()
        {
            double officeType = ReadWordVersion();
            if (officeType >0)
                return true;
            return false;
        }

        public static bool IsNeedRunAsAdmin()
        {
            //return true;
            //操作系统版本号6及以上，代表Vista/Win7以后的操作系统。
            if (Environment.OSVersion.Version.Major >= 6)
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                if (identity != null)
                {
                    var principal = new WindowsPrincipal(identity);

                    //如果已使用管理员身份运行
                    return !principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            return false;
        }

        private static bool SetAddinReg(string path,bool bClear)
        {
            try
            {
                Log.Debug("SetAddinReg---Begin");
                // 检查路径是否为空
                if (string.IsNullOrEmpty(path)) return false;
                var domain = RegisterHelper.RegDomain.CurrentUser;
                //if (!IsNeedRunAsAdmin())
                //{
                //    domain = RegisterHelper.RegDomain.LocalMachine;
                //}
                var dir = new DirectoryInfo(path);
                if (!dir.Exists) return false;
                path=path.Replace('/', '\\');
                if (path.EndsWith("\\"))
                {
                    path=path.Remove(path.Length - 1, 1);
                }
                string addinPath = @"#Path#\Bin\Cooperativeness.OBA.Word.Addins.vsto|vstolocal";
                addinPath = addinPath.Replace("#Path#", path);
                Log.Debug("addinReg---addinPath:"+addinPath);
                // 清理卸载插件的注册表
                var reg = new RegisterHelper(RegCooperativenessAddins, domain);
                reg.CreateSubKey();
                Log.Debug("addinReg----CreateSubKey");
                string[] name = { "Description", "FriendlyName", "LoadBehavior", "Manifest" };
                string[] value = { "Cooperativeness.OBA.Word.Addins V2.0", "协同文档工具", "00000003", addinPath };
                for (int i = 0; i < 4; i++)
                {
                    if (reg.IsRegeditKeyExist(name[i]))
                    {
                        reg.DeleteRegeditKey(name[i]);
                        Log.Debug("addinReg----DeleteRegeditKey----name:" + name[i]);
                    }
                    if (!bClear)
                    {
                        if (name[i].Equals("LoadBehavior", StringComparison.InvariantCultureIgnoreCase))
                        {
                            reg.WriteRegeditKey(name[i], value[i], RegisterHelper.RegValueKind.DWord);
                            Log.Debug("addinReg----WriteRegeditKey----name:" + name[i]);
                        }
                        else
                        {
                            reg.WriteRegeditKey(name[i], value[i], RegisterHelper.RegValueKind.String);
                            Log.Debug("addinReg----WriteRegeditKey----name:" + name[i]);
                        }
                    }
                }
                if (bClear)
                {
                    reg.DeleteSubKey();
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Debug(e);
                return false;
            }
        }

        private static bool SetVstoReg(string path,bool bClear)
        {
            try
            {
                // 检查路径是否为空
                if (string.IsNullOrEmpty(path)) return false;
                var domain = RegisterHelper.RegDomain.CurrentUser;
                //if (!IsNeedRunAsAdmin())
                //{
                //    domain = RegisterHelper.RegDomain.LocalMachine;
                //}
                var dir = new DirectoryInfo(path);
                if (!dir.Exists) return false;
                path=path.Replace("\\", "/");
                if (path.EndsWith("/"))
                {
                    path=path.Remove(path.Length - 1, 1);
                }
                string addinPath = @"file://#UrlPath#/Bin/Cooperativeness.OBA.Word.Addins.vsto";
                addinPath = addinPath.Replace("#UrlPath#", path);
                Log.Debug("SetVstoReg---addinPath:"+addinPath);
                // 清理卸载插件的注册表
                var reg = new RegisterHelper(RegAddinsRsakey, domain);
                if (!reg.IsSubKeyExist())
                {
                    reg.CreateSubKey();
                }
                Log.Debug("SetVstoReg----CreateSubKey");
                string[] name = { "Url", "PublicKey"};
                string[] value ={ addinPath, "<RSAKeyValue><Modulus>8Z2Xak1KQWtPxWNvUeAIDi4N8jvlOnCdhvdvZ4CU0HruSqF7LK1+bi89slJuiQ5dsn7y2nbMB8hIV8HXmXEgn6U+wu6Pqcl6uleuzoYbRbTHLbjkoTDTyy6XeDynno9W6vCcKD8CrrKv08Jc60LFgHcqlLwqm6UegL1Uu9IGQmU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>" };
                for (int i = 0; i < 2; i++)
                {
                    if (reg.IsRegeditKeyExist(name[i]))
                    {
                        reg.DeleteRegeditKey(name[i]);
                        Log.Debug("SetVstoReg----DeleteRegeditKey---name:"+name[i]);
                    }
                    if (!bClear)
                    {
                        reg.WriteRegeditKey(name[i], value[i], RegisterHelper.RegValueKind.String);
                        Log.Debug("SetVstoReg----WriteRegeditKey---name:" + name[i]);
                    }
                }
                if (bClear)
                {
                    reg.DeleteSubKey();
                } 
                return true;
            }
            catch (Exception e)
            {
                Log.Debug(e);
                return false;
            }
        }

        private static bool InstallVsto(string path)
        {
            try
            {
                // 检查路径是否为空
                if (string.IsNullOrEmpty(path)) return false;
                var dir = new DirectoryInfo(path);
                if (!dir.Exists) return false;

                string addinPath = Path.Combine(path, @"Bin\Cooperativeness.OBA.Word.Addins.vsto");
                Log.Debug("OfficeUtils--:" + addinPath);
                // 启动注册程序
                var installProcess = new Process();
                installProcess.StartInfo.FileName = addinPath;
                installProcess.EnableRaisingEvents = true;
                //installProcess.StartInfo.Arguments = string.Format(" /s {0}", regPath);
                installProcess.Start();
                installProcess.WaitForExit();

                return true;
            }
            catch (Exception e)
            {
                Log.Debug(e);
                return false;
            }
        }

        /// <summary>
        /// 根据OBA的路径，进行场景设计插件的安装
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Install(string path)
        {
            bool addin = SetAddinReg(path,false);
            bool vsto=SetVstoReg(path,false);
            return addin && vsto; 
        }

        public static bool UnInStall(string path)
        {
            bool addin = SetAddinReg(path, true);
            bool vsto = SetVstoReg(path, true);
            return addin && vsto;
        }

        /// <summary>
        /// 关闭所有场景设计器的的msiexec进程
        /// </summary>
        private static void KillCooperativenessProcess()
        {
            try
            {
                // 获取所有的Msi进程
                Process[] results = Process.GetProcessesByName("msiexec")
                                           .Where(p => p.MainWindowTitle.Equals("Cooperativeness.OBA.Word")).ToArray();
                // Kill掉所有Msi进程
                foreach (var p in results)
                {
                    p.Kill();
                }
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }
        }

        /// <summary>
        /// 获取场景设计器的msiexec进程
        /// </summary>
        /// <returns></returns>
        private static Process GetCooperativenessProcess()
        {
            try
            {
                return Process.GetProcessesByName("msiexec")
                        .Where(p => p.MainWindowTitle.Equals("Cooperativeness.OBA.Word")).FirstOrDefault();
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }

            return null;
        }

        /// <summary>
        /// 获取资源文本
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        private static string GetResourceText(string resourcePath)
        {
            if (!File.Exists(resourcePath)) return string.Empty;
            using (var resourceReader = new StreamReader(resourcePath))
            {
                return resourceReader.ReadToEnd();
            }
        }

        private static string GetRegeditInfo()
        {
            var sbr = new StringBuilder();
            sbr.AppendLine("Windows Registry Editor Version 5.00");
            sbr.AppendLine();
            sbr.AppendLine(@"[HKEY_CURRENT_USER\Software\Microsoft\Office\Word\Addins\Cooperativeness.OBA.Word.Addins]");
            sbr.AppendLine("\"Description\"=\"Cooperativeness.OBA.Word.Addins V2.0\"");
            sbr.AppendLine("\"FriendlyName\"=\"协同工作插件\"");
            sbr.AppendLine("\"LoadBehavior\"=dword:00000003");
            sbr.AppendLine("\"Manifest\"=\"#Path#\\Bin\\Cooperativeness.OBA.Word.Addins.vsto|vstolocal\"");
            sbr.AppendLine();
            sbr.AppendLine(@"[HKEY_CURRENT_USER\Software\Microsoft\VSTO\Security\Inclusion\0a2826fc-8842-4602-83f6-8830c9b4baea]");
            sbr.AppendLine("\"Url\"=\"file:///#UrlPath#/Bin/Cooperativeness.OBA.Word.Addins.vsto\"");
            sbr.AppendLine("\"PublicKey\"=\"<RSAKeyValue><Modulus>8Z2Xak1KQWtPxWNvUeAIDi4N8jvlOnCdhvdvZ4CU0HruSqF7LK1+bi89slJuiQ5dsn7y2nbMB8hIV8HXmXEgn6U+wu6Pqcl6uleuzoYbRbTHLbjkoTDTyy6XeDynno9W6vCcKD8CrrKv08Jc60LFgHcqlLwqm6UegL1Uu9IGQmU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>\"");
            return sbr.ToString();
        }

        #endregion
        #region 安全策略

        //证书吊销状态检查等注册表项更改


        public static void SetWinTrust()
        {
            object uncheckPublisherTag = "146944";

            //不检查发行商的证书吊销状态，这样可以增加VSTO ADD IN 加载及运行速度
            try
            {
                var reg=new RegisterHelper(WinTrustRegistryKey,RegisterHelper.RegDomain.CurrentUser);
                reg.WriteRegeditKey(StateSubKey, uncheckPublisherTag);
            }
            catch(Exception e)
            {
                //不处理
                Log.Debug(e);
            }
        }


        #endregion

        public static Process[] GetWordProcess()
        {
            try
            {
                Process[] procs = null;

                //try
                //{
                //获取WORD进程
                Process[] wordProcess = Process.GetProcessesByName("WINWORD");

                //定义返回
                int procsNumber = 0;

                procsNumber += wordProcess.Length;
                procs = new Process[procsNumber];

                //获取进程信息
                int iTemp = wordProcess.Length;
                int j = 0;
                for (int i = 0; i < iTemp; i++, j++)
                {
                    procs[j] = wordProcess[i];
                }

                return procs;
            }
            catch (Exception e)
            {
                Log.Debug(e);
                return null;
            }

        }

        public static String[] GetWordProcess(Process[] procs)
        {

            string[] procsName = null;
            //try
            //{
            if (procs != null)
            {
                //定义返回
                int procsNumber = procs.Length;
                procsName = new string[procsNumber];

                //获取进程信息
                string strTemp;

                for (int i = 0; i < procsNumber; i++)
                {
                    strTemp = procs[i].MainWindowTitle;
                    if (!String.IsNullOrEmpty(strTemp))
                    {
                        procsName[i] = strTemp;
                    }
                    else
                    {
                        procsName[i] = procs[i].ProcessName;
                    }
                }
            }

            //}
            //catch
            //{


            //}

            return procsName;

        }
    }
}

