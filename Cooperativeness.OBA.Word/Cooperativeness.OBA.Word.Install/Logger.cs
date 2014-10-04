using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Cooperativeness.OBA.Word.Install
{
    internal enum LogEnum
    {
        Info,
        Debug,
        Warn,
        Error
    }
    /// <summary>
    /// 定义一个调试日志类
    /// </summary>
    public class Logger
    {
        private readonly string _typeName = "[Unknown TypeName]";

        public Logger(Type type)
        {
            _typeName = "[" + type.FullName + "]";
        }
        #region 静态方法
        /// <summary>
        /// 输出到DebugView的接口
        /// </summary>
        /// <param name="msg"></param>
        [DllImport("Kernel32", CharSet = CharSet.Auto)]
        private static extern void OutputDebugString(string msg);

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="msg">格式</param>
        /// <param name="args">参数</param>
        public void Error(string msg, params object[] args)
        {
            Print(msg, LogEnum.Error, args);
        }

        public void Error(Exception e, params object[] args)
        {
            Error(GetExceptionDetail(e), args);
        }

        public void Debug(string msg, params object[] args)
        {
            Print(msg, LogEnum.Debug, args);
        }

        public void Debug(Exception e, params object[] args)
        {
            Debug(GetExceptionDetail(e), args);
        }

        public void Warn(string msg, params object[] args)
        {
            Print(msg, LogEnum.Warn, args);
        }

        public void Warn(Exception e, params object[] args)
        {
            Warn(GetExceptionDetail(e), args);
        }

        public void Info(string msg, params object[] args)
        {
            Print(msg, LogEnum.Info, args);
        }

        public void Info(Exception e, params object[] args)
        {
            Info(GetExceptionDetail(e), args);
        }

        public static string GetExceptionDetail(Exception err)
        {
            string result = err.Message + "\n";

            result += " Details:\t" + err.Source + "\n" + err.StackTrace;
            Exception innerErr = err.InnerException;
            while (innerErr != null)
            {
                result += "\n" + innerErr.Message + "\t" + innerErr.Source + "\t" + innerErr.StackTrace;
                innerErr = innerErr.InnerException;
            }
            return result;
        }

        private void Print(string msg, LogEnum type, params object[] args)
        {
            try
            {
                string logtype = "";
                switch (type)
                {
                    case LogEnum.Info:
                        logtype = "[Info]";
                        break;
                    case LogEnum.Warn:
                        logtype = "[Warn]";
                        break;
                    case LogEnum.Debug:
                        logtype = "[Debug]";
                        break;
                    case LogEnum.Error:
                        logtype = "[Error]";
                        break;
                    default:
                        logtype = "[Info]";
                        break;
                }
                DateTime dtNow = DateTime.Now;
                if (args != null && args.Length > 0)
                    OutputDebugString("Cooperativeness.OBA.Word:" + logtype + _typeName + "[" + dtNow.ToString() + ":" + dtNow.Millisecond +
                                      "]-" + String.Format(msg, args));
                else
                    OutputDebugString("Cooperativeness.OBA.Word:" + logtype + _typeName + "[" + dtNow.ToString() + ":" + dtNow.Millisecond +
                                      "]-" + msg);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        #endregion Static Methods

        /// <summary>
        /// 获取临时目录
        /// </summary>
        public static string TemporaryDirectory
        {
            get
            {
                string temp = Environment.GetEnvironmentVariable("TEMP");
                if (string.IsNullOrEmpty(temp))
                {
                    temp = @"C:\\Logs\";
                }
                if (!Directory.Exists(temp)) Directory.CreateDirectory(temp);
                return temp;
            }
        }

        
    }
}
