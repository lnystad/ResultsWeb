using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileUploaderService
{
    using System.IO;

    using FileUploaderService.KME;

    //using FileUploaderService.KME;

    // using FileUploaderService.KME;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            LeonDirInfo info=new LeonDirInfo();
            DirectoryInfo infoDir= new DirectoryInfo(@"C: \Users\lan\Source\Repos\ResultsWeb\UnitTestSendingResult\TestData\");
            
            info.ParseIndexHtmlFile(infoDir);
        }
    }
}
