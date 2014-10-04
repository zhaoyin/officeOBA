using System;
using Microsoft.Win32;
using System.Security.AccessControl;

namespace Cooperativeness.Files.Server.Install
{

    public class RegisterHelper
    {
        private static readonly Logger Log = new Logger(typeof(RegisterHelper));
        ///  <summary>
        ///  注册表基项静态域
        ///  主要包括：
        ///1.Registry.ClassesRoot  对应于 HKEY_CLASSES_ROOT 主键
        ///2.Registry.CurrentUser  对应于 HKEY_CURRENT_USER 主键
        ///3.Registry.LocalMachine  对应于HKEY_LOCAL_MACHINE 主键
        ///4.Registry.User  对应于HKEY_USER 主键
        ///5.Registry.CurrentConfig  对应于 HEKY_CURRENT_CONFIG 主键
        ///6.Registry.DynDa  对应于 HKEY_DYN_DATA 主键
        ///7.Registry.PerformanceData  对应于 HKEY_PERFORMANCE_DATA 主键
        ///  版本:1.0 
        ///  </summary> 
        public enum RegDomain
        {
            ///  <summary>
            ///  对应于 HKEY_CLASSES_ROOT 主键
            ///  </summary> 
            ClassesRoot = 0,

            ///  <summary>
            ///  对应于 HKEY_CURRENT_USER 主键
            ///  </summary> 
            CurrentUser = 1,

            ///  <summary>
            ///  对应于HKEY_LOCAL_MACHINE 主键
            ///  </summary> 
            LocalMachine = 2,

            ///  <summary>
            ///  对应于HKEY_USER 主键
            ///  </summary> 
            User = 3,

            ///  <summary>
            ///  对应于 HEKY_CURRENT_CONFIG 主键
            ///  </summary> 
            CurrentConfig = 4,

            ///  <summary>
            ///  对应于 HKEY_DYN_DATA 主键
            ///  </summary> 
            DynDa = 5,

            ///  <summary>
            ///  对应于 HKEY_PERFORMANCE_DATA 主键
            ///  </summary> 
            PerformanceData = 6,
        }
        

        ///  <summary>
        ///  指定在注册表中存储值时所用的数据类型，或标识注册表中某个值的数据类型
        ///  主要包括：
        ///1.RegistryValueKind.Unknown 
        ///2.RegistryValueKind.String 
        ///3.RegistryValueKind.ExpandString 
        ///4.RegistryValueKind.Binary 
        ///5.RegistryValueKind.DWord 
        ///6.RegistryValueKind.MultiString 
        ///7.RegistryValueKind.QWord 
        ///  版本:1.0 
        ///  </summary> 
        public enum RegValueKind
        {

            ///  <summary>
            ///  指示一个不受支持的注册表数据类型。例如，不支持Microsoft Win32 API  注册表数据类型REG_RESOURCE_LIST。使用此值指定
            ///  </summary> 
            Unknown = 0,
            ///  <summary>
            ///  指定一个以Null  结尾的字符串。此值与Win32 API  注册表数据类型REG_SZ  等效。
            ///  </summary> 
            String = 1,
            ///  <summary>
            ///  指定一个以NULL  结尾的字符串，该字符串中包含对环境变量（如%PATH%，当值被检索时，就会展开）的未展开的引用。
            ///  此值与Win32 API 注册表数据类型REG_EXPAND_SZ  等效。
            ///  </summary> 
            ExpandString = 2,
            ///  <summary>
            ///  指定任意格式的二进制数据。此值与Win32 API  注册表数据类型REG_BINARY  等效。
            ///  </summary> 
            Binary = 3,
            ///  <summary>
            ///  指定一个32  位二进制数。此值与Win32 API  注册表数据类型REG_DWORD  等效。
            ///  </summary> 
            DWord = 4,

            ///  <summary>
            ///  指定一个以NULL  结尾的字符串数组，以两个空字符结束。此值与Win32 API  注册表数据类型REG_MULTI_SZ  等效。
            ///  </summary> 
            MultiString = 5,

            ///  <summary>
            ///  指定一个64  位二进制数。此值与Win32 API  注册表数据类型REG_QWORD  等效。
            ///  </summary> 
            QWord = 6,

        }

        ///  <summary>
        ///  注册表操作类
        ///  主要包括以下操作：
        ///1.创建注册表项
        ///2.读取注册表项
        ///3.判断注册表项是否存在
        ///4.删除注册表项
        ///5.创建注册表键值
        ///6.读取注册表键值
        ///7.判断注册表键值是否存在
        ///8.删除注册表键值

        ///

        ///  版本:1.0 

        ///  </summary> 

        #region  字段定义

        ///  <summary>
        ///  注册表项名称
        ///  </summary> 

        private string _subkey;

        ///  <summary>
        ///  注册表基项域
        ///  </summary> 
        private RegDomain _domain;

        #endregion

        #region  构造函数
        public RegisterHelper()
        {
            ///默认注册表项名称
            _subkey = "software\\";
            ///默认注册表基项域
            _domain = RegDomain.LocalMachine;
        }



        ///  <summary>
        ///  构造函数
        ///  </summary>
        ///  <param name="subKey">注册表项名称</param>
        ///  <param name="regDomain">注册表基项域</param> 
        public RegisterHelper(string subKey, RegDomain regDomain)
        {
            ///设置注册表项名称
            _subkey = subKey;
            ///设置注册表基项域
            _domain = regDomain;
        }

        #endregion



        #region  公有方法

        #region  创建注册表项

        ///  <summary>
        ///  创建注册表项，默认创建在注册表基项HKEY_LOCAL_MACHINE 下面（请壬柚?SubKey 属性）
        ///  虚方法，子类可进行重写
        ///  </summary> 
        public virtual void CreateSubKey()
        {
            CreateSubKey(_subkey,_domain);
        }

        ///  <summary>
        ///  创建注册表项，默认创建在注册表基项HKEY_LOCAL_MACHINE 下面
        ///  虚方法，子类可进行重写
        ///  例子：如 subkey 是 software\\higame\\，则将创建 HKEY_LOCAL_MACHINE\\software\\higame\\注册表项
        ///  </summary> 
        ///  <param name="subKey">注册表项名称</param> 
       public virtual void CreateSubKey(string subKey,RegDomain domain)
        {
            ///判断注册表项名称是否为空，如果为空，返回 false
            if (string.IsNullOrEmpty(subKey)) return;

            ///创建基于注册表基项的节点
            RegistryKey key = GetRegDomain(domain);

            ///要创建的注册表项的节点
            RegistryKey sKey;
            if (!IsSubKeyExist(subKey,domain))
            {
                sKey = key.CreateSubKey(subKey,RegistryKeyPermissionCheck.ReadWriteSubTree);
            }

            //sKey.Close(); 

            //关闭对注册表项的更改
            key.Close();
        }
        #endregion

        #region  判断注册表项是否存在
        ///  <summary>
        ///  判断注册表项是否存在，默认是在注册表基项 HKEY_LOCAL_MACHINE 下判断（请先设置 SubKey 属性）
        ///  虚方法，子类可进行重写
        ///  例子：如果设置了 Domain 和 SubKey 属性，则判断 Domain\\SubKey，否则默认判断 HKEY_LOCAL_MACHINE\\software\\ 
        ///  </summary>
        ///  <returns>返回注册表项是否存在，存在返回 true，否则返回 false</returns> 
        public virtual bool IsSubKeyExist()
        {
            return IsSubKeyExist(_subkey, _domain);
        }


        ///  <summary>
        ///  判断注册表项是否存在（请先设置 SubKey 属性）
        ///  虚方法，子类可进行重写
        ///  例子：如 regDomain 是 HKEY_CLASSES_ROOT，subkey 是 software\\higame\\，则将判断 HKEY_CLASSES_ROOT\\software\\higame\\注册表项是否存在
        ///  </summary>
        ///  <param name="subKey">注册表项名称</param>
        ///  <param name="regDomain">注册表基项域</param>
        ///  <returns>返回注册表项是否存在，存在返回 true，否则返回 false</returns> 
        public virtual bool IsSubKeyExist(string subKey, RegDomain regDomain)
        {
            //判断注册表项名称是否为空，如果为空，返回 false
            if (string.IsNullOrEmpty(subKey)) return false;

            //检索注册表子项
            //如果 sKey 为 null,说明没有该注册表项不存在，否则存在
            RegistryKey sKey = OpenSubKey(subKey, regDomain);

            if (sKey == null)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region  删除注册表项

        ///  <summary>
        ///  删除注册表项（请先设置 SubKey 属性）
        ///  虚方法，子类可进行重写
        ///  </summary>
        ///  <returns>如果删除成功，则返回 true，否则为 false</returns> 
        public virtual bool DeleteSubKey()
        {
            return DeleteSubKey(_subkey, _domain);
        }

        ///  <summary>
        ///  删除注册表项
        ///  虚方法，子类可进行重写
        ///  </summary>
        ///  <param name="subKey">注册表项名称</param>
        ///  <param name="regDomain">注册表基项域</param>
        ///  <returns>如果删除成功，则返回 true，否则为 false</returns> 
        public virtual bool DeleteSubKey(string subKey, RegDomain regDomain)
        {
            //返回删除是否成功
            bool result = false;

            //判断注册表项名称是否为空，如果为空，返回 false
            if (string.IsNullOrEmpty(subKey)) return false;

            //创建基于注册表基项的节点
            RegistryKey key = GetRegDomain(regDomain);

            if (IsSubKeyExist(subKey, regDomain))
            {
                try
                {
                    //删除注册表项
                    key.DeleteSubKey(subKey);
                    result = true;
                }

                catch
                {
                    result = false;
                }
            }

            //关闭对注册表项的更改
            key.Close();
            return result;
        }

        #endregion

        #region  判断键值是否存在

        ///  <summary>
        ///  判断键值是否存在（请先设置 SubKey 属性）
        ///  虚方法，子类可进行重写
        ///  如果 SubKey 为空、null 或者 SubKey 指定的注册表项不存在，返回 false 
        ///  </summary>
        ///  <param name="name">键值名称</param>
        ///  <returns>返回键值是否存在，存在返回 true，否则返回 false</returns> 
        public virtual bool IsRegeditKeyExist(string name)
        {
            return IsRegeditKeyExist(name, _subkey, _domain);
        }

        ///  <summary>
        ///  判断键值是否存在
        ///  虚方法，子类可进行重写
        ///  </summary>
        ///  <param name="name">键值名称</param>
        ///  <param name="subKey">注册表项名称</param>
        ///  <param name="regDomain">注册表基项域</param>
        ///  <returns>返回键值是否存在，存在返回 true，否则返回 false</returns> 
        public virtual bool IsRegeditKeyExist(string name, string subKey, RegDomain regDomain)
        {
            ///返回结果
            bool result = false;

            ///判断是否设置键值属性
            if (string.IsNullOrEmpty(name)) return false;

            ///判断注册表项是否存在
            if (IsSubKeyExist(subKey, regDomain))
            {
                //打开注册表项
                RegistryKey key = OpenSubKey(subKey, regDomain);
                if (key == null) return false;
                //键值集合
                string[] regeditKeyNames;

                //获取键值集合
                regeditKeyNames = key.GetValueNames();
                //遍历键值集合，如果存在键值，则退出遍历
                foreach (string regeditKey in regeditKeyNames)
                {
                    if (regeditKey.Equals(name))
                    {
                        result = true;
                        break;
                    }
                }

                //关闭对注册表项的更改
                key.Close();
            }
            return result;
        }

        #endregion

        #region  设置键值内容

        ///  <summary>
        ///  设置指定的键值内容，不指定内容数据类型（请先设置 SubKey 属性）
        ///  存在改键值则修改键值内容，不存在键值则先创建键值，再设置键值内容
        ///  </summary>
        ///  <param name="name">键值名称</param>
        ///  <param name="content">键值内容</param>
        ///  <returns>键值内容设置成功，则返回 true，否则返回 false</returns> 
        public virtual bool WriteRegeditKey(string name, object content)
        {
            return WriteRegeditKey(name, _subkey, _domain, content);
        }

        public virtual bool WriteRegeditKey(string name, string subkey, RegDomain domain, object content)
        {
            //返回结果
            bool result = false;

            //判断注册表项是否存在，如果不存在，则直接创建
            if (!IsSubKeyExist(subkey,domain))
            {
                CreateSubKey(subkey,domain);
            }

            if (string.IsNullOrEmpty(name)) return true;

            //以可写方式打开注册表项
            RegistryKey key = OpenSubKey(subkey,domain,true);
            //如果注册表项打开失败，则返回 false
            if (key == null)
            {
                return false;
            }

            try
            {
                key.SetValue(name, content);
                result = true;
            }
            catch (Exception e)
            {
                Log.Debug(e);
                result = false;
            }

            finally
            {
                //关闭对注册表项的更改
                key.Close();
            }
            return result;
        }

        public virtual bool WriteRegeditKey(string name, string subkey, RegDomain domain, object content, RegValueKind regValueKind)
        {
            //返回结果
            bool result = false;

            //判断注册表项是否存在，如果不存在，则直接创建
            if (!IsSubKeyExist(subkey, domain))
            {
                CreateSubKey(subkey, domain);
            }

            if (string.IsNullOrEmpty(name)) return true;

            //以可写方式打开注册表项
            RegistryKey key = OpenSubKey(subkey, domain, true);
            //如果注册表项打开失败，则返回 false
            if (key == null)
            {
                return false;
            }

            try
            {
                key.SetValue(name, content, GetRegValueKind(regValueKind));
                result = true;
            }
            catch (Exception e)
            {
                Log.Debug(e);
                result = false;
            }

            finally
            {
                //关闭对注册表项的更改
                key.Close();
            }
            return result;
        }

        ///  <summary>
        ///  设置指定的键值内容，指定内容数据类型（请先设置 SubKey 属性）
        ///  存在改键值则修改键值内容，不存在键值则先创建键值，再设置键值内容
        ///  </summary>
        ///  <param name="name">键值名称</param>
        ///  <param name="content">键值内容</param>
        ///  <returns>键值内容设置成功，则返回 true，否则返回 false</returns> 
        public virtual bool WriteRegeditKey(string name, object content, RegValueKind regValueKind)
        {
            return WriteRegeditKey(name, _subkey, _domain, content, regValueKind);
        }

        #endregion

        #region  读取键值内容

        ///  <summary>
        ///  读取键值内容（请先设置 RegeditKey 和 SubKey 属性）
        ///1.如果 RegeditKey 为空、 null 或者 RegeditKey 指示的键值不存在，返回 null 
        ///2.如果 SubKey 为空、 null 或者 SubKey 指示的注册表项不存在，返回 null 
        ///3.反之，则返回键值内容
        ///  </summary>
        ///  <returns>返回键值内容</returns> 
        public virtual object ReadRegeditKey(string name)
        {
            return ReadRegeditKey(name, _subkey, _domain);
        }


        ///  <summary>
        ///  读取键值内容
        ///  </summary>
        ///  <param name="name">键值名称</param>
        ///  <param name="subKey">注册表项名称</param>
        ///  <param name="regDomain">注册表基项域</param>
        ///  <returns>返回键值内容</returns> 
        public virtual object ReadRegeditKey(string name, string subKey, RegDomain regDomain)
        {
            //键值内容结果
            object obj = null;

            //判断是否设置键值属性
            if (string.IsNullOrEmpty(name)) return null;
 
            //判断键值是否存在
            if (IsRegeditKeyExist(name,subKey,regDomain))
            {
                //打开注册表项
                RegistryKey key = OpenSubKey(subKey, regDomain);
                if (key != null)
                {
                    obj = key.GetValue(name,null);
                    //关闭对注册表项的更改
                    key.Close();
                }
            }
            return obj;
        }

        #endregion

        #region  删除键值

        ///  <summary>
        ///  删除键值（请先设置 RegeditKey 和 SubKey 属性）
        ///1.如果 RegeditKey 为空、 null 或者 RegeditKey 指示的键值不存在，返回 false 
        ///2.如果 SubKey 为空、 null 或者 SubKey 指示的注册表项不存在，返回 false 
        ///  </summary>
        ///  <returns>如果删除成功，返回 true，否则返回 false</returns> 
        public virtual bool DeleteRegeditKey(string name)
        {
            return DeleteRegeditKey(name, _subkey, _domain);
        }

        ///  <summary>
        ///  删除键值
        ///  </summary>
        ///  <param name="name">键值名称</param>
        ///  <param name="subKey">注册表项名称</param>
        ///  <param name="regDomain">注册表基项域</param>
        ///  <returns>如果删除成功，返回 true，否则返回 false</returns> 
        public virtual bool DeleteRegeditKey(string name, string subKey, RegDomain regDomain)
        {
            //删除结果
            bool result = false;

            //判断键值名称和注册表项名称是否为空，如果为空，则返回 false
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(subKey))
            {
                return false;
            }

            //判断键值是否存在
            if (IsRegeditKeyExist(name,subKey,regDomain))
            {
                //以可写方式打开注册表项
                RegistryKey key = OpenSubKey(subKey, regDomain, true);
                if (key != null)
                {
                    try
                    {
                        ///删除键值
                        key.DeleteValue(name);
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }

                    finally
                    {
                        ///关闭对注册表项的更改
                        key.Close();

                    }
                }
            }
            return result;
        }

        #endregion

        #endregion

        #region  受保护方法

        ///  <summary>
        ///  获取注册表基项域对应顶级节点
        ///  例子：如 regDomain 是 ClassesRoot，则返回 Registry.ClassesRoot 
        ///  </summary>
        ///  <param name="regDomain">注册表基项域</param>
        ///  <returns>注册表基项域对应顶级节点</returns> 
        protected RegistryKey GetRegDomain(RegDomain regDomain)
        {
            //创建基于注册表基项的节点
            RegistryKey key;

           #region  判断注册表基项域
            switch (regDomain)
            {
                case RegDomain.ClassesRoot:
                    key = Registry.ClassesRoot;
                    break;
                case RegDomain.CurrentUser:
                    key = Registry.CurrentUser;
                    break;
                case RegDomain.LocalMachine:
                    key = Registry.LocalMachine;
                    break;
                case RegDomain.User:
                    key = Registry.Users;
                    break;
                case RegDomain.CurrentConfig:
                    key = Registry.CurrentConfig;
                    break;
                case RegDomain.DynDa:
                    key = Registry.DynData;
                    break;
                case RegDomain.PerformanceData:
                    key = Registry.PerformanceData;
                    break;
                default:
                    key = Registry.LocalMachine;
                    break;
            }
            #endregion



            return key;

        }



