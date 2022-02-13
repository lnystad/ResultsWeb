using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileUploaderService.Ftp
{
    using System.IO;
    using System.Threading;

    using FileUploaderService.Diagnosis;
    using FileUploaderService.Orion;
    using FluentFTP;

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
            public listedFile(FtpListItem fileinfo)
            {
                OriginalFilname = fileinfo.Name;
                Filname = fileinfo.Name.ToUpper().Trim();
                Size = fileinfo.Size;
                LastUpdated = fileinfo.Modified;
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

        public delegate bool HandleFileFinished(string filetype,string message, int count, int total);
        public event HandleFileFinished OnHandleFileFinishedEvent;
        public void OnHandleFileFinished(string filetype, string message, int count, int total)
        {
           
            if (OnHandleFileFinishedEvent == null)
            {
                return;
            }

            OnHandleFileFinishedEvent(filetype,message, count, total);
        }

        public List<FtpDirectory> ListDirectories()
        {
            List<FtpDirectory> retList = new List<FtpDirectory>();
            FtpClient ftp = null;
            try
            {

                ftp = new FtpClient(m_hostName, m_userName, m_passWord);
                ftp.DataConnectionType = FtpDataConnectionType.PASV;
                ftp.Encoding = Encoding.GetEncoding("ISO8859-1");
                ftp.UploadDataType = FtpDataType.Binary;

                // login
                OnHandleFtpLog("Logging in");
                ftp.Connect();

                // set up passive ASCII transfers
                OnHandleFtpLog("Setting up passive, BINARY transfers");
                /*ftp.ConnectMode = FTPConnectMode.PASV;
                ftp.TransferType = FTPTransferType.BINARY;
                ftp.ControlEncoding = Encoding.GetEncoding("ISO8859-1");
                //ftp.DataEncoding = new UTF8Encoding(false);
                ftp.Timeout = 5000;
                ftp.TransferBufferSize = 4096;
                ftp.TransferNotifyInterval = ((long)(4096));*/

                var details = ftp.GetListing();
                foreach (var item in details)
                {
                    if (item.Type == FtpFileSystemObjectType.Directory)
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
                
            } finally
            {
                if (ftp != null)
                {
                    ftp.Dispose();
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
            FtpClient ftp = null;
            try
            {
                List<string> SubDirs = path.Split(new char[] { '/', '\\' }).ToList();

                ftp = new FtpClient(m_hostName, m_userName, m_passWord);
                ftp.DataConnectionType = FtpDataConnectionType.PASV;
                ftp.Encoding = Encoding.GetEncoding("ISO8859-1");
                ftp.UploadDataType = FtpDataType.Binary;
                // login
                OnHandleFtpLog("Logging in");
                ftp.Connect();

                // set up passive ASCII transfers
                OnHandleFtpLog("Setting up passive, BINARY transfers");

                //ftp.DataEncoding = new UTF8Encoding(false);
               /* ftp.Timeout = 5000;
                ftp.TransferBufferSize = 4096;
                ftp.TransferNotifyInterval = ((long)(4096));*/
                bool foundDir = false;
                foreach(var dir in SubDirs)
                {
                    var details = ftp.GetListing();
                    bool found = false;
                    foreach (var item in details)
                    {
                        if (item.Type == FtpFileSystemObjectType.Directory)
                        {
                            if (item.Name == dir)
                            {
                                ftp.SetWorkingDirectory(item.Name);
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
                        foundDir = true;
                    }
                }
                if(foundDir)
                {
                    var details = ftp.GetListing();
                    foreach (var item in details)
                    {
                        if (item.Type == FtpFileSystemObjectType.Directory)
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
            }
            finally
            {
                if (ftp != null)
                {
                    ftp.Dispose();
                }
            }
            return retList;
        }

        public bool UploadFiles(bool fullUpload,string filetype, string remoteDir, string remoteSubDir, string[] files, string subsubdir = null)
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

            FtpClient ftp = null;
            try
            {
                ftp = this.Connect(false, remoteDir, remoteSubDir, subsubdir);


                var ftpDetails = ftp.GetListing();
                List<listedFile> foundFiles = new List<listedFile>();
                foreach (var element in ftpDetails)
                {
                    if (element.Type == FtpFileSystemObjectType.File)
                    {
                        foundFiles.Add(new listedFile(element));
                    }
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
                                var remoteFile =
                                foundFiles.FirstOrDefault(x => x.Filname == Path.GetFileName(filesToAdd[countFiles].Filname).Trim().ToUpper());
                                if (remoteFile != null)
                                {
                                    
                                    var size = ftp.GetFileSize(remoteFile.OriginalFilname);
                                    if (size == filelen)
                                    {
                                        OnHandleFtpLog(string.Format("file of same size not sent {0} {1}", filesToAdd[countFiles].Filname, filelen));
                                        OnHandleFileFinished(filetype, filesToAdd[countFiles].Filname, countFiles, filesToAdd.Count);
                                        countFiles++;
                                        continue;
                                    }
                                }
                            }

                            OnHandleFtpLog(string.Format("Putting file {0}", remotefileName));
                            ftp.UploadFile(filesToAdd[countFiles].Filname, remotefileName);
                            OnHandleFileFinished(filetype,filesToAdd[countFiles].Filname, countFiles, filesToAdd.Count);
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

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return false;
            }
            finally
            {
                OnHandleFtpLog("Quitting client");
                if (ftp != null)
                {
                    ftp.Dispose();
                }
            }

        }

        FtpClient Connect(bool remoteDirExsist, string remoteDir, string remoteSubDir, string subsubdir = null)
        {
            remoteSubDir = remoteSubDir.Replace('/', ' ');

            FtpClient ftp = new FtpClient(m_hostName, m_userName, m_passWord);
            ftp.DataConnectionType = FtpDataConnectionType.PASV;
            ftp.Encoding = Encoding.GetEncoding("ISO8859-1");
            ftp.UploadDataType = FtpDataType.Binary;

            // login
            OnHandleFtpLog("Logging in");
            ftp.Connect();

            // set up passive ASCII transfers
            OnHandleFtpLog("Setting up passive, BINARY transfers");
            //ftp.DataEncoding = new UTF8Encoding(false);
            /*ftp.Timeout = 5000;
            ftp.TransferBufferSize = 4096;
            ftp.TransferNotifyInterval = ((long)(4096));*/

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
                    ftp.SetWorkingDirectory(remdir);
                }
            }
            else
            {
                remoteParts = new List<string>();
                foreach (var remotePaty in remoteDirParts)
                {
                    var details = ftp.GetListing();
                    bool found = false;
                    foreach (var detailsRemoteDir in details)
                    {
                        if (detailsRemoteDir.Type == FtpFileSystemObjectType.Directory)
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
                        ftp.CreateDirectory(remotePaty);
                        OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", remotePaty));
                        ftp.SetWorkingDirectory(remotePaty);
                        remoteParts.Add(remotePaty);
                    }
                    else
                    {
                        OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", remotePaty));
                        ftp.SetWorkingDirectory(remotePaty);
                    }

                }


                var detailsStevner = ftp.GetListing();
                bool foundStevne = false;
                foreach (var detailsRemoteDir in detailsStevner)
                {
                    if (detailsRemoteDir.Type == FtpFileSystemObjectType.Directory)
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
                    ftp.CreateDirectory(remoteSubDir);
                    OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", remoteSubDir));
                    ftp.SetWorkingDirectory(remoteSubDir);
                    remoteParts.Add(remoteSubDir);
                }
                else
                {
                    OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", remoteSubDir));
                    ftp.SetWorkingDirectory(remoteSubDir);
                }




                if (!string.IsNullOrEmpty(subsubdir))
                {
                    var detailssubsub = ftp.GetListing();
                    bool foundsubsub = false;
                    foreach (var detailsRemoteDir in detailssubsub)
                    {
                        if (detailsRemoteDir.Type == FtpFileSystemObjectType.Directory)
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
                        ftp.CreateDirectory(subsubdir);
                        OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", subsubdir));
                        ftp.SetWorkingDirectory(subsubdir);
                        remoteParts.Add(subsubdir);
                    }
                    else
                    {
                        OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", subsubdir));
                        ftp.SetWorkingDirectory(subsubdir);
                    }
                }

                HandleDirs.Add(key, remoteParts);
            }

            return ftp;
        }

        internal bool UploadFile(string remoteDir, string remoteSubDir, string localFile, string m_remotePdfFileName)
        {
            FtpClient ftp = null;
            try
            {
                remoteSubDir = remoteSubDir.Replace('/', ' ');

                ftp = new FtpClient(m_hostName, m_userName, m_passWord);
                ftp.DataConnectionType = FtpDataConnectionType.PASV;
                ftp.Encoding = Encoding.GetEncoding("ISO8859-1");
                ftp.UploadDataType = FtpDataType.Binary;

                // login
                OnHandleFtpLog("Logging in");
                ftp.Connect();

                OnHandleFtpLog("Setting up passive, BINARY transfers");
                OnHandleFtpLog(string.Format("Changing remote Dir to {0}", remoteDir));

                var remoteDirParts = remoteDir.Split(new char[] { '/', '\\' });


                foreach (var remotePaty in remoteDirParts)
                {
                    var details = ftp.GetListing();
                    bool found = false;
                    foreach (var detailsRemoteDir in details)
                    {
                        if (detailsRemoteDir.Type == FtpFileSystemObjectType.Directory)
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
                        ftp.CreateDirectory(remotePaty);
                        OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", remotePaty));
                        ftp.SetWorkingDirectory(remotePaty);
                    }
                    else
                    {
                        OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", remotePaty));
                        ftp.SetWorkingDirectory(remotePaty);
                    }

                }


                var detailsStevner = ftp.GetListing();
                bool foundStevne = false;
                foreach (var detailsRemoteDir in detailsStevner)
                {
                    if (detailsRemoteDir.Type == FtpFileSystemObjectType.Directory)
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
                    ftp.CreateDirectory(remoteSubDir);
                    OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", remoteSubDir));
                    ftp.SetWorkingDirectory(remoteSubDir);
                }
                else
                {
                    OnHandleFtpLog(string.Format("Changing RemoteDir ={0}", remoteSubDir));
                    ftp.SetWorkingDirectory(remoteSubDir);
                }

                if (File.Exists(localFile))
                {
                    OnHandleFtpLog(string.Format("Putting file {0} to {1}", localFile, m_remotePdfFileName));

                    ftp.UploadFile(localFile, m_remotePdfFileName);
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return false;
            }finally
            {
                OnHandleFtpLog("Quitting client");
                if (ftp != null)
                {
                    ftp.Dispose();
                }
            }
        }

        internal bool UploadBitMapFiles(string remoteBitMapDir, List<Lag> bitmapFiles)
        {
            FtpClient ftp = null;
            try
            {
                ftp = new FtpClient(m_hostName, m_userName, m_passWord);
                ftp.DataConnectionType = FtpDataConnectionType.PASV;
                ftp.Encoding = Encoding.GetEncoding("ISO8859-1");
                ftp.UploadDataType = FtpDataType.Binary;

                // login
                Log.Info("Logging in");
                ftp.Connect();

                Log.Info("Setting up passive, BINARY transfers");
                Log.Info("Changing remote Dir to {0}", remoteBitMapDir);

                var remoteDirParts = remoteBitMapDir.Split(new char[] { '/', '\\' });
                foreach (var remotePaty in remoteDirParts)
                {
                    var details = ftp.GetListing();
                    bool found = false;
                    foreach (var detailsRemoteDir in details)
                    {
                        if (detailsRemoteDir.Type == FtpFileSystemObjectType.Directory)
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
                        ftp.CreateDirectory(remotePaty);
                    }

                }

                Log.Info("Changing RemoteDir ={0}", remoteBitMapDir);
                ftp.SetWorkingDirectory(remoteBitMapDir);

                foreach (var remoteLag in bitmapFiles)
                {
                    string arrangeDate = remoteLag.ArrangeTime.ToString("yyyyMMddHHmm");
                    var details = ftp.GetListing();
                    bool found = false;
                    foreach (var detailsRemoteDir in details)
                    {
                        if (detailsRemoteDir.Type == FtpFileSystemObjectType.Directory)
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
                        ftp.CreateDirectory(arrangeDate);
                        Log.Info("Changing RemoteDir ={0}", arrangeDate);
                        ftp.SetWorkingDirectory(arrangeDate);

                    }
                    else
                    {
                        Log.Info("Changing RemoteDir ={0}", arrangeDate);
                        ftp.SetWorkingDirectory(arrangeDate);
                    }

                    var detailsLag = ftp.GetListing();
                    bool foundLag = false;
                    foreach (var detailsRemoteLagDir in detailsLag)
                    {
                        if (detailsRemoteLagDir.Type == FtpFileSystemObjectType.Directory)
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
                        ftp.CreateDirectory(remoteLag.LagNr.ToString());
                        Log.Info("Changing RemoteDir ={0}", remoteLag.LagNr.ToString());
                        ftp.SetWorkingDirectory(remoteLag.LagNr.ToString());
                    }
                    else
                    {
                        Log.Info("Changing RemoteDir ={0}", remoteLag.LagNr.ToString());
                        ftp.SetWorkingDirectory(remoteLag.LagNr.ToString());
                    }

                    foreach (var Skiver in remoteLag.Skiver)
                    {
                        if (File.Exists(Skiver.Info.FileInfo.FullName))
                        {
                            Log.Info("Putting file {0} to {1}", Skiver.Info.FileInfo.FullName, Skiver.Info.FileInfo.Name);
                            ftp.UploadFile(Skiver.Info.FileInfo.FullName, Skiver.Info.FileInfo.Name);
                        }
                    }

                    ftp.SetWorkingDirectory("..");
                    ftp.SetWorkingDirectory("..");
                }

                return true;
            }
            catch (Exception e)
            {

                Log.Error(e, "Error");
                return false;
            }finally
            {
                Log.Info("Quitting client");
                if (ftp != null)
                {
                    ftp.Dispose();
                }
            }
        }
    }
}
