using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileUploaderService
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;

    using FileUploaderService.Diagnosis;
    using FileUploaderService.KME;
    using FileUploaderService.ListeSort;

    //using FileUploaderService.KME;

    // using FileUploaderService.KME;

    [TestClass]
    public class UnitTest1
    {
        [TestInitialize]
        void init()
        {
            var logfile = ConfigurationManager.AppSettings["LogFile"];

            var LoggingLevelsString = ConfigurationManager.AppSettings["LoggingLevels"];
            LoggingLevels enumLowestTrace = LoggingLevels.Info;
            if (!string.IsNullOrEmpty(LoggingLevelsString))
            {
                if (Enum.TryParse(LoggingLevelsString, true, out enumLowestTrace))
                {
                    enumLowestTrace = enumLowestTrace;
                }
                else
                {
                    enumLowestTrace = LoggingLevels.Info;
                }
            }

            var fileAppsender = new FileAppender(logfile, enumLowestTrace, LoggingLevels.Trace);
            Log.AddAppender(fileAppsender);

        }

        [TestMethod]
        public void TestMethod1()
        {
            LeonDirInfo info=new LeonDirInfo();
            DirectoryInfo infoDir= new DirectoryInfo(@"C: \Users\lan\Source\Repos\ResultsWeb\UnitTestSendingResult\TestData\Web");
            
            info.ParseIndexHtmlFile(infoDir);
        }

        [TestMethod]
        public void TestMethod2()
        {
            LeonDirInfo info = new LeonDirInfo();
            DirectoryInfo infoDir = new DirectoryInfo(@"C: \Users\lan\Source\Repos\ResultsWeb\UnitTestSendingResult\TestData\Web2");

            var st=info.ParseIndexHtmlFile(infoDir);
        }

        [TestMethod]
        public void TestMethodopprop()
        {
            LeonDirInfo info = new LeonDirInfo();
            var alleopropslister = info.GetOppropsLister(@"C: \Users\lan\Source\Repos\ResultsWeb\UnitTestSendingResult\TestData\Web\");
            ParseXmlOpp parser = new ParseXmlOpp();
            List<StartListBane> lister = new List<StartListBane>();
            foreach (var ovelser in alleopropslister)
            {
                StartListBane bn=parser.ParseOvelse(ovelser);
                lister.Add(bn);
            }

            var List = parser.GetOppropKat(lister);
            //foreach (var list in lister)
            //{
            //    foreach (var lag in list.StevneLag)
            //    {

            //    }
            //}
        }


        [TestMethod]
        public void TestMethod3()
        {
            var fileLoader = new LeonFileLoader(@"C:\Users\lan\Source\Repos\ResultsWeb\UnitTestSendingResult\TestData\Resultater");
            fileLoader.InitRapportTransform(@"C:\Users\lan\Source\Repos\ResultsWeb\FileUploaderService\Leon\RapportXslt.xslt");
            fileLoader.InitToppListInfoTransform(@"C:\Users\lan\Source\Repos\ResultsWeb\FileUploaderService\Leon\TopListeXslt.xslt", @"C:\Users\lan\Source\Repos\ResultsWeb\FileUploaderService\Leon\SkytteriLag.xslt");
            fileLoader.DebugMergedXml = true;
            var webDir1 = fileLoader.CheckWebDir(true);
            var webDir2 = fileLoader.CheckWebDir(true);


        }

        [TestMethod]
        public void TestMethodMinne()
        {
            LeonDirInfo info = new LeonDirInfo();
            DirectoryInfo infoDir = new DirectoryInfo(@"C:\Users\lan\Source\Repos\ResultsWeb\UnitTestSendingResult\TestData\Resultater\Lagmesterskap Grovfelt\Web");

            info.ParseIndexHtmlFile(infoDir);
        }


        [TestMethod]
        public void TestMethodBane()
        {
            var fileLoader = new LeonFileLoader(@"C:\Users\lan\Source\Repos\ResultsWeb\UnitTestSendingResult\TestData\ResultaterBane");
            fileLoader.InitRapportTransform(@"C:\Users\lan\Source\Repos\ResultsWeb\FileUploaderService\Leon\RapportXslt.xslt");
            fileLoader.InitToppListInfoTransform(@"C:\Users\lan\Source\Repos\ResultsWeb\FileUploaderService\Leon\TopListeXslt.xslt", @"C:\Users\lan\Source\Repos\ResultsWeb\FileUploaderService\Leon\SkytteriLag.xslt");
            fileLoader.DebugMergedXml = true;
            var webDir1 = fileLoader.CheckWebDir(true);
            var webDir2 = fileLoader.CheckWebDir(true);


        }

        [TestMethod]
        public void TestMethodIndexBane()
        {
            LeonDirInfo info = new LeonDirInfo();
            DirectoryInfo infoDir = new DirectoryInfo(@"C:\Users\lan\Source\Repos\ResultsWeb\UnitTestSendingResult\TestData\ResultaterBane\Vårmønstring AN 2 100m\Web");

            info.ParseIndexHtmlFile(infoDir);


        }

        [TestMethod]
        public void TestMethodIndexNewBane()
        {
            LeonDirInfo info = new LeonDirInfo();
            DirectoryInfo infoDir = new DirectoryInfo(@"C:\Users\lan\source\repos\ResultsWeb\WebResults\Tests\UnitTestSendingResult\TestData\ResultaterBane\Nytt stevne\Web");

            info.ParseIndexHtmlFile(infoDir);


        }
    }
}