        ///  <summary>
        ///  获取在注册表中对应的值数据类型
        ///  例子：如 regValueKind 是 DWord，则返回 RegistryValueKind.DWord 
        ///  </summary>
        ///  <param name="regValueKind">注册表数据类型</param>
        ///  <returns>注册表中对应的数据类型</returns> 
        protected RegistryValueKind GetRegValueKind(RegValueKind regValueKind)
        {
            RegistryValueKind regValueK;
            #region  判断注册表数据类型
            switch (regValueKind)
            {
                case RegValueKind.Unknown:
                    regValueK = RegistryValueKind.Unknown;
                    break;
                case RegValueKind.String:
                    regValueK = RegistryValueKind.String;
                    break;
                case RegValueKind.ExpandString:
                    regValueK = RegistryValueKind.ExpandString;
                    break;
                case RegValueKind.Binary:
                    regValueK = RegistryValueKind.Binary;
                    break;
                case RegValueKind.DWord:
                    regValueK = RegistryValueKind.DWord;
                    break;
                case RegValueKind.MultiString:
                    regValueK = RegistryValueKind.MultiString;
                    break;
                case RegValueKind.QWord:
                    regValueK = RegistryValueKind.QWord;
                    break;
                default:
                    regValueK = RegistryValueKind.String;
                    break;
            }

            #endregion
            return regValueK;
        }

        #region  打开注册表项

        ///  <summary>
        ///  打开注册表项节点，以只读方式检索子项
        ///  虚方法，子类可进行重写
        ///  </summary>
        ///  <returns>如果 SubKey 为空、 null 或者 SubKey 指示注册表项不存在，则返回 null，否则返回注册表节点</returns> 
        protected virtual RegistryKey OpenSubKey()
        {
            return OpenSubKey(_subkey, _domain);
        }



        ///  <summary>
        ///  打开注册表项节点
        ///  虚方法，子类可进行重写
        ///  </summary>
        ///  <param name="writable">如果需要项的写访问权限，则设置为true</param>
        ///  <returns>如果 SubKey 为空、 null 或者 SubKey 指示注册表项不存在，则返回 null，否则返回注册表节点</returns> 
        protected virtual RegistryKey OpenSubKey(bool writable)
        {
            return OpenSubKey(_subkey, _domain, writable);
        }

        ///  <summary>
        ///  打开注册表项节点，以只读方式检索子项
        ///  虚方法，子类可进行重写
        ///  </summary>
        ///  <param name="subKey">注册表项名称</param>
        ///  <param name="regDomain">注册表基项域</param>
        ///  <returns>如果 SubKey 为空、 null 或者 SubKey 指示注册表项不存在，则返回 null，否则返回注册表节点</returns> 
        protected virtual RegistryKey OpenSubKey(string subKey, RegDomain regDomain)
        {
            try
            {
                //判断注册表项名称是否为空
                if (string.IsNullOrEmpty(subKey)) return null;

                //创建基于注册表基项的节点
                RegistryKey key = GetRegDomain(regDomain);

                //要打开的注册表项的节点
                RegistryKey sKey = null;

                //打开注册表项
                sKey = key.OpenSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree,RegistryRights.FullControl);

                //关闭对注册表项的更改
                key.Close();

                //返回注册表节点
                return sKey;
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }
            return null;
        }


        ///  <summary>
        ///  打开注册表项节点
        ///  虚方法，子类可进行重写
        ///  </summary>
        ///  <param name="subKey">注册表项名称</param>
        ///  <param name="regDomain">注册表基项域</param>
        ///  <param name="writable">如果需要项的写访问权限，则设置为true</param>
        ///  <returns>如果 SubKey 为空、 null 或者 SubKey 指示注册表项不存在，则返回 null，否则返回注册表节点</returns> 
        protected virtual RegistryKey OpenSubKey(string subKey, RegDomain regDomain, bool writable)
        {
            try
            {
                //判断注册表项名称是否为空
                if (string.IsNullOrEmpty(subKey)) return null;

                //创建基于注册表基项的节点
                RegistryKey key = GetRegDomain(regDomain);

                //要打开的注册表项的节点
                RegistryKey sKey = null;

                //打开注册表项
                sKey = key.OpenSubKey(subKey, writable);

                //关闭对注册表项的更改
                key.Close();

                //返回注册表节点
                return sKey;
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }
            return null;
        }

        #endregion

        #endregion

    }

}