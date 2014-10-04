
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.DocumentScene.Plugin.Hook;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin
{
    public class Activator : IBundleActivator
    {
        #region 字段
        internal static IBundleContext Context;
        private IAssemblyResolverHook hook;
        #endregion

        #region 方法
        /// <summary>
        /// 激活器启动
        /// </summary>
        /// <param name="context"></param>
        public void Start(IBundleContext context)
        {
            hook = new AssemblyResolverHook();
            Context = context;
            context.RegisterAssemblyResolverHook(hook);
        }

        /// <summary>
        /// 激活器停止
        /// </summary>
        /// <param name="context"></param>
        public void Stop(IBundleContext context)
        {
            SceneContext.Instance.Close();
        }
        #endregion
    }
}
