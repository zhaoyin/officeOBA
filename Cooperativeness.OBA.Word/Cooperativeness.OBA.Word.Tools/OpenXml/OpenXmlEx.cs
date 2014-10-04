using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace Cooperativeness.OBA.Word.Tools.OpenXml
{
    /// <summary>
    /// 定义用户自定义部件扩展类对象
    /// </summary>
    public static class OpenXmlEx
    {
        public const string RelationShipType = "";


        /// <summary>
        /// 创建用户自定义部件
        /// </summary>
        /// <param name="mainDocumentPart"></param>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        public static CustomXmlPart CreateCustomXmlPart(this MainDocumentPart mainDocumentPart, XDocument xdoc)
        {
            CustomXmlPart xmlPart = mainDocumentPart.AddCustomXmlPart(CustomXmlPartType.CustomXml);
            xmlPart.WriteXml(xdoc);
            return xmlPart;
        }

        /// <summary>
        /// 向制定的部件中写数据
        /// </summary>
        /// <param name="xmlPart"></param>
        /// <param name="xdoc"></param>
        public static void WriteXml(this CustomXmlPart xmlPart, XDocument xdoc)
        {
            Stream stream = xmlPart.GetStream(FileMode.Create, FileAccess.Write);
            var writer = new XmlTextWriter(stream, Encoding.UTF8);
            xdoc.Save(writer);
            writer.Flush();
            writer.Close();
            stream.Close();
        }

        /// <summary>
        /// 获取用户自定义部件XML数据
        /// </summary>
        /// <param name="xmlPart"></param>
        /// <returns></returns>
        public static XDocument GetXDocument(this CustomXmlPart xmlPart)
        {
            using (Stream stream = xmlPart.GetStream(FileMode.Open, FileAccess.Read))
            {
                using (XmlReader xReader = XmlReader.Create(stream, new XmlReaderSettings()))
                {
                    XDocument xDocument = XDocument.Load(xReader);
                    return xDocument;
                }
            }
        }
    }
}
