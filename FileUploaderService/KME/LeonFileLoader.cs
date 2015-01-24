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
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    using SendingResults.Diagnosis;

    /// <summary>
    ///     The leon file loader.
    /// </summary>
    public class LeonFileLoader
    {
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

            // m_ser = new XmlSerializer(typeof(StartingListLag));
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
        public LeonDirInfo CheckWebDir()
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
                            element.CheckWebFiles();
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
                                element.CheckWebFiles();
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
                                    if (element.CheckWebFiles())
                                    {
                                        Log.Info("Updated Directory Detected name {0} ", element.Info.Name);
                                        element.CheckRapporter();
                                        if (this.GenerateNewReports(element))
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
                                        
                                        if (this.GenerateNewReports(element))
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
                                        lag.Skiver = this.ParseLagInfo(files);
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
                                        foreach (var bane in dirFound.StevneForAlleBaner.DynamiskeBaner)
                                        {
                                            if (bane.BaneType == baneType && !string.IsNullOrEmpty(bane.StevneNavn))
                                            {
                                                var stevneNavn = bane.StevneNavn.Replace(',', ' ');
                                                stevneNavn = stevneNavn.Replace('/', ' ');
                                                stevneNavn = stevneNavn.Replace('\\', ' ');
                                                stevneNavn = stevneNavn.Replace(" ", string.Empty);
                                                var bitmapdir = bitmapLagDir.StevneNavnInDirName.Replace(" ", string.Empty);
                                                if (string.Compare(stevneNavn, bitmapdir, StringComparison.OrdinalIgnoreCase) == 0)
                                                {
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

                                                    var files = bitmapLagDir.Directory.GetFiles(string.Format("TR-{0}*.*", bitmapLagDir.LagNr));
                                                    lag.Skiver = this.ParseLagInfo(files);
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

            return retList;
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
        internal void InitToppListInfoTransform(string filename, string skytterxsltfilenavn)
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
            webDir.CheckWebFiles();
            int idx = this.m_DetectedDirs.IndexOf(webDir);
            if (idx >= 0)
            {
                this.m_DetectedDirs[idx].LastWrittenWebFile = webDir.LastWrittenWebFile;
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
            if (inputList != null)
            {
                foreach (var stevne in inputList)
                {
                    if (string.Compare(searchnavn, stevne.StevneNavn, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return stevne;
                    }
                }
            }

            return null;
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
                    if (File.Exists(skiveNr.RawBitmapFile.FullName))
                    {
                        var destFileName = Path.Combine(destDir, skiveNr.RawBitmapFile.Name);
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

                            Log.Info("Backing up Raw bitmaps to stevner From={0} To={1}", skiveNr.RawBitmapFile.FullName, destFileName);
                            File.Copy(skiveNr.RawBitmapFile.FullName, destFileName);
                            var sourceFileName = Path.GetFileName(skiveNr.RawBitmapFile.FullName);
                            var sourceDirName = Path.GetDirectoryName(skiveNr.RawBitmapFile.FullName);
                            var newSourceFileName = "MOV" + sourceFileName;
                            var newSourceFileNameWithPath = Path.Combine(sourceDirName, newSourceFileName);
                            File.Copy(skiveNr.RawBitmapFile.FullName, newSourceFileNameWithPath);
                            File.Delete(skiveNr.RawBitmapFile.FullName);
                            skiveNr.RawBitmapFile = null;
                            skiveNr.RawBitmapFileName = string.Empty;
                            skiveNr.BackUpBitMapFile = new FileInfo(destFileName);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "Error");
                        }
                    }
                }
            }
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
        private bool GenerateNewReports(LeonDirInfo element)
        {
            if (this.m_xsltRapport == null)
            {
                return false;
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
                                var encServer = Encoding.GetEncoding("ISO-8859-1");
                                XmlTextWriter writer = new XmlTextWriter(rapport.Filnavn, encServer);
                                writer.Formatting = Formatting.Indented;
                                docSaver.Save(writer);
                                writer.Flush();
                                writer.Close();
                                writer.Dispose();
                            }

                            if (this.m_xsltToppListSkyttere != null)
                            {
                                var outputXmlSkyttereStream = new MemoryStream { Position = 0 };
                                this.m_xsltToppListSkyttere.Transform(xpathDoc, null, outputXmlSkyttereStream);
                                outputXmlSkyttereStream.Position = 0;
                                XmlDocument docSkyttereSaver = new XmlDocument();
                                StreamReader readerSkyttereOut = new StreamReader(outputXmlSkyttereStream, enc, true);
                                XmlTextReader xmlReaderSkyttereOut = new XmlTextReader(readerSkyttereOut);
                                docSkyttereSaver.Load(xmlReaderSkyttereOut);
                                SkyttereiLaget.Add(docSkyttereSaver);
                                readerSkyttereOut.Dispose();
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

                if (this.m_xsltToppList != null && SkyttereiLaget.Count > 0)
                {
                    foreach (var toppListe in bane.ToppListeRapporter)
                    {
                        if (toppListe.ToppListInfoWithRef != null)
                        {
                            try
                            {
                                List<XmlDocument> docs = new List<XmlDocument>();
                                foreach (var skytterdoc in SkyttereiLaget)
                                {
                                    docs.Add(skytterdoc);
                                }

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
                                    var encServer = Encoding.GetEncoding("ISO-8859-1");
                                    XmlTextWriter writer = new XmlTextWriter(toppListe.Filnavn, encServer);
                                    writer.Formatting = Formatting.Indented;
                                    docSaver.Save(writer);
                                    writer.Flush();
                                    writer.Close();
                                    writer.Dispose();
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
                }
            }

            return update;
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
                    retVal.Add(skive);
                }
            }

            return retVal;
        }

        #endregion
    }
}