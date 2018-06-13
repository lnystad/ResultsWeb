// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSnifferEngine.cs" company="">
//   
// </copyright>
// <summary>
//   The file sniffer engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUploaderService
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Threading;
    using System.Web.Configuration;

    using FileUploaderService.Diagnosis;
    using FileUploaderService.Ftp;
    using FileUploaderService.KME;
    using FileUploaderService.Orion;
    using FileUploaderService.Utils;

    /// <summary>
    /// The file sniffer engine.
    /// </summary>
    public class FileSnifferEngine
    {
        #region Fields

        /// <summary>
        /// The bk up bitmap.
        /// </summary>
        private bool BkUpBitmap;

        /// <summary>
        /// The upload bitmap.
        /// </summary>
        private bool UploadBitmap;

        /// <summary>
        /// The m_file loader.
        /// </summary>
        private LeonFileLoader m_fileLoader;

        /// <summary>
        /// The m_install dir.
        /// </summary>
        private string m_installDir;

        /// <summary>
        /// The m_my ftp util.
        /// </summary>
        private FtpUtility m_myFtpUtil;

        /// <summary>
        /// The m_my orion file loader 100 m.
        /// </summary>
        private OrionFileLoader m_myOrionFileLoader100m;

        /// <summary>
        /// The m_my orion file loader 15 m.
        /// </summary>
        private OrionFileLoader m_myOrionFileLoader15m;

        /// <summary>
        /// The m_my orion file loader 200 m.
        /// </summary>
        private List<OrionFileLoader> m_myOrionFileLoader200m;

        /// <summary>
        /// The m_my orion file loader bkup.
        /// </summary>
        private OrionFileLoader m_myOrionFileLoaderBkup;

        /// <summary>
        /// The m_remote bit map dir 100 m.
        /// </summary>
        private string m_remoteBitMapDir100m;

        /// <summary>
        /// The m_remote bit map dir 15 m.
        /// </summary>
        private string m_remoteBitMapDir15m;

        /// <summary>
        /// The m_remote bit map dir 200 m.
        /// </summary>
        private string m_remoteBitMapDir200m;

        /// <summary>
        /// The m_remote dir.
        /// </summary>
        private string m_remoteDir;

        /// <summary>
        /// The m_remote pdf file name.
        /// </summary>
        private string m_remotePdfFileName;

        /// <summary>
        /// The m_remote presse liste file name.
        /// </summary>
        private string m_remotePresseListeFileName;

        /// <summary>
        /// The m_stop me.
        /// </summary>
        private bool m_stopMe;


        private bool m_FullFtpUpload;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the exit code.
        /// </summary>
        public int ExitCode { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            try
            {
                this.InitApplication();
                Log.Info("Starting");
                while (!this.m_stopMe)
                {
                    if (this.m_fileLoader != null)
                    {
                        var webDir = this.m_fileLoader.CheckWebDir();
                        if (webDir != null)
                        {
                            Thread.Sleep(5000);
                            if (webDir.Command.HasFlag(UploadCommand.Reports))
                            {
                                this.UploadWebReportAndBitMap(webDir);
                            }
                            else if (webDir.Command.HasFlag(UploadCommand.Web))
                            {
                                this.UploadWeb(webDir);
                            }

                            //if (webDir.Command.HasFlag(UploadCommand.Pdf) && !string.IsNullOrEmpty(this.m_remotePdfFileName))
                            //{
                            //    this.UploadPdf(webDir);
                            //}

                            //if (webDir.Command.HasFlag(UploadCommand.PresseListe) && !string.IsNullOrEmpty(this.m_remotePresseListeFileName))
                            //{
                            //    this.UploadPresseListe(webDir);
                            //}

                            if (webDir.Command.HasFlag(UploadCommand.BitMap))
                            {
                                this.UploadBitMaps(webDir);
                            }

                            // if (webDir.Command.HasFlag(UploadCommand.Reports))
                            // {
                            // this.UploadWeb(webDir);
                            // }

                            // if (webDir.Command.HasFlag(UploadCommand.StartingList))
                            // {
                            // this.UploadStartingList(webDir);
                            // }
                        }

                        if (this.UploadBitmap )
                        {
                              if (this.m_myOrionFileLoader15m != null)
                                {
                                    var bbkupDir = this.m_myOrionFileLoader15m.CheckForNewBackupFiles();
                                    if (bbkupDir != null && bbkupDir.Count > 0)
                                    {
                                        var stevnerToUpload = this.m_fileLoader.GetStartListForDate(bbkupDir, BaneType.Femtenmeter);
                                        if (stevnerToUpload != null && stevnerToUpload.Count > 0)
                                        {
                                            this.m_fileLoader.BakupBitmapInStevner(stevnerToUpload);
                                        }
                                    }
                                }

                                if (this.m_myOrionFileLoader100m != null)
                                {
                                    var bbkupDir = this.m_myOrionFileLoader100m.CheckForNewBackupFiles();
                                    if (bbkupDir != null && bbkupDir.Count > 0)
                                    {
                                        var stevnerToUpload = this.m_fileLoader.GetStartListForDate(bbkupDir, BaneType.Hundremeter);
                                        if (stevnerToUpload != null && stevnerToUpload.Count > 0)
                                        {
                                            this.m_fileLoader.BakupBitmapInStevner(stevnerToUpload);
                                        }

                                        var stevnerToUploadFelt = this.m_fileLoader.GetStartListForDate(bbkupDir, BaneType.FinFelt);
                                        if (stevnerToUploadFelt != null && stevnerToUploadFelt.Count > 0)
                                        {
                                            this.m_fileLoader.BakupBitmapInStevner(stevnerToUploadFelt);
                                        }
                                    }
                                }

                                if (this.m_myOrionFileLoader200m != null && this.m_myOrionFileLoader200m.Count > 0)
                                {
                                    foreach (var orion200mfileLoader in this.m_myOrionFileLoader200m)
                                    {
                                        var bbkupDir = orion200mfileLoader.CheckForNewBackupFiles();
                                        if (bbkupDir != null && bbkupDir.Count > 0)
                                        {
                                            var stevnerToUpload = this.m_fileLoader.GetStartListForDate(bbkupDir, BaneType.Tohundremeter);
                                            if (stevnerToUpload != null && stevnerToUpload.Count > 0)
                                            {
                                                this.m_fileLoader.BakupBitmapInStevner(stevnerToUpload);
                                            }

                                            var stevnerToUploadGrov = this.m_fileLoader.GetStartListForDate(bbkupDir, BaneType.GrovFelt);
                                            if (stevnerToUploadGrov != null && stevnerToUploadGrov.Count > 0)
                                            {
                                                this.m_fileLoader.BakupBitmapInStevner(stevnerToUploadGrov);
                                            }
                                        }
                                    }
                                }
                            }
                    }

                    //if (this.BkUpBitmap && this.m_myOrionFileLoaderBkup != null)
                    //{
                    //    bool upload = this.m_myOrionFileLoaderBkup.CheckForNewBitmapFiles();
                    //    if (upload)
                    //    {
                    //        var bitmapFiles = this.m_myOrionFileLoaderBkup.BackUpBitMaps();
                    //    }
                    //}

                    Thread.Sleep(10000);
                }

                Log.Info("Stopping");
                this.ExitCode = 0;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                this.ExitCode = 10;
                throw;
            }
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.m_stopMe = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The init application.
        /// </summary>
        private void InitApplication()
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
            Log.Info("Starting Config");
            this.m_installDir = ConfigurationManager.AppSettings["LeonInstallDir"];

            var ftpServer = ConfigurationManager.AppSettings["FtpServer"];
            var ftpUserName = ConfigurationManager.AppSettings["FtpUserName"];
            var ftpPassWord = ConfigurationManager.AppSettings["FtpPassWord"];
            this.m_remoteDir = ConfigurationManager.AppSettings["RemoteSubDir"];
            this.m_remotePdfFileName = ConfigurationManager.AppSettings["RemotePdfFileName"];
            this.m_remotePresseListeFileName = ConfigurationManager.AppSettings["RemotePresseListeFileName"];

            string hostIp = ConfigurationManager.AppSettings["FtpHostIp"];
            string hostPort = ConfigurationManager.AppSettings["FtpHostPort"];

            m_FullFtpUpload = false;
            string FtpFullUpload = ConfigurationManager.AppSettings["FtpFullUpload"];
            if (!string.IsNullOrEmpty(FtpFullUpload))
            {
                bool test;
                if (bool.TryParse(FtpFullUpload, out test))
                {
                    m_FullFtpUpload = test;
                }
            }


            string bitMapFeltstr = ConfigurationManager.AppSettings["BitMapStevneType"];
            BaneType bitmapfelt = BaneType.Undefined;
            if (!string.IsNullOrEmpty(bitMapFeltstr))
            {
                BaneType testVal;
                if (Enum.TryParse(bitMapFeltstr, out testVal))
                {
                    // We now have an enum type.
                    bitmapfelt = testVal;
                }
            }

            string bitMapSkiveriLagetstr = ConfigurationManager.AppSettings["BitMapSkiveriLaget"];
            int bitMapSkiveriLaget = 0;
            if (!string.IsNullOrEmpty(bitMapSkiveriLagetstr))
            {
                int testValint;
                if (int.TryParse(bitMapSkiveriLagetstr, out testValint))
                {
                    bitMapSkiveriLaget = testValint;
                }
            }

            string bitMapStartHoldstr = ConfigurationManager.AppSettings["BitMapStartHold"];
            int bitMapStartHold = 1;
            if (!string.IsNullOrEmpty(bitMapStartHoldstr))
            {
                int testValint;
                if (int.TryParse(bitMapStartHoldstr, out testValint))
                {
                    bitMapStartHold = testValint;
                }
            }
            

            //string bitMapDir = ConfigurationManager.AppSettings["BitMapDir"];
            //string bitMapBackupDir = ConfigurationManager.AppSettings["BitMapBackupDir"];
            //string bitMapFetchTimeOutstr = ConfigurationManager.AppSettings["BitMapFetchTimeOut"];

            this.m_remoteBitMapDir15m = ConfigurationManager.AppSettings["RemoteBitMapDir15m"];
            this.m_remoteBitMapDir100m = ConfigurationManager.AppSettings["RemoteBitMapDir100m"];
            this.m_remoteBitMapDir200m = ConfigurationManager.AppSettings["RemoteBitMapDir200m"];

            string uploadBitmapstr = ConfigurationManager.AppSettings["UploadBitmap"];

       


            string RapportXsltFilFileName = ConfigurationManager.AppSettings["RapportXsltFil"];

            string TopListSkyttereXsltFileName = ConfigurationManager.AppSettings["TopListSkyttereXsltFil"];
            string TopListXsltFileName = ConfigurationManager.AppSettings["TopListXsltFil"];

            string TopListLagSkyttereXsltFilFileName = ConfigurationManager.AppSettings["TopListLagSkyttereXsltFil"];

            bool debugMerge = false;
            string debugXslt = ConfigurationManager.AppSettings["DebugXslt"];
            if (!string.IsNullOrEmpty(debugXslt))
            {
                bool val;
                if (bool.TryParse(debugXslt, out val))
                {
                    debugMerge = val;
                }
            }

            //this.BkUpBitmap = false;
            this.UploadBitmap = false;
            //if (!string.IsNullOrEmpty(bitMapDir) && !string.IsNullOrEmpty(bitMapBackupDir))
            //{
            //    if (!Directory.Exists(bitMapDir))
            //    {
            //        Log.Error("Bitmap dir not exsist {0} ", bitMapDir);
            //        this.BkUpBitmap = false;
            //    }
            //    else
            //    {
            //        if (!Directory.Exists(bitMapBackupDir))
            //        {
            //            Log.Error("bitMapBackupDir dir not exsist {0} ", bitMapBackupDir);
            //            this.BkUpBitmap = false;
            //        }
            //        else
            //        {
            //            this.BkUpBitmap = true;
            //            Log.Error("Bitmaps will be backedup from {0} to {1} ", bitMapDir, bitMapBackupDir);
            //        }
            //    }
            //}

            if (!string.IsNullOrEmpty(uploadBitmapstr))
            {
                bool result = false;
                if (bool.TryParse(uploadBitmapstr, out result))
                {
                    this.UploadBitmap = result;
                }
            }

            //if (this.BkUpBitmap)
            //{
            //    Log.Info("Bitmap Bakup from Orion Initiated {0} {1}", bitMapDir, bitMapBackupDir);
            //    this.m_myOrionFileLoaderBkup = new OrionFileLoader(bitMapDir, bitMapBackupDir, bitMapFetchTimeOutstr, bitmapfelt, bitMapSkiveriLaget, bitMapStartHold);
            //}
            //else
            //{
            //    Log.Info("Bitmap Bakup from Orion not Initiated");
            //}

            if (!string.IsNullOrEmpty(this.m_remoteBitMapDir15m))
            {
                if (Directory.Exists(this.m_remoteBitMapDir15m))
                {
                    Log.Info("bitmaps fetch for 15m on dir ={0}", this.m_remoteBitMapDir15m);
                    this.m_myOrionFileLoader15m = new OrionFileLoader(this.m_remoteBitMapDir15m);
                }
                else
                {
                    Log.Info("No bitmaps fetch for 15m");
                }
            }

            if (!string.IsNullOrEmpty(this.m_remoteBitMapDir100m))
            {
                if (Directory.Exists(this.m_remoteBitMapDir100m))
                {
                    Log.Info("bitmaps fetch for 100m on dir ={0}", this.m_remoteBitMapDir100m);
                    this.m_myOrionFileLoader100m = new OrionFileLoader(this.m_remoteBitMapDir100m);
                }
                else
                {
                    Log.Info("No bitmaps fetch for 100m");
                }
            }
            this.m_myOrionFileLoader200m = new List<OrionFileLoader>();
            if (!string.IsNullOrEmpty(this.m_remoteBitMapDir200m))
            {
                var remote200dirs = this.m_remoteBitMapDir200m.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var mDirs200 in remote200dirs)
                {
                    var mDirs200trimmed = mDirs200.Trim();
                    if (string.IsNullOrEmpty(mDirs200trimmed))
                    {
                        continue;
                    }

                    if (Directory.Exists(mDirs200trimmed))
                    {
                        Log.Info("bitmaps fetch for 200m on dir ={0}", mDirs200trimmed);
                        this.m_myOrionFileLoader200m.Add(new OrionFileLoader(mDirs200trimmed));
                    }
                    else
                    {
                        Log.Info("No bitmaps fetch for 200m {0} No specified dir found", mDirs200trimmed);
                    }
                }
            }

            string UploadLoadWebstr = ConfigurationManager.AppSettings["UploadLoadWeb"];
            bool uploadWeb = true;
            if (!string.IsNullOrEmpty(UploadLoadWebstr))
            {
                bool result = false;
                if (bool.TryParse(UploadLoadWebstr, out result))
                {
                    uploadWeb = result;
                }
            }

            if (this.UploadBitmap || !string.IsNullOrEmpty(this.m_installDir))
            {
                this.m_myFtpUtil = new FtpUtility(uploadWeb, ftpServer, hostIp, hostPort, ftpUserName, ftpPassWord);
            }

            if (!string.IsNullOrEmpty(this.m_installDir))
            {
                this.m_fileLoader = new LeonFileLoader(this.m_installDir);
                this.m_fileLoader.DebugMergedXml = debugMerge;
                if (!string.IsNullOrEmpty(RapportXsltFilFileName))
                {
                    this.m_fileLoader.InitRapportTransform(RapportXsltFilFileName);
                }

                if (!string.IsNullOrEmpty(RapportXsltFilFileName) && !string.IsNullOrEmpty(TopListSkyttereXsltFileName))
                {
                    this.m_fileLoader.InitToppListInfoTransform(TopListXsltFileName, TopListSkyttereXsltFileName);
                }

                if (!string.IsNullOrEmpty(TopListLagSkyttereXsltFilFileName))
                {
                    this.m_fileLoader.InitToppListLagSkytingTransform(TopListLagSkyttereXsltFilFileName);
                }
            }
        }

        /// <summary>
        /// The upload bit maps.
        /// </summary>
        /// <param name="webDir">
        /// The web dir.
        /// </param>
        private void UploadBitMaps(LeonDirInfo webDir)
        {
            if (webDir.StevneForAlleBaner == null)
            {
                return;
            }

            if (webDir.StevneForAlleBaner.DynamiskeBaner == null)
            {
                return;
            }

            if (webDir.StevneForAlleBaner.DynamiskeBaner.Count <= 0)
            {
                return;
            }

            foreach (var bane in webDir.StevneForAlleBaner.DynamiskeBaner)
            {
                if (bane.BitmapsStoredInBane == null)
                {
                    continue;
                }

                string navn = null;
                if (bane.BitmapsStoredInBane.Updated)
                {
                    bane.BitmapsStoredInBane.Updated = false;
                    List<string> bitmap = new List<string>();
                    foreach (var file in bane.BitmapsStoredInBane.BitmapFiles)
                    {
                        bitmap.Add(file.FullName);
                    }

                    if (bitmap.Count <= 0)
                    {
                        Log.Trace(" No bitmaps");
                        continue;
                    }

                    var dirbitmapBaner = Path.GetDirectoryName(bitmap[0]);
                    var dirbitmap = Path.GetDirectoryName(dirbitmapBaner);
                    var dirStevne = Path.GetDirectoryName(dirbitmap);
                    dirStevne = Path.GetFileName(dirStevne);
                    navn = ParseHelper.RemoveDirLetters(dirStevne);
                    string prefix = null;
                    switch (bane.BaneType)
                    {
                        case BaneType.Femtenmeter:
                            prefix = Constants.Prefix15m;
                            break;
                        case BaneType.Hundremeter:
                            prefix = Constants.Prefix100m;
                            break;
                        case BaneType.Tohundremeter:
                            prefix = Constants.Prefix200m;
                            break;
                        case BaneType.FinFelt:
                            prefix = Constants.PrefixFinFelt;
                            break;
                        case BaneType.GrovFelt:
                            prefix = Constants.PrefixGrovFelt;
                            break;
                    }

                    if (string.IsNullOrEmpty(prefix))
                    {
                        Log.Info(" Prefix for bitmaps is missing stevne={0}", navn);
                        continue;
                    }

                    this.m_myFtpUtil.UploadFiles(m_FullFtpUpload,this.m_remoteDir, webDir.TargetName, bitmap.ToArray(), prefix);
                }
            }
        }

        // private void UploadBitMaps(System.Collections.Generic.List<StartingListStevne> bitmapStevner)
        // {
        // foreach (var stevne in bitmapStevner)
        // {
        // List<string> backupBitmapMatch = new List<string>();
        // foreach (var startingListLag in stevne.StevneLag)
        // {

        // foreach (var skive in startingListLag.Skiver)
        // {
        // if (skive.BackUpBitMapFile != null)
        // {
        // backupBitmapMatch.Add(skive.BackUpBitMapFile.FullName);
        // }
        // }

        // if (!string.IsNullOrEmpty(startingListLag.FullFileNameGravLapper))
        // {
        // backupBitmapMatch.Add(startingListLag.FullFileNameGravLapper);
        // }

        // }

        // if (backupBitmapMatch.Count > 0)
        // {
        // this.m_myFtpUtil.UploadFiles(this.m_remoteDir, stevne.StevneNavn, backupBitmapMatch.ToArray());
        // }
        // }

        // }

        /// <summary>
        /// The upload pdf.
        /// </summary>
        /// <param name="webDir">
        /// The web dir.
        /// </param>
        private void UploadPdf(LeonDirInfo webDir)
        {
            this.m_fileLoader.UpdatePdfTimeStamp(webDir);
            this.m_myFtpUtil.UploadFile(this.m_remoteDir, webDir.TargetName, webDir.PdfFileName, this.m_remotePdfFileName);
        }

        /// <summary>
        /// The upload presse liste.
        /// </summary>
        /// <param name="webDir">
        /// The web dir.
        /// </param>
        private void UploadPresseListe(LeonDirInfo webDir)
        {
            this.m_myFtpUtil.UploadFile(this.m_remoteDir, webDir.TargetName, webDir.PresseListeFileName, this.m_remotePresseListeFileName);
        }

        /// <summary>
        /// The upload web.
        /// </summary>
        /// <param name="webDir">
        /// The web dir.
        /// </param>
        private void UploadWeb(LeonDirInfo webDir)
        {
            if (Directory.Exists(webDir.WebName))
            {
                var files = Directory.GetFiles(webDir.WebName);
                if (files.Length <= 0)
                {
                    return;
                }

                this.m_fileLoader.UpdateWebTimeStamp(webDir);
                this.m_myFtpUtil.UploadFiles(m_FullFtpUpload, this.m_remoteDir, webDir.TargetName, files);
            }
        }

        /// <summary>
        /// The upload web report and bit map.
        /// </summary>
        /// <param name="webDir">
        /// The web dir.
        /// </param>
        private void UploadWebReportAndBitMap(LeonDirInfo webDir)
        {
            if (Directory.Exists(webDir.WebName))
            {
                var files = Directory.GetFiles(webDir.WebName);
                if (files.Length <= 0)
                {
                    return;
                }

                if (webDir.StevneForAlleBaner != null)
                {
                    foreach (var baner in webDir.StevneForAlleBaner.DynamiskeBaner)
                    {
                        if (baner.BitmapsStoredInBane != null)
                        {
                            baner.BitmapsStoredInBane.Updated = true;
                            this.UploadBitMaps(webDir);
                        }
                    }
                }

                this.m_fileLoader.UpdateWebTimeStamp(webDir);
                this.m_myFtpUtil.UploadFiles(m_FullFtpUpload, this.m_remoteDir, webDir.TargetName, files);
            }
        }

        #endregion
    }
}