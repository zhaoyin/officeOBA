using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Cooperativeness.OBA.Word.Install;

namespace Cooperativeness.OBA.Word.UnitTest
{
    [TestFixture]
    public class Install
    {
        [SetUp]
        public void Init()
        {
        }

        [TearDown]
        public void Close()
        {
        }


        [Test]
        public void IsNeedAdmin()
        {
            bool result = OfficeUtils.IsNeedRunAsAdmin();
            Assert.IsTrue(result);
        }

        [Test]
        public void InstallTest()
        {
            bool result = OfficeUtils.Install(@"C:\Program Files (x86)\ZHAOYIN\Cooperativeness.Office");
            Assert.IsTrue(result);
        }
    }
}
