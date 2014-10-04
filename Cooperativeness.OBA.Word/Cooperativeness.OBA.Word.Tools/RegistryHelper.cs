using System;
using System.IO;
using Microsoft.Win32;
using System.Globalization;
using System.Security.AccessControl;

namespace Cooperativeness.OBA.Word.Tools
{
    /// <summary>
    /// 定义一个注册表助手类
    /// </summary>
    public class RegistryHelper
    {
        #region 常量
        public const string IIsRegistry =
            @"SOFTWARE\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_IGNORE_ZONES_INITIALIZATION_FAILURE_KB945701";
        public const string IUSR_HKLM_Registry =
            @"HKLM\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones";
        public const string IUSR_HKU_Registry =
            @"HKU\S-1-5-20\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones";
        public const string IUSR_HKCU_Registry =
            @"HKCU\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones";

        #endregion

        #region 注册表读写权限

        /// <summary>
        /// 设置用户注册表的读写权限
        /// </summary>
        public static void SetRegistryRight(string user, string subKey)
        {
            if (string.IsNullOrEmpty(user) ||
                string.IsNullOrEmpty(subKey))
                return;

            try
            {
                // Create a security object that grants no access.
                RegistrySecurity mSec = new RegistrySecurity();

                // Add a rule that grants the user the right
                // to read and enumerate the name/value pairs in a key,
                // to read its access and audit rules, to enumerate
                // its subkeys, to create subkeys, and to delete subkeys.
                // The rule is inherited by all contained subkeys.
                //
                RegistryAccessRule rule = new RegistryAccessRule(user,
                    RegistryRights.ReadKey | RegistryRights.WriteKey | RegistryRights.Delete,
                    InheritanceFlags.ContainerInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow);
                mSec.AddAccessRule(rule);

                // Add a rule that allows the user the right
                // to set the name/value pairs in a key.
                // This rule is inherited by contained subkeys, but
                // propagation flags limit it to immediate child
                // subkeys.
                rule = new RegistryAccessRule(user,
                    RegistryRights.ChangePermissions,
                    InheritanceFlags.ContainerInherit,
                    PropagationFlags.InheritOnly |
                    PropagationFlags.NoPropagateInherit,
                    AccessControlType.Allow);
                mSec.AddAccessRule(rule);

                int length = subKey.IndexOf('\\');
                RegistryKey root = null;
                switch (subKey.Substring(0, length).ToUpper())
                {
                    case "HKLM":
                        root = Registry.LocalMachine;
                        break;

                    case "HKU":
                        root = Registry.Users;
                        break;

                    case "HKCU":
                        root = Registry.CurrentUser;
                        break;

                    case "HKCC":
                        root = Registry.CurrentConfig;
                        break;

                    case "HKCR":
                        root = Registry.ClassesRoot;
                        break;

                    default:
                        break;
                }
                if (root != null)
                {
                    RegistryKey soft = root.OpenSubKey(subKey.Substring(length + 1), true);
                    if (soft != null)
                    {
                        soft.SetAccessControl(mSec);
                        soft.Close();
                    }
                    root.Close();
                }
            }
            catch
            { }
        }

        #endregion

        #region 读写注册表

        /// <summary>
        /// 读注册表(通用函数)
        /// </summary>
        /// <param name="subKey"></param>
        /// <param name="sValue"></param>
        /// <returns></returns>
        public static string ReadRegValue(string subKey, string sValue)
        {
            try
            {
                using (Microsoft.Win32.RegistryKey root = Registry.LocalMachine)
                {
                    using (Microsoft.Win32.RegistryKey soft = root.OpenSubKey(subKey))
                    {
                        if (soft != null)
                        {
                            if (soft.GetValue(sValue) != null)
                            {
                                return (string)soft.GetValue(sValue);
                            }
                        }
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 读注册表(通用函数)
        /// </summary>
        /// <param name="subKey"></param>
        /// <returns></returns>
        public static string ReadRegValue(string subKey)
        {
            try
            {
                using (Microsoft.Win32.RegistryKey root = Registry.LocalMachine)
                {
                    using (Microsoft.Win32.RegistryKey soft = root.OpenSubKey(subKey))
                    {
                        if (soft != null)
                        {
                            if (soft.GetValue(null) != null)
                            {
                                return (string)soft.GetValue(null);
                            }
                        }
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 写注册表值(通用函数)
        /// </summary>
        public static void WriteRegValue(string subKey, string sName, string sValue, RegistryValueKind kind)
        {
            try
            {
                using (Microsoft.Win32.RegistryKey root = Registry.LocalMachine)
                {
                    using (Microsoft.Win32.RegistryKey soft = root.OpenSubKey(subKey))
                    {
                        if (soft != null)
                        {
                            if (sName != null && sValue != null)
                            {
                                soft.SetValue(sName, sValue, kind);
                            }
                        }
                    }
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 写注册表值(通用函数)
        /// </summary>
        public static void WriteRegValue(string subKey, string sName, string sValue)
        {
            WriteRegValue(subKey, sName, sValue, RegistryValueKind.DWord);
        }

        #endregion

        #region 通用泛型

        public static bool GetRegValue<T>(RegistryKey root, string key, string value, RegistryValueKind kind, out T data)
        {
            bool success = false;
            data = default(T);

            //using (RegistryKey baseKey = RegistryKey.OpenRemoteBaseKey(hive, String.Empty))
            using (RegistryKey baseKey = root)
            {
                //if (baseKey != null)
                //{
                    using (RegistryKey registryKey = baseKey.OpenSubKey(key, RegistryKeyPermissionCheck.ReadSubTree))
                    {
                        if (registryKey != null)
                        {
                            try
                            {
                                // If the key was opened, try to retrieve the value.
                                RegistryValueKind kindFound = registryKey.GetValueKind(value);
                                if (kindFound == kind)
                                {
                                    object regValue = registryKey.GetValue(value, null);
                                    if (regValue != null)
                                    {
                                        data = (T)Convert.ChangeType(regValue, typeof(T), CultureInfo.InvariantCulture);
                                        success = true;
                                    }
                                }
                            }
                            catch (IOException)
                            {
                                // The registry value doesn't exist. Since the
                                // value doesn't exist we have to assume that
                                // the component isn't installed and return
                                // false and leave the data param as the
                                // default value.
                            }
                        }
                    //}
                }
            }
            return success;
        }

        public static bool GetRegValue<T>(string key, string value, out T data)
        {
            return GetRegValue<T>(Registry.LocalMachine, key, value, RegistryValueKind.DWord, out data);
        }

        public static string GetRegValue(string szRegPath)
        {
            string ret = ReadRegValue(szRegPath);
            if (ret != null && ret.Trim().Length > 0)
            {
                return ret.Trim();
            }
            return null;
        }

        #endregion
    }
}
