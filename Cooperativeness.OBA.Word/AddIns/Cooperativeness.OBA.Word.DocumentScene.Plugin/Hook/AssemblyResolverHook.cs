using System;
using System.Reflection;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Hook
{
    /// <summary>
    /// 定义场景程序集解析钩子
    /// </summary>
    public class AssemblyResolverHook : IAssemblyResolverHook
    {
        private static readonly Logger Log=new Logger(typeof(AssemblyResolverHook));
        #region  字段
        private string[] probings;

        #endregion

        #region 属性
        /// <summary>
        /// 获取程序集配置信息
        /// </summary>
        public string[] Probings
        {
            get
            {
                if (this.probings == null)
                    LoadProbings();
                return this.probings;
            }
        }

        #endregion

        #region 方法
        public Assembly Find(string fullName)
        {
            try
            {
                // 首先获取所有程序集配置目录列表
                if (this.Probings == null) return null;
                // 获取加载程序集的名称
                var strArray = fullName.Split(",".ToCharArray());
                //var index = fullName.IndexOf(',');
                if (strArray.Length <= 0 || string.IsNullOrEmpty(strArray[0])) return null;
                string sAsmName = strArray[0] +/*fullName.Substring(0, index) +*/ ".dll";
                // 扫描程序集配置目录
                foreach (string probing in this.Probings)
                {
                    String asmPath = System.IO.Path.Combine(probing, sAsmName);
                    if (System.IO.File.Exists(asmPath))
                    {
                        Log.Debug("[AssemblyResolverHook:Finded:{0}]", asmPath);
                        return Assembly.LoadFrom(asmPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Debug("[AssemblyResolverHook->Find]-[Trace:{0}]-[Messge:{0}]"
                              , ex.StackTrace
                              , ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 加载执行配置文件
        /// </summary>
        private void LoadProbings()
        {
        }

        #endregion
    }
}
