using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitmapSnifferEngine.Orion
{
    using System.IO;
    using System.Threading;

    using BitmapSnifferEngine.Common;
    using BitmapSnifferEngine.Logging;

    public class OrionFileLoader
    {
        #region Fields

        /// <summary>
        ///     The lag info.
        /// </summary>
        private List<Lag> m_lagInfo;

        public List<Lag> LagInfo { get {return this.m_lagInfo;} }

       
        /// <summary>
        ///     The m_orion stevne info.
        /// </summary>
        private OrionStevneInfo m_orionStevneInfo;

        private SetupConfiguration m_config;

        private int m_runorder;

        #endregion

        #region Constructors and Destructors

        public OrionFileLoader()
        {
            m_runorder = 1;
        }

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
        /// 
        public bool Init(SetupConfiguration config)
        {
            this.m_config = config;
            if (!this.m_config.BitMapFetchTimeOut.HasValue)
            {
                this.m_config.BitMapFetchTimeOut = 60000;
            }
            else if (this.m_config.BitMapFetchTimeOut < 30)
            {
                this.m_config.BitMapFetchTimeOut = 60000;
            }
            else
            {
                
                m_config.BitMapFetchTimeOut= this.m_config.BitMapFetchTimeOut* 1000;
            }

            if (config.BaneType == BaneType.Undefined)
            {
                config.BaneType = BaneType.Tohundremeter;
                Log.Warning("BaneType defaults to Tohundremeter");
            }

            if (string.IsNullOrEmpty(this.m_config.BitMapErrorDir))
            {
                this.m_config.BitMapErrorDir = Path.Combine(this.m_config.BitMapDir, "Error");
            }

            if (!Directory.Exists(this.m_config.BitMapErrorDir))
            {
                Directory.CreateDirectory(this.m_config.BitMapErrorDir);
            }

            if (string.IsNullOrEmpty(this.m_config.BitMapRunOriginalDir))
            {
                this.m_config.BitMapRunOriginalDir = Path.Combine(this.m_config.BitMapDir, "Original");
            }
            if (!Directory.Exists(this.m_config.BitMapRunOriginalDir))
            {
                Directory.CreateDirectory(this.m_config.BitMapRunOriginalDir);
            }

            FindRunOrder(this.m_config.BitMapRunOriginalDir);

            Log.Info("Reading From set  to {0}", this.m_config.BitMapDir);

            Log.Info("Runorder dir set  to {0}", this.m_config.BitMapRunOriginalDir);

            Log.Info("Sending to set  to {0}", this.m_config.BitMapBackupDir);

            Log.Info("BaneType set  to {0}", config.BaneType);

            Log.Info("Bakup Timeout = {0}", config.BitMapFetchTimeOut);
            return true;
        }

        private void FindRunOrder(string bitMapRunOriginalDir)
        {
            if (!Directory.Exists(this.m_config.BitMapRunOriginalDir))
            {
                return;
            }

            var dirs = Directory.GetDirectories(bitMapRunOriginalDir);

            if (dirs.Length == 0)
            {
                return;
            }
            List<Tuple<DateTime,int>> sorteddirs = new List<Tuple<DateTime, int>>();

            foreach (var dirname in dirs)
            {
                var splitString = dirname.Split(new[] { '_' });
                if (splitString.Length != 2)
                {
                    continue;
                }
                string onlydirname = Path.GetFileName(splitString[0]);

                if (string.IsNullOrEmpty(onlydirname) || onlydirname.Length != 8)
                {
                    continue;
                }


                try
                {
                    int year;
                    if (!int.TryParse(onlydirname.Substring(0, 4), out year))
                    {
                        continue;
                    }
                    int month;
                    if (!int.TryParse(onlydirname.Substring(4, 2), out month))
                    {
                        continue;
                    }
                    int day;
                    if (!int.TryParse(onlydirname.Substring(6, 2), out day))
                    {
                        continue;
                    }

                    var dateTime = new DateTime(year, month, day);

                    int runorderOfDay;
                        if (!int.TryParse(splitString[1], out runorderOfDay))
                    {
                        continue;
                    }

                    sorteddirs.Add(new Tuple<DateTime, int>(dateTime, runorderOfDay));
                }
                catch (Exception)
                {
                    
                    throw;
                }
            }

            if (!(sorteddirs.Count > 0))
            {
                return;
            }

            var date = sorteddirs.OrderByDescending(x => x.Item1).ThenByDescending(y => y.Item2);
            var element = date.ElementAt(0);
            DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            if (element.Item1 != now)
            {
                this.m_runorder = 1;
            }
            else
            {
                this.m_runorder = element.Item2;
            }
        }

        //public Init(string bitMapDir, string bitMapBackupDir, string timeOut, BaneType felt, int skiverilaget, int bitMapStartHold)
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
                if (!Directory.Exists(this.m_config.BitMapBackupDir))
                {
                    Directory.CreateDirectory(this.m_config.BitMapBackupDir);
                }

                foreach (var Lag in this.m_lagInfo)
                {
                    var copyLag = new Lag(Lag.ArrangeTime, Lag.LagNr);
                    BackupFiles.Add(copyLag);
                    var dateDir = string.Format("{0:D4}{1:D2}{2:D2}", Lag.ArrangeDate.Year, Lag.ArrangeDate.Month, Lag.ArrangeDate.Day);
                    string fulldateDir = Path.Combine(this.m_config.BitMapBackupDir, dateDir);
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
                        foreach (var fileFound in skive.SerieInfo)
                        {
                            string destFileName = Path.Combine(lagDir, fileFound.FileInfo.Name);
                            if (File.Exists(destFileName))
                            {
                                var name = Path.GetFileNameWithoutExtension(fileFound.FileInfo.Name);
                                destFileName = Path.Combine(lagDir, string.Format("{0}_1.PNG", name));
                            }

                            try
                            {
                                Log.Info("Backup file {0} to {1}", fileFound.FileInfo.FullName, destFileName);
                                File.Copy(fileFound.FileInfo.FullName, destFileName);
                                File.Delete(fileFound.FileInfo.FullName);
                                fileFound.FileInfo = new FileInfo(destFileName);
                                var cpySkive = new Skive(skive);
                                copyLag.InsertSkive(cpySkive);
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "Could Not backup file {0}", fileFound.FileInfo.FullName);
                            }
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
                Log.Trace("Scanning for updated bitmaps {0}", this.m_config.BitMapBackupDir);

                if (Directory.Exists(this.m_config.BitMapBackupDir))
                {
                    var dirList = Directory.GetDirectories(this.m_config.BitMapBackupDir);
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
                    Log.Trace("Directory not exsist {0}", this.m_config.BitMapBackupDir);
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
        public bool CheckForNewBitmapFiles(bool timeout=true)
        {
            this.m_lagInfo = new List<Lag>();
            bool foundSkiver = false;
            if (Directory.Exists(this.m_config.BitMapDir))
            {
                var webInfo = new DirectoryInfo(this.m_config.BitMapDir);
                var listInfo = webInfo.GetFiles("TR*.PNG");
                if (listInfo.Length > 0)
                {
                    //this.SetOrionStevneInfo();
                    if (timeout)
                    {
                        Thread.Sleep(this.m_config.BitMapFetchTimeOut.Value);
                    }
                }
                else
                {
                    Thread.Sleep(2000);
                    return false;
                }

                listInfo = webInfo.GetFiles("TR*.PNG");
                if (listInfo.Length > 0)
                {
                    this.m_runorder++;
                    var list = listInfo.OrderByDescending(x => x.LastWriteTime);

                    foreach (var file in list)
                    {
                        var inf = new OrionFileInfo(file);
                        CopyFile(file);
                        if (inf.ParseTarget())
                        {
                            var converted = ConvertToFeltLag(inf);

                            if (converted == null)
                            {
                                this.HandleErrorFile(file);
                                continue;
                            }

                            var foundLag = this.m_lagInfo.FirstOrDefault(x => x.LagNr == converted.LagInfo.LagNr && x.ArrangeDate == converted.EventDate);
                            if (foundLag != null)
                            {
                                foundLag.InsertSkive(converted);
                                foundSkiver = true;
                            }
                            else
                            {
                                foundLag = new Lag(inf.EventDateTime, converted.LagInfo.LagNr);
                                this.m_lagInfo.Add(foundLag);
                                foundLag.InsertSkive(converted);
                                foundSkiver = true;
                            }
                        }
                        else
                        {
                            this.HandleErrorFile(file);
                        }
                    }
                }
            }

            return foundSkiver;
        }

        private void CopyFile(FileInfo file)
        {
            if (file == null)
            {
                return;
            }

            DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string dirName = Path.Combine(this.m_config.BitMapRunOriginalDir, string.Format("{0}_{1}", now.ToString("yyyyMMdd"), this.m_runorder));
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            var copyFileName = Path.Combine(dirName, file.Name);
            Log.Info("Copy file to dir {0} {1}", dirName, file.Name);
            file.CopyTo(copyFileName);
        }

        private OrionFileInfo ConvertToFeltLag(OrionFileInfo OrionFileInfo)
        {
            if (!(this.m_config.BaneType == BaneType.GrovFelt || this.m_config.BaneType == BaneType.FinFelt))
            {
                return OrionFileInfo;
            }
            if (OrionFileInfo.LagInfo.LagNr > 99)
            {
                return OrionFileInfo;
            }

            OrionFileInfo retVal = new OrionFileInfo();
            retVal.EventDate = OrionFileInfo.EventDate;
            retVal.EventDateTime = OrionFileInfo.EventDateTime;

            HoldTypeConfiguration info = FindHoldInfo(OrionFileInfo.LagInfo);
            if (info == null)
            {
                Log.Error("Could not find matching definition for {0}", OrionFileInfo.FileInfo.FullName);
                return null;
            }

            if (info.VenteBenk > 0)
            {
                if (info.FixLagnr)
                {
                    retVal.LagInfo.LagNr = OrionFileInfo.LagInfo.LagNr - info.HoldNr - info.VenteBenk;
                }
                else
                {
                    retVal.LagInfo.LagNr = OrionFileInfo.LagInfo.LagNr - info.VenteBenk;
                }

                retVal.LagInfo.HoldType = info.HoldType;
                retVal.LagInfo.SkiveNr = OrionFileInfo.LagInfo.SkiveNr - info.StartSkive+1;
            }
            else
            {
                if (info.FixLagnr)
                {
                    retVal.LagInfo.LagNr = OrionFileInfo.LagInfo.LagNr - info.HoldNr;
                }
                else
                {
                    retVal.LagInfo.LagNr = OrionFileInfo.LagInfo.LagNr;
                }

                retVal.LagInfo.HoldType = info.HoldType;
                retVal.LagInfo.SkiveNr = OrionFileInfo.LagInfo.SkiveNr - info.StartSkive + 1;
            }

            if (retVal.LagInfo.LagNr < 0)
            {
                Log.Error(
                    "LagNr is invalid={0} using holdconfig={1} filename={2}",
                    retVal.LagInfo.LagNr,
                    info.HoldNr,
                    OrionFileInfo.FileInfo.FullName);
                return null;
            }

            retVal.LagInfo.SerieNr = info.HoldNr;
            retVal.LagInfo.HoldType = info.HoldType;
           
            string newFileName = string.Empty;
            
            newFileName = string.Format("TR-{0}-{1}-{2}.png", retVal.LagInfo.LagNr, retVal.LagInfo.SkiveNr, retVal.LagInfo.SerieNr);
            
            
            string path = Path.GetDirectoryName(OrionFileInfo.FileInfo.FullName);
            string newFile = Path.Combine(path, newFileName);
            if (File.Exists(newFile))
            {
                var test=Path.GetFileNameWithoutExtension(newFile);
                var testPAth = Path.GetDirectoryName(newFile);
                var newName = "DUP"+test + "-"+ DateTime.Now.ToString("yyyyMMddHHmmss")+".png";
                File.Move(newFile, Path.Combine(testPAth,newName));
            }

            Log.Info("Renaming file from {0} to {1}", OrionFileInfo.FileInfo.Name, newFileName);
            OrionFileInfo.FileInfo.MoveTo(newFile);

            retVal.FileInfo = new FileInfo(newFile);

            return retVal;
        }

        private HoldTypeConfiguration FindHoldInfo(LagInfo lagInfo)
        {
            foreach (var hold in this.m_config.HoldConfig)
            {
                if (lagInfo.SkiveNr >= hold.StartSkive && lagInfo.SkiveNr <= hold.SluttSkive)
                {
                    return hold;
                }
            }

            return null;
        }

        /// <summary>
        /// The handle error file.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        private void HandleErrorFile(FileInfo file)
        {
            try
            {
                Log.Error("Error file handled {0}", file.FullName);
                if (!Directory.Exists(this.m_config.BitMapErrorDir))
                {
                    Directory.CreateDirectory(this.m_config.BitMapErrorDir);
                }

                string dateDir = string.Format("{0:D4}{1:D2}{2:D2}", file.CreationTime.Year, file.CreationTime.Month, file.CreationTime.Day);
                string fulldir = Path.Combine(this.m_config.BitMapErrorDir, dateDir);
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
        private void SetOrionStevneInfo()
        {
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
                        parsedName = parsedName.Replace(" ", "");
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
                        parsedName = parsedName.Replace(" ", "");
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

        #endregion
    }
}
