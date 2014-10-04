using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Cooperativeness.Files.Server.Install;
using NUnit.Framework;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer.Test
{
    [TestFixture]
    public class ClientServiceTest
    {
        private ClientService _client;

        [SetUp]
        public void SetUp()
        {
            _client=new ClientService();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
        }

        [Test]
        public void IisTest()
        {
            Iis6WebSite.HostName = "localhost";
            Iis6WebSite.CreateNewWebSite("localhost","91","abc","def","c:\test");
            Assert.IsTrue(true);
        }

        [Test]
        public void GetFiles()
        {
            SetSeverInfo();
            _client.Upload(@"C:\Documents and Settings\zhaoyin3\桌面\Process Monitor 助力Windows用户态程序高效排错.docx");
            _client.GetFiles("", "*.*.txt", false);
            Assert.IsTrue(true);
        }

        [Test]
        public void SetSeverInfo()
        {
            /*
             *
            <transferinfo>
                <fileserverinfo hostname='' port='' connect='' virtualdirectory='' filedirectory='' />
                <fileserverproxy isused='' uriaddress='' port='' />
                <fileserverauth isused='' username='' password='' />
            </transferinfo> 
             */
            var sbr = new StringBuilder();
            sbr.AppendLine("<transferinfo>");
            sbr.AppendLine("<fileserverinfo hostname='ZHAOYIN' port='80' connect='HTTP' virtualdirectory='FileTransfer' filedirectory=''  uploadposturl='WebUploaderPages.aspx'  brokenresume='true' debug='true'  />");
            //sbr.AppendLine("<fileserverinfo hostname='20.1.42.140' port='80' connect='HTTP' virtualdirectory='FileTransfer' filedirectory='org_songyjd2_UFDATA_301_2011'  uploadposturl='WebUploaderPages.aspx'  brokenresume='true' debug='true'  />");
            //sbr.AppendLine("<fileserverinfo hostname='20.1.42.140' port='80' connect='HTTP' virtualdirectory='FileTransfer' filedirectory=''  uploadposturl='WebUploaderPages.aspx'  brokenresume='true' debug='true'  />");
            //sbr.AppendLine("<fileserverinfo hostname='20.1.42.53' port='80' connect='HTTP' virtualdirectory='FileTransfer' filedirectory=''  uploadposturl='WebUploaderPages.aspx'  brokenresume='true' debug='true'  />");
            sbr.AppendLine("<fileserverproxy isused='' uriaddress='' port='' />");
            sbr.AppendLine("<fileserverauth isused='' username='' password='' />");
            sbr.AppendLine("</transferinfo>");
            _client.SetServerInfo(sbr.ToString());
            Assert.IsTrue(true);
        }

        [Test]
        public void UploadFile()
        {
            SetSeverInfo();
            string remoteFileName="";
            remoteFileName = _client.Upload(@"D:\跟我学自由泳.RMVB",3);
            Assert.IsNotNullOrEmpty(remoteFileName);
        }

        [Test]
        public void MergeFile()
        {
            FileUtil.MergeFiles(@"D:\", "3e78bd6c-9f52-476d-8578-7b8cc3172b37", false,"");
            Assert.IsTrue(true);
        }

        [Test]
        public void SplitFile()
        {
            //FileUtil.SplitFiles(@"D:\3e78bd6c-9f52-476d-8578-7b8cc3172b37.zip.txt", 1932735283);
            FileUtil.SplitFiles(@"D:\3e78bd6c-9f52-476d-8578-7b8cc3172b37.zip.txt", 1932735283);
            Assert.IsTrue(true);
        }

        [Test]
        public void DownloadFile()
        {
            SetSeverInfo();
            //string remoteFile = "2091f1ee-62ed-49e9-bd66-d09b1daa4da9.RMVB";
            //var fileArr = new string[] { "2091f1ee-62ed-49e9-bd66-d09b1daa4da9.RMVB", "1b0d7639-43e1-4c21-bad2-5ad8ef82157e.txt", "cf13d216-fe2d-427d-9c97-107cc59a6c85.txt" };
            //int iCount = 0;
            //foreach (var s in fileArr)
            //{
            //    bool result = _client.Download(s, @"D:\Test\" + s);
            //    if (result) iCount = iCount + 1;
            //}
            bool result = _client.Download("8fea956f-c651-4241-a9f0-0ec163e1f1d3.zip", @"D:\abcd.zip");
            Assert.IsTrue(result);
            //Assert.IsTrue(iCount==fileArr.Length);
        }

        [Test]
        public void PackageFiles()
        {
            SetSeverInfo();
            string result = _client.PackageFiles("org_20.1.41.203_UFDATA_111_2012", "");
            Assert.IsNotNullOrEmpty(result);
        }

        [Test]
        public void BackUpFiles()
        {
            SetSeverInfo();
            bool result = _client.BackUpFiles("org_songyjd2_UFDATA_301_2011", "", "D:\\abcd.zip");
            //bool result = _client.BackUpFiles("org_songyjd2_UFDATA_301_2011", "", "D:\\abc.zip");
            //bool result = _client.BackUpFiles("org_20.1.41.39_UFDATA_001_2014", "", "D:\\def.zip");
            Assert.IsTrue(result);
        }

        [Test]
        public void RestoreFiles()
        {
            SetSeverInfo();
            bool result = _client.RestoreFiles("D:\\abcd.zip", "", "org_songyjd2_UFDATA_301_2011");
            //bool result = _client.RestoreFiles("D:\\def.zip", "", "org_20.1.41.39_UFDATA_001_2014");
            Assert.IsTrue(result);
        }

        [Test]
        public void UnPackageFiles()
        {
            SetSeverInfo();
            //bool ret = _client.UnpackageFiles("2a0863f5-9373-4b9f-83f3-d8c3878726d4.zip.txt", "", "", "org_20.1.41.203_UFDATA_111_2012");
            bool ret = _client.UnpackageFiles("abcd.zip", "", "", "org_20.1.41.203_UFDATA_111_2012");

            Assert.IsTrue(ret);
        }

        [Test]
        public void LocalPackageFiles()
        {
            //var zip=new SharpZipLibHelper();
            //bool result=zip.Compress(@"D:\\AAAA", @"D:\\" + Guid.NewGuid().ToString() + ".zip", true);
            bool result = SharpZipLibHelper.Compress(@"D:\源代码\U8V12.0\FileManager\Demo",
                                       @"D:\源代码\U8V12.0\FileManager\" + Guid.NewGuid().ToString() + ".zip", true, "");
            Assert.IsTrue(result);
        }

        [Test]
        public void LocalUnPackageFiles()
        {
            //var zip = new SharpZipLibHelper();
            //zip.DeCompress(@"D:\源代码\U8V12.0\FileManager\c9294d75-3357-4d3a-bffb-e2f16a382e97.zip",@"D:\源代码\U8V12.0\FileManager\aaa", "");
            SharpZipLibHelper.DeCompress(@"D:\abcd.zip", @"D:\abcd", "");
            Assert.IsTrue(true);
        }
    }
}
