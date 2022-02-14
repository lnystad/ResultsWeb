﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestSendingResult
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;

    using FileUploaderService.Diagnosis;
    using FileUploaderService.KME;
    using FileUploaderService.ListeSort;
    using Microsoft.Extensions.Configuration;
    using WebResultsClient.Viewmodels;

    [TestClass]
    public class UpLoadStevneViewModelUnitTest
    {
        private IConfiguration _configuration;

        [TestInitialize]
        public void init()
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
            _configuration = builder.Build();

            var logfile = _configuration["LogFile"];

            var LoggingLevelsString = _configuration["LoggingLevels"];
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
            LeonDirInfo info = new LeonDirInfo();
            DirectoryInfo infoDir = new DirectoryInfo(@"C: \Users\lan\Source\Repos\ResultsWeb\UnitTestSendingResult\TestData\Web");

            info.ParseIndexHtmlFile(infoDir);
        }

        [TestMethod]
        public void TestMethod2()
        {
            LeonDirInfo info = new LeonDirInfo();
            DirectoryInfo infoDir = new DirectoryInfo(@"C: \Users\lan\Source\Repos\ResultsWeb\UnitTestSendingResult\TestData\Web2");

            var st = info.ParseIndexHtmlFile(infoDir);
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
                StartListBane bn = parser.ParseOvelse(ovelser);
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
           
            UpLoadStevneViewModel mod = new UpLoadStevneViewModel(_configuration, "","","");
            mod.StevneDir = @"C:\Users\lan\source\repos\ResultsWeb\WebResults\Tests\UnitTestSendingResult\TestData\ResultaterBane";
            mod.StevneNavn = "Vårmønstring AN 2 100m";
            string rapportXsltFile = @"C:\Users\lan\Source\repos\ResultsWeb\WebResults\Common\FileUploaderService\Leon\RapportXslt.xslt";
            string topListSkyttere = @"C:\Users\lan\Source\repos\ResultsWeb\WebResults\Common\FileUploaderService\Leon\SkytteriLag.xslt";
            string TopListXslt = @"C:\Users\lan\Source\repos\ResultsWeb\WebResults\Common\FileUploaderService\Leon\TopListeXslt.xslt";
            string topListLagSkyttere = @"C:\Users\lan\Source\repos\ResultsWeb\WebResults\Common\FileUploaderService\Leon\SkytteriLagskyting.xslt";

            mod.InitLeonFileLoader(true, rapportXsltFile, topListSkyttere, TopListXslt, topListLagSkyttere);
            mod.SetBitmapLinks();

        }

       
    }
}
