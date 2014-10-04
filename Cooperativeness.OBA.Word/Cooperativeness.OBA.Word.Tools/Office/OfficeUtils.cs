using System;
using System.Linq;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Cooperativeness.OBA.Word.Tools.Office
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

        #endregion

        #region 方法
        /// <summary>
        /// 读取Office Word的版本号
        /// </summary>
        /// <returns></returns>
        public static OfficeType ReadWordVersion()
        {
            try
            {
                RegistryKey key =
                    Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\Winword.exe");
                if (key != null)
                {
                    string wordPath = key.GetValue("Path").ToString();
                    wordPath = wordPath.Substring(0, wordPath.LastIndexOf('\\'));
                    string officeVersion = wordPath.Substring(wordPath.LastIndexOf('\\') + 1).ToUpper();
                    const string pattern = @"^OFFICE(?'number'\d+)";
                    var regex = new Regex(pattern);
                    int wordVersion = int.Parse(regex.Match(officeVersion).Groups["number"].Value);
                    return (OfficeType) wordVersion;
                }
            }
            catch(Exception e)
            {
                Log.Debug(e);
            }

            return OfficeType.None;
        }

        /// <summary>
        ///  检测是否安装office
        /// </summary>
        /// <returns></returns>
        public static bool IsOfficeInstalled()
        {
            OfficeType officeType = ReadWordVersion();
            if (officeType != OfficeType.None)
                return true;
            return false;
        }

        /// <summary>
        /// 根据OBA的路径，进行场景设计插件的安装
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Install(string path)
        {
            try
            {
                // 检查路径是否为空
                if (string.IsNullOrEmpty(path)) return false;
                // 清理卸载插件的注册表
                if (Registry.LocalMachine.OpenSubKey(RegCooperativenessAddins) != null)
                {
                    Registry.LocalMachine.DeleteSubKey(RegCooperativenessAddins);
                }
                // 定义变量路径
                string addinsPath = path;
                string urlPath = path;
                // 检查路径结尾
                if (path.EndsWith("\\"))
                {
                    addinsPath = path.Remove(path.Length - 1, 1);
                    urlPath = addinsPath;
                }
                // 处理单斜杠为file：类型的/
                urlPath = urlPath.Replace("\\", "/");
                // 处理单斜杠为双斜杠
                addinsPath = addinsPath.Replace("\\", "\\\\");
                // 读取注册表资源文本
                string regText = GetResourceText("Cooperativeness.OBA.Word.Tools.addins.reg");
                if (string.IsNullOrEmpty(regText)) return false;
                // 进行变量处理
                regText = regText.Replace("#Path#", addinsPath);
                regText = regText.Replace("#UrlPath#", urlPath);
                // 注册信息保存到本地
                string regPath = Path.Combine(path, "addins.reg");
                using (Stream stream = File.Open(regPath, FileMode.OpenOrCreate))
                {
                    using (var sw = new StreamWriter(stream))
                    {
                        sw.Write(regText);
                        sw.Flush();
                    }
                }
                // 启动注册程序
                var installProcess = new Process();
                installProcess.StartInfo.FileName = "regedit";
                installProcess.EnableRaisingEvents = true;
                installProcess.StartInfo.Arguments = string.Format(" /s {0}", regPath);
                installProcess.Start();
                installProcess.WaitForExit();

                // 删除临时注册文件
                try
                {
                    File.Delete(regPath);
                }
                catch (Exception e)
                {
                    Log.Debug(e);
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }

            return false;

        }

        /// <summary>
        /// 检查插件依赖组件o2007PIA是否安装
        /// </summary>
        /// <returns></returns>
        public static bool IsO2007PiaInstalled()
        {
            RegistryKey addinKey = Registry.LocalMachine.OpenSubKey(RegO2007Pia);
            IntPtr wow64KeyPtr = Registry64Ex.OpenSubKey(Registry64Ex.HKEY_LOCAL_MACHINE, RegO2007Pia);
            return addinKey != null || wow64KeyPtr != IntPtr.Zero;
        }

        /// <summary>
        /// 检查插件依赖组件o2010PIA是否安装
        /// </summary>
        /// <returns></returns>
        public static bool IsO2010PiaInstalled()
        {
            RegistryKey addinKey = Registry.LocalMachine.OpenSubKey(RegO2010Pia);
            IntPtr wow64KeyPtr = Registry64Ex.OpenSubKey(Registry64Ex.HKEY_LOCAL_MACHINE, RegO2010Pia);
            return addinKey != null || wow64KeyPtr != IntPtr.Zero;
        }

        /// <summary>
        /// 检查场景插件是否安装
        /// </summary>
        /// <returns></returns>
        public static bool IsAddinsInstalled()
        {
            try
            {
                // 返回安装结果
                RegistryKey rsaKey = Registry.CurrentUser.OpenSubKey(RegAddinsRsakey);
                RegistryKey addinKey = Registry.CurrentUser.OpenSubKey(RegCooperativenessAddins);
                RegistryKey unloadAddinKey = Registry.LocalMachine.OpenSubKey(RegCooperativenessAddins);
                string loadBehavior = ReadRegValueByCurrentUser(RegCooperativenessAddins, "LoadBehavior");

                bool installed = (unloadAddinKey == null && rsaKey != null && addinKey != null)
                                 && (!string.IsNullOrEmpty(loadBehavior) && loadBehavior.Equals("3"));

                if (rsaKey != null) rsaKey.Close();
                if (addinKey != null) addinKey.Close();
                if (unloadAddinKey != null) unloadAddinKey.Close();

                return installed;
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }

            return false;
            //return addinKey != null;
        }

        /// <summary>
        /// 读取当前用户区注册表值
        /// </summary>
        /// <param name="regKey"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string ReadRegValueByCurrentUser(string regKey,string name)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(regKey))
                {
                    object keyValue = key.GetValue(name);
                    if (keyValue != null)
                    {
                        return keyValue.ToString();
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }
            return null;
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
        /// <param name="resourceName"></param>
        /// <returns></returns>
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
                        return resourceReader.ReadToEnd();
                    }
                }
            }
            return null;
        }

        #endregion
    }
}

