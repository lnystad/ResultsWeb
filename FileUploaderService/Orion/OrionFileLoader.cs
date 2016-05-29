// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrionFileLoader.cs" company="">
//   
// </copyright>
// <summary>
//   The orion file loader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUploaderService.Orion
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using FileUploaderService.Diagnosis;
    using FileUploaderService.KME;
    using FileUploaderService.Utils;

    /// <summary>
    ///     The orion file loader.
    /// </summary>
    public class OrionFileLoader
    {
        #region Fields

        /// <summary>
        ///     The lag info.
        /// </summary>
        private List<Lag> m_lagInfo;

        /// <summary>
        ///     The m_bit map backup dir.
        /// </summary>
        private string m_bitMapBackupDir;

        /// <summary>
        ///     The m_bit map dir.
        /// </summary>
        //private string m_bitMapDir;

        ///// <summary>
        /////     The m_bit map error dir.
        ///// </summary>
        //private string m_bitMapErrorDir;

        ///// <summary>
        /////     The m_bkup time out.
        ///// </summary>
        //private int m_bkupTimeOut;

        //private BaneType m_bitMapBaneType;

        //private int m_skiverilaget;

        //private int m_startHold;
        ///// <summary>
        ///     The m_orion stevne info.
        /// </summary>
        private OrionStevneInfo m_orionStevneInfo;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OrionFileLoader"/> class.
        /// </summary>
        /// <param name="bitMapDir">
        /// The bit map dir.
        /// </param>
        /// <param name="bitMapBackupDir">
        /// The bit map backup dir.
        /// </param>
        /// <param name="timeOut">
        /// The time out.
        /// </param>
        //public OrionFileLoader(string bitMapDir, string bitMapBackupDir, string timeOut,BaneType felt, int skiverilaget, int bitMapStartHold)
        //{
        //    // TODO: Complete member initialization
        //    this.m_skiverilaget = skiverilaget;
        //    this.m_bitMapBaneType = felt;
        //    this.m_startHold = bitMapStartHold;
        //    this.m_bitMapDir = bitMapDir;
        //    this.m_bitMapBackupDir = bitMapBackupDir;
        //    this.m_bitMapErrorDir = Path.Combine(this.m_bitMapBackupDir, "Error");
        //    this.m_bkupTimeOut = 60000;
        //    if (!string.IsNullOrEmpty(timeOut))
        //    {
        //        int timeoutInt = -1;
        //        if (int.TryParse(timeOut, out timeoutInt))
        //        {
        //            this.m_bkupTimeOut = timeoutInt;
        //            if (this.m_bkupTimeOut < 60)
        //            {
        //                this.m_bkupTimeOut = 60000;
        //            }
        //            else
        //            {
        //                this.m_bkupTimeOut = this.m_bkupTimeOut * 1000;
        //            }
        //        }
        //    }

        //    Log.Info("Bakup Timeout = {0}", this.m_bkupTimeOut);
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="OrionFileLoader"/> class.
        /// </summary>
        /// <param name="remotebitMapBackupDir">
        /// The remotebit map backup dir.
        /// </param>
        public OrionFileLoader(string remotebitMapBackupDir)
        {
            // TODO: Complete member initialization
            //this.m_bitMapDir = null;
            this.m_bitMapBackupDir = remotebitMapBackupDir;
            //this.m_bitMapErrorDir = Path.Combine(this.m_bitMapBackupDir, "Error");
            //this.m_bkupTimeOut = 60000;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The back up bit maps.
        /// </summary>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        public List<Lag> BackUpBitMaps()
        {
            List<Lag> BackupFiles = new List<Lag>();
            try
            {
                if (!Directory.Exists(this.m_bitMapBackupDir))
                {
                    Directory.CreateDirectory(this.m_bitMapBackupDir);
                }

                foreach (var Lag in this.m_lagInfo)
                {
                    var copyLag = new Lag(Lag.ArrangeTime, Lag.LagNr);
                    BackupFiles.Add(copyLag);
                    var dateDir = string.Format("{0:D4}{1:D2}{2:D2}", Lag.ArrangeDate.Year, Lag.ArrangeDate.Month, Lag.ArrangeDate.Day);
                    string fulldateDir = Path.Combine(this.m_bitMapBackupDir, dateDir);
                    if (!Directory.Exists(fulldateDir))
                    {
                        Directory.CreateDirectory(fulldateDir);
                    }

                    var lagDir = Path.Combine(fulldateDir, string.Format("Lag{0} {1}", Lag.LagNr, Lag.ArrangeTime.ToString("yyyyMMdd-HHmm")));
                    if (this.m_orionStevneInfo != null)
                    {
                        lagDir = lagDir + "-" + this.m_orionStevneInfo.Navn;
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

        /// <summary>
        /// The is numeric.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsNumeric(string input)
        {
            int test;
            return int.TryParse(input, out test);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The check for new backup files.
        /// </summary>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        internal List<DirectoryInfo> CheckForNewBackupFiles()
        {
            List<DirectoryInfo> retList = new List<DirectoryInfo>();
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

                        if (this.IsValidDirectory(inf))
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

        /// <summary>
        ///     The check for new bitmap files.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        //internal bool CheckForNewBitmapFiles()
        //{
        //    this.m_lagInfo = new List<Lag>();
        //    bool foundSkiver = false;
        //    if (Directory.Exists(this.m_bitMapDir))
        //    {
        //        var webInfo = new DirectoryInfo(this.m_bitMapDir);
        //        var listInfo = webInfo.GetFiles("TR*.PNG");
        //        if (listInfo.Length > 0)
        //        {
        //            this.SetOrionStevneInfo();
        //            Thread.Sleep(this.m_bkupTimeOut);
        //        }
        //        else
        //        {
        //             Thread.Sleep(2000);
        //            return false;
        //        }

        //        listInfo = webInfo.GetFiles("TR*.PNG");
        //        if (listInfo.Length > 0)
        //        {
        //            var list = listInfo.OrderByDescending(x => x.LastWriteTime);
                    
        //            foreach (var file in list)
        //            {
        //                var inf = new OrionFileInfo(file);
        //                if (inf.ParseTarget())
        //                {
        //                    var converted = ConvertToFeltLag(inf);

        //                    var foundLag = this.m_lagInfo.FirstOrDefault(x => x.LagNr == converted.Lag && x.ArrangeDate == converted.EventDate);
        //                    if (foundLag != null)
        //                    {
        //                        foundLag.InsertSkive(converted);
        //                        foundSkiver = true;
        //                    }
        //                    else
        //                    {
        //                        foundLag = new Lag(inf.EventDateTime, converted.Lag);
        //                        this.m_lagInfo.Add(foundLag);
        //                        foundLag.InsertSkive(converted);
        //                        foundSkiver = true;
        //                    }
        //                }
        //                else
        //                {
        //                    this.HandleErrorFile(file);
        //                }
        //            }
        //        }
        //    }

        //    return foundSkiver;
        //}

        //private OrionFileInfo ConvertToFeltLag(OrionFileInfo OrionFileInfo)
        //{
        //    if (!(this.m_bitMapBaneType == BaneType.GrovFelt || this.m_bitMapBaneType == BaneType.FinFelt))
        //    {
        //        return OrionFileInfo;
        //    }

        //    int StartLeonLag = OrionFileInfo.Lag;
        //    int Leonserienr = 0;
        //    int LeonLagNr;
        //    int leonSkivenr;

        //    OrionFileInfo retVal = new OrionFileInfo();
            
        //    int serieNr = 0;
        //    if (OrionFileInfo.Skive <= this.m_skiverilaget)
        //    {
        //        retVal.Lag = StartLeonLag;
        //        retVal.Skive = OrionFileInfo.Skive;
        //        retVal.Serie = 1;
        //    }
        //    else
        //    {
        //        int brok = (OrionFileInfo.Skive - 1) / this.m_skiverilaget;
        //        Leonserienr = brok + 1;
        //        LeonLagNr = StartLeonLag - brok;
        //        leonSkivenr = this.m_skiverilaget - OrionFileInfo.Skive % this.m_skiverilaget;
        //        retVal.Lag = LeonLagNr;
        //        retVal.Skive = leonSkivenr;
        //        retVal.Serie = Leonserienr;
        //    }

        //    string newFileName = string.Format("TR-{0}-{1}-{2}.png", retVal.Lag, retVal.Skive, retVal.Serie);
        //    string path = Path.GetDirectoryName(OrionFileInfo.FileInfo.FullName);
        //    string newFile = Path.Combine(path, newFileName);
        //    OrionFileInfo.FileInfo.MoveTo(newFile);

        //    retVal.FileInfo = new FileInfo(newFile);

        //    return retVal;
        //}

        /// <summary>
        /// The handle error file.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        //private void HandleErrorFile(FileInfo file)
        //{
        //    try
        //    {
        //        if (!Directory.Exists(this.m_bitMapErrorDir))
        //        {
        //            Directory.CreateDirectory(this.m_bitMapErrorDir);
        //        }

        //        string dateDir = string.Format("{0:D4}{1:D2}{2:D2}", file.CreationTime.Year, file.CreationTime.Month, file.CreationTime.Day);
        //        string fulldir = Path.Combine(this.m_bitMapErrorDir, dateDir);
        //        if (!Directory.Exists(fulldir))
        //        {
        //            Directory.CreateDirectory(fulldir);
        //        }

        //        var errorFileName = Path.Combine(fulldir, file.Name);
        //        if (File.Exists(errorFileName))
        //        {
        //            errorFileName += DateTime.Now.ToString("yyyyMMddHHmmss");
        //        }

        //        File.Copy(file.FullName, errorFileName);
        //        File.Delete(file.FullName);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(e, "Error in HandleErrorFile");
        //    }
        //}

        /// <summary>
        /// The is valid directory.
        /// </summary>
        /// <param name="inf">
        /// The inf.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsValidDirectory(DirectoryInfo inf)
        {
            // Thread.Sleep(2000);
            if (this.IsNumeric(inf.Name))
            {
                var dirList = Directory.GetDirectories(inf.FullName);
                foreach (var lagDir in dirList)
                {
                    DirectoryInfo laginf = new DirectoryInfo(lagDir);
                    if (laginf.Name.StartsWith("Lag"))
                    {
                        // LANTODO
                        // FileInfo[] infos = laginf.GetFiles("MOVTR*.PNG");
                        // foreach(FileInfo f in infos)
                        // {
                        // File.Move(f.FullName, f.FullName.ToString().Replace("MOV",""));
                        // }
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
                if (string.Compare(inf.Name, "ERROR", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    Log.Trace("Not a Valid in Dir {0}", inf.FullName);
                }
            }

            return false;
        }

        /// <summary>
        /// The set orion stevne info.
        /// </summary>
        //private void SetOrionStevneInfo()
        //{
        //    try
        //    {
        //        var navn = OrionProgramInfo.GetStevneNavn();
        //        if (string.IsNullOrEmpty(navn))
        //        {
        //            if (this.m_orionStevneInfo != null)
        //            {
        //                TimeSpan span3 = TimeSpan.FromMinutes(30);
        //                var timeOutTime = this.m_orionStevneInfo.LastCheckedTime.Add(span3);
        //                if (timeOutTime < DateTime.Now)
        //                {
        //                    Log.Info("Too long since last found STevneNavn {0}", this.m_orionStevneInfo.LastCheckedTime);
        //                    this.m_orionStevneInfo = null;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (this.m_orionStevneInfo != null)
        //            {
        //                var parsedName = ParseHelper.RemoveDirLetters(navn);
        //                parsedName = parsedName.Replace(" ", "");
        //                if (string.Compare(parsedName, this.m_orionStevneInfo.Navn, StringComparison.OrdinalIgnoreCase) == 0)
        //                {
        //                    this.m_orionStevneInfo.LastCheckedTime = DateTime.Now;
        //                }
        //                else
        //                {
        //                    Log.Info("Stevne changed in Orion old={0} new={1}", this.m_orionStevneInfo.Navn, parsedName);
        //                }

        //                this.m_orionStevneInfo.Navn = parsedName;
        //                this.m_orionStevneInfo.LastCheckedTime = DateTime.Now;
        //            }
        //            else
        //            {
        //                var parsedName = ParseHelper.RemoveDirLetters(navn);
        //                parsedName = parsedName.Replace(" ", "");
        //                Log.Info("New Stevne detected={0}", parsedName);
        //                this.m_orionStevneInfo = new OrionStevneInfo();
        //                this.m_orionStevneInfo.Navn = parsedName;
        //                this.m_orionStevneInfo.LastCheckedTime = DateTime.Now;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Trace(e, "GetStevneNavn");
        //    }
        //}

        #endregion
    }
}