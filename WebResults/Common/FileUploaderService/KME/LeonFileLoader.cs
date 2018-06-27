// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LeonFileLoader.cs" company="">
//   
// </copyright>
// <summary>
//   The leon file loader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUploaderService.KME
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    using FileUploaderService.Diagnosis;
    using FileUploaderService.Orion;
    using FileUploaderService.Utils;

    /// <summary>
    ///     The leon file loader.
    /// </summary>
    public class LeonFileLoader
    {
        private const string rapportEnCoding = "UTF-8";
        #region Fields

        /// <summary>
        ///     The m_ detected dirs.
        /// </summary>
        private List<LeonDirInfo> m_DetectedDirs;

        /// <summary>
        ///     The m_install dir.
        /// </summary>
        private string m_installDir;

        /// <summary>
        ///     The m_ser.
        /// </summary>
        private XmlSerializer m_ser;

        private XmlSerializer m_serSKive;
        /// <summary>
        ///     The m_xslt rapport.
        /// </summary>
        private XslCompiledTransform m_xsltRapport;

        /// <summary>
        /// The m_xslt topp list.
        /// </summary>
        private XslCompiledTransform m_xsltToppList;

        /// <summary>
        /// The m_xslt topp list skyttere.
        /// </summary>
        private XslCompiledTransform m_xsltToppListSkyttere;


        private XslCompiledTransform m_xsltToppListLagSkyttere;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LeonFileLoader"/> class.
        /// </summary>
        /// <param name="Path">
        /// The path.
        /// </param>
        public LeonFileLoader(string Path)
        {
            this.m_installDir = Path;
            this.m_DetectedDirs = new List<LeonDirInfo>();
            this.m_serSKive = new XmlSerializer(typeof(List<StartingListSkive>));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether debug merged xml.
        /// </summary>
        public bool DebugMergedXml { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The check web dir.
        /// </summary>
        /// <returns>
        ///     The <see cref="LeonDirInfo" />.
        /// </returns>
        public LeonDirInfo CheckWebDir(bool forceWebParse=false)
        {
            try
            {
                Log.Trace("Scanning for updated results {0}", this.m_installDir);

                if (Directory.Exists(this.m_installDir))
                {
                    var dirList = Directory.GetDirectories(this.m_installDir);
                    List<DirectoryInfo> allInfo = new List<DirectoryInfo>();
                    foreach (var theDir in dirList)
                    {
                        DirectoryInfo inf = new DirectoryInfo(theDir);
                        allInfo.Add(inf);
                    }

                    var sortedDir = allInfo.OrderBy(x => x.LastWriteTime);
                    if (this.m_DetectedDirs.Count == 0)
                    {
                        foreach (var dir in sortedDir)
                        {
                            var element = new LeonDirInfo(dir);
                            element.CheckOppropsListe();
                            element.CheckWebFiles(forceWebParse);
                            element.CheckPdfFiles();
                            element.CheckPresselisteFiles();
                            element.InitTimeStamps();
                            this.m_DetectedDirs.Add(element);
                        }
                    }
                    else
                    {
                        foreach (var dir in sortedDir)
                        {
                            Log.Trace("Checking Drectory {0}", dir.Name);
                            var element = new LeonDirInfo(dir);

                            if (!this.m_DetectedDirs.Contains(element))
                            {
                                element.CheckWebFiles(forceWebParse);
                                element.CheckPdfFiles();
                                element.CheckBitMap();
                                element.CheckPresselisteFiles();
                                this.m_DetectedDirs.Add(element);
                                element.InitTimeStamps(false);
                                Log.Info("New Directory Detected name {0}", element.Info.Name);

                                return element;
                            }
                            else
                            {
                                int idx = this.m_DetectedDirs.IndexOf(element);

                                if (idx >= 0)
                                {
                                    element = this.m_DetectedDirs[idx];
                                    element.Command = UploadCommand.none;
                                    Log.Trace("Checking Drectory {0}", element.Info.Name);
                                    if (element.CheckWebFiles(forceWebParse))
                                    {
                                        Log.Info("Updated Directory Detected name {0} wait 4sec", element.Info.Name);
                                        if (!forceWebParse)
                                        {
                                            Thread.Sleep(4000);
                                        }

                                        element.CheckRapporter();
                                        if (this.GenerateNewReports(element, forceWebParse))
                                        {
                                            Log.Info("Updated Reports Detected name");
                                            element.Command = element.Command | UploadCommand.Reports;
                                        }

                                        element.Command = element.Command | UploadCommand.Web;
                                    }

                                    if (element.CheckPdfFiles())
                                    {
                                        Log.Info("Updated PdfFile Detected name {0} ", element.PdfFileName);
                                        element.Command = element.Command | UploadCommand.Pdf;
                                    }

                                    if (element.CheckBitMap())
                                    {
                                        Log.Info("Updated Bitmap Detected name");
                                        
                                        if (this.GenerateNewReports(element, forceWebParse))
                                        {
                                            Log.Info("Updated Reports Detected name");
                                            element.Command = element.Command | UploadCommand.Reports;
                                        }

                                        element.Command = element.Command | UploadCommand.BitMap;
                                    }

                                    if (element.CheckPresselisteFiles())
                                    {
                                        Log.Info("Updated Presseliste Detected name");
                                        element.Command = element.Command | UploadCommand.PresseListe;
                                    }

                                    if (element.Command != UploadCommand.none)
                                    {
                                        return element;
                                    }
                                }
                                else
                                {
                                    Log.Trace("No element Found {0}", element.Info.Name);
                                }
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        /// <summary>
        /// The init rapport transform.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public void InitRapportTransform(string filename)
        {
            if (File.Exists(filename))
            {
                this.m_xsltRapport = new XslCompiledTransform(true);
                try
                {
                    // var myxsltResv = new MyXmlResolver();
                    // myxsltResv.SetConfigParams(customerId, customConfigPath, configPath, statemachineName);
                    this.m_xsltRapport.Load(filename, null, null);
                }
                catch (XsltException e)
                {
                    Log.Error(e, "Line={0} pos={1} File={2}", e.LineNumber, e.LinePosition, filename);
                    throw;
                }
            }
            else
            {
                Log.Info("Could not find file {0}", filename);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The bakup bitmap in stevner.
        /// </summary>
        /// <param name="stevnerToUpload">
        /// The stevner to upload.
        /// </param>
        internal void BakupBitmapInStevner(List<StartingListStevne> stevnerToUpload)
        {
            foreach (var stevne in stevnerToUpload)
            {
                if (string.IsNullOrEmpty(stevne.ReportDirStevneNavn))
                {
                    Log.Error("ReportDirStevneNavn is empty for stevne={0}", stevne.StevneNavn);
                    continue;
                }

                var stevnePath = Path.Combine(this.m_installDir, stevne.ReportDirStevneNavn);
                if (!Directory.Exists(stevnePath))
                {
                    Directory.CreateDirectory(stevnePath);
                }

                var bitMapPath = Path.Combine(stevnePath, "BitMap");
                if (!Directory.Exists(bitMapPath))
                {
                    Directory.CreateDirectory(bitMapPath);
                }

                foreach (var bane in stevne.DynamiskeBaner)
                {
                    string subdir = null;
                    switch (bane.BaneType)
                    {
                        case BaneType.Femtenmeter:
                            subdir = Constants.Prefix15m;
                            break;
                        case BaneType.Hundremeter:
                            subdir = Constants.Prefix100m;
                            break;
                        case BaneType.Tohundremeter:
                            subdir = Constants.Prefix200m;
                            break;
                       case BaneType.FinFelt:
                            subdir = Constants.PrefixFinFelt;
                            break;
                        case BaneType.GrovFelt:
                            subdir = Constants.PrefixGrovFelt;
                            break;
                    }

                    if (string.IsNullOrEmpty(subdir))
                    {
                        Log.Error("BakupBitmapInStevner wrong stevnetype={0} navn={1}", bane.BaneType, stevne.StevneNavn);
                        continue;
                    }

                    foreach (var LagsNr in bane.StevneLag)
                    {
                        LagsNr.SortSkiver();
                        MoveAllFoundFiles(bitMapPath, subdir, LagsNr);
                    }
                }
            }
        }

        /// <summary>
        /// The get start list for date.
        /// </summary>
        /// <param name="bbkupDir">
        /// The bbkup dir.
        /// </param>
        /// <param name="baneType">
        /// The bane Type.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        internal List<StartingListStevne> GetStartListForDate(List<DirectoryInfo> bbkupDir, BaneType baneType)
        {
            List<StartingListStevne> retList = new List<StartingListStevne>();

            if (bbkupDir != null)
            {
                foreach (var bbBitmapDir in bbkupDir)
                {
                    List<LagTimeInfo> currentBitmapDirInfo = ParseTimeInfo(bbBitmapDir);
                    DateTime arrangeDate;
                    int result = -1;
                    int year = -1;
                    int month = -1;
                    int day = -1;
                    if (int.TryParse(bbBitmapDir.Name.Substring(0, 4), out result))
                    {
                        year = result;
                    }

                    result = -1;
                    if (int.TryParse(bbBitmapDir.Name.Substring(4, 2), out result))
                    {
                        month = result;
                    }

                    result = -1;
                    if (int.TryParse(bbBitmapDir.Name.Substring(6, 2), out result))
                    {
                        day = result;
                    }

                    if (year > 0 && month > 0 && day > 0)
                    {
                        arrangeDate = new DateTime(year, month, day);
                    }
                    else
                    {
                        Log.Warning("Error in parsing date {0}", bbBitmapDir.FullName);
                        continue;
                    }

                    List<StartListBane> candidates = new List<StartListBane>();
                    foreach (var dirFound in this.m_DetectedDirs)
                    {
                        if (dirFound.StevneForAlleBaner != null && dirFound.StevneForAlleBaner.DynamiskeBaner != null)
                        {
                            foreach (var bane in dirFound.StevneForAlleBaner.DynamiskeBaner)
                            {
                                if (bane.BaneType == baneType)
                                {
                                    StartListBane.SetStartAndEndDate(bane);
                                    if (arrangeDate >= bane.StartDate && arrangeDate <= bane.EndDate)
                                    {
                                        foreach (var lag in bane.StevneLag)
                                        {
                                            if (lag.StartTime == null)
                                            {
                                                continue;
                                            }

                                            DateTime dato = new DateTime(lag.StartTime.Value.Year, lag.StartTime.Value.Month, lag.StartTime.Value.Day);
                                            if (dato == arrangeDate)
                                            {
                                                Log.Info(
                                                    "Found matching start list {0} {1} dato={2} stevnenavn={3} type={4}", 
                                                    bane.StevneNavn, 
                                                    bane.StartDate, 
                                                    dato, 
                                                    bane.BaneType, 
                                                    bane.StevneNavn);

                                                var found = candidates.FirstOrDefault(x => x.StevneNavn == dirFound.StevneForAlleBaner.StevneNavn);
                                                if (found == null)
                                                {
                                                    candidates.Add(bane);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (candidates.Count > 0)
                    {
                        if (candidates.Count == 1)
                        {
                            foreach (var bitmapLagDir in currentBitmapDirInfo)
                            {
                                foreach (var startingLag in candidates[0].StevneLag)
                                {
                                    if (startingLag.LagNr == bitmapLagDir.LagNr)
                                    {
                                        var stevne = FindStevne(retList, candidates[0].StevneNavn);
                                        StartingListLag lag;
                                        if (stevne == null)
                                        {
                                            stevne = new StartingListStevne();
                                            var bane = new StartListBane();
                                            stevne.StevneNavn = candidates[0].StevneNavn;
                                            bane.BaneType = candidates[0].BaneType;
                                            stevne.ReportDirStevneNavn = candidates[0].ReportDirStevneNavn;
                                            Log.Info(
                                                "Create new stevne {0} type={1} dir={2}", 
                                                stevne.StevneNavn, 
                                                bane.BaneType, 
                                                stevne.ReportDirStevneNavn);
                                            lag = new StartingListLag(startingLag);
                                            bane.AddLag(lag);
                                            stevne.DynamiskeBaner.Add(bane);
                                            bane.StevneLag.Add(lag);
                                            retList.Add(stevne);
                                        }
                                        else
                                        {
                                            StartListBane bane = this.FindBane(stevne, baneType);
                                            if (bane == null)
                                            {
                                                Log.Error("Could not find bane candidate for stevne={0}", stevne.StevneNavn);
                                            }

                                            Log.Info("Foud stevne {0} type={1}", stevne.StevneNavn, bane.BaneType);
                                            lag = this.FindLag(bane, bitmapLagDir.LagNr);
                                            if (lag == null)
                                            {
                                                lag = new StartingListLag(startingLag);
                                                bane.StevneLag.Add(lag);
                                            }
                                        }

                                        var files = bitmapLagDir.Directory.GetFiles(string.Format("TR-{0}*.*", lag.LagNr));
                                        var skiverFound= this.ParseLagInfo(files);
                                        foreach (var skive in skiverFound)
                                        {
                                            var skiveF = lag.Skiver.FirstOrDefault(x => x.SkiveNr == skive.SkiveNr);
                                            if (skiveF!=null && skive.Serier.Count > 0)
                                            {
                                                skiveF.InsertSerier(skive.Serier);
                                            }
                                            else
                                            {
                                                if (skiveF == null)
                                                {
                                                    lag.Skiver.Add(skive);
                                                }
                                                else
                                                {
                                                    lag.Skiver.Add(skive);
                                                }
                                            }
                                        }
                                        
                                        break;
                                    }
                                }
                            }
                        }
                        else if (candidates.Count > 1)
                        {
                            foreach (var bitmapLagDir in currentBitmapDirInfo)
                            {
                                if (string.IsNullOrEmpty(bitmapLagDir.StevneNavnInDirName))
                                {
                                    continue;
                                }

                                foreach (var dirFound in this.m_DetectedDirs)
                                {
                                    if (dirFound.StevneForAlleBaner != null && dirFound.StevneForAlleBaner.DynamiskeBaner != null)
                                    {
                                        bool foundStevne = false;
                                        foreach (var bane in dirFound.StevneForAlleBaner.DynamiskeBaner)
                                        {
                                            if (bane.BaneType == baneType && !string.IsNullOrEmpty(bane.StevneNavn))
                                            {
                                                if (CompareStevneNavn(bane.StevneNavn, bitmapLagDir.StevneNavnInDirName))
                                                {
                                                    foundStevne = true;
                                                    var stevne = FindStevne(retList, bane.StevneNavn);
                                                    StartingListLag lag;
                                                    if (stevne == null)
                                                    {
                                                        stevne = new StartingListStevne();
                                                        stevne.StevneNavn = dirFound.StevneForAlleBaner.StevneNavn;
                                                        stevne.ReportDirStevneNavn = dirFound.StevneForAlleBaner.ReportDirStevneNavn;
                                                        var nybane = new StartListBane();
                                                        nybane.BaneType = bane.BaneType;
                                                        stevne.DynamiskeBaner.Add(nybane);
                                                        stevne.ReportDirStevneNavn = bane.ReportDirStevneNavn;
                                                        Log.Info(
                                                            "Create new stevne {0} type={1} dir={2}", 
                                                            stevne.StevneNavn, 
                                                            bane.BaneType, 
                                                            bane.ReportDirStevneNavn);

                                                        lag = new StartingListLag(bitmapLagDir.LagNr);
                                                        nybane.StevneLag.Add(lag);
                                                        retList.Add(stevne);
                                                    }
                                                    else
                                                    {
                                                        StartListBane currentbane = this.FindBane(stevne, baneType);
                                                        Log.Info("Foud stevne {0} type={1}", stevne.StevneNavn, currentbane.BaneType);
                                                        lag = this.FindLag(currentbane, bitmapLagDir.LagNr);
                                                        if (lag == null)
                                                        {
                                                            lag = new StartingListLag(bitmapLagDir.LagNr);
                                                            currentbane.StevneLag.Add(lag);
                                                        }
                                                    }

                                                    var files = bitmapLagDir.Directory.GetFiles(string.Format("TR-{0}*.PNG", bitmapLagDir.LagNr));
                                                    if (files != null && files.Length > 0)
                                                    {
                                                        lag.Skiver = this.ParseLagInfo(files);
                                                    }
                                                    break;
                                                }
                                            }
                                        }

                                        if (foundStevne)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return retList;
        }


        public void InitToppListLagSkytingTransform(string filename)
        {
            if (File.Exists(filename))
            {
                this.m_xsltToppListLagSkyttere = new XslCompiledTransform(true);
                try
                {
                    // var myxsltResv = new MyXmlResolver();
                    // myxsltResv.SetConfigParams(customerId, customConfigPath, configPath, statemachineName);
                    XsltSettings settings = new XsltSettings();
                    settings.EnableScript = true;
                    this.m_xsltToppListLagSkyttere.Load(filename, settings, null);
                }
                catch (XsltException e)
                {
                    Log.Error(e, "Line={0} pos={1} File={2}", e.LineNumber, e.LinePosition, filename);
                    throw;
                }
            }
            else
            {
                Log.Info("Could not find file {0}", filename);
            }
            
            //if (File.Exists(lagSkytingReportfilename))
            //{
            //    this.m_xsltToppListLagSkytingReport = new XslCompiledTransform(true);
            //    try
            //    {
            //        XsltSettings settings = new XsltSettings();
            //        settings.EnableScript = true;

            //        // var myxsltResv = new MyXmlResolver();
            //        // myxsltResv.SetConfigParams(customerId, customConfigPath, configPath, statemachineName);
            //        this.m_xsltToppListLagSkytingReport.Load(lagSkytingReportfilename, settings, null);
            //    }
            //    catch (XsltException e)
            //    {
            //        Log.Error(e, "Line={0} pos={1} File={2}", e.LineNumber, e.LinePosition, lagSkytingReportfilename);
            //        throw;
            //    }
            //}
            //else
            //{
            //    Log.Info("Could not find file {0}", lagSkytingReportfilename);
            //}
        }

        /// <summary>
        /// The init topp list info transform.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="skytterxsltfilenavn">
        /// The skytterxsltfilenavn.
        /// </param>
        public void InitToppListInfoTransform(string filename, string skytterxsltfilenavn)
        {
            if (File.Exists(filename))
            {
                this.m_xsltToppList = new XslCompiledTransform(true);
                try
                {
                    // var myxsltResv = new MyXmlResolver();
                    // myxsltResv.SetConfigParams(customerId, customConfigPath, configPath, statemachineName);
                    XsltSettings settings = new XsltSettings();
                    settings.EnableScript = true;
                    this.m_xsltToppList.Load(filename, settings, null);
                }
                catch (XsltException e)
                {
                    Log.Error(e, "Line={0} pos={1} File={2}", e.LineNumber, e.LinePosition, filename);
                    throw;
                }
            }
            else
            {
                Log.Info("Could not find file {0}", filename);
            }

            if (File.Exists(skytterxsltfilenavn))
            {
                this.m_xsltToppListSkyttere = new XslCompiledTransform(true);
                try
                {
                    XsltSettings settings = new XsltSettings();
                    settings.EnableScript = true;

                    // var myxsltResv = new MyXmlResolver();
                    // myxsltResv.SetConfigParams(customerId, customConfigPath, configPath, statemachineName);
                    this.m_xsltToppListSkyttere.Load(skytterxsltfilenavn, settings, null);
                }
                catch (XsltException e)
                {
                    Log.Error(e, "Line={0} pos={1} File={2}", e.LineNumber, e.LinePosition, skytterxsltfilenavn);
                    throw;
                }
            }
            else
            {
                Log.Info("Could not find file {0}", filename);
            }
        }

        /// <summary>
        /// The parse target.
        /// </summary>
        /// <param name="inf">
        /// The inf.
        /// </param>
        /// <returns>
        /// The <see cref="StartingListSkive"/>.
        /// </returns>
        internal StartingListSkive ParseTarget(FileInfo inf)
        {
            try
            {
                StartingListSkive retVal = new StartingListSkive();
                retVal.RawBitmapFile = inf;
                var splitInf = inf.Name.Split(new[] { '-', '.' });

                int tall = 0;

                tall = 0;
                if (int.TryParse(splitInf[2], out tall))
                {
                    retVal.SkiveNr = tall;
                }

                if (string.Compare(splitInf[3], "PNG", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (retVal.SkiveNr > 0)
                    {
                        return retVal;
                    }
                }

                if (splitInf.Length>4 && string.Compare(splitInf[4], "PNG", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (retVal.SkiveNr > 0)
                    {
                        

                        int tallSerie = 0;

                        tallSerie = 0;
                        if (int.TryParse(splitInf[3], out tallSerie))
                        {
                            StartingListSerie serie = new StartingListSerie();
                            serie.SerieNr = tallSerie;
                            serie.RawBitmapFile= inf;
                            retVal.Serier.Add(serie);
                            retVal.RawBitmapFile = null;
                            retVal.RawBitmapFileName = null;
                        }

                        return retVal;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error Parsing {0}", inf.Name);
            }

            return null;
        }

        /// <summary>
        /// The update pdf time stamp.
        /// </summary>
        /// <param name="webDir">
        /// The web dir.
        /// </param>
        internal void UpdatePdfTimeStamp(LeonDirInfo webDir)
        {
            webDir.CheckPdfFiles();
            int idx = this.m_DetectedDirs.IndexOf(webDir);
            if (idx >= 0)
            {
                this.m_DetectedDirs[idx].LastWrittenPdfFile = webDir.LastWrittenPdfFile;
            }
        }

        /// <summary>
        /// The update web time stamp.
        /// </summary>
        /// <param name="webDir">
        /// The web dir.
        /// </param>
        internal void UpdateWebTimeStamp(LeonDirInfo webDir)
        {
            DateTime now= DateTime.Now;
            webDir.LastWrittenWebFile = now;
            if (this.m_DetectedDirs != null)
            {
                int idx = this.m_DetectedDirs.IndexOf(webDir);
                if (idx >= 0)
                {
                    this.m_DetectedDirs[idx].LastWrittenWebFile = now;
                }
            }
            
        }

        /// <summary>
        /// The find stevne.
        /// </summary>
        /// <param name="inputList">
        /// The input list.
        /// </param>
        /// <param name="searchnavn">
        /// The searchnavn.
        /// </param>
        /// <returns>
        /// The <see cref="StartingListStevne"/>.
        /// </returns>
        private static StartingListStevne FindStevne(List<StartingListStevne> inputList, string searchnavn)
        {
            if (string.IsNullOrEmpty(searchnavn))
            {
                Log.Error("FindStevne searchnavn is empty");
                return null;
            }

            if (inputList != null)
            {
                foreach (var stevne in inputList)
                {
                    
                    if (string.IsNullOrEmpty(stevne.StevneNavn))
                    {
                        Log.Error("FindStevne stevne.StevneNavn is empty");
                        continue;
                    }

                    if (CompareStevneNavn(searchnavn, stevne.StevneNavn))
                    {
                        return stevne;
                    }
                   
                }
            }

            return null;
        }


        public static bool CompareStevneNavn(string stevne1, string stevne2)
        {

            if (string.IsNullOrEmpty(stevne1))
            {
                return false;
            }

            if (string.IsNullOrEmpty(stevne2))
            {
                return false;
            }

            string newsearchnavn = ParseHelper.RemoveAllSpecialLetters(stevne1);
            string newStevneNavn = ParseHelper.RemoveAllSpecialLetters(stevne2);


            newsearchnavn = newsearchnavn.Replace(" ", string.Empty);
            newStevneNavn = newStevneNavn.Replace(" ", string.Empty);

            if (string.Compare(newsearchnavn, newStevneNavn, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// The merge documents input.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The <see cref="MemoryStream"/>.
        /// </returns>
        private static MemoryStream MergeDocumentsInput(XmlDocument a, XmlDocument b)
        {
            MemoryStream combine;
            Encoding enc;
            StreamReader reader;

            foreach (XmlNode node in a)
            {
                if (node.NodeType == XmlNodeType.XmlDeclaration)
                {
                    a.RemoveChild(node);
                }
            }

            foreach (XmlNode node in b)
            {
                if (node.NodeType == XmlNodeType.XmlDeclaration)
                {
                    b.RemoveChild(node);
                }
            }

            XDocument ax;
            using (var nodeReader = new XmlNodeReader(a))
            {
                nodeReader.MoveToContent();
                ax = XDocument.Load(nodeReader);
            }

            XDocument bx;
            using (var nodeReader = new XmlNodeReader(b))
            {
                nodeReader.MoveToContent();
                bx = XDocument.Load(nodeReader);
            }

            XElement root = new XElement("Merged");
            root.Add(ax.Root);
            root.Add(bx.Root);

            XDocument docMerged = new XDocument(root);

            combine = new MemoryStream();

            docMerged.Save(combine);
            combine.Position = 0;

            // enc = new UTF8Encoding(false);
            // reader = new StreamReader(combine, enc, true);
            // XmlTextReader xmlReader = new XmlTextReader(reader);

            // var XmlDOc = new XmlDocument();
            // XmlDOc.Load(xmlReader);
            // combine.Position = 0;
            return combine;
        }

        /// <summary>
        /// The merge documents input.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <returns>
        /// The <see cref="MemoryStream"/>.
        /// </returns>
        private static MemoryStream MergeDocumentsInput(List<XmlDocument> a)
        {
            MemoryStream combine;
            Encoding enc;
            StreamReader reader;
            if (a == null)
            {
                return null;
            }

            XElement root = new XElement("Merged");

            foreach (var doc in a)
            {
                foreach (XmlNode node in doc)
                {
                    if (doc.NodeType == XmlNodeType.XmlDeclaration)
                    {
                        doc.RemoveChild(node);
                    }
                }

                XDocument docx;
                using (var nodeReader = new XmlNodeReader(doc))
                {
                    nodeReader.MoveToContent();
                    docx = XDocument.Load(nodeReader);
                }

                root.Add(docx.Root);
            }

            XDocument docMerged = new XDocument(root);

            combine = new MemoryStream();
            docMerged.Save(combine);
            combine.Position = 0;

            // var XmlDOc = new XmlDocument();
            // XmlDOc.Load(xmlReader);
            // combine.Position = 0;
            return combine;
        }

        /// <summary>
        /// The move all found files.
        /// </summary>
        /// <param name="bitMapPath">
        /// The bit map path.
        /// </param>
        /// <param name="subdir">
        /// The subdir.
        /// </param>
        /// <param name="LagsNr">
        /// The lags nr.
        /// </param>
        private static void MoveAllFoundFiles(string bitMapPath, string subdir, StartingListLag LagsNr)
        {
            var destDir = Path.Combine(bitMapPath, subdir);
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            foreach (var skiveNr in LagsNr.Skiver)
            {
                if (skiveNr.RawBitmapFile != null)
                {
                    skiveNr.BackUpBitMapFile = MoveBitmapFile(skiveNr.RawBitmapFile, destDir);
                    skiveNr.RawBitmapFile = null;
                    skiveNr.RawBitmapFileName = string.Empty;
                }
                else if (skiveNr.Serier != null && skiveNr.Serier.Count > 0)
                {
                    foreach (var serie in skiveNr.Serier)
                    {
                       if (serie.RawBitmapFile != null)
                       {
                            serie.BackUpBitMapFile = MoveBitmapFile(serie.RawBitmapFile, destDir);
                            serie.RawBitmapFile = null;
                            serie.RawBitmapFileName = string.Empty;
                       } 
                    }
                }
            }
        }

        private static FileInfo MoveBitmapFile(FileInfo rawBitmapFile, string destDir)
        {
            if (File.Exists(rawBitmapFile.FullName))
            {
                var destFileName = Path.Combine(destDir, rawBitmapFile.Name);
                try
                {
                    FileInfo inf = new FileInfo(destFileName);

                    if (inf.Exists)
                    {
                        var filenameonly = Path.GetFileNameWithoutExtension(inf.Name);
                        var path = Path.GetDirectoryName(inf.FullName);
                        var destDirDublettDir = Path.Combine(path, "Copies");
                        if (!Directory.Exists(destDirDublettDir))
                        {
                            Directory.CreateDirectory(destDirDublettDir);
                        }

                        Log.Warning("Bitmap destination file already exstst {0}", inf.FullName);
                        DateTime time = DateTime.Now;

                        string backup = time.ToString("yyyyMMddHHmmss");
                        string oldFileName = string.Format("{0}old{1}.PNG", filenameonly, backup);
                        string totfileName = Path.Combine(destDirDublettDir, oldFileName);
                        inf.MoveTo(totfileName);
                    }

                    Log.Info("Backing up Raw bitmaps to stevner From={0} To={1}", rawBitmapFile.FullName, destFileName);
                    File.Copy(rawBitmapFile.FullName, destFileName);
                    var sourceFileName = Path.GetFileName(rawBitmapFile.FullName);
                    var sourceDirName = Path.GetDirectoryName(rawBitmapFile.FullName);
                    var newSourceFileName = "MOV" + sourceFileName;
                    var newSourceFileNameWithPath = Path.Combine(sourceDirName, newSourceFileName);
                    if (File.Exists(newSourceFileNameWithPath))
                    {
                        inf = new FileInfo(newSourceFileNameWithPath);
                        var filenameonly = Path.GetFileNameWithoutExtension(newSourceFileNameWithPath);
                        var path = Path.GetDirectoryName(newSourceFileNameWithPath);
                        var destDirDublettDir = Path.Combine(path, "Copies");
                        if (!Directory.Exists(destDirDublettDir))
                        {
                            Directory.CreateDirectory(destDirDublettDir);
                        }

                        Log.Warning("Bitmap Backup destination file already exstst {0}", newSourceFileNameWithPath);
                        DateTime time = DateTime.Now;

                        string backup = time.ToString("yyyyMMddHHmmss");
                        string oldFileName = string.Format("{0}old{1}.PNG", filenameonly, backup);
                        string totfileName = Path.Combine(destDirDublettDir, oldFileName);

                        inf.MoveTo(totfileName);
                    }

                    File.Copy(rawBitmapFile.FullName, newSourceFileNameWithPath);
                    File.Delete(rawBitmapFile.FullName);
                    return new FileInfo(destFileName);
                    
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error");
                }
            }

            return null;
        }

        /// <summary>
        /// The parse time info.
        /// </summary>
        /// <param name="bbBitmapDir">
        /// The bb bitmap dir.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private static List<LagTimeInfo> ParseTimeInfo(DirectoryInfo bbBitmapDir)
        {
            List<LagTimeInfo> listRet = new List<LagTimeInfo>();
            var dirs = bbBitmapDir.GetDirectories("Lag*");
            foreach (var direc in dirs)
            {
                LagTimeInfo inf = LagTimeInfo.ParseTimeInfo(direc.Name);
                if (inf != null)
                {
                    inf.Directory = direc;
                    listRet.Add(inf);
                }
            }

            return listRet;
        }

        /// <summary>
        /// The copy reader.
        /// </summary>
        /// <param name="readerStream">
        /// The reader stream.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        private void CopyReader(MemoryStream readerStream, string filename)
        {
            if (this.DebugMergedXml)
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = new UTF8Encoding(false);

                MemoryStream cpy = new MemoryStream(readerStream.ToArray());
                cpy.Position = 0;
                XmlDocument doc = new XmlDocument();
                doc.Load(cpy);
                var dirname = Path.GetDirectoryName(filename);
                var dirnameDebug = Path.Combine(dirname, "Debug");
                if (!Directory.Exists(dirnameDebug))
                {
                    Directory.CreateDirectory(dirnameDebug);
                }

                var newfilename = Path.GetFileName(filename);
                var newfileanem = Path.Combine(dirnameDebug, "Merge" + newfilename);
                doc.Save(newfileanem);
            }
        }

        /// <summary>
        /// The find bane.
        /// </summary>
        /// <param name="stevne">
        /// The stevne.
        /// </param>
        /// <param name="baneType">
        /// The bane type.
        /// </param>
        /// <returns>
        /// The <see cref="StartListBane"/>.
        /// </returns>
        private StartListBane FindBane(StartingListStevne stevne, BaneType baneType)
        {
            if (stevne == null)
            {
                return null;
            }

            if (stevne.DynamiskeBaner == null)
            {
                return null;
            }

            foreach (var bane in stevne.DynamiskeBaner)
            {
                if (bane.BaneType == baneType)
                {
                    return bane;
                }
            }

            return null;
        }

        /// <summary>
        /// The find lag.
        /// </summary>
        /// <param name="stevne">
        /// The stevne.
        /// </param>
        /// <param name="Lagnr">
        /// The lagnr.
        /// </param>
        /// <returns>
        /// The <see cref="StartingListLag"/>.
        /// </returns>
        private StartingListLag FindLag(StartListBane stevne, int Lagnr)
        {
            if (stevne == null)
            {
                return null;
            }

            if (stevne.StevneLag == null)
            {
                stevne.StevneLag = new List<StartingListLag>();
                return null;
            }

            foreach (var startingListLag in stevne.StevneLag)
            {
                if (startingListLag.LagNr == Lagnr)
                {
                    return startingListLag;
                }
            }

            return null;
        }

        /// <summary>
        /// The generate new reports.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool GenerateNewReports(LeonDirInfo element,bool forceWebParse,bool forceReportFix=false)
        {
            if (this.m_xsltRapport == null)
            {
                return false;
            }

            if (element == null)
            {
                Log.Warning("element in GenerateNewReports is null");
                return false;
            }
            if (element.StevneForAlleBaner == null)
            {
                Log.Warning("StevneForAlleBaner in GenerateNewReports is null");
                return false;
            }
            if(forceReportFix)
            {
                RemoveAllPreviousReport(element);
            }
            

            bool update = false;
            foreach (var bane in element.StevneForAlleBaner.DynamiskeBaner)
            {
                List<XmlDocument> SkyttereiLaget = new List<XmlDocument>();
                foreach (var rapport in bane.BaneRapporter)
                {
                    if (rapport.BitMapInfo != null && rapport.Rapport != null)
                    {
                        try
                        {
                            XPathDocument xpathDoc;

                            var outputXmlStream = new MemoryStream { Position = 0 };
                            var readerStream = MergeDocumentsInput(rapport.Rapport, rapport.BitMapInfo);
                            
                            this.CopyReader(readerStream, rapport.Filnavn);

                            var enc = new UTF8Encoding(false);
                            var reader = new StreamReader(readerStream, enc, true);

                            // XmlTextReader xmlReader = new XmlTextReader(reader);
                            xpathDoc = new XPathDocument(reader);

                            this.m_xsltRapport.Transform(xpathDoc, null, outputXmlStream);
                            update = true;
                            outputXmlStream.Position = 0;
                            XmlDocument docSaver = new XmlDocument();
                            StreamReader readerOut = new StreamReader(outputXmlStream, enc, true);
                            XmlTextReader xmlReaderOut = new XmlTextReader(readerOut);
                            docSaver.Load(xmlReaderOut);
                            if (!string.IsNullOrEmpty(rapport.Filnavn))
                            {
                                Log.Info("Generating new Start List {0}", rapport.Filnavn);
                                var encServer = Encoding.GetEncoding(rapportEnCoding);
                                var newFileName = FindNewFileName(rapport.Filnavn, forceWebParse);
                                using (XmlTextWriter writer = new XmlTextWriter(newFileName, encServer))
                                {
                                    writer.Formatting = Formatting.Indented;
                                    docSaver.Save(writer);
                                    writer.Flush();
                                    writer.Close();
                                }
                                
                            
                           
                            }

                            readerOut.Dispose();
                            reader.Dispose();
                        }
                        catch (XmlException xmlex)
                        {
                            Log.Error(xmlex, string.Empty);
                        }
                        catch (XsltException exp)
                        {
                            Log.Error(exp, string.Empty);

                            return false;
                        }
                        catch (Exception exp)
                        {
                            Log.Error(exp, string.Empty);
                        }
                    }
                    else if(forceReportFix && rapport.Rapport != null)
                    {
                        try
                        {
                            XPathDocument xpathDoc;

                            var outputXmlStream = new MemoryStream { Position = 0 };
                           // var readerStream = MergeDocumentsInput(rapport.Rapport, rapport.BitMapInfo);
                            var readerStream = new MemoryStream();

                            rapport.Rapport.Save(readerStream);
                            readerStream.Position = 0;
                            this.CopyReader(readerStream, rapport.Filnavn);

                            var enc = new UTF8Encoding(false);
                            var reader = new StreamReader(readerStream, enc, true);

                            // XmlTextReader xmlReader = new XmlTextReader(reader);
                            xpathDoc = new XPathDocument(reader);

                            this.m_xsltRapport.Transform(xpathDoc, null, outputXmlStream);
                            update = true;
                            outputXmlStream.Position = 0;
                            XmlDocument docSaver = new XmlDocument();
                            StreamReader readerOut = new StreamReader(outputXmlStream, enc, true);
                            XmlTextReader xmlReaderOut = new XmlTextReader(readerOut);
                            docSaver.Load(xmlReaderOut);
                            if (!string.IsNullOrEmpty(rapport.Filnavn))
                            {
                                Log.Info("Generating new Start List {0}", rapport.Filnavn);
                                var encServer = Encoding.GetEncoding(rapportEnCoding);
                                var newFileName = FindNewFileName(rapport.Filnavn, forceWebParse);
                                using (XmlTextWriter writer = new XmlTextWriter(newFileName, encServer))
                                {
                                    writer.Formatting = Formatting.Indented;
                                    docSaver.Save(writer);
                                    writer.Flush();
                                    writer.Close();
                                }



                            }

                            readerOut.Dispose();
                            reader.Dispose();
                        }
                        catch (XmlException xmlex)
                        {
                            Log.Error(xmlex, string.Empty);
                        }
                        catch (XsltException exp)
                        {
                            Log.Error(exp, string.Empty);

                            return false;
                        }
                        catch (Exception exp)
                        {
                            Log.Error(exp, string.Empty);
                        }
                    }
                }

                //var allSKyttere = bane.StevneLag.Where(x => x.BaneType == rapport.BaneType && x.ProgramType == rapport.ProgramType);
                //if (this.m_xsltToppListSkyttere != null)
                //{
                //    var outputXmlSkyttereStream = new MemoryStream { Position = 0 };
                //    this.m_xsltToppListSkyttere.Transform(xpathDoc, null, outputXmlSkyttereStream);
                //    outputXmlSkyttereStream.Position = 0;
                //    XmlDocument docSkyttereSaver = new XmlDocument();
                //    StreamReader readerSkyttereOut = new StreamReader(outputXmlSkyttereStream, enc, true);
                //    XmlTextReader xmlReaderSkyttereOut = new XmlTextReader(readerSkyttereOut);
                //    docSkyttereSaver.Load(xmlReaderSkyttereOut);

                //    SkyttereiLaget.Add(docSkyttereSaver);
                //    readerSkyttereOut.Dispose();
                //}

                if (this.m_xsltToppList != null )
                {
                    if (bane.ToppListeRapporter.Count > 0)
                    {
                            string dirName = string.Empty;
                            if (!string.IsNullOrEmpty(bane.ToppListeRapporter[0].Filnavn))
                            {
                                dirName = Path.GetDirectoryName(bane.ToppListeRapporter[0].Filnavn);
                            }

                       FinnSkyttere(bane.StevneLag, dirName, forceWebParse);
                       
                    }

                    foreach (var toppListe in bane.ToppListeRapporter)
                    {
                     
                        List<StartingListSkive> skyttereIRapporten= new List<StartingListSkive>();
                        var actualLag= bane.StevneLag.Where(x => x.BaneType == toppListe.BaneType && x.ProgramType == toppListe.ProgramType).ToList();

                        if (actualLag != null)
                        {
                            if (actualLag.Count > 0)
                            {
                                foreach (var lag in actualLag)
                                {
                                    foreach (var skive in lag.Skiver)
                                    {
                                        if (toppListe.Klasse.Contains(skive.Klasse))
                                        {
                                            skive.LagSkiveNr = lag.LagNr;
                                            skyttereIRapporten.Add(skive);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (toppListe.ProgramType == ProgramType.Mesterskap)
                                {
                                    var actualAllLag = bane.StevneLag.Where(x => x.BaneType == toppListe.BaneType).ToList();
                                    if (actualAllLag != null && actualAllLag.Count > 0)
                                    {
                                        List<StartingListLag> innledendeLag =
                                            actualAllLag.Where(x => x.ProgramType == ProgramType.Innledende).ToList();
                                        var finaleLagLag = actualAllLag.Where(x => x.ProgramType == ProgramType.Finale).ToList();
                                        if (finaleLagLag != null && finaleLagLag.Count > 0)
                                        {
                                            foreach (var lag in finaleLagLag)
                                            {
                                                foreach (var finaleSkive in lag.Skiver)
                                                {
                                                    if (!string.IsNullOrEmpty(finaleSkive.BackUpBitMapFileName))
                                                    {
                                                        // Finn SKytter og legg inn
                                                        var finnSKytter = FindMarksman(finaleSkive, innledendeLag);
                                                        if (finnSKytter != null)
                                                        {
                                                            if (!string.IsNullOrEmpty(finaleSkive.BackUpBitMapFileName))
                                                            {
                                                                finnSKytter.BackUpBitMapFileNameFinale = finaleSkive.BackUpBitMapFileName;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {

                                                        var finnSKytter = FindMarksman(finaleSkive, innledendeLag);
                                                        if (finnSKytter != null)
                                                        {
                                                            foreach (var serier in finaleSkive.Serier)
                                                            {
                                                                if (!string.IsNullOrEmpty(serier.BackUpBitMapFileName))
                                                                {
                                                                    // Finn SKytter og legg inn

                                                                    if (finnSKytter != null)
                                                                    {
                                                                        if (!string.IsNullOrEmpty(finnSKytter.BackUpBitMapFileName))
                                                                        {
                                                                            finnSKytter.BackUpBitMapFileNameFinale = finaleSkive.BackUpBitMapFileName;
                                                                        }

                                                                        foreach (var ski in finaleSkive.Serier)
                                                                        {
                                                                            if (!string.IsNullOrEmpty(ski.BackUpBitMapFileName))
                                                                            {
                                                                                //ski.SerieNr
                                                                                finnSKytter.Serier.Add(ski);
                                                                            }

                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        foreach (var innLag in innledendeLag.AsReadOnly())
                                        {
                                            foreach (var skive in innLag.Skiver)
                                            {
                                                if (toppListe.Klasse.Contains(skive.Klasse))
                                                {
                                                    skive.LagSkiveNr = innLag.LagNr;
                                                    skyttereIRapporten.Add(skive);
                                                }
                                            }


                                        }
                                    }

                                    else
                                    {
                                        Log.Info(
                                            "Zero lag found for toppListe Mesterskap toppListe.BaneType = {0} toppListe.ProgramType={1}",
                                            toppListe.BaneType,
                                            toppListe.ProgramType);
                                    }
                                }
                                else if (toppListe.ProgramType == ProgramType.Minne)
                                {
                                    var actualAlleLag = bane.StevneLag.Where(x => x.BaneType == toppListe.BaneType).ToList();
                                    if (actualAlleLag != null && actualAlleLag.Count > 0)
                                    {
                                        List<StartingListLag> innledendeLag =
                                            actualAlleLag.Where(x => x.ProgramType == ProgramType.Innledende).ToList();

                                        if (innledendeLag != null && innledendeLag.Count > 0)
                                        {
                                            foreach (var lag in innledendeLag)
                                            {
                                                foreach (var skive in lag.Skiver)
                                                {
                                                    if (toppListe.Klasse.Contains(skive.Klasse))
                                                    {
                                                        skive.LagSkiveNr = lag.LagNr;
                                                        if (bane.BitmapsStoredInBane != null)
                                                        {
                                                            if (bane.BitmapsStoredInBane.BitmapFileNames != null
                                                                && bane.BitmapsStoredInBane.BitmapFileNames.Count > 0)
                                                            {
                                                                string minneskive = string.Format("TR-{0}-{1}-0.", lag.LagNr, skive.SkiveNr);
                                                                var foundBitMap=bane.BitmapsStoredInBane.BitmapFileNames.Where(
                                                                    x => x.ToUpper().StartsWith(minneskive.ToUpper())).ToList();

                                                                string prefix = string.Empty;
                                                                if (toppListe.BaneType == BaneType.GrovFelt)
                                                                {
                                                                    prefix = Constants.PrefixGrovFelt;
                                                                }
                                                                else
                                                                {
                                                                    prefix = Constants.PrefixFinFelt;
                                                                }
        
                                                                if (foundBitMap != null && foundBitMap.Count > 0)
                                                                {
                                                                    skive.BackUpBitMapFileNameMinne = string.Format("{0}/{1}", prefix,foundBitMap[0]);
                                                                }
                                                            }
                                                        }

                                                        skyttereIRapporten.Add(skive);
                                                    }
                                                }
                                            }
                                        }

                                        //foreach (var innLag in innledendeLag.AsReadOnly())
                                        //{
                                        //    foreach (var skive in innLag.Skiver)
                                        //    {
                                        //        if (toppListe.Klasse.Contains(skive.Klasse))
                                        //        {
                                        //            skive.LagSkiveNr = innLag.LagNr;
                                        //            skyttereIRapporten.Add(skive);
                                        //        }
                                        //    }


                                        //}
                                    }
                                }
                                else
                                {
                                    Log.Info(
                                        "Zero lag found for toppListe toppListe.BaneType = {0} toppListe.ProgramType={1}",
                                        toppListe.BaneType,
                                        toppListe.ProgramType);
                                }

                            }
                        }

                        XmlDocument skytterdoc = null;
                        if (skyttereIRapporten.Count > 0)
                        {
                           
                            MemoryStream strem= new MemoryStream();
                            m_serSKive.Serialize(strem,skyttereIRapporten);
                            skytterdoc = new XmlDocument();
                            strem.Position = 0;
                            skytterdoc.Load(strem);
                        }

                        if (toppListe.ToppListInfoWithRef != null)
                        {
                            try
                            {
                                List<XmlDocument> docs = new List<XmlDocument>();
                                if (skytterdoc != null)
                                {
                                    docs.Add(skytterdoc);
                                }

                                //foreach (var skytterdoc in SkyttereiLaget)
                                //{
                                //    docs.Add(skytterdoc);
                                //}

                                docs.Add(toppListe.ToppListInfoWithRef);

                                XPathDocument xpathDoc;
                                var outputXmlStream = new MemoryStream { Position = 0 };

                                var readerStream = MergeDocumentsInput(docs);
                                this.CopyReader(readerStream, toppListe.Filnavn);
                                Encoding enc = new UTF8Encoding(false);
                                readerStream.Position = 0;
                                var reader = new StreamReader(readerStream, enc, true);

                                xpathDoc = new XPathDocument(reader);

                                this.m_xsltToppList.Transform(xpathDoc, null, outputXmlStream);
                                update = true;
                                outputXmlStream.Position = 0;
                                XmlDocument docSaver = new XmlDocument();

                                StreamReader readerOut = new StreamReader(outputXmlStream, enc, true);
                                XmlTextReader xmlReaderOut = new XmlTextReader(readerOut);
                                docSaver.Load(xmlReaderOut);
                                if (!string.IsNullOrEmpty(toppListe.Filnavn))
                                {
                                    Log.Info("Generating new Top List {0}", toppListe.Filnavn);
                                    var encServer = Encoding.GetEncoding(rapportEnCoding);
                                    var newFileName = FindNewFileName(toppListe.Filnavn, forceWebParse);
                                    using (XmlTextWriter writer = new XmlTextWriter(newFileName, encServer))
                                    {
                                        writer.Formatting = Formatting.Indented;
                                        docSaver.Save(writer);
                                        writer.Flush();
                                        writer.Close();
                                    }
                                }

                                readerOut.Dispose();
                                reader.Dispose();
                            }
                            catch (XmlException xmlex)
                            {
                                Log.Error(xmlex, string.Empty);
                            }
                            catch (XsltException exp)
                            {
                                Log.Error(exp, string.Empty);

                                return false;
                            }
                            catch (Exception exp)
                            {
                                Log.Error(exp, string.Empty);
                            }
                        }
                    }

                    foreach (var toppListe in bane.ToppListeLagRapporter)
                    {
                        if (toppListe.BitMapInfo!=null && toppListe.ToppListInfoLagWithRef != null && m_xsltToppListLagSkyttere != null)
                        {
                            try
                            {
                                List<XmlDocument> docs = new List<XmlDocument>();
                                List<StartingListLag> lagSkyting= new List<StartingListLag>();
                                foreach (var lag in bane.StevneLag)
                                {
                                    if (lag.ProgramType == ProgramType.Lagskyting)
                                    {
                                        lagSkyting.Add(lag);
                                    }
                                }
                                var lagArray = lagSkyting.ToArray();
                                XmlSerializer ser = new XmlSerializer(typeof(StartingListLag[]));
                                MemoryStream strem = new MemoryStream();
                                ser.Serialize(strem,lagArray);
                                XmlDocument doc = new XmlDocument();
                                strem.Position = 0;
                                doc.Load(strem);
                                docs.Add(doc);
                                 docs.Add(toppListe.BitMapInfo);
                                //
                                XPathDocument xpathDoc;
                                var outputXmlStream = new MemoryStream { Position = 0 };

                                var readerStream = MergeDocumentsInput(docs);
                                this.CopyReader(readerStream, toppListe.Filnavn);
                                Encoding enc = new UTF8Encoding(false);
                                readerStream.Position = 0;
                                var reader = new StreamReader(readerStream, enc, true);

                                xpathDoc = new XPathDocument(reader);

                                this.m_xsltToppListLagSkyttere.Transform(xpathDoc, null, outputXmlStream);
                                update = true;
                                outputXmlStream.Position = 0;
                                XmlDocument docSaver = new XmlDocument();

                                StreamReader readerOut = new StreamReader(outputXmlStream, enc, true);
                                XmlTextReader xmlReaderOut = new XmlTextReader(readerOut);
                                docSaver.Load(xmlReaderOut);

                                docs.Clear();
                                docs.Add(docSaver);
                                docs.Add(toppListe.ToppListInfoLagWithRef);
                                

                                var readerStreamReport = MergeDocumentsInput(docs);
                                this.CopyReader(readerStreamReport, toppListe.Filnavn);
                                readerStreamReport.Position = 0;
                                var readerReport = new StreamReader(readerStreamReport, enc, true);

                                var xpathDocReport = new XPathDocument(readerReport);

                                var outputXmlStreamReport = new MemoryStream { Position = 0 };
                                this.m_xsltToppList.Transform(xpathDocReport, null, outputXmlStreamReport);
                                update = true;
                                outputXmlStreamReport.Position = 0;
                                XmlDocument docSaverReport = new XmlDocument();

                                StreamReader readerOutReport = new StreamReader(outputXmlStreamReport, enc, true);
                                XmlTextReader xmlReaderOutReport = new XmlTextReader(readerOutReport);
                                docSaverReport.Load(xmlReaderOutReport);

                                if (!string.IsNullOrEmpty(toppListe.Filnavn))
                                {
                                    Log.Info("Generating new Top List {0}", toppListe.Filnavn);
                                    var encServer = Encoding.GetEncoding(rapportEnCoding);
                                    var newFileName = FindNewFileName(toppListe.Filnavn, forceWebParse);
                                    using (XmlTextWriter writer = new XmlTextWriter(newFileName, encServer))
                                    {
                                        writer.Formatting = Formatting.Indented;
                                        docSaverReport.Save(writer);
                                        writer.Flush();
                                        writer.Close();
                                    }
                                }
                                readerOutReport.Dispose();
                                readerOut.Dispose();
                                reader.Dispose();
                            }
                            catch (XmlException xmlex)
                            {
                                Log.Error(xmlex, string.Empty);
                            }
                            catch (XsltException exp)
                            {
                                Log.Error(exp, string.Empty);

                                return false;
                            }
                            catch (Exception exp)
                            {
                                Log.Error(exp, string.Empty);
                            }
                        }
                    }
                    
                }


                
            }
            if (forceReportFix)
            {
                CopyAllNewReport(element);
            }

            return update;
        }

        private void RemoveAllPreviousReport(LeonDirInfo element)
        {
            if(element==null)
            {
                return;
            }

            var newDir = Path.Combine(element.WebName, "NewXml");

            if(Directory.Exists(newDir))
            {
                var sllfiles=Directory.GetFiles(newDir);
                foreach(var file in sllfiles)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch(Exception e)
                    {
                        Log.Error("Error", e);
                    }
                }
            }
        }

        private void CopyAllNewReport(LeonDirInfo element)
        {
            if (element == null)
            {
                return;
            }

            var newDir = Path.Combine(element.WebName, "NewXml");

            if (Directory.Exists(newDir))
            {
                var sllfiles = Directory.GetFiles(newDir);
                foreach (var file in sllfiles)
                {
                    try
                    {
                        var oldFile= Path.Combine(element.WebName, Path.GetFileName(file));
                        if (File.Exists(oldFile))
                        {
                            File.Delete(oldFile);
                        }
                        File.Copy(file, oldFile);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Error", e);
                    }
                }
            }
        }

        private string FindNewFileName(string filnavn, bool forceWebParse)
        {
            if (forceWebParse)
            {
                if (string.IsNullOrEmpty(filnavn))
                {
                    return filnavn;
                }

                var dir = Path.GetDirectoryName(filnavn);
                if (dir == null)
                {
                    return filnavn;
                }

                var newfilename = Path.GetFileName(filnavn);
                var newDir = Path.Combine(dir, "NewXml");
                var newfilenameWithDir = Path.Combine(newDir, newfilename);
                if (!Directory.Exists(newDir))
                {
                    Directory.CreateDirectory(newDir);
                }

                return newfilenameWithDir;
            }
            return filnavn;
        }

        private StartingListSkive FindMarksman(StartingListSkive finaleSkive, List<StartingListLag> innledendeLag)
        {
            if (string.IsNullOrEmpty(finaleSkive.SkytterNavn))
            {
                return null;
            }
            string navn = finaleSkive.SkytterNavn.Trim().ToUpper();
            string skytterlag = finaleSkive.SkytterLag.Trim().ToUpper();
            foreach (var lag in innledendeLag)
            {
                foreach (var skive in lag.Skiver)
                {
                    if (skive.SkytterNavn.ToUpper().Trim() == navn)
                    {
                        if (skive.SkytterLag.ToUpper().Trim() == skytterlag)
                        {
                            return skive;
                        }
                    }
                }
            }

            return null;
        }

        private void FinnSkyttere(IEnumerable<StartingListLag> allSKyttere,string dirName,bool forceWebParse)
        {
            foreach (var lag in allSKyttere)
            {
                if (!string.IsNullOrEmpty(lag.XmlOppropsListe))
                {
                    var fullFileNAme = Path.Combine(dirName, string.Concat(lag.XmlOppropsListe,".xml"));
                    fullFileNAme = FindNewFileName(fullFileNAme, forceWebParse);
                    if (File.Exists(fullFileNAme))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(fullFileNAme);
                        var lagnode = doc.SelectSingleNode("/report/header/@name");
                        if (lagnode == null)
                        {
                            continue;
                        }
                        int lagnummer = StartingListLag.ParseLagNr(lagnode.InnerText);
                        if (lagnummer <= 0)
                        {
                            continue;
                        }

                        var Skivenode = doc.SelectNodes("/report/data/result");
                        if (Skivenode == null)
                        {
                            continue;
                        }

                        lag.Skiver= new List<StartingListSkive>();
                        foreach (XmlNode Skive in Skivenode)
                        {
                            var skiveno=Skive.SelectSingleNode("@num");
                            var Name = Skive.SelectSingleNode("@name");
                            var club = Skive.SelectSingleNode("@club");
                            var classSK = Skive.SelectSingleNode("@class");
                            var refserie = Skive.SelectSingleNode("@ref");
                            if (skiveno == null || Name == null || club == null || classSK == null)
                            {
                                continue;
                            }

                            var skive = new StartingListSkive();
                            skive.SkiveNr = Convert.ToInt32(skiveno.InnerText);
                            skive.SkytterNavn = Name.InnerText;
                            skive.SkytterLag = club.InnerText;
                            skive.Klasse = classSK.InnerText;
                            if (refserie != null)
                            {
                                skive.BackUpBitMapFileName = refserie.InnerText;
                            }
                            
                            var serieNoder=Skive.SelectNodes("series");
                            if (serieNoder != null)
                            {
                                foreach (XmlNode serienode in serieNoder)
                                {
                                    var png = serienode.SelectSingleNode("@ref");
                                    var serieno = serienode.SelectSingleNode("@id");
                                    var totsum = serienode.SelectSingleNode("@sum");
                                    if (serieno != null && png != null)
                                    {
                                        StartingListSerie serie = new StartingListSerie();
                                        serie.SerieNr = Convert.ToInt32(serieno.InnerText);
                                        serie.BackUpBitMapFileName = png.InnerText;
                                        skive.Serier.Add(serie);
                                    }
                                    
                                }   
                            }

                            lag.Skiver.Add(skive);


                        }

                    }
                }

            }
            return ;
        }

        /// <summary>
        /// The parse lag info.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<StartingListSkive> ParseLagInfo(FileInfo[] info)
        {
            List<StartingListSkive> retVal = new List<StartingListSkive>();
            if (info == null)
            {
                return retVal;
            }

            foreach (var fileinf in info)
            {
                var skive = this.ParseTarget(fileinf);
                if (skive != null)
                {
                    var Skive= retVal.FirstOrDefault(x => x.SkiveNr == skive.SkiveNr);
                    if (Skive != null)
                    {
                        Skive.InsertSerier(skive.Serier);
                    }
                    else
                    {
                        retVal.Add(skive);
                    }
                    
                }
            }

            return retVal;
        }

        #endregion

      
    }
}