using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Cooperativeness.OBA.Word.Tools.Office
{
    /// <summary>
    /// 注册表对象扩展对象
    /// </summary>
    public static class Registry64Ex
    {
        private static readonly Logger Log = new Logger(typeof(Registry64Ex));
        #region 注册表根目录
        public static UIntPtr HKEY_CLASSES_ROOT = (UIntPtr)0x80000000;
        public static UIntPtr HKEY_CURRENT_USER = (UIntPtr)0x80000001;
        public static UIntPtr HKEY_LOCAL_MACHINE = (UIntPtr)0x80000002;
        public static UIntPtr HKEY_USERS = (UIntPtr)0x80000003;
        public static UIntPtr HKEY_CURRENT_CONFIG = (UIntPtr)0x80000005;

        #endregion

        #region 注册表API
        // 关闭64位（文件系统）的操作转向
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);
        // 开启64位（文件系统）的操作转向
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);

        // 获取操作Key值句柄
        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint RegOpenKeyEx(UIntPtr hKey, string lpSubKey, uint ulOptions, int samDesired, out IntPtr phkResult);
        //关闭注册表转向（禁用特定项的注册表反射）
        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern long RegDisableReflectionKey(IntPtr hKey);
        //使能注册表转向（开启特定项的注册表反射）
        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern long RegEnableReflectionKey(IntPtr hKey);
        //获取Key值（即：Key值句柄所标志的Key对象的值）
        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, int lpReserved,
                                                  out uint lpType, System.Text.StringBuilder lpData,
                                                  ref uint lpcbData);
        #endregion

        #region 方法
        /// <summary>
        /// 打开注册表子键
        /// </summary>
        /// <param name="hKey"></param>
        /// <param name="subKeyName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static IntPtr OpenSubKey(UIntPtr hKey, string subKeyName)
        {
            int KEY_QUERY_VALUE = (0x0001);
            int KEY_WOW64_64KEY = (0x0100);
            int KEY_ALL_WOW64 = (KEY_QUERY_VALUE | KEY_WOW64_64KEY);

            try
            {
                //声明将要获取Key值的句柄
                IntPtr pHKey = IntPtr.Zero;
                //关闭文件系统转向 
                var oldWow64State = new IntPtr();
                if (Wow64DisableWow64FsRedirection(ref oldWow64State))
                {
                    //获得操作Key值的句柄
                    RegOpenKeyEx(hKey, subKeyName, 0, KEY_ALL_WOW64, out pHKey);
                    
                }

                //打开文件系统转向
                Wow64RevertWow64FsRedirection(oldWow64State);

                //返回Key值
                return pHKey;
            }
            catch(Exception ex)
            {
                Log.Debug(ex);
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// 获取注册表值
        /// </summary>
        /// <param name="hKey"></param>
        /// <param name="subKeyName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static string GetValue(UIntPtr hKey, string subKeyName, string keyName)
        {
            int KEY_QUERY_VALUE = (0x0001);
            int KEY_WOW64_64KEY = (0x0100);
            int KEY_ALL_WOW64 = (KEY_QUERY_VALUE | KEY_WOW64_64KEY);

            try
            {
                //声明将要获取Key值的句柄
                IntPtr pHKey = IntPtr.Zero;
                //记录读取到的Key值
                var result = new StringBuilder("".PadLeft(1024));
                uint resultSize = 1024;

                //关闭文件系统转向 
                var oldWow64State = new IntPtr();
                if (Wow64DisableWow64FsRedirection(ref oldWow64State))
                {
                    uint lpType = 0;
                    //获得操作Key值的句柄
                    RegOpenKeyEx(hKey, subKeyName, 0, KEY_ALL_WOW64, out pHKey);

                    //关闭注册表转向（禁止特定项的注册表反射）
                    RegDisableReflectionKey(pHKey);

                    //获取访问的Key值
                    RegQueryValueEx(pHKey, keyName, 0, out lpType, result, ref resultSize);

                    //打开注册表转向（开启特定项的注册表反射）
                    RegEnableReflectionKey(pHKey);
                }

                //打开文件系统转向
                Wow64RevertWow64FsRedirection(oldWow64State);

                //返回Key值
                return result.ToString().Trim();
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                return null;
            }
        }
        #endregion
    }
}
