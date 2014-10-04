using System;
using System.IO;
using System.Xml;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer.Downloader
{
    internal class DownloadHistory
    {
        private static string GetFilePath(string remotefile)
        {
            return Logger.DownloadLogDirectory+"\\" + DateTime.Now.ToString("yyyy-MM")+remotefile+ ".xml";
        }

        public static void Insert(string remotefile)
        {
            var document = new XmlDocument();
            var filepath = GetFilePath(remotefile);
            if (!File.Exists(filepath))
            {
                document.LoadXml(
                    "<?xml version=\"1.0\" encoding=\"gb2312\" ?>" +
                    "<FileList></FileList>");
            }
            else
            {
                document.Load(filepath);
            }
            XmlNode rootNode = document.DocumentElement;
            if (rootNode == null) return;
            XmlNode fileItemNode = document.CreateElement("FileItem");
            DownloadState.CurrentFileItem.Guid = Guid.NewGuid().ToString();
            AddAttribute(document, fileItemNode, "Guid", DownloadState.CurrentFileItem.Guid);
            AddAttribute(document, fileItemNode, "StartTime", DownloadState.CurrentFileItem.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
            AddAttribute(document, fileItemNode, "EndTime", DownloadState.CurrentFileItem.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
            AddAttribute(document, fileItemNode, "IsCompleted", DownloadState.CurrentFileItem.IsCompleted.ToString());
            AddAttribute(document, fileItemNode, "HasBreaked", DownloadState.CurrentFileItem.HasBreaked.ToString());
            AddAttribute(document, fileItemNode, "BreakInfo", DownloadState.CurrentFileItem.BreakInfo);
            rootNode.AppendChild(fileItemNode);

            XmlNode applyNode = document.CreateElement("Apply");
            AddAttribute(document, applyNode, "LastModified", DownloadState.CurrentApply.LastModified.ToString("yyyy-MM-dd HH:mm:ss"));
            AddAttribute(document, applyNode, "FileSize", DownloadState.CurrentApply.FileSize.ToString());
            AddAttribute(document, applyNode, "AllowRanges", DownloadState.CurrentApply.AllowRanges.ToString());
            AddAttribute(document, applyNode, "BlockSize", DownloadState.CurrentApply.BlockSize.ToString());
            AddAttribute(document, applyNode, "ActuallyChunks", DownloadState.CurrentApply.ActuallyChunks.ToString());
            fileItemNode.AppendChild(applyNode);

            XmlNode transferParameterNode = document.CreateElement("TransferParameter");
            AddAttribute(document, transferParameterNode, "TransferUrl", DownloadState.Parameter.TransferUrl);
            AddAttribute(document, transferParameterNode, "ChunkCount", DownloadState.Parameter.ChunkCount.ToString());
            AddAttribute(document, transferParameterNode, "LocalFile", DownloadState.Parameter.LocalFile);
            fileItemNode.AppendChild(transferParameterNode);

            document.Save(filepath);
        }

        public static bool Update(string remotefile,FileItem oUpdateInfo)
        {
            var filepath = GetFilePath(remotefile);
            if (!File.Exists(filepath)) return false;
            bool isExist = false;
            var document = new XmlDocument();
            document.Load(filepath);
            XmlNode rootNode = document.DocumentElement;
            if (rootNode == null) return false;

            foreach (XmlNode fileItem in rootNode.ChildNodes)
            {
                if (fileItem.Attributes != null && oUpdateInfo.Guid == fileItem.Attributes["Guid"].Value)
                {
                    fileItem.Attributes["EndTime"].Value = oUpdateInfo.EndTime.ToString("yyyy-MM-dd HH:mm:ss");
                    fileItem.Attributes["IsCompleted"].Value = oUpdateInfo.IsCompleted.ToString();
                    fileItem.Attributes["HasBreaked"].Value = oUpdateInfo.HasBreaked.ToString();
                    fileItem.Attributes["BreakInfo"].Value = oUpdateInfo.BreakInfo;
                    isExist = true;
                    break;
                }
            }
            document.Save(filepath);
            return isExist;
        }

        public static bool Exists(string remotefile, ref FileItem oFileItem, ref Apply oApply, ref TransferParameter oInputParam, bool bHasFinished)
        {
            var filepath = GetFilePath(remotefile);
            if (!File.Exists(filepath)) return false;
            bool isExist = false;
            var document = new XmlDocument();
            document.Load(filepath);
            XmlNode rootNode = document.DocumentElement;
            if (rootNode == null) return false;
            foreach (XmlNode fileItem in rootNode.ChildNodes)
            {
                if(fileItem.Attributes==null) continue;
                oFileItem.Guid = fileItem.Attributes["Guid"].Value;
                oFileItem.StartTime = TypeConvert.ToDateTime(fileItem.Attributes["StartTime"].Value, DateTime.MinValue);
                oFileItem.EndTime = TypeConvert.ToDateTime(fileItem.Attributes["EndTime"].Value, DateTime.MinValue);
                oFileItem.IsCompleted = TypeConvert.ToBool(fileItem.Attributes["IsCompleted"].Value, false);
                oFileItem.HasBreaked = TypeConvert.ToBool(fileItem.Attributes["HasBreaked"].Value, false);
                oFileItem.BreakInfo = fileItem.Attributes["BreakInfo"].Value;

                foreach (XmlNode item in fileItem.ChildNodes)
                {
                    if (item.Attributes == null) continue;
                    if (item.Name == "Apply")
                    {
                        oApply.LastModified = TypeConvert.ToDateTime(item.Attributes["LastModified"].Value, DateTime.MinValue);
                        oApply.FileSize = TypeConvert.ToLong(item.Attributes["FileSize"].Value, 0);
                        oApply.AllowRanges = TypeConvert.ToBool(item.Attributes["AllowRanges"].Value, false);
                        oApply.BlockSize = TypeConvert.ToInt(item.Attributes["BlockSize"].Value, 0);
                        oApply.ActuallyChunks = TypeConvert.ToInt(item.Attributes["ActuallyChunks"].Value, 0);
                    }
                    else
                    {
                        oInputParam.TransferUrl = item.Attributes["TransferUrl"].Value;
                        oInputParam.LocalFile = item.Attributes["LocalFile"].Value;
                        oInputParam.ChunkCount = (short)TypeConvert.ToInt(item.Attributes["ChunkCount"].Value, 0);

                        if (DownloadState.Parameter.Equals(oInputParam) && !oFileItem.IsCompleted && !bHasFinished)
                        {
                            isExist = true;
                            break;
                        }
						if (DownloadState.Parameter.Equals(oInputParam) && oFileItem.IsCompleted && bHasFinished)
						{
							isExist = true;
							break;
						}
                    }
                }
                if (isExist) break;
            }
            return isExist;
        }

        private static void AddAttribute(XmlDocument doc, XmlNode node, string name, string text)
        {
            XmlAttribute xmlattr = doc.CreateAttribute(string.Empty, name, string.Empty);
            xmlattr.InnerText = text;
            if (node==null || node.Attributes==null) return;
            node.Attributes.Append(xmlattr);
        }
    }
}
