namespace FileUploaderService
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Threading;
    using System.Xml;
    using System.Xml.Serialization;

    using FileUploaderService.Ftp;
    using FileUploaderService.KME;
    using FileUploaderService.Orion;
    using FileUploaderService.Utils;

    using SendingResults.Diagnosis;

    public class FileSnifferEngine
    {
        private bool m_stopMe;

        
        private FtpUtility m_myFtpUtil;
        private LeonFileLoader m_fileLoader;

        private OrionFileLoader m_myOrionFileLoaderBkup;
        private OrionFileLoader m_myOrionFileLoader15m;
        private OrionFileLoader m_myOrionFileLoader100m;
        private OrionFileLoader m_myOrionFileLoader200m;

        private string m_installDir;

        private string m_remoteDir;

        private string m_remotePdfFileName;

        private string m_remotePresseListeFileName;

        private string m_remoteBitMapDir15m;

        private string m_remoteBitMapDir100m;

        private string m_remoteBitMapDir200m;


        private bool UploadBitmap;
        private bool BkUpBitmap;
        public void Start()
        {
            try
            {
                this.InitApplication();
                Log.Info("Starting");
                while (!this.m_stopMe)
                {
                    if (m_fileLoader != null)
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

                            if (webDir.Command.HasFlag(UploadCommand.Pdf) && !string.IsNullOrEmpty(m_remotePdfFileName))
                            {
                                this.UploadPdf(webDir);
                            }

                            if (webDir.Command.HasFlag(UploadCommand.PresseListe) && !string.IsNullOrEmpty(m_remotePresseListeFileName))
                            {
                                this.UploadPresseListe(webDir);
                            }

                            if (webDir.Command.HasFlag(UploadCommand.BitMap))
                            {
                                this.UploadBitMaps(webDir);
                            }

                            //if (webDir.Command.HasFlag(UploadCommand.Reports))
                            //{
                            //    this.UploadWeb(webDir);
                            //}

                            //if (webDir.Command.HasFlag(UploadCommand.StartingList))
                            //{
                            //    this.UploadStartingList(webDir);
                            //}
                        }

                        if (UploadBitmap)
                        {
                           if (m_myOrionFileLoader15m != null)
                            {
                                var bbkupDir = m_myOrionFileLoader15m.CheckForNewBackupFiles();
                                if (bbkupDir != null)
                                {
                                    var stevnerToUpload = m_fileLoader.GetStartListForDate(bbkupDir, BaneType.Femtenmeter);
                                    m_fileLoader.BakupBitmapInStevner(stevnerToUpload);
                                } 
                            }

                            if (m_myOrionFileLoader100m != null)
                            {
                                var bbkupDir = m_myOrionFileLoader100m.CheckForNewBackupFiles();
                                if (bbkupDir != null)
                                {
                                    var stevnerToUpload = m_fileLoader.GetStartListForDate(bbkupDir, BaneType.Hundremeter);
                                    m_fileLoader.BakupBitmapInStevner(stevnerToUpload);
                                }
                            }

                            if (m_myOrionFileLoader200m != null)
                            {
                                var bbkupDir = m_myOrionFileLoader200m.CheckForNewBackupFiles();
                                if (bbkupDir != null)
                                {
                                    var stevnerToUpload = m_fileLoader.GetStartListForDate(bbkupDir, BaneType.Tohundremeter);
                                    m_fileLoader.BakupBitmapInStevner(stevnerToUpload);
                                }
                            }
                            
                        }
                    }

                    if (this.BkUpBitmap && this.m_myOrionFileLoaderBkup != null)
                    {
                        bool upload = this.m_myOrionFileLoaderBkup.CheckForNewBitmapFiles();
                        if (upload)
                        {
                            var bitmapFiles = this.m_myOrionFileLoaderBkup.BackUpBitMaps();
                        }
                    }

                    Thread.Sleep(2000);

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

    

        private void UploadBitMaps(LeonDirInfo webDir)
        {

            if (webDir.StevneForAlleBaner == null)
            {
                return;
            }

            if (webDir.StevneForAlleBaner.DynamiskeBaner == null )
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
                        }

                        if (string.IsNullOrEmpty(prefix))
                        {
                            Log.Info(" Prefix for bitmaps is missing stevne={0}", navn);
                            continue;
                        }

                        this.m_myFtpUtil.UploadFiles(this.m_remoteDir, webDir.TargetName, bitmap.ToArray(), prefix);
                    }
                }
            }

        //private void UploadBitMaps(System.Collections.Generic.List<StartingListStevne> bitmapStevner)
        //{
        //    foreach (var stevne in bitmapStevner)
        //    {
        //        List<string> backupBitmapMatch = new List<string>();
        //        foreach (var startingListLag in stevne.StevneLag)
        //        {
                  

        //            foreach (var skive in startingListLag.Skiver)
        //            {
        //                if (skive.BackUpBitMapFile != null)
        //                {
        //                    backupBitmapMatch.Add(skive.BackUpBitMapFile.FullName);
        //                }
        //            }

        //            if (!string.IsNullOrEmpty(startingListLag.FullFileNameGravLapper))
        //            {
        //                backupBitmapMatch.Add(startingListLag.FullFileNameGravLapper);
        //            }
                
        //        }

        //        if (backupBitmapMatch.Count > 0)
        //        {
        //            this.m_myFtpUtil.UploadFiles(this.m_remoteDir, stevne.StevneNavn, backupBitmapMatch.ToArray());
        //        }
        //    }
           
        //}



        private void UploadPresseListe(LeonDirInfo webDir)
        {
            this.m_myFtpUtil.UploadFile(this.m_remoteDir, webDir.TargetName, webDir.PresseListeFileName, this.m_remotePresseListeFileName);
        }

        private void UploadPdf(LeonDirInfo webDir)
        {
            this.m_fileLoader.UpdatePdfTimeStamp(webDir);
            this.m_myFtpUtil.UploadFile(this.m_remoteDir, webDir.TargetName, webDir.PdfFileName, this.m_remotePdfFileName);
        }

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
                this.m_myFtpUtil.UploadFiles(this.m_remoteDir, webDir.TargetName, files);
            }
        }

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
                        UploadBitMaps(webDir);
                    }
                    
                }
                }
                

                this.m_fileLoader.UpdateWebTimeStamp(webDir);
                this.m_myFtpUtil.UploadFiles(this.m_remoteDir, webDir.TargetName, files);
            }
        }

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

            string bitMapDir = ConfigurationManager.AppSettings["BitMapDir"];
            string bitMapBackupDir = ConfigurationManager.AppSettings["BitMapBackupDir"];


            m_remoteBitMapDir15m  = ConfigurationManager.AppSettings["RemoteBitMapDir15m"];
            m_remoteBitMapDir100m = ConfigurationManager.AppSettings["RemoteBitMapDir100m"];
            m_remoteBitMapDir200m = ConfigurationManager.AppSettings["RemoteBitMapDir200m"];

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
                if (Boolean.TryParse(debugXslt, out val))
                {
                    debugMerge = val;
                }
            }



            this.BkUpBitmap = false;
            this.UploadBitmap = false;
            if (!string.IsNullOrEmpty(bitMapDir) && !string.IsNullOrEmpty(bitMapBackupDir))
            {
                if (!Directory.Exists(bitMapDir))
                {
                    Log.Error("Bitmap dir not exsist {0} ", bitMapDir);
                    this.BkUpBitmap = false;
                }
                else
                {
                    if (!Directory.Exists(bitMapBackupDir))
                    {
                        Log.Error("bitMapBackupDir dir not exsist {0} ", bitMapBackupDir);
                        this.BkUpBitmap = false;
                    }
                    else
                    {
                        this.BkUpBitmap = true;
                        Log.Error("Bitmaps will be backedup from {0} to {1} ", bitMapDir, bitMapBackupDir);
                    }
                }
            }

            if (!string.IsNullOrEmpty(uploadBitmapstr))
            {
                bool result = false;
                if (bool.TryParse(uploadBitmapstr, out result))
                {
                    UploadBitmap = result;
                } 
            }
            

            if (this.BkUpBitmap)
            {
                this.m_myOrionFileLoaderBkup = new OrionFileLoader(bitMapDir, bitMapBackupDir);
               
            }
            
            if (!string.IsNullOrEmpty(m_remoteBitMapDir15m))
            {
                if (Directory.Exists(m_remoteBitMapDir15m))
                {
                    Log.Info("bitmaps fetch for 15m on dir ={0}", m_remoteBitMapDir15m);
                    m_myOrionFileLoader15m = new OrionFileLoader(m_remoteBitMapDir15m);
                }
                else
                {
                    Log.Info("No bitmaps fetch for 15m");
                }
            }

            if (!string.IsNullOrEmpty(m_remoteBitMapDir100m))
            {
                if (Directory.Exists(m_remoteBitMapDir100m))
                {
                    Log.Info("bitmaps fetch for 100m on dir ={0}", m_remoteBitMapDir100m);
                    m_myOrionFileLoader100m = new OrionFileLoader(m_remoteBitMapDir100m);
                }
                else
                {
                    Log.Info("No bitmaps fetch for 100m");
                }
            }

            if (!string.IsNullOrEmpty(m_remoteBitMapDir200m))
            {
                if (Directory.Exists(m_remoteBitMapDir200m))
                {
                    Log.Info("bitmaps fetch for 200m on dir ={0}", m_remoteBitMapDir200m);
                    m_myOrionFileLoader200m = new OrionFileLoader(m_remoteBitMapDir200m);
                }
                else
                {
                    Log.Info("No bitmaps fetch for 200m");
                } 
            }

            if (UploadBitmap || !string.IsNullOrEmpty(m_installDir))
            {
                this.m_myFtpUtil = new FtpUtility(ftpServer, hostIp, hostPort, ftpUserName, ftpPassWord);
            }
            
            if (!string.IsNullOrEmpty(m_installDir))
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

                if (!string.IsNullOrEmpty(TopListLagSkyttereXsltFilFileName) )
                {
                    this.m_fileLoader.InitToppListLagSkytingTransform(TopListLagSkyttereXsltFilFileName);
                }

                
            }
        }

        public int ExitCode { get; set; }

        public void Stop()
        {
            this.m_stopMe = true;
        }

       
    }
}
