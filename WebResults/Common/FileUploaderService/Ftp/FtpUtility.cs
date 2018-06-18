using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.Ftp
{
    using System.IO;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;

    using EnterpriseDT.Net.Ftp;

    using FileUploaderService.Diagnosis;
    using FileUploaderService.Orion;

    public class FtpUtility
    {
        private string m_hostName = null;
        private string m_userName = null;
        private string m_passWord = null;
        private bool m_enableFtp = true;
        public class UploadedFiles
        {
            public string Filname { get; set; }

            public bool AddedToServer { get; set; }

            public int Order { get; set; }
        }


        public class listedFile
        {

            public listedFile()
            {

            }
            public listedFile(FTPFile fileinfo)
            {
                OriginalFilname = fileinfo.Name;
                Filname = fileinfo.Name.ToUpper().Trim();
                Size = fileinfo.Size;
                LastUpdated = fileinfo.LastModified;
            }

            public string OriginalFilname { get; set; }
            public string Filname { get; set; }

            public long Size { get; set; }

            public DateTime LastUpdated { get; set; }
        }

        private Dictionary<string, List<string>> HandleDirs;


        private Encoding inputEncoding;
        public FtpUtility(bool uploadWeb, string hostName, string hostIp, string hostPort, string userName, string password)
        {
            m_enableFtp = uploadWeb;
            m_hostName = hostName;
            m_userName = userName;
            m_passWord = password;
            inputEncoding = Encoding.GetEncoding("ISO-8859-1");
            HandleDirs = new Dictionary<string, List<string>>();
        }

        public delegate bool HandleFtpLog(string message);
        public event HandleFtpLog OnLogEvent;

        

        public void OnHandleFtpLog(string message)
        {
            Log.Info(message);
            if (OnLogEvent == null)
            {
                return;
            }

            OnLogEvent(message);
        }

        public delegate bool HandleFileFinished(string message, int count, int total);
        public event HandleFileFinished OnHandleFileFinishedEvent;
        public void OnHandleFileFinished(string message, int count, int total)
        {
           
            if (OnHandleFileFinishedEvent == null)
            {
                return;
            }

            OnHandleFileFinishedEvent(message, count, total);
        }

        public List<FtpDirectory> ListDirectories()
        {
            List<FtpDirectory> retList = new List<FtpDirectory>();
            FTPClient ftp = null;
            try
            {

                ftp = new FTPClient();
                ftp.RemoteHost = m_hostName;
                ftp.Connect();
                // login
                OnHandleFtpLog("Logging in");
                ftp.Login(m_userName, m_passWord);

                // set up passive ASCII transfers
                OnHandleFtpLog("Setting up passive, BINARY transfers");
                ftp.ConnectMode = FTPConnectMode.PASV;
                ftp.TransferType = FTPTransferType.BINARY;
                ftp.ControlEncoding = Encoding.GetEncoding("ISO8859-1");
                //ftp.DataEncoding = new UTF8Encoding(false);
                ftp.Timeout = 5000;
                ftp.TransferBufferSize = 4096;
                ftp.TransferNotifyInterval = ((long)(4096));

                var details = ftp.DirDetails();
                foreach (var item in details)
                {
                    if (item.Dir)
                    {
                        if (item.Name == "." ||
                           item.Name == "..")
                        { }
                        else
                        {
                            var dir = new FtpDirectory() { Name = item.Name };
                            retList.Add(dir);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                if (ftp != null)
                {
                    ftp.Quit();
                }
            }
            return retList;
        }

        public List<FtpDirectory> SubDirectories(string path)
        {
            List<FtpDirectory> retList = new List<FtpDirectory>();
            if(string.IsNullOrEmpty(path))
            {
                return retList;
            }
            FTPClient ftp = null;
            try
            {
                List<string> SubDirs = path.Split(new char[] { '/', '\\' }).ToList();

                ftp = new FTPClient();
                ftp.RemoteHost = m_hostName;
                ftp.Connect();
                // login
                OnHandleFtpLog("Logging in");
                ftp.Login(m_userName, m_passWord);

                // set up passive ASCII transfers
                OnHandleFtpLog("Setting up passive, BINARY transfers");
                ftp.ConnectMode = FTPConnectMode.PASV;
                ftp.TransferType = FTPTransferType.BINARY;
                ftp.ControlEncoding = Encoding.GetEncoding("ISO8859-1");
                //ftp.DataEncoding = new UTF8Encoding(false);
                ftp.Timeout = 5000;
                ftp.TransferBufferSize = 4096;
                ftp.TransferNotifyInterval = ((long)(4096));
                bool foudDir = false;
                foreach(var dir in SubDirs)
                {
                    var details = ftp.DirDetails();
                    bool found = false;
                    foreach (var item in details)
                    {
                        if (item.Dir)
                        {
                            if (item.Name == dir)
                            {
                                ftp.ChDir(item.Name);
                                found = true;
                                break;
                            }
                            
                        }
                    }

                    if(!found)
                    {
                        break;
                    }
                    else
                    {
                        foudDir = true;
                    }
                }
                if(foudDir)
                {
                    var details = ftp.DirDetails();
                    foreach (var item in details)
                    {
                        if (item.Dir)
                        {
                            if (item.Name == "." ||
                               item.Name == "..")
                            { }
                            else
                            {
                                var dir = new FtpDirectory() { Name = item.Name };
                                retList.Add(dir);
                            }
                        }
                    }
                }
                
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                if (ftp != null)
                {
                    ftp.Quit();
                }
            }
            return retList;
        }

        public bool UploadFiles(bool fullUpload, string remoteDir, string remoteSubDir, string[] files, string subsubdir = null)
        {
            if (!m_enableFtp)
            {
                OnHandleFtpLog("UploadLoadWeb parameter is set to false ftp not enabled");
                Log.Warning("UploadLoadWeb parameter is set to false ftp not enabled");
                return false;
            }
            if (files == null)
            {
                return false;
            }

            if (files.Length <= 0)
            {
                return false;
            }

            List<UploadedFiles> filesToAdd = new List<UploadedFiles>();
            int OrderCount = 0;
            foreach (var file in files)
            {
                filesToAdd.Add(new UploadedFiles() { Filname = file, Order = OrderCount++ });
            }

            FTPClient ftp = null;

            try
            {
                ftp = this.Connect(false, remoteDir, remoteSubDir, subsubdir);


                var ftpDetails = ftp.DirDetails();
                List<listedFile> foundFiles = new List<listedFile>();
                foreach (var element in ftpDetails)
                {
                    foundFiles.Add(new listedFile(element));
                }


                int errorCount = 0;

                int countFiles = 0;
                do
                {
                    try
                    {
                        if (File.Exists(filesToAdd[countFiles].Filname))
                        {
                            var remotefileName = Path.GetFileName(filesToAdd[countFiles].Filname);

                            if (!fullUpload)
                            {
                                FileInfo info = new FileInfo(filesToAdd[countFiles].Filname);
                                long filelen = info.Length;
                                var remoteFille =
                                foundFiles.FirstOrDefault(x => x.Filname == Path.GetFileName(filesToAdd[countFiles].Filname).Trim().ToUpper());
                                if (remoteFille != null)
                                {
                                    var size = ftp.Size(remoteFille.OriginalFilname);
                                    if (size == filelen)
                                    {
                                        Log.Trace("file of same size not sent {0} {1}", filesToAdd[countFiles].Filname, filelen);
                                        countFiles++;
                                        continue;
                                    }
                                }
                            }

                            OnHandleFtpLog(string.Format("Putting file {0}", remotefileName));
                            ftp.Put(filesToAdd[countFiles].Filname, remotefileName, false);
                            OnHandleFileFinished(filesToAdd[countFiles].Filname, countFiles, filesToAdd.Count);
                        }
                        countFiles++;
                    }
                    catch (Exception e)
                    {
                        string InnerMessage = string.Empty;
                        if (e.InnerException != null)
                        {
                            InnerMessage = e.InnerException.Message;
                        }

                        Log.Error(e, "Error sending file {0} {1}", filesToAdd[countFiles].Filname, InnerMessage);

                        errorCount++;
                        Thread.Sleep(1000);
                        ftp = this.Connect(true, remoteDir, remoteSubDir, subsubdir);
                    }

                }
                while (errorCount < 10 && countFiles < filesToAdd.Count);


                OnHandleFtpLog("Quitting client");
                ftp.Quit();
                return true;
            }
            catch (Exception e)
            {
                if (ftp != null)
                {
                    ftp.Quit();
                }

                Log.Error(e, "Error");
                return false;
            }

        }

        FTPClient Connect(bool remoteDirExsist, string remoteDir, string remoteSubDir, string subsubdir = null)
        {

            remoteSubDir = remoteSubDir.Replace('/', ' ');
            var ftp = new FTPClient();
            ftp.RemoteHost = m_hostName;
            ftp.Connect();
            // login
            OnHandleFtpLog("Logging in");
            ftp.Login(m_userName, m_passWord);

            // set up passive ASCII transfers
            OnHandleFtpLog("Setting up passive, BINARY transfers");
            ftp.ConnectMode = FTPConnectMode.PASV;
            ftp.TransferType = FTPTransferType.BINARY;
            ftp.ControlEncoding = Encoding.GetEncoding("ISO8859-1");
            //ftp.DataEncoding = new UTF8Encoding(false);
            ftp.Timeout = 5000;
            ftp.TransferBufferSize = 4096;
            ftp.TransferNotifyInterval = ((long)(4096));

            OnHandleFtpLog(string.Format("Changing remote Dir to {0}", remoteDir));
            List<string> remoteParts = null;
            var remoteDirParts = remoteDir.Split(new char[] { '/', '\\' });
            string key = remoteDir + remoteSubDir + subsubdir;
            if (HandleDirs.ContainsKey(key))
            {
                remoteParts = HandleDirs[key];
            }

            if (remoteParts != null && remoteParts.Count > 0)
            {
                foreach (var remdir in remoteParts)
                {
                    ftp.ChDir(remdir);
                }
            }
            else
            {
                remoteParts = new List<string>();
                foreach (var remotePaty in remoteDirParts)
                {
                    var details = ftp.DirDetails();
                    bool found = false;
                    foreach (var detailsRemoteDir in details)
                    {
                        if (detailsRemoteDir.Dir)
                        {
                            Log.Info("RemoteDir ={0}", detailsRemoteDir.Name);
                            if (detailsRemoteDir.Name == remotePaty)
                            {
                                remoteParts.Add(remotePaty);
                                found = true;
                                break;
                            }
                        }
                    }

                    if (!found)
                    {
                        OnHandleFtpLog(string.Format("Creating RemoteDir ={0}", remotePaty));
                        ftp.MkDir(remotePaty);
                        OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", remotePaty));
                        ftp.ChDir(remotePaty);
                        remoteParts.Add(remotePaty);
                    }
                    else
                    {
                        OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", remotePaty));
                        ftp.ChDir(remotePaty);
                    }

                }


                var detailsStevner = ftp.DirDetails();
                bool foundStevne = false;
                foreach (var detailsRemoteDir in detailsStevner)
                {
                    if (detailsRemoteDir.Dir)
                    {
                        OnHandleFtpLog(string.Format("RemoteDir ={0}", detailsRemoteDir.Name));
                        if (detailsRemoteDir.Name == remoteSubDir)
                        {
                            foundStevne = true;
                            remoteParts.Add(remoteSubDir);
                            break;
                        }
                    }
                }

                if (!foundStevne)
                {
                    OnHandleFtpLog(string.Format("Creating RemoteDir ={0}", remoteSubDir));
                    ftp.MkDir(remoteSubDir);
                    OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", remoteSubDir));
                    ftp.ChDir(remoteSubDir);
                    remoteParts.Add(remoteSubDir);
                }
                else
                {
                    OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", remoteSubDir));
                    ftp.ChDir(remoteSubDir);
                }




                if (!string.IsNullOrEmpty(subsubdir))
                {
                    var detailssubsub = ftp.DirDetails();
                    bool foundsubsub = false;
                    foreach (var detailsRemoteDir in detailssubsub)
                    {
                        if (detailsRemoteDir.Dir)
                        {
                            OnHandleFtpLog(string.Format("RemoteDir ={0}", detailsRemoteDir.Name));
                            if (detailsRemoteDir.Name == subsubdir)
                            {
                                remoteParts.Add(subsubdir);
                                foundsubsub = true;
                                break;
                            }
                        }
                    }

                    if (!foundsubsub)
                    {
                        OnHandleFtpLog(string.Format("Creating RemoteDir ={0}", subsubdir));
                        ftp.MkDir(subsubdir);
                        OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", subsubdir));
                        ftp.ChDir(subsubdir);
                        remoteParts.Add(subsubdir);
                    }
                    else
                    {
                        OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", subsubdir));
                        ftp.ChDir(subsubdir);
                    }
                }

                HandleDirs.Add(key, remoteParts);
            }

            return ftp;
        }

        internal bool UploadFile(string remoteDir, string remoteSubDir, string localFile, string m_remotePdfFileName)
        {
            FTPClient ftp = null;
            try
            {
                remoteSubDir = remoteSubDir.Replace('/', ' ');
                ftp = new FTPClient(m_hostName);

                // login
                OnHandleFtpLog("Logging in");
                ftp.Login(m_userName, m_passWord);

                // set up passive ASCII transfers

                OnHandleFtpLog("Setting up passive, BINARY transfers");
                ftp.ConnectMode = FTPConnectMode.PASV;
                ftp.TransferType = FTPTransferType.BINARY;
                ftp.ControlEncoding = Encoding.GetEncoding("ISO8859-1");
                OnHandleFtpLog(string.Format("Changing remote Dir to {0}", remoteDir));

                var remoteDirParts = remoteDir.Split(new char[] { '/', '\\' });


                foreach (var remotePaty in remoteDirParts)
                {
                    var details = ftp.DirDetails();
                    bool found = false;
                    foreach (var detailsRemoteDir in details)
                    {
                        if (detailsRemoteDir.Dir)
                        {
                            OnHandleFtpLog(string.Format("RemoteDir ={0}", detailsRemoteDir.Name));
                            if (detailsRemoteDir.Name == remotePaty)
                            {
                                found = true;
                                break;
                            }
                        }
                    }

                    if (!found)
                    {
                        OnHandleFtpLog(string.Format("Creating RemoteDir ={0}", remotePaty));
                        ftp.MkDir(remotePaty);
                        OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", remotePaty));
                        ftp.ChDir(remotePaty);
                    }
                    else
                    {
                        OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", remotePaty));
                        ftp.ChDir(remotePaty);
                    }

                }


                var detailsStevner = ftp.DirDetails();
                bool foundStevne = false;
                foreach (var detailsRemoteDir in detailsStevner)
                {
                    if (detailsRemoteDir.Dir)
                    {
                        OnHandleFtpLog(string.Format("RemoteDir ={0}", detailsRemoteDir.Name));
                        if (detailsRemoteDir.Name == remoteSubDir)
                        {
                            foundStevne = true;
                            break;
                        }
                    }
                }

                if (!foundStevne)
                {
                    OnHandleFtpLog(string.Format("Creating RemoteDir ={0}", remoteSubDir));
                    ftp.MkDir(remoteSubDir);
                    OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", remoteSubDir));
                    ftp.ChDir(remoteSubDir);
                }
                else
                {
                    OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", remoteSubDir));
                    ftp.ChDir(remoteSubDir);
                }

                if (File.Exists(localFile))
                {
                    OnHandleFtpLog(string.Format("Putting file {0} to {1}", localFile, m_remotePdfFileName));

                    ftp.Put(localFile, m_remotePdfFileName);
                }

                OnHandleFtpLog("Quitting client");
                ftp.Quit();
                return true;
            }
            catch (Exception e)
            {
                if (ftp != null)
                {
                    ftp.Quit();
                }

                Log.Error(e, "Error");
                return false;
            }
        }

        internal bool UploadBitMapFiles(string remoteBitMapDir, List<Lag> bitmapFiles)
        {
            FTPClient ftp = null;
            try
            {
                ftp = new FTPClient(m_hostName);

                // login
                Log.Info("Logging in");
                ftp.Login(m_userName, m_passWord);

                // set up passive ASCII transfers
                Log.Info("Setting up passive, BINARY transfers");
                ftp.ConnectMode = FTPConnectMode.PASV;
                ftp.TransferType = FTPTransferType.BINARY;
                ftp.ControlEncoding = Encoding.GetEncoding("ISO8859-1");
                Log.Info("Changing remote Dir to {0}", remoteBitMapDir);

                var remoteDirParts = remoteBitMapDir.Split(new char[] { '/', '\\' });
                foreach (var remotePaty in remoteDirParts)
                {
                    var details = ftp.DirDetails();
                    bool found = false;
                    foreach (var detailsRemoteDir in details)
                    {
                        if (detailsRemoteDir.Dir)
                        {
                            Log.Info("RemoteDir ={0}", detailsRemoteDir.Name);
                            if (detailsRemoteDir.Name == remotePaty)
                            {
                                found = true;
                                break;
                            }
                        }
                    }

                    if (!found)
                    {
                        Log.Info("Creating RemoteDir ={0}", remotePaty);
                        ftp.MkDir(remotePaty);
                    }

                }

                Log.Info("Changing RemoteDir ={0}", remoteBitMapDir);
                ftp.ChDir(remoteBitMapDir);
                var currenta = ftp.Dir();
                foreach (var remoteLag in bitmapFiles)
                {
                    string arrangeDate = remoteLag.ArrangeTime.ToString("yyyyMMddHHmm");
                    var details = ftp.DirDetails();
                    bool found = false;
                    foreach (var detailsRemoteDir in details)
                    {
                        if (detailsRemoteDir.Dir)
                        {
                            Log.Info("RemoteDir ={0}", detailsRemoteDir.Name);
                            if (detailsRemoteDir.Name == arrangeDate)
                            {
                                found = true;
                                break;
                            }
                        }
                    }

                    if (!found)
                    {
                        Log.Info("Creating RemoteDir ={0}", arrangeDate);
                        ftp.MkDir(arrangeDate);
                        Log.Info("Changing RemoteDir ={0}", arrangeDate);
                        ftp.ChDir(arrangeDate);

                    }
                    else
                    {
                        Log.Info("Changing RemoteDir ={0}", arrangeDate);
                        ftp.ChDir(arrangeDate);
                    }

                    var detailsLag = ftp.DirDetails();
                    bool foundLag = false;
                    foreach (var detailsRemoteLagDir in detailsLag)
                    {
                        if (detailsRemoteLagDir.Dir)
                        {
                            Log.Info("RemoteDir ={0}", detailsRemoteLagDir.Name);
                            if (detailsRemoteLagDir.Name == remoteLag.LagNr.ToString())
                            {
                                foundLag = true;
                                break;
                            }
                        }
                    }
                    if (!foundLag)
                    {
                        Log.Info("Creating RemoteDir ={0}", remoteLag.LagNr.ToString());
                        ftp.MkDir(remoteLag.LagNr.ToString());
                        Log.Info("Changing RemoteDir ={0}", remoteLag.LagNr.ToString());
                        ftp.ChDir(remoteLag.LagNr.ToString());
                    }
                    else
                    {
                        Log.Info("Changing RemoteDir ={0}", remoteLag.LagNr.ToString());
                        ftp.ChDir(remoteLag.LagNr.ToString());
                    }

                    foreach (var Skiver in remoteLag.Skiver)
                    {
                        if (File.Exists(Skiver.Info.FileInfo.FullName))
                        {
                            Log.Info("Putting file {0} to {1}", Skiver.Info.FileInfo.FullName, Skiver.Info.FileInfo.Name);
                            ftp.Put(Skiver.Info.FileInfo.FullName, Skiver.Info.FileInfo.Name);
                        }
                    }

                    ftp.CdUp();
                    ftp.CdUp();
                    var current = ftp.Dir();
                }

                Log.Info("Quitting client");
                ftp.Quit();
                return true;
            }
            catch (Exception e)
            {
                if (ftp != null)
                {
                    ftp.Quit();
                }

                Log.Error(e, "Error");
                return false;
            }
        }
    }
}
