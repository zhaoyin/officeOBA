using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;

namespace Cooperativeness.OBA.Word.Tools.Office
{
    /// <summary>
    /// 定义Word文档扩展对象
    /// </summary>
    public static class DocumentEx
    {
        private static readonly Logger _logger=new Logger(typeof(DocumentEx));
        /// <summary>
        /// 从XML中取得命名空间
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static string GetCustomXmlNameSpace(string xml)
        {
            XDocument xDoc = XDocument.Parse(xml);
            XElement xRoot = xDoc.Root;
            if (xRoot != null)
            {
                XAttribute xAttribute = xRoot.Attribute("xmlns");
                if (xAttribute != null)
                {
                    return xAttribute.Value;
                }
            }
            return "";
        }
        /// <summary>
        /// 插入用户自定义的XML文档
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static CustomXMLPart InsertCustomerXml(this Document doc, string xml)
        {
            string nameSpace = GetCustomXmlNameSpace(xml);
            if (!string.IsNullOrEmpty(nameSpace))
            {
                // 首先删除指定的自定义文档
                DeletePartByNamespace(doc, nameSpace);
            }
            
            Object type = Type.Missing;
            return doc.CustomXMLParts.Add(xml, type);
        }

        /// <summary>
        /// 更新用户自定义的XML文档
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static void UpdateCustomerXml(this Document document, string xml)
        {
            try
            {
                string nameSpace = GetCustomXmlNameSpace(xml);
                if (!string.IsNullOrEmpty(nameSpace))
                {
                    // 首先删除指定的自定义文档
                    DeletePartByNamespace(document, nameSpace);
                }
                // 重新添加部件
                Object type = Type.Missing;
                document.CustomXMLParts.Add(xml, type);
            }
            catch(Exception e)
            {
                _logger.Debug(e);
            }
        }

        /// <summary>
        /// 根据命名空间选择获取用户自定义的XML部件
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        public static CustomXMLPart SelectPartByNamespace(this Document document, string nameSpace)
        {
            if (nameSpace == null)
                nameSpace = string.Empty;
            CustomXMLParts parts = document.CustomXMLParts.SelectByNamespace(nameSpace);
            foreach (CustomXMLPart part in parts)
            {
                return part;
            }
            return null;
        }

        /// <summary>
        /// 根据命名空间删除指定的部件
        /// </summary>
        /// <param name="document"></param>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        public static bool DeletePartByNamespace(this Document document, string nameSpace)
        {
            if (nameSpace == null)
                nameSpace = string.Empty;
            CustomXMLParts parts = document.CustomXMLParts.SelectByNamespace(nameSpace);
            foreach (CustomXMLPart part in parts)
            {
                part.Delete();
            }
            return true;
        }

        /// <summary>
        /// 根据命名空间选择获取用户自定义的XML部件
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        public static string SelectPartXmlByNamespace(this Document document, string nameSpace)
        {
            if (nameSpace == null)
                nameSpace = string.Empty;
            CustomXMLParts parts = document.CustomXMLParts.SelectByNamespace(nameSpace);
            
            foreach (CustomXMLPart part in parts)
            {
                return part.XML;
            }
            return null;
        }

        /// <summary>
        /// 根据命名空间选择用户自定义的XML部件列表
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        public static IEnumerable<CustomXMLPart> SelectPartsByNamespace(this Document document, string nameSpace)
        {
            if (nameSpace == null)
                nameSpace = string.Empty;

            CustomXMLParts parts = document.CustomXMLParts.SelectByNamespace(nameSpace);
            var list = new List<CustomXMLPart>();
            foreach (CustomXMLPart part in parts)
            {
                list.Add(part);
            }
            return list;
        }

        /// <summary>
        /// 根据ID选择用户自定义的XML部件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CustomXMLPart SelectPartById(this Document document, string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            return document.CustomXMLParts.SelectByID(id);
        }

        /// <summary>
        /// 另存为Word 2007
        /// </summary>
        /// <param name="document"></param>
        public static void SaveAsWord2007(this Document document)
        {
            object tmpDocName = document.Name;
            object missing = Type.Missing;
            object lockComments = false;
            object addToRecentFiles = false;
            object readOnlyRecommended = false;
            object embedTrueTypeFonts = false;
            object saveNativePictureFormat = true;
            object saveFormsData = false;
            object saveAsAoceLetter = false;
            object encoding = MsoEncoding.msoEncodingUSASCII;
            object insertLineBreaks = false;
            object allowSubstitutions = false;
            object lineEnding = WdLineEndingType.wdCRLF;
            object addBiDiMarks = false;

            document.SaveAs(ref tmpDocName, ref missing, ref lockComments,
                ref missing, ref addToRecentFiles, ref missing, 
                ref readOnlyRecommended,ref embedTrueTypeFonts, 
                ref saveNativePictureFormat, ref saveFormsData,
                ref saveAsAoceLetter, ref encoding, ref insertLineBreaks, 
                ref allowSubstitutions, ref lineEnding,ref addBiDiMarks);
        }

        /// <summary>
        /// 插入用户自定义链接
        /// </summary>
        /// <param name="uri"></param>
        public static void InsertHyperlink(this Document document,string uri)
        {
            if (string.IsNullOrEmpty(uri)) return;
            Selection selection = document.Application.ActiveWindow.Selection;
            if(selection==null) return;
            if (string.IsNullOrEmpty(selection.Text.Trim())) return;
            object missing = Type.Missing;
            Range range = selection.Range;
            Hyperlinks hyperlinks = document.Hyperlinks;
            object address = uri;
            Hyperlink hyperlink = document.Hyperlinks._Add(range, ref address, ref missing);
        }

        /// <summary>
        /// 获取一个值，用来指示当前是否有文本已选择
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static bool DocumentTextSelected(this Document document)
        {
            Selection selection = document.Application.ActiveWindow.Selection;
            if (selection != null && selection.End == selection.Start)
                return false;
            return true;
        }

        /// <summary>
        /// 获取寻则区域的超链接
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static Hyperlinks DocumentHyperlinksSelected(this Document document)
        {
            Selection selection = document.Application.ActiveWindow.Selection;
            if (selection != null)
            {
                return selection.Hyperlinks;
            }
            return null;
        }
    }
}
