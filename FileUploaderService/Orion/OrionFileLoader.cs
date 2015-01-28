using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.Orion
{
    using System.IO;
    using System.Threading;

    using FileUploaderService.KME;
    using FileUploaderService.Utils;

    using SendingResults.Diagnosis;

    public class OrionFileLoader
    {
        private string m_bitMapDir;
        private string m_bitMapBackupDir;
        private string m_bitMapErrorDir;
        private List<Lag> LagInfo;

        private OrionStevneInfo m_orionStevneInfo;
        private DateTime? LastWrittenPngFile;
        public OrionFileLoader(string bitMapDir, string bitMapBackupDir)
        {
            // TODO: Complete member initialization
            this.m_bitMapDir = bitMapDir;
            this.m_bitMapBackupDir = bitMapBackupDir;
            m_bitMapErrorDir = Path.Combine(m_bitMapBackupDir, "Error");
        }

        public OrionFileLoader(string remotebitMapBackupDir)
        {
            // TODO: Complete member initialization
            this.m_bitMapDir = null;
            this.m_bitMapBackupDir = remotebitMapBackupDir;
            m_bitMapErrorDir = Path.Combine(m_bitMapBackupDir, "Error");
        }

        internal bool CheckForNewBitmapFiles()
        {
            LagInfo = new List<Lag>();
            bool foundSkiver = false;
            if (Directory.Exists(m_bitMapDir))
            {
                var webInfo = new DirectoryInfo(m_bitMapDir);
                var listInfo = webInfo.GetFiles("TR*.PNG");
                if (listInfo.Length > 0)
                {

                    Thread.Sleep(60000);
                }
                else
                {
                    Thread.Sleep(2000);
                    return false;
                }

                listInfo = webInfo.GetFiles("TR*.PNG");
                if (listInfo.Length > 0)
                {
                    var list = listInfo.OrderByDescending(x => x.LastWriteTime);
                    if (list.FirstOrDefault() != null)
                    {
                        LastWrittenPngFile = list.FirstOrDefault().LastWriteTime;
                    }

                    foreach (var file in list)
                    {
                        var inf = new OrionFileInfo(file);
                        if (inf.ParseTarget())
                        {
                            var foundLag = LagInfo.FirstOrDefault(x => x.LagNr == inf.Lag && x.ArrangeDate == inf.EventDate);
                            if (foundLag != null)
                            {
                                foundLag.InsertSkive(inf);
                                foundSkiver = true;
                            }
                            else
                            {
                                foundLag = new Lag(inf.EventDateTime, inf.Lag);
                                LagInfo.Add(foundLag);
                                foundLag.InsertSkive(inf);
                                foundSkiver = true;
                            }
                        }
                        else
                        {
                            HandleErrorFile(file);
                        }

                    }

                    try
                    {
                        
                        var navn = OrionProgramInfo.GetStevneNavn();
                        if (string.IsNullOrEmpty(navn))
                        {
                            if (this.m_orionStevneInfo != null)
                            {
                                TimeSpan span3 = TimeSpan.FromMinutes(30);
                                var timeOutTime = this.m_orionStevneInfo.LastCheckedTime.Add(span3);
                                if (timeOutTime < DateTime.Now)
                                {
                                    Log.Info("Too long since last found STevneNavn {0}", this.m_orionStevneInfo.LastCheckedTime);
                                    this.m_orionStevneInfo = null;
                                }

                            }
                        }
                        else
                        {
                            if (this.m_orionStevneInfo != null)
                            {
                                var parsedName = ParseHelper.RemoveDirLetters(navn);
                                if (string.Compare(parsedName, this.m_orionStevneInfo.Navn, StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    this.m_orionStevneInfo.LastCheckedTime = DateTime.Now;
                                }
                                else
                                {
                                    Log.Info("Stevne changed in Orion old={0} new={1}", this.m_orionStevneInfo.Navn, parsedName);
                                }

                                this.m_orionStevneInfo.Navn = parsedName;
                                this.m_orionStevneInfo.LastCheckedTime = DateTime.Now;
                            }
                            else
                            {
                                var parsedName = ParseHelper.RemoveDirLetters(navn);
                                Log.Info("New Stevne detected={0}", parsedName);
                                this.m_orionStevneInfo = new OrionStevneInfo();
                                this.m_orionStevneInfo.Navn = parsedName;
                                this.m_orionStevneInfo.LastCheckedTime = DateTime.Now;
                            }

                        }

                       
                    }
                    catch (Exception e)
                    {

                        Log.Trace(e, "GetStevneNavn");
                    }
                }
            }

            return foundSkiver;
        }


        public List<Lag> BackUpBitMaps()
        {
            List<Lag> BackupFiles = new List<Lag>();
            try
            {
             if (!Directory.Exists(m_bitMapBackupDir))
             {
                   Directory.CreateDirectory(m_bitMapBackupDir);
             }

             foreach (var Lag in LagInfo)
             {
              var copyLag = new Lag(Lag.ArrangeTime, Lag.LagNr);
              BackupFiles.Add(copyLag);
              var dateDir = string.Format("{0:D4}{1:D2}{2:D2}", Lag.ArrangeDate.Year, Lag.ArrangeDate.Month, Lag.ArrangeDate.Day);
              string fulldateDir = Path.Combine(m_bitMapBackupDir, dateDir);
              if (!Directory.Exists(fulldateDir))
              {
                  Directory.CreateDirectory(fulldateDir);
              }

              var lagDir = Path.Combine(fulldateDir, string.Format("Lag{0} {1}", Lag.LagNr, Lag.ArrangeTime.ToString("yyyyMMdd-HHmm")));
                if (m_orionStevneInfo!=null)
                {
                    lagDir = lagDir + "-" + m_orionStevneInfo.Navn;
                }
              if (!Directory.Exists(lagDir))
              {
                  Directory.CreateDirectory(lagDir);
              }
                 
               
                foreach (var skive in Lag.Skiver)
                {
                    string destFileName = Path.Combine(lagDir, skive.Info.FileInfo.Name);
                    if (File.Exists(destFileName))
                    {
                        var name = Path.GetFileNameWithoutExtension(skive.Info.FileInfo.Name);
                        destFileName = Path.Combine(lagDir, string.Format("{0}_1.PNG", name));
                    }

                    try
                    {
                        Log.Info("Backup file {0} to {1}", skive.Info.FileInfo.FullName, destFileName);
                        File.Copy(skive.Info.FileInfo.FullName, destFileName);
                        File.Delete(skive.Info.FileInfo.FullName);
                        skive.Info.FileInfo = new FileInfo(destFileName);
                        var cpySkive = new Skive(skive);
                        copyLag.InsertSkive(cpySkive);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Could Not backup file {0}", skive.Info.FileInfo.FullName);
                    }
                    
                }
             }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error in HandleErrorFile");
            }

            return BackupFiles;
        }

        private void HandleErrorFile(FileInfo file)
        {
            try
            {


                if (!Directory.Exists(this.m_bitMapErrorDir))
                {
                    Directory.CreateDirectory(this.m_bitMapErrorDir);
                }

                string dateDir = string.Format("{0:D4}{1:D2}{2:D2}", file.CreationTime.Year, file.CreationTime.Month, file.CreationTime.Day);
                string fulldir = Path.Combine(this.m_bitMapErrorDir, dateDir);
                if (!Directory.Exists(fulldir))
                {
                    Directory.CreateDirectory(fulldir);
                }

                var errorFileName = Path.Combine(fulldir, file.Name);
                if (File.Exists(errorFileName))
                {
                    errorFileName += DateTime.Now.ToString("yyyyMMddHHmmss");
                }

                File.Copy(file.FullName, errorFileName);
                File.Delete(file.FullName);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error in HandleErrorFile");
            }
        }

        internal List<DirectoryInfo> CheckForNewBackupFiles()
        {
            List<DirectoryInfo> retList= new List<DirectoryInfo>();
            try
            {
                Log.Trace("Scanning for updated bitmaps {0}", this.m_bitMapBackupDir);

                if (Directory.Exists(this.m_bitMapBackupDir))
                {
                    var dirList = Directory.GetDirectories(this.m_bitMapBackupDir);
                    List<DirectoryInfo> allInfo = new List<DirectoryInfo>();
                    foreach (var theDir in dirList)
                    {
                        DirectoryInfo inf = new DirectoryInfo(theDir);

                       


                        if (IsValidDirectory(inf))
                        {
                           


                            retList.Add(inf);
                        }
                    }
                }
                else
                {
                    Log.Trace("Directory not exsist {0}", this.m_bitMapBackupDir);
                }


                
                return retList;
   
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        private bool IsValidDirectory(DirectoryInfo inf)
        {
 	        ///Thread.Sleep(2000);
            if (IsNumeric(inf.Name))
            {
                var dirList = Directory.GetDirectories(inf.FullName);
                foreach (var lagDir in dirList)
                {
                    DirectoryInfo laginf = new DirectoryInfo(lagDir);
                    if (laginf.Name.StartsWith("Lag"))
                    {

                        // LANTODO
                        //FileInfo[] infos = laginf.GetFiles("MOVTR*.PNG");
                        //foreach(FileInfo f in infos)
                        //{
                        //    File.Move(f.FullName, f.FullName.ToString().Replace("MOV",""));
                        //}




                        var files = laginf.GetFiles("TR*.PNG");
                        if (files.Length > 0)
                        {
                            Log.Info("Found new bitmaps in Dir {0}", inf.FullName);
                            return true;
                        }
                        else
                        {
                            Log.Trace("No bitmapFiles in Dir {0}", inf.FullName);
                        }
                    }
                }
            }
            else
            {
                if (String.Compare(inf.Name, "ERROR", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    Log.Trace("Not a Valid in Dir {0}", inf.FullName);
                }
            }

           

            return false;
        }

        public bool IsNumeric(string input)
        {
            int test;
            return int.TryParse(input, out test);
        }

}

}
