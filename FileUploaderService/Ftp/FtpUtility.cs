using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.Ftp
{
    using System.IO;

    using EnterpriseDT.Net.Ftp;

    using FileUploaderService.Orion;

    using SendingResults.Diagnosis;

    public class FtpUtility
    {
        private string m_hostName = null;
        private string m_userName = null;
        private string m_passWord = null;

        private Encoding inputEncoding;
        public FtpUtility(string hostName,string hostIp,string hostPort, string userName, string password)
        {
            m_hostName = hostName;
            m_userName = userName; 
            m_passWord = password;
            inputEncoding = Encoding.GetEncoding("ISO-8859-1");
        }

        internal bool UploadFiles(string remoteDir, string remoteSubDir,  string[] files, string subsubdir=null)
        {

            if (files == null)
            {
                return false;
            }

            if (files.Length <= 0)
            {
                return false;
            }

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
                ftp.DataEncoding = new UTF8Encoding(false);
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
                    }
                    else
                    {
                        Log.Info("Changing RemoteDir ={0}", subsubdir);
                        ftp.ChDir(subsubdir);
                    }
                }

                foreach (var localFileWithPath in files)
                {
                    if (File.Exists(localFileWithPath))
                    {
                        var remotefileName = Path.GetFileName(localFileWithPath);
                        Log.Info("Putting file {0}", remotefileName);
                        ftp.Put(localFileWithPath, remotefileName);
                    }
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
