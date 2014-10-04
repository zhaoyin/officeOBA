using System;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Async
{
    /// <summary>
    /// 定义异步工作参数
    /// </summary>
    public class AsyncWorkerEventArgs : EventArgs
    {
        #region 构造函数
        public AsyncWorkerEventArgs(object argument)
            : base()
        {
            this.Argument = argument;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置工作状态
        /// </summary>
        public WorkerState State { get; set; }

        /// <summary>
        /// 获取参数
        /// </summary>
        public object Argument { get; private set; }

        /// <summary>
        /// 获取或设置用户数据
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 获取记录数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 获取或设置当前的已处理的记录数
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 获取或设置提示信息
        /// </summary>
        public string Tip { get; set; }

        #endregion
    }

    /// <summary>
    /// 定义工作状态
    /// </summary>
    public enum WorkerState
    {
        /// <summary>
        /// 初始状态
        /// </summary>
        None = 0,

        /// <summary>
        /// 完成
        /// </summary>
        Completed,

        /// <summary>
        /// 取消
        /// </summary>
        Canceled,

        /// <summary>
        /// 错误
        /// </summary>
        Error
    }
}
