using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.Ftp
{
    using System.IO;
    using System.Threading;

    using EnterpriseDT.Net.Ftp;

    using FileUploaderService.Diagnosis;
    using FileUploaderService.Orion;

    public class FtpUtility
    {
        private string m_hostName = null;
        private string m_userName = null;
        private string m_passWord = null;
        private bool  m_enableFtp = true;
        public class UploadedFiles
        {
            public string Filname { get; set; }

            public bool AddedToServer { get; set; }

            public int Order { get; set; }
        }

        private Dictionary<string, List<string>> HandleDirs;
       

        private Encoding inputEncoding;
        public FtpUtility(bool uploadWeb,string hostName,string hostIp,string hostPort, string userName, string password)
        {
            m_enableFtp = uploadWeb;
            m_hostName = hostName;
            m_userName = userName; 
            m_passWord = password;
            inputEncoding = Encoding.GetEncoding("ISO-8859-1");
            HandleDirs = new Dictionary<string, List<string>>();
        }

        internal bool UploadFiles(string remoteDir, string remoteSubDir,  string[] files, string subsubdir=null)
        {
            if (!m_enableFtp)
            {
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
                filesToAdd.Add(new UploadedFiles() { Filname = file, Order= OrderCount++ });
            }

            FTPClient ftp = null;
       
            try
            {
                ftp = this.Connect(false, remoteDir,remoteSubDir, subsubdir);
                int errorCount = 0;

                int countFiles = 0;
                do
                {
                    try
                    {
                        if (File.Exists(filesToAdd[countFiles].Filname))
                        {
                            var remotefileName = Path.GetFileName(filesToAdd[countFiles].Filname);
                            Log.Info("Putting file {0}", remotefileName);
                            FileInfo info = new FileInfo(filesToAdd[countFiles].Filname);
                            long filelen = info.Length;
                            byte[] buffer = new byte[filelen];
                            //using (FileStream stream = new FileStream(filesToAdd[countFiles].Filname, FileMode.Open))
                            //{
                            //    // Read bytes from stream and interpret them as ints
                            //    int count;
                            //    // Read from the IO stream fewer times.
                            //    while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                            //    {
                            //        // Copy the bytes into the memory space of the Int32 array in one big swoop
                            //        //Buffer.BlockCopy(buffer, 0, intArray, count);

                            //        //for (int i = 0; i < count; i += 4)
                            //        //    Console.WriteLine(intArray[i]);
                            //    }
                            //}

                            //MemoryStream st = new MemoryStream(buffer.ToArray());
                            //if (st.Length >= 4096)
                            //{
                            //    int incomingOffset = 0;
                            //    byte[] outboundBuffer = new byte[2048];
                            //    bool append = false;
                            //    while (incomingOffset < buffer.Length)
                            //    {
                            //        int length = Math.Min(outboundBuffer.Length, buffer.Length - incomingOffset);

                            //        // Changed from Array.Copy as per Marc's suggestion
                            //        Buffer.BlockCopy(buffer, incomingOffset, outboundBuffer, 0, length);

                            //        incomingOffset += length;
                            //        ftp.Put(outboundBuffer, remotefileName, append);
                            //        append = true;
                            //        // Transmit outbound buffer
                            //    }
                            //}
                            //else
                            //{
                                ftp.Put(filesToAdd[countFiles].Filname, remotefileName, false);
                            //}

                        }
                        countFiles++;
                    }
                    catch (Exception e)
                    {
                        string InnerMessage = string.Empty;
                        if (e.InnerException!=null)
                        {
                            InnerMessage = e.InnerException.Message;
                        }

                        Log.Error(e, "Error sending file {0} {1}", filesToAdd[countFiles].Filname, InnerMessage);
                        
                        errorCount++;
                        Thread.Sleep(1000);
                        ftp = this.Connect(true,remoteDir, remoteSubDir, subsubdir);
                    }

                }
                while (errorCount < 10 && countFiles < filesToAdd.Count);


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

        FTPClient Connect(bool remoteDirExsist,string remoteDir, string remoteSubDir, string subsubdir = null)
        {

            remoteSubDir = remoteSubDir.Replace('/', ' ');
            var ftp = new FTPClient();
            ftp.RemoteHost = m_hostName;
            ftp.Connect();
            // login
            Log.Info("Logging in");
            ftp.Login(m_userName, m_passWord);

            // set up passive ASCII transfers
            Log.Info("Setting up passive, BINARY transfers");
            ftp.ConnectMode = FTPConnectMode.PASV;
            ftp.TransferType = FTPTransferType.BINARY;
            ftp.ControlEncoding = Encoding.GetEncoding("ISO8859-1");
            //ftp.DataEncoding = new UTF8Encoding(false);
            ftp.Timeout = 5000;
            ftp.TransferBufferSize = 4096;
            ftp.TransferNotifyInterval = ((long)(4096));

            Log.Info("Changing remote Dir to {0}", remoteDir);
            List<string> remoteParts=null;
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
                remoteParts= new List<string>();
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
                    Log.Info("Creating RemoteDir ={0}", remotePaty);
                    ftp.MkDir(remotePaty);
                    Log.Info("Changing RemoteDir ={0}", remotePaty);
                    ftp.ChDir(remotePaty);
                    remoteParts.Add(remotePaty);
                }
                else
                {
                    Log.Info("Changing RemoteDir ={0}", remotePaty);
                    ftp.ChDir(remotePaty);
                }

            }


            var detailsStevner = ftp.DirDetails();
            bool foundStevne = false;
            foreach (var detailsRemoteDir in detailsStevner)
            {
                if (detailsRemoteDir.Dir)
                {
                    Log.Info("RemoteDir ={0}", detailsRemoteDir.Name);
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
                Log.Info("Creating RemoteDir ={0}", remoteSubDir);
                ftp.MkDir(remoteSubDir);
                Log.Info("Changing RemoteDir ={0}", remoteSubDir);
                ftp.ChDir(remoteSubDir);
                remoteParts.Add(remoteSubDir);
            }
            else
            {
                Log.Info("Changing RemoteDir ={0}", remoteSubDir);
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
                            Log.Info("RemoteDir ={0}", detailsRemoteDir.Name);
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
                        Log.Info("Creating RemoteDir ={0}", subsubdir);
                        ftp.MkDir(subsubdir);
                        Log.Info("Changing RemoteDir ={0}", subsubdir);
                        ftp.ChDir(subsubdir);
                        remoteParts.Add(subsubdir);
                    }
                    else
                    {
                        Log.Info("Changing RemoteDir ={0}", subsubdir);
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
                Log.Info("Logging in");
                ftp.Login(m_userName, m_passWord);

                // set up passive ASCII transfers
                
                Log.Info("Setting up passive, BINARY transfers");
                ftp.ConnectMode = FTPConnectMode.PASV;
                ftp.TransferType = FTPTransferType.BINARY;
                ftp.ControlEncoding = Encoding.GetEncoding("ISO8859-1");
                Log.Info("Changing remote Dir to {0}", remoteDir);

                var remoteDirParts = remoteDir.Split(new char[] { '/', '\\' });


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
                        Log.Info("Changing RemoteDir ={0}", remotePaty);
                        ftp.ChDir(remotePaty);
                    }
                    else
                    {
                        Log.Info("Changing RemoteDir ={0}", remotePaty);
                        ftp.ChDir(remotePaty);
                    }

                }


                var detailsStevner = ftp.DirDetails();
                bool foundStevne = false;
                foreach (var detailsRemoteDir in detailsStevner)
                {
                    if (detailsRemoteDir.Dir)
                    {
                        Log.Info("RemoteDir ={0}", detailsRemoteDir.Name);
                        if (detailsRemoteDir.Name == remoteSubDir)
                        {
                            foundStevne = true;
                            break;
                        }
                    }
                }

                if (!foundStevne)
                {
                    Log.Info("Creating RemoteDir ={0}", remoteSubDir);
                    ftp.MkDir(remoteSubDir);
                    Log.Info("Changing RemoteDir ={0}", remoteSubDir);
                    ftp.ChDir(remoteSubDir);
                }
                else
                {
                    Log.Info("Changing RemoteDir ={0}", remoteSubDir);
                    ftp.ChDir(remoteSubDir);
                }

                if (File.Exists(localFile))
                {
                    Log.Info("Putting file {0} to {1}", localFile, m_remotePdfFileName);

                    ftp.Put(localFile, m_remotePdfFileName);
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
                    {   Log.Info("Changing RemoteDir ={0}", arrangeDate);
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
