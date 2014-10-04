using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Cooperativeness.FileTransfer.Core
{
    public enum TransferMode
    {
        Local,
        Remote
    };

    /*
     *  <transferinfo>
	        <fileserverinfo hostname='' port='' connect='' virtualdirectory='' filedirectory='' uploadposturl='' brokenresume='' debug='' />
	        <fileserverproxy isused='' uriaddress='' port='' />
	        <fileserverauth isused='' username='' password='' />
        </transferinfo>
     */

    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlRoot("transferinfo")]
    public partial class TransferInfo
    {
        [XmlElement(ElementName = "fileserverinfo")]
        public FileServerInfo FileServerInfo { get; set; }

        [XmlElement(ElementName = "fileserverproxy")]
        public FileServerProxy FileServerProxy { get; set; }

        [XmlElement(ElementName = "fileserverauth")]
        public FileServerAuth FileServerAuth { get; set; }
    }

    [Serializable()]
    public class FileServerInfo
    {

        [XmlAttribute("hostname")]
        public string HostName { get; set; }

        [XmlAttribute(DataType = "integer",AttributeName = "port")]
        public string Port { get; set; }

        [XmlAttribute("connect")]
        public string Connect { get; set; }

        [XmlAttribute("virtualdirectory")]
        public string VirtualDirectory { get; set; }

        [XmlAttribute("filedirectory")]
        public string FileDirectory { get; set; }

        [XmlAttribute("uploadposturl")]
        public string UploadPostUrl { get; set; }

        private string _supportBrokenResume;

        [XmlAttribute("brokenresume")]
        public string SupportBrokenResume { 
                get 
                { 
                    if (string.IsNullOrEmpty(_supportBrokenResume)) _supportBrokenResume = "true";
                    return _supportBrokenResume;
                }
            set { _supportBrokenResume = value; }
        }

        private string _supportDebug;

        [XmlAttribute("debug")]
        public string SupportDebug { 
            get 
            {
                if (string.IsNullOrEmpty(_supportDebug)) _supportDebug = "false";
                return _supportDebug;
            }
            set { _supportDebug = value; }
        }
    }

    [Serializable()]
    public class FileServerProxy
    {
        private string _isUsed;
        [XmlAttribute(AttributeName = "isused")]
        public string IsUsed { 
            get 
            { 
                if (string.IsNullOrEmpty(_isUsed)) _isUsed= "false";
                return _isUsed;
            }
            set { _isUsed = value; }
        }

        [XmlAttribute("uriaddress")]
        public string UriAddress { get; set; }

        [XmlAttribute(DataType = "integer",AttributeName = "port")]
        public string Port { get; set; }
    }

    [Serializable()]
    public class FileServerAuth
    {
        private string _isUsed;

        [XmlAttribute("isused")]
        public string IsUsed { 
            get 
            { 
                if (string.IsNullOrEmpty(_isUsed)) _isUsed = "false";
                return _isUsed;
            }
            set { _isUsed = value; }
        }


        [XmlAttribute("username")]
        public string UserName { get; set; }


        [XmlAttribute("password")]
        public string Password { get; set; }
    }

}
