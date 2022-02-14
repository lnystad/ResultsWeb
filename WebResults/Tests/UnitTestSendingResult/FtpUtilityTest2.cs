using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileUploaderService.Ftp;

namespace UnitTestSendingResult
{
    [TestClass]
    public class FtpUtilityTest2
    {
     
        [TestMethod]
        public void TestMethod1()
        {
            FtpUtility test = new FtpUtility(true, "ftp.livevisning.com", "", "", "bos@livevisning.com", "rxkx9g826ro3");
            var list = test.ListDirectories();
        }

        [TestMethod]
        public void TestMethod2()
        {
            FtpUtility test = new FtpUtility(true, "ftp.livevisning.com", "", "", "bos@livevisning.com", "rxkx9g826ro3");
            var list = test.SubDirectories("15m");
        }
    }
}
