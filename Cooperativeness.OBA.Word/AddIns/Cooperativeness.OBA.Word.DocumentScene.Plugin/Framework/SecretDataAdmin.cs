using System.Collections;
using OfficeWord = Microsoft.Office.Interop.Word;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Framework
{
    /// <summary>
    /// 定义私密数据管理器
    /// </summary>
    public class SecretDataAdmin
    {
        #region 字段
        internal Hashtable datas;

        #endregion

        #region 构造方法
        public SecretDataAdmin()
        {
            datas = new Hashtable();
        }

        #endregion

        #region 方法
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="document"></param>
        /// <param name="data"></param>
        public void Set(OfficeWord.Document document, object data)
        {
            if (!datas.ContainsKey(document))
            {
                datas.Add(document, data);
            }
            else
            {
                datas[document] = data;
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public object Get(OfficeWord.Document document)
        {
            if (datas.ContainsKey(document))
            {
                return datas[document];
            }

            return null;
        }

        /// <summary>
        /// 删除指定的数据
        /// </summary>
        /// <param name="document"></param>
        public void Remove(OfficeWord.Document document)
        {
            if (datas.ContainsKey(document))
            {
               datas.Remove(document);
            }
        }

        /// <summary>
        /// 检查是否存在指定的文档数据
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool Contains(OfficeWord.Document document)
        {
            return this.datas.ContainsKey(document);
        }
        #endregion
    }
}
