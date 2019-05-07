namespace FileUploaderService.KME
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.Serialization;

    using FileUploaderService.Diagnosis;
    using FileUploaderService.ListeSort;
    using FileUploaderService.Utils;

    using HtmlAgilityPack;

    public class fileOpprop
    {
        public fileOpprop()
        {
        }

        public fileOpprop(FileInfo info, int prefix, int ovelseNo, int lagno)
        {
            Info = info;
            Prefix = prefix;
            OvelseNo = ovelseNo;
            Lagno = lagno;
        }

        public FileInfo Info { get; set; }
        public int Prefix { get; set; }

        public int OvelseNo { get; set; }

        public int Lagno { get; set; }
    }

    /// <summary>
    ///     The leon dir info.
    /// </summary>
    public class LeonDirInfo : IEquatable<LeonDirInfo>
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The equals.
        /// </summary>
        /// <param name="other">
        ///     The other.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Equals(LeonDirInfo other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.Info == null && this.Info == null)
            {
                return true;
            }

            if (other.Info != null && this.Info == null)
            {
                return false;
            }

            if (other.Info == null && this.Info != null)
            {
                return false;
            }

            if (other.Info != null && this.Info != null)
            {
                if (other.Info.Name == this.Info.Name)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        public bool CheckOppropsListe()
        {
            if (this.Info == null)
            {
                return false;
            }

            if (this.LastWrittenOppropFile == null)
            {
                this.LastWrittenOppropFile = DateTime.MinValue;
                Log.Trace("Setting LastWrittenOppropFile to Min Val");
            }

            var web = Path.Combine(this.Info.FullName, WebDirName);
            var stevneNavn = Path.GetDirectoryName(this.Info.FullName);
            if (Directory.Exists(web))
            {
                this.WebName = web;
                var webInfo = new DirectoryInfo(web);

                var listInfo = webInfo.GetFiles("*.*");

                if (listInfo.Length > 0)
                {
                    bool update = false;
                    var list = listInfo.OrderByDescending(x => x.LastWriteTime);
                    var fileelement = list.FirstOrDefault();
                    if (fileelement != null)
                    {
                        DateTime LastWriteTime = new DateTime(
                            fileelement.LastWriteTime.Year,
                            fileelement.LastWriteTime.Month,
                            fileelement.LastWriteTime.Day,
                            fileelement.LastWriteTime.Hour,
                            fileelement.LastWriteTime.Minute,
                            0);
                        if (this.LastWrittenOppropFile < LastWriteTime)
                        {
                            if (this.LastWrittenOppropFile != DateTime.MinValue)
                            {
                                Log.Info("Update detected for stevne {0}", webInfo.FullName);
                                update = true;
                                Thread.Sleep(5000);
                                Log.Info("Start parsing index file for stevne {0}", webInfo.FullName);
                                this.ParseIndexHtmlFile(webInfo);
                            }

                            this.LastWrittenOppropFile = LastWriteTime;
                        }
                    }

                    return update;
                }
            }

            return false;
        }

        public List<List<fileOpprop>> GetOppropsLister(string dir)
        {
            var returnValue = new List<List<fileOpprop>>();
            var webInfo = new DirectoryInfo(dir);
            var listInfo = webInfo.GetFiles("*_*_*.xml");
            List < fileOpprop > allopprop= new List<fileOpprop>();
            foreach (FileInfo filename in listInfo)
            {
                var nameelementpart = Path.GetFileNameWithoutExtension(filename.Name).Split(new char[] { '_' });
                if (nameelementpart.Length != 3)
                {
                    continue;
                }
                int ovelse = -1;
                int lag = -1;
                if (int.TryParse(nameelementpart[1], out ovelse))
                {
                    if (int.TryParse(nameelementpart[2], out lag))
                    {
                        fileOpprop opprop = new fileOpprop(filename, 1, ovelse, lag);
                        allopprop.Add(opprop);
                    }
                }
            }

            SortOvelse sortOvelse = new SortOvelse();
            allopprop.Sort(sortOvelse);
            var listOfOvelser = allopprop.Select(x => x.OvelseNo).Distinct();
            foreach (var ovelseNo in listOfOvelser)
            {
                List<fileOpprop> newOvelse = allopprop.Where(Y => Y.OvelseNo == ovelseNo).ToList();
                returnValue.Add(newOvelse);
            }

            return returnValue;
        }

        public List<fileOpprop> GetTopLister(string dir)
        {
            var returnValue = new List<fileOpprop>();
            var webInfo = new DirectoryInfo(dir);
            var listInfo = webInfo.GetFiles("*_*.xml");
            List<fileOpprop> allopprop = new List<fileOpprop>();
            foreach (FileInfo filename in listInfo)
            {
                var nameelementpart = Path.GetFileNameWithoutExtension(filename.Name).Split(new char[] { '_' });
                if (nameelementpart.Length != 2)
                {
                    continue;
                }
                int ovelse = -1;
                if (int.TryParse(nameelementpart[1], out ovelse))
                {
                    fileOpprop opp = new fileOpprop();
                    opp.Info = filename;
                    allopprop.Add(opp);
                }
            }

            return allopprop;
        }

        #region Constants

        private XmlSerializer m_serBitInfo = null;

        /// <summary>
        ///     The bit map dir name.
        /// </summary>
        public const string BitMapDirName = "BitMap";

        /// <summary>
        ///     The web dir name.
        /// </summary>
        public const string PressDirName = "Presseliste";

        /// <summary>
        ///     The rapport dir name.
        /// </summary>
        public const string RapportDirName = "Rapport";

        /// <summary>
        ///     The web dir name.
        /// </summary>
        public const string WebDirName = "Web";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LeonDirInfo" /> class.
        /// </summary>
        public LeonDirInfo()
        {
            this.Command = UploadCommand.none;
            this.m_serBitInfo = new XmlSerializer(typeof(BitmapDirInfo));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LeonDirInfo" /> class.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        public LeonDirInfo(DirectoryInfo info)
        {
            this.Info = info;
            this.TargetName = info.Name;
            this.Command = UploadCommand.none;
            this.m_serBitInfo = new XmlSerializer(typeof(BitmapDirInfo));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the command.
        /// </summary>
        public UploadCommand Command { get; set; }

        /// <summary>
        ///     Gets or sets the info.
        /// </summary>
        public DirectoryInfo Info { get; set; }

        /// <summary>
        ///     Gets or sets the last create 100 m bitmap file.
        /// </summary>
        public DateTime? LastCreate100mBitmapFile { get; set; }

        /// <summary>
        ///     Gets or sets the last create 15 m bitmap file.
        /// </summary>
        public DateTime? LastCreate15mBitmapFile { get; set; }

        /// <summary>
        ///     Gets or sets the last create 200 m bitmap file.
        /// </summary>
        public DateTime? LastCreate200mBitmapFile { get; set; }

        public DateTime? LastCreateFinFeltBitmapFile { get; set; }

        public DateTime? LastCreateGrovFeltBitmapFile { get; set; }

        /// <summary>
        ///     Gets or sets the last written 100 m bitmap file.
        /// </summary>
        public DateTime? LastWritten100mBitmapFile { get; set; }

        /// <summary>
        ///     Gets or sets the last written 15 m bitmap file.
        /// </summary>
        public DateTime? LastWritten15mBitmapFile { get; set; }

        public DateTime? LastWrittenFinFeltBitmapFile { get; set; }

        public DateTime? LastWrittenGrovFeltBitmapFile { get; set; }

        /// <summary>
        ///     Gets or sets the last written 200 m bitmap file.
        /// </summary>
        public DateTime? LastWritten200mBitmapFile { get; set; }

        /// <summary>
        ///     Gets or sets the last written pdf file.
        /// </summary>
        public DateTime? LastWrittenPdfFile { get; set; }

        /// <summary>
        ///     Gets or sets the last written pdf file.
        /// </summary>
        public DateTime? LastWrittenPresseFile { get; set; }

        /// <summary>
        ///     Gets or sets the last written starting file.
        /// </summary>
        public DateTime? LastWrittenStartingFile { get; set; }

        /// <summary>
        ///     Gets or sets the last written web file.
        /// </summary>
        public DateTime? LastWrittenWebFile { get; set; }

        public DateTime? LastWrittenOppropFile { get; set; }

        /// <summary>
        ///     Gets or sets the pdf file name.
        /// </summary>
        public string PdfFileName { get; set; }

        /// <summary>
        ///     Gets or sets the presse liste file name.
        /// </summary>
        public string PresseListeFileName { get; set; }

        /// <summary>
        ///     Gets or sets the start list file name.
        /// </summary>
        public string StartListFileName { get; set; }

        /// <summary>
        ///     Gets or sets the stevne for alle baner.
        /// </summary>
        public StartingListStevne StevneForAlleBaner { get; set; }

        ///// <summary>
        ///// Gets or sets the stevne info.
        ///// </summary>
        // public StartingListStevne StevneInfo { get; set; }

        /// <summary>
        ///     Gets or sets the target name.
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        ///     Gets or sets the web name.
        /// </summary>
        public string WebName { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The check bit map.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool CheckBitMap()
        {
            bool update = false;
            if (this.Info == null)
            {
                return false;
            }

            this.InitTimeStamps(false);

            var bitMapDir = Path.Combine(this.Info.FullName, BitMapDirName);
            var stevneNavn = Path.GetFileName(this.Info.FullName);
            if (Directory.Exists(bitMapDir))
            {
                BitmapDirInfo[] bitmapScan = new BitmapDirInfo[5];
                bitmapScan[0] = new BitmapDirInfo() { BaneType = BaneType.Femtenmeter };
                bitmapScan[1] = new BitmapDirInfo() { BaneType = BaneType.Hundremeter };
                bitmapScan[2] = new BitmapDirInfo() { BaneType = BaneType.Tohundremeter };
                bitmapScan[3] = new BitmapDirInfo() { BaneType = BaneType.FinFelt };
                bitmapScan[4] = new BitmapDirInfo() { BaneType = BaneType.GrovFelt };

                bitmapScan[0].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix15m);
                bitmapScan[1].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix100m);
                bitmapScan[2].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix200m);
                bitmapScan[3].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.PrefixFinFelt);
                bitmapScan[4].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.PrefixGrovFelt);

                foreach (var dir2Check in bitmapScan)
                {
                    dir2Check.Updated = false;
                    var sortlist = dir2Check.BitmapFiles.OrderByDescending(x => x.LastWriteTime);
                    if (sortlist.FirstOrDefault() != null)
                    {
                        switch (dir2Check.BaneType)
                        {
                            case BaneType.Femtenmeter:
                                if (this.LastWritten15mBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    if (this.LastWritten15mBitmapFile != DateTime.MinValue)
                                    {
                                        dir2Check.Updated = true;
                                        dir2Check.Initial = false;
                                    }
                                    else
                                    {
                                        dir2Check.Initial = true;
                                    }

                                    this.LastWritten15mBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                }

                                break;
                            case BaneType.Hundremeter:
                                if (this.LastWritten100mBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    if (this.LastWritten100mBitmapFile != DateTime.MinValue)
                                    {
                                        dir2Check.Updated = true;
                                        dir2Check.Initial = false;
                                    }
                                    else
                                    {
                                        dir2Check.Initial = true;
                                    }

                                    this.LastWritten100mBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                }

                                break;
                            case BaneType.Tohundremeter:
                                if (this.LastWritten200mBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    if (this.LastWritten200mBitmapFile != DateTime.MinValue)
                                    {
                                        dir2Check.Updated = true;
                                        dir2Check.Initial = false;
                                    }
                                    else
                                    {
                                        dir2Check.Initial = true;
                                    }
                                    this.LastWritten200mBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                }

                                break;
                            case BaneType.FinFelt:
                                if (this.LastWrittenFinFeltBitmapFile.Value < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    if (this.LastWrittenFinFeltBitmapFile != DateTime.MinValue)
                                    {
                                        dir2Check.Updated = true;
                                        dir2Check.Initial = false;
                                    }
                                    else
                                    {
                                        dir2Check.Initial = true;
                                    }
                                    this.LastWrittenFinFeltBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                }

                                break;
                            case BaneType.GrovFelt:
                                if (this.LastWrittenGrovFeltBitmapFile.Value < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    if (this.LastWrittenGrovFeltBitmapFile != DateTime.MinValue)
                                    {
                                        dir2Check.Updated = true;
                                        dir2Check.Initial = false;
                                    }
                                    else
                                    {
                                        dir2Check.Initial = true;
                                    }
                                    this.LastWrittenGrovFeltBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                }

                                break;
                        }
                    }

                    var sortlistCreat = dir2Check.BitmapFiles.OrderByDescending(x => x.CreationTime);
                    if (sortlistCreat.FirstOrDefault() != null)
                    {
                        switch (dir2Check.BaneType)
                        {
                            case BaneType.Femtenmeter:
                                if (this.LastCreate15mBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    if (this.LastCreate15mBitmapFile != DateTime.MinValue)
                                    {
                                        dir2Check.Updated = true;
                                        dir2Check.Initial = false;
                                    }
                                    else
                                    {
                                        dir2Check.Initial = true;
                                    }

                                    this.LastCreate15mBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                }

                                break;
                            case BaneType.Hundremeter:
                                if (this.LastCreate100mBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    if (this.LastCreate100mBitmapFile != DateTime.MinValue)
                                    {
                                        dir2Check.Updated = true;
                                        dir2Check.Initial = false;
                                    }
                                    else
                                    {
                                        dir2Check.Initial = true;
                                    }

                                    this.LastCreate100mBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                }

                                break;
                            case BaneType.Tohundremeter:
                                if (this.LastCreate200mBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    if (this.LastCreate200mBitmapFile != DateTime.MinValue)
                                    {
                                        dir2Check.Updated = true;
                                        dir2Check.Initial = false;
                                    }
                                    else
                                    {
                                        dir2Check.Initial = true;
                                    }

                                    this.LastCreate200mBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                }

                                break;
                            case BaneType.FinFelt:
                                if (this.LastCreateFinFeltBitmapFile.Value < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    if (this.LastCreateFinFeltBitmapFile != DateTime.MinValue)
                                    {
                                        dir2Check.Updated = true;
                                        dir2Check.Initial = false;
                                    }
                                    else
                                    {
                                        dir2Check.Initial = true;
                                    }

                                    this.LastCreateFinFeltBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                }

                                break;
                            case BaneType.GrovFelt:
                                if (this.LastCreateGrovFeltBitmapFile.Value < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    if (this.LastCreateGrovFeltBitmapFile != DateTime.MinValue)
                                    {
                                        dir2Check.Updated = true;
                                        dir2Check.Initial = false;
                                    }
                                    else
                                    {
                                        dir2Check.Initial = true;
                                    }

                                    this.LastCreateGrovFeltBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                }

                                break;
                        }
                    }
                }

                foreach (var dirs in bitmapScan)
                {
                    StartListBane baneFound = null;
                    string prefix = null;
                    if (this.StevneForAlleBaner == null)
                    {
                        if (dirs.BitmapFiles != null && dirs.BitmapFiles.Count > 0)
                        {
                            Log.Info("Ingen baner funnet bitmap ikke hentet {0}", stevneNavn);
                        }
                        break;
                    }

                    foreach (var bane in this.StevneForAlleBaner.DynamiskeBaner)
                    {
                        if (dirs.BaneType == bane.BaneType)
                        {
                            baneFound = bane;
                            switch (bane.BaneType)
                            {
                                case BaneType.Femtenmeter:
                                    dirs.BitmapSubDir = Constants.Prefix15m;
                                    break;
                                case BaneType.Hundremeter:
                                    dirs.BitmapSubDir = Constants.Prefix100m;
                                    break;
                                case BaneType.Tohundremeter:
                                    dirs.BitmapSubDir = Constants.Prefix200m;
                                    break;
                                case BaneType.FinFelt:
                                    dirs.BitmapSubDir = Constants.PrefixFinFelt;
                                    break;
                                case BaneType.GrovFelt:
                                    dirs.BitmapSubDir = Constants.PrefixGrovFelt;
                                    break;
                            }
                            break;
                        }
                    }

                    if (baneFound == null)
                    {
                        continue;
                    }

                    if (dirs.Initial || dirs.Updated)
                    {
                        if (baneFound.BitmapsStoredInBane != null)
                        {
                            baneFound.BitmapsStoredInBane.Updated = dirs.Updated;
                            baneFound.BitmapsStoredInBane.BitmapFiles.Clear();
                            baneFound.BitmapsStoredInBane.BitmapFiles = dirs.BitmapFiles;
                            baneFound.BitmapsStoredInBane.BaneType = dirs.BaneType;
                        }
                        else
                        {
                            baneFound.BitmapsStoredInBane = new BitmapDirInfo { Updated = true };
                            baneFound.BitmapsStoredInBane.BitmapFiles.Clear();
                            baneFound.BitmapsStoredInBane.BitmapFiles = dirs.BitmapFiles;
                            baneFound.BitmapsStoredInBane.BaneType = dirs.BaneType;
                            update = true;
                        }

                        var memStevne = new MemoryStream();
                        XmlTextWriter write = new XmlTextWriter(memStevne, new UTF8Encoding(false));
                        dirs.InitAllNames();
                        this.m_serBitInfo.Serialize(write, dirs);
                        write.Flush();
                        memStevne.Position = 0;
                        var bitMapDoc = new XmlDocument();
                        bitMapDoc.Load(memStevne);

                        foreach (var rapport in baneFound.BaneRapporter)
                        {
                            rapport.BitMapInfo = bitMapDoc;
                        }

                        foreach (var rapport in baneFound.ToppListeRapporter)
                        {
                            rapport.BitMapInfo = bitMapDoc;
                        }

                        foreach (var rapport in baneFound.ToppListeLagRapporter)
                        {
                            rapport.BitMapInfo = bitMapDoc;
                        }
                    }
                    if (dirs.Updated)
                    {
                        update = dirs.Updated;
                    }
                }
            }

            return update;
        }

        public List<BitmapDirInfo> ListBitMapByRange(string stevnePath)
        {
            bool update = false;
            if (this.Info == null)
            {
                return null;
            }

            this.InitTimeStamps(false);
            List<BitmapDirInfo> bitmapScan = new List<BitmapDirInfo>();
            var bitMapDir = Path.Combine(stevnePath, BitMapDirName);
            var stevneNavn = Path.GetFileName(stevnePath);
            if (Directory.Exists(bitMapDir))
            {

                bitmapScan.Add( new BitmapDirInfo() { BaneType = BaneType.Femtenmeter,BitmapSubDir= Constants.Prefix15m });
                bitmapScan.Add(new BitmapDirInfo() { BaneType = BaneType.Hundremeter, BitmapSubDir = Constants.Prefix100m });
                bitmapScan.Add(new BitmapDirInfo() { BaneType = BaneType.Tohundremeter, BitmapSubDir = Constants.Prefix200m });
                bitmapScan.Add(new BitmapDirInfo() { BaneType = BaneType.FinFelt, BitmapSubDir = Constants.PrefixFinFelt });
                bitmapScan.Add(new BitmapDirInfo() { BaneType = BaneType.GrovFelt, BitmapSubDir = Constants.PrefixGrovFelt });

                bitmapScan[0].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix15m);
                bitmapScan[1].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix100m);
                bitmapScan[2].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix200m);
                bitmapScan[3].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.PrefixFinFelt);
                bitmapScan[4].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.PrefixGrovFelt);

               
            }

            return bitmapScan;
        }

        /// <summary>
        ///     The check pdf files.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        internal bool CheckPdfFiles()
        {
            if (this.Info == null)
            {
                return false;
            }

            if (this.LastWrittenPdfFile == null)
            {
                this.LastWrittenPdfFile = DateTime.MinValue;
            }

            var rapportDir = Path.Combine(this.Info.FullName, RapportDirName);

            if (Directory.Exists(rapportDir))
            {
                var webInfo = new DirectoryInfo(rapportDir);
                var listInfo = webInfo.GetFiles("*.pdf");
                if (listInfo.Length > 0)
                {
                    var list = listInfo.OrderByDescending(x => x.LastWriteTime);
                    if (list.FirstOrDefault() != null)
                    {
                        this.PdfFileName = list.FirstOrDefault().FullName;
                        if (this.LastWrittenPdfFile < list.FirstOrDefault().LastWriteTime)
                        {
                            bool update = true;
                            if (this.LastWrittenPdfFile == DateTime.MinValue)
                            {
                                this.LastWrittenPdfFile = list.FirstOrDefault().LastWriteTime;
                                update = false;
                            }

                            return update;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///     The check pdf files.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        internal bool CheckPresselisteFiles()
        {
            if (this.Info == null)
            {
                return false;
            }

            if (this.LastWrittenPresseFile == null)
            {
                this.LastWrittenPresseFile = DateTime.MinValue;
            }

            var presseDir = Path.Combine(this.Info.FullName, PressDirName);

            if (Directory.Exists(presseDir))
            {
                var webInfo = new DirectoryInfo(presseDir);
                var listInfo = webInfo.GetFiles("*.rtf");
                if (listInfo.Length > 0)
                {
                    var list = listInfo.OrderByDescending(x => x.LastWriteTime);
                    if (list.FirstOrDefault() != null)
                    {
                        this.PresseListeFileName = list.FirstOrDefault().FullName;
                        if (this.LastWrittenPresseFile < list.FirstOrDefault().LastWriteTime)
                        {
                            bool Update = true;
                            if (this.LastWrittenPresseFile != DateTime.MinValue)
                            {
                                Update = false;
                            }

                            this.LastWrittenPresseFile = list.FirstOrDefault().LastWriteTime;
                            return Update;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///     The check rapporter.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        internal bool CheckRapporter()
        {
            bool update = false;
            if (this.Info == null)
            {
                return false;
            }

            this.InitTimeStamps(false);

            var bitMapDir = Path.Combine(this.Info.FullName, BitMapDirName);
            var stevneNavn = Path.GetFileName(this.Info.FullName);
            if (Directory.Exists(bitMapDir))
            {
                BitmapDirInfo[] bitmapScan = new BitmapDirInfo[5];
                bitmapScan[0] = new BitmapDirInfo() { BaneType = BaneType.Femtenmeter };
                bitmapScan[1] = new BitmapDirInfo() { BaneType = BaneType.Hundremeter };
                bitmapScan[2] = new BitmapDirInfo() { BaneType = BaneType.Tohundremeter };
                bitmapScan[3] = new BitmapDirInfo() { BaneType = BaneType.FinFelt };
                bitmapScan[4] = new BitmapDirInfo() { BaneType = BaneType.GrovFelt };

                bitmapScan[0].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix15m);
                bitmapScan[1].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix100m);
                bitmapScan[2].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix200m);
                bitmapScan[3].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.PrefixFinFelt);
                bitmapScan[4].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.PrefixGrovFelt);
                foreach (var dir2Check in bitmapScan)
                {
                    dir2Check.Updated = false;
                    var sortlist = dir2Check.BitmapFiles.OrderByDescending(x => x.LastWriteTime);
                    if (sortlist.FirstOrDefault() != null)
                    {
                        switch (dir2Check.BaneType)
                        {
                            case BaneType.Femtenmeter:
                                if (this.LastWritten15mBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    this.LastWritten15mBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                            case BaneType.Hundremeter:
                                if (this.LastWritten100mBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    this.LastWritten100mBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                            case BaneType.Tohundremeter:
                                if (this.LastWritten200mBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    this.LastWritten200mBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                            case BaneType.FinFelt:
                                if (this.LastWrittenFinFeltBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    this.LastWrittenFinFeltBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                            case BaneType.GrovFelt:
                                if (this.LastWrittenGrovFeltBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    this.LastWrittenGrovFeltBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                        }
                    }

                    var sortlistCreat = dir2Check.BitmapFiles.OrderByDescending(x => x.CreationTime);
                    if (sortlistCreat.FirstOrDefault() != null)
                    {
                        switch (dir2Check.BaneType)
                        {
                            case BaneType.Femtenmeter:
                                if (this.LastWritten15mBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    this.LastWritten15mBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                            case BaneType.Hundremeter:
                                if (this.LastWritten100mBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    this.LastWritten100mBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                            case BaneType.Tohundremeter:
                                if (this.LastWritten200mBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    this.LastWritten200mBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                            case BaneType.FinFelt:
                                if (this.LastWrittenFinFeltBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    this.LastWrittenFinFeltBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                            case BaneType.GrovFelt:
                                if (this.LastWrittenGrovFeltBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    this.LastWrittenGrovFeltBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                    dir2Check.Updated = true;
                                }
                                break;
                        }
                    }
                }

                foreach (var dirs in bitmapScan)
                {
                    if (dirs.Updated)
                    {
                        update = true;
                        break;
                    }
                }

                foreach (var bane in this.StevneForAlleBaner.DynamiskeBaner)
                {
                    XmlDocument bitMapDoc = null;
                    if (bane.BaneRapporter.Count > 0)
                    {
                        BitmapDirInfo bitmaps = null;
                        switch (bane.BaneType)
                        {
                            case BaneType.Femtenmeter:
                                bitmaps = bitmapScan[0];
                                bane.BitmapsStoredInBane = bitmapScan[0];
                                bitmaps.BitmapSubDir = Constants.Prefix15m;
                                break;
                            case BaneType.Hundremeter:
                                bitmaps = bitmapScan[1];
                                bane.BitmapsStoredInBane = bitmapScan[1];
                                bitmaps.BitmapSubDir = Constants.Prefix100m;
                                break;
                            case BaneType.Tohundremeter:
                                bitmaps = bitmapScan[2];
                                bane.BitmapsStoredInBane = bitmapScan[2];
                                bitmaps.BitmapSubDir = Constants.Prefix200m;
                                break;
                            case BaneType.MinneFin:
                            case BaneType.FinFelt:
                                bitmaps = bitmapScan[3];
                                bane.BitmapsStoredInBane = bitmapScan[3];
                                bitmaps.BitmapSubDir = Constants.PrefixFinFelt;
                                break;
                            case BaneType.MinneGrov:
                            case BaneType.GrovFelt:
                                bitmaps = bitmapScan[4];
                                bane.BitmapsStoredInBane = bitmapScan[4];
                                bitmaps.BitmapSubDir = Constants.PrefixGrovFelt;
                                break;
                        }

                        if (bitmaps != null)
                        {
                            var memStevne = new MemoryStream();
                            XmlTextWriter write = new XmlTextWriter(memStevne, new UTF8Encoding(false));
                            bitmaps.InitAllNames();
                            this.m_serBitInfo.Serialize(write, bitmaps);
                            write.Flush();
                            memStevne.Position = 0;
                            bitMapDoc = new XmlDocument();
                            bitMapDoc.Load(memStevne);
                        }
                    }

                    foreach (var rapport in bane.BaneRapporter)
                    {
                        rapport.BitMapInfo = bitMapDoc;
                    }

                    foreach (var rapport in bane.ToppListeRapporter)
                    {
                        rapport.BitMapInfo = bitMapDoc;
                    }

                    foreach (var rapport in bane.ToppListeLagRapporter)
                    {
                        rapport.BitMapInfo = bitMapDoc;
                    }
                }

                return update;
            }

            return false;
        }

        public bool UpdateWebFiles(bool forceWebParse = false)
        {
            if (this.Info == null)
            {
                return false;
            }

            if (this.LastWrittenWebFile == null)
            {
                this.LastWrittenWebFile = DateTime.MinValue;
                Log.Trace("Setting LastWrittenWebFile to Min Val");
            }

            var web = Path.Combine(this.Info.FullName, WebDirName);
            var stevneNavn = Path.GetDirectoryName(this.Info.FullName);
            if (Directory.Exists(web))
            {
                this.WebName = web;
                var webInfo = new DirectoryInfo(web);

                var listInfo = webInfo.GetFiles("*.*");

                if (listInfo.Length > 0)
                {
                    bool update = false;
                    var list = listInfo.OrderByDescending(x => x.LastWriteTime);
                    var fileelement = list.FirstOrDefault();
                    if (fileelement != null)
                    {
                        
                        Log.Info("Update detected for stevne {0}", webInfo.FullName);
                        update = true;
                        if (!forceWebParse)
                        {
                            Thread.Sleep(5000);
                        }

                        Log.Info("Start parsing index file for stevne {0}", webInfo.FullName);
                        this.ParseIndexHtmlFile(webInfo);
                        
                    }

                    return true;
                }
            }

            return false;
        }
        /// <summary>
        ///     The check web files.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool CheckWebFiles(bool forceWebParse = false)
        {
            if (this.Info == null)
            {
                return false;
            }

            if (this.LastWrittenWebFile == null)
            {
                this.LastWrittenWebFile = DateTime.MinValue;
                Log.Trace("Setting LastWrittenWebFile to Min Val");
            }

            var web = Path.Combine(this.Info.FullName, WebDirName);
            var stevneNavn = Path.GetDirectoryName(this.Info.FullName);
            if (Directory.Exists(web))
            {
                this.WebName = web;
                var webInfo = new DirectoryInfo(web);

                var listInfo = webInfo.GetFiles("*.*");

                if (listInfo.Length > 0)
                {
                    bool update = false;
                    var list = listInfo.OrderByDescending(x => x.LastWriteTime);
                    var fileelement = list.FirstOrDefault();
                    if (fileelement != null)
                    {
                        DateTime LastWriteTime = new DateTime(
                            fileelement.LastWriteTime.Year,
                            fileelement.LastWriteTime.Month,
                            fileelement.LastWriteTime.Day,
                            fileelement.LastWriteTime.Hour,
                            fileelement.LastWriteTime.Minute,
                            0);
                        if (this.LastWrittenWebFile < LastWriteTime || forceWebParse)
                        {
                            if (this.LastWrittenWebFile != DateTime.MinValue || forceWebParse)
                            {
                                Log.Info("Update detected for stevne {0}", webInfo.FullName);
                                update = true;
                                if (!forceWebParse)
                                {
                                    Thread.Sleep(5000);
                                }

                                Log.Info("Start parsing index file for stevne {0}", webInfo.FullName);
                                this.ParseIndexHtmlFile(webInfo);
                            }

                            this.LastWrittenWebFile = LastWriteTime;
                        }
                    }

                    return update;
                }
            }

            return false;
        }

        public bool ParseIndexHtmlFile(DirectoryInfo webInfo)
        {
            Log.Info("Parsing new index File {0}", webInfo.FullName);

            var indfile = Path.Combine(webInfo.FullName, "index1.html");

            if (File.Exists(indfile))
            {
                StartingListStevne st = this.ParseStevneInfo(indfile);
                if (st != null)
                {
                    this.StevneForAlleBaner = st;
                }
                else
                {
                    return false;
                }
            }

           // LANTODO
           //var alleopropslister = GetOppropsLister(webInfo.FullName);
           // ParseXmlOpp parser = new ParseXmlOpp();
           // List<StartListBane> lister = new List<StartListBane>();
           // foreach (var ovelser in alleopropslister)
           // {
           //     StartListBane bn = parser.ParseOvelse(ovelser);
           //     if (bn != null)
           //     {
           //         lister.Add(bn);
           //     }
           // }

           // var XmlOpprop = parser.GetOppropKat(lister);


            if (this.StevneForAlleBaner != null)
            {
                var listopprop = webInfo.GetFiles("*.xml");
                if (listopprop.Length > 0)
                {
                    XmlSerializer ser = new XmlSerializer(typeof(StartListBane));

                    foreach (var baneStevne in this.StevneForAlleBaner.DynamiskeBaner)
                    {
                        baneStevne.BaneRapporter.Clear();
                        var memStevne = new MemoryStream();
                        XmlTextWriter write = new XmlTextWriter(memStevne, new UTF8Encoding(false));
                        ser.Serialize(write, baneStevne);
                        write.Flush();
                        memStevne.Position = 0;
                        XmlDocument doc = new XmlDocument();
                        doc.Load(memStevne);

                        foreach (var lagInfo in baneStevne.StevneLag)
                        {
                            if (!string.IsNullOrEmpty(lagInfo.XmlOppropsListe))
                            {
                                var filenavn = string.Format("{0}.xml", lagInfo.XmlOppropsListe);
                                var funnetRapportFil = listopprop.FirstOrDefault(x => x.Name == filenavn);
                                if (funnetRapportFil != null)
                                {
                                    RapportXmlClass nyRapport = new RapportXmlClass();
                                    nyRapport.Filnavn = funnetRapportFil.FullName;
                                    nyRapport.BaneType = baneStevne.BaneType;
                                    nyRapport.ProgramType = lagInfo.ProgramType;
                                    nyRapport.Rapport = XmlFileReaderHelper.ReadFile(funnetRapportFil.FullName);
                                    if (nyRapport.Rapport != null)
                                    {
                                        baneStevne.BaneRapporter.Add(nyRapport);
                                    }
                                }
                            }
                        }

                        if (baneStevne.ToppListeFilPrefix != null && baneStevne.ToppListeFilPrefix.Count > 0)
                        {
                            foreach (var element in baneStevne.ToppListeFilPrefix)
                            {
                                if (!string.IsNullOrEmpty(element.ReportName))
                                {
                                    var filenavn = string.Format("{0}.xml", element.ReportName);
                                    var funnetToppListeFil = listopprop.FirstOrDefault(x => x.Name == filenavn);
                                    if (funnetToppListeFil != null)
                                    {
                                        ProgramType program = FinnProgramTypeFromXml(funnetToppListeFil);
                                        RapportXmlClass nyRapport = new RapportXmlClass();
                                        nyRapport.Filnavn = funnetToppListeFil.FullName;
                                        nyRapport.BaneType = baneStevne.BaneType;
                                        nyRapport.ProgramType = program;
                                        nyRapport.Klasse = element.Klasse;
                                        nyRapport.ToppListInfoWithRef = XmlFileReaderHelper.ReadFile(funnetToppListeFil.FullName);
                                        if (nyRapport.ToppListInfoWithRef != null)
                                        {
                                            baneStevne.ToppListeRapporter.Add(nyRapport);
                                        }
                                    }
                                }
                            }
                        }

                        if (baneStevne.ToppListeLagFilPrefix != null && baneStevne.ToppListeLagFilPrefix.Count > 0)
                        {
                            foreach (var filnavn in baneStevne.ToppListeLagFilPrefix)
                            {
                                if (!string.IsNullOrEmpty(filnavn))
                                {
                                    var filenavn = string.Format("{0}.xml", filnavn);
                                    var funnetToppListeLagFil = listopprop.FirstOrDefault(x => x.Name == filenavn);
                                    if (funnetToppListeLagFil != null)
                                    {
                                        RapportXmlClass nyRapport = new RapportXmlClass();
                                        nyRapport.Filnavn = funnetToppListeLagFil.FullName;
                                        nyRapport.BaneType = baneStevne.BaneType;
                                        nyRapport.ProgramType = ProgramType.Lagskyting;
                                        nyRapport.ToppListInfoWithRef = XmlFileReaderHelper.ReadFile(funnetToppListeLagFil.FullName);
                                        if (nyRapport.ToppListInfoWithRef != null)
                                        {
                                            baneStevne.ToppListeRapporter.Add(nyRapport);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        private ProgramType FinnProgramTypeFromXml(FileInfo funnetToppListeFil)
        {
            if (funnetToppListeFil == null)
            {
                return ProgramType.UnKnown;
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(funnetToppListeFil.FullName);
                var node=doc.SelectSingleNode("/report/resulttotsum/@name");
                if (node == null)
                {
                    var nodeMinne = doc.SelectSingleNode("/report/header/@group_name");
                    if (nodeMinne != null)
                    {
                        if (nodeMinne.InnerText.Trim().ToUpper() == "MINNESKYTING")
                        {
                            return ProgramType.Minne;
                        }
                    }
                    
                    return ProgramType.UnKnown;
                }

                switch (node.InnerText.Trim().ToUpper())
                {
                    case "30-SKUDD":
                        return ProgramType.Innledende;
                    case "25-SKUDD":
                    case "25 SKUDD":
                        return ProgramType.Innledende;
                    case "FINALE":
                        return ProgramType.Finale;
                    case "35 SKUDD":
                    case "35-SKUDD":
                        return ProgramType.Mesterskap;
                    case "42-SKUDD":
                        return ProgramType.Mesterskap;
                    default:
                        Log.Warning("FinnProgramTypeFromXml unknown {0}", node.InnerText.Trim().ToUpper());
                        break;
                }


            }
            catch (Exception e)
            {
                
                Log.Error(e,"Error prsing file {0}", funnetToppListeFil.Name);
            }

            return ProgramType.UnKnown;
        }

        /// <summary>
        ///     The init time stamps.
        /// </summary>
        /// <param name="forcemin">
        ///     The forcemin.
        /// </param>
        internal void InitTimeStamps(bool forcemin = true)
        {
            if (forcemin)
            {
                Log.Trace("Init all Timestams to min forced");
                this.LastWrittenOppropFile = DateTime.MinValue;
                this.LastWrittenWebFile = DateTime.MinValue;
                this.LastWrittenPdfFile = DateTime.MinValue;
                this.LastWrittenStartingFile = DateTime.MinValue;
                this.LastWritten15mBitmapFile = DateTime.MinValue;
                this.LastWritten100mBitmapFile = DateTime.MinValue;
                this.LastWritten200mBitmapFile = DateTime.MinValue;
                this.LastWrittenFinFeltBitmapFile = DateTime.MinValue;
                this.LastWrittenGrovFeltBitmapFile = DateTime.MinValue;
                this.LastCreate15mBitmapFile = DateTime.MinValue;
                this.LastCreate100mBitmapFile = DateTime.MinValue;
                this.LastCreate200mBitmapFile = DateTime.MinValue;
                this.LastCreateFinFeltBitmapFile = DateTime.MinValue;
                this.LastCreateGrovFeltBitmapFile = DateTime.MinValue;
                this.LastWrittenPresseFile = DateTime.MinValue;
            }
            else
            {
                Log.Trace("Init all Timestams Null to min");
                if (this.LastWritten15mBitmapFile == null)
                {
                    this.LastWritten15mBitmapFile = DateTime.MinValue;
                }

                if (this.LastWritten100mBitmapFile == null)
                {
                    this.LastWritten100mBitmapFile = DateTime.MinValue;
                }

                if (this.LastWritten200mBitmapFile == null)
                {
                    this.LastWritten200mBitmapFile = DateTime.MinValue;
                }

                if (this.LastWrittenFinFeltBitmapFile == null)
                {
                    this.LastWrittenFinFeltBitmapFile = DateTime.MinValue;
                }

                if (this.LastWrittenGrovFeltBitmapFile == null)
                {
                    this.LastWrittenGrovFeltBitmapFile = DateTime.MinValue;
                }

                if (this.LastCreate15mBitmapFile == null)
                {
                    this.LastCreate15mBitmapFile = DateTime.MinValue;
                }

                if (this.LastCreate100mBitmapFile == null)
                {
                    this.LastCreate100mBitmapFile = DateTime.MinValue;
                }

                if (this.LastCreate200mBitmapFile == null)
                {
                    this.LastCreate200mBitmapFile = DateTime.MinValue;
                }

                if (this.LastCreateFinFeltBitmapFile == null)
                {
                    this.LastCreateFinFeltBitmapFile = DateTime.MinValue;
                }

                if (this.LastCreateGrovFeltBitmapFile == null)
                {
                    this.LastCreateGrovFeltBitmapFile = DateTime.MinValue;
                }

                if (this.LastWrittenPresseFile == null)
                {
                    this.LastWrittenPresseFile = DateTime.MinValue;
                }

                if (this.LastWrittenOppropFile == null)
                {
                    this.LastWrittenOppropFile = DateTime.MinValue;
                }
            }
        }

        /// <summary>
        ///     The get bit map files.
        /// </summary>
        /// <param name="stevnePath">
        ///     The stevne path.
        /// </param>
        /// <param name="subDir">
        ///     The sub dir.
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        private static List<FileInfo> GetBitMapFiles(string stevnePath, string subDir)
        {
            List<FileInfo> retList = new List<FileInfo>();
            var bitMapDir = Path.Combine(stevnePath, BitMapDirName);
            var bitMapSubDir = Path.Combine(bitMapDir, subDir);
            if (Directory.Exists(bitMapSubDir))
            {
                var bitmapSubInfo = new DirectoryInfo(bitMapSubDir);
                var listInfo = bitmapSubInfo.GetFiles("TR*.PNG");
                if (listInfo.Length > 0)
                {
                    return listInfo.ToList();
                }
            }

            return retList;
        }

        /// <summary>
        ///     The get lagvis opprop.
        /// </summary>
        /// <param name="nodeinfoH3">
        ///     The nodeinfo h 3.
        /// </param>
        /// <param name="baneInfo">
        ///     The bane info.
        /// </param>
        private static void GetLagvisOpprop(HtmlNode nodeinfoH3, StartListBane baneInfo)
        {
            if (nodeinfoH3 != null && baneInfo != null)
            {
                HtmlNode Lag = null;
                HtmlNode LagType = null;
                HtmlNodeCollection alleLagVise = null;
                var nodeSibling = nodeinfoH3.NextSibling;
                while (nodeSibling != null)
                {
                    if (nodeSibling.NodeType == HtmlNodeType.Element && nodeSibling.Name == "div")
                    {
                        var attrValClass = nodeSibling.GetAttributeValue("id", "DEF");
                        ProgramType typeOvelse = ProgramType.Innledende;
                        foreach (var noder in nodeSibling.ChildNodes)
                        {
                            if (noder.NodeType == HtmlNodeType.Element && noder.Name == "h3")
                            {
                                typeOvelse = StartingListLag.ParseOvelse(noder.InnerText);
                            }
                            else if (noder.NodeType == HtmlNodeType.Element && noder.Name == "div")
                            {
                                var attrVal = noder.GetAttributeValue("id", "DEF");
                                if (attrVal.StartsWith("accordionsub"))
                                {
                                    if (typeOvelse == ProgramType.Finale || typeOvelse == ProgramType.Innledende)
                                    {
                                        ParseLagListe(baneInfo, noder, typeOvelse);
                                    }
                                }
                            }
                        }
                    }
                    else if (nodeSibling.NodeType == HtmlNodeType.Element && nodeSibling.Name == "h3")
                    {
                        break;
                    }

                    nodeSibling = nodeSibling.NextSibling;
                }
            }
        }

        private static void GetLagSkytingOpprop(HtmlNode nodeinfoH3, StartListBane baneInfo)
        {
            if (nodeinfoH3 != null && baneInfo != null)
            {
                HtmlNode Lag = null;
                HtmlNode LagType = null;
                HtmlNodeCollection alleLagVise = null;
                var nodeSibling = nodeinfoH3.NextSibling;
                while (nodeSibling != null)
                {
                    if (nodeSibling.NodeType == HtmlNodeType.Element && nodeSibling.Name == "div")
                    {
                        var attrValClass = nodeSibling.GetAttributeValue("id", "DEF");
                        ProgramType typeOvelse = ProgramType.Innledende;
                        foreach (var noder in nodeSibling.ChildNodes)
                        {
                            if (noder.NodeType == HtmlNodeType.Element && noder.Name == "h3")
                            {
                                typeOvelse = StartingListLag.ParseOvelse(noder.InnerText);
                            }
                            else if (noder.NodeType == HtmlNodeType.Element && noder.Name == "div")
                            {
                                var attrVal = noder.GetAttributeValue("id", "DEF");
                                if (attrVal.StartsWith("accordionsub"))
                                {
                                    ParseLagListe(baneInfo, noder, typeOvelse);
                                }
                            }
                        }
                    }
                    else if (nodeSibling.NodeType == HtmlNodeType.Element && nodeSibling.Name == "h3")
                    {
                        break;
                    }

                    nodeSibling = nodeSibling.NextSibling;
                }
            }
        }

        public static void ParseLagListe(StartListBane baneInfo, HtmlNode lagnode, ProgramType lagType)
        {
            int dag = 1;
            var datonode = lagnode.SelectSingleNode(string.Format("h4[{0}]", dag));
            while (datonode != null)
            {
                var dagnode = lagnode.SelectSingleNode(string.Format("div[{0}]/table", dag));
                DateTime? oppropsDato = StartingListLag.ParseStartDate(baneInfo.StevneNavn, datonode.InnerText);
                var lagnoder = dagnode.SelectNodes("tr");
                if (lagnoder != null)
                {
                    foreach (var lag in lagnoder)
                    {
                        string xmlfilNavn = null;
                        var xmlfil = lag.SelectSingleNode("td[1]");
                        if (xmlfil != null)
                        {
                            xmlfilNavn = xmlfil.GetAttributeValue("id", "NULL");
                        }

                        var lagnrnode = lag.SelectSingleNode("td[1]/a");
                        int lagnummer = -1;
                        if (lagnrnode != null)
                        {
                            lagnummer = StartingListLag.ParseLagNr(lagnrnode.InnerText);
                        }
                        else
                        {
                            Log.Error("Lagnr ikke satt {0}", lag.InnerText);
                        }

                        DateTime? lagtid = null;
                        var klokkeslett = lag.SelectSingleNode("td[2]");
                        if (klokkeslett != null)
                        {
                            lagtid = StartingListLag.ParseStartTime(oppropsDato, klokkeslett.InnerText);
                        }
                        else
                        {
                            Log.Error("klokkeslett ikke satt {0}", lag.InnerText);
                        }

                        if (lagtid != null && lagnummer > -1)
                        {
                            // if (stevneInfo == null)
                            // {
                            // Log.Error("Fant korrekt skive men stevne mangler{0} {1}", lagtid, lagnummer);
                            // continue;
                            // }
                            switch (lagType)
                            {
                                case ProgramType.Innledende:
                                    break;
                                case ProgramType.Finale:
                                    lagnummer = lagnummer + 100;
                                    break;
                                case ProgramType.Lagskyting:
                                    lagnummer = lagnummer + 200;
                                    break;
                                case ProgramType.SamLagskyting:
                                    lagnummer = lagnummer + 200;
                                    break;
                            }

                            StartingListLag nyttlag = new StartingListLag(lagnummer);
                            nyttlag.StartTime = lagtid;
                            nyttlag.BaneType = baneInfo.BaneType;
                            nyttlag.StevneNavn = baneInfo.StevneNavn;
                            nyttlag.XmlOppropsListe = xmlfilNavn;
                            nyttlag.ProgramType = lagType;
                            baneInfo.AddLag(nyttlag);
                        }
                    }
                }

                dag++;
                datonode = lagnode.SelectSingleNode(string.Format("h4[{0}]", dag));
            }
        }

        /// <summary>
        ///     The get report files for bane.
        /// </summary>
        /// <param name="nodeinfo">
        ///     The nodeinfo.
        /// </param>
        /// <param name="bane">
        ///     The bane.
        /// </param>
        private static void GetReportFilesForBane(HtmlNode nodeinfo, StartListBane bane)
        {
            HtmlNode nodeinfoH3;

            // if (nodeinfo.InnerText.ToUpper() == "15M" && bane != null)
            // {
            nodeinfoH3 = nodeinfo;
            var nodeSibling = nodeinfoH3.NextSibling;
            while (nodeSibling != null)
            {
                if (nodeSibling.NodeType == HtmlNodeType.Element && nodeSibling.Name == "div")
                {
                    string valclass = nodeSibling.GetAttributeValue("class", "NOVAL");
                    if (string.Compare(valclass, "ACCORDION_BUTTON", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        var allreportNodesBane = nodeSibling.SelectNodes("div");
                        if (allreportNodesBane != null && allreportNodesBane.Count > 0)
                        {
                            foreach (var nodeKlasse in allreportNodesBane)
                            {
                                var val = nodeKlasse.GetAttributeValue("id", "NOVAL");
                                if (!string.IsNullOrEmpty(val) && string.Compare(val, "NOVAL") != 0)
                                {
                                    var overskriftNode = nodeKlasse.SelectSingleNode("a");
                                    if (overskriftNode != null)
                                    {
                                        if (!string.IsNullOrEmpty(overskriftNode.InnerText))
                                        {
                                            if (string.Compare(overskriftNode.InnerText, "UNGDOM", StringComparison.OrdinalIgnoreCase) == 0
                                                || string.Compare(overskriftNode.InnerText, "UNGDOM INDIVIDUELL", StringComparison.OrdinalIgnoreCase)
                                                == 0)
                                            {
                                                Log.Info("Found report Ungdom Lag File {0}", val);
                                                bane.ToppListeLagFilPrefix.Add(val);
                                            }
                                            else if (string.Compare(overskriftNode.InnerText, "VETERAN", StringComparison.OrdinalIgnoreCase) == 0
                                                     || string.Compare(
                                                         overskriftNode.InnerText,
                                                         "VETERAN INDIVIDUELL",
                                                         StringComparison.OrdinalIgnoreCase) == 0)
                                            {
                                                Log.Info("Found report Veteran Lag File {0}", val);
                                                bane.ToppListeLagFilPrefix.Add(val);
                                            }
                                            else if (string.Compare(overskriftNode.InnerText, "SENIOR", StringComparison.OrdinalIgnoreCase) == 0
                                                     || string.Compare(
                                                         overskriftNode.InnerText,
                                                         "SENIOR INDIVIDUELL",
                                                         StringComparison.OrdinalIgnoreCase) == 0)
                                            {
                                                Log.Info("Found report Lag File {0}", val);
                                                bane.ToppListeLagFilPrefix.Add(val);
                                            }
                                            else
                                            {
                                                Log.Info("Found report File {0}", val);
                                                List<string> klasse=GetKlasse(overskriftNode.InnerText);
                                                var element=new TopListElement() { Klasse = klasse,ReportName = val }; 
                                                bane.ToppListeFilPrefix.Add(element);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
                }

                nodeSibling = nodeSibling.NextSibling;
            }

            // }
        }

        private static List<string> GetKlasse(string innerText)
        {
            List < string > retVal = new List<string>();
            if (string.IsNullOrEmpty(innerText))
            {
                return retVal;
            }
            string klasse = innerText.Trim().ToUpper();
            switch (klasse)
            {
               
                case "JUNIOR":
                    retVal.Add("J");
                    break;
             
                case "V73":
                    retVal.Add("V73");
                    break;
                case "V65":
                    retVal.Add("V65");
                    break;
                case "ELDRE REKRUTT":
                    retVal.Add("ER");
                    break;
                case "REKRUTT":
                    retVal.Add("R");
                    break;
                case "ASPIRANT":
                    retVal.Add("ASP");
                    break;
              

                case "AG3":
                    retVal.Add("AG3");
                    break;
                case "HK4":
                    retVal.Add("HK416");
                    break;
                case "KLASSE 1":
                    retVal.Add("1");
                    break;
                case "KLASSE 2":
                    retVal.Add("2");
                    break;
                case "KLASSE 3":
                    retVal.Add("3");
                    break;
                case "KLASSE 4":
                    retVal.Add("4");
                    break;
                case "KLASSE 5":
                    retVal.Add("5");
                    break;
                case "KLASSE V55":
                case "V55":
                    retVal.Add("V55");
                    break;
                case "KLASSE 2-5, V55":
                    retVal.Add("2");
                    retVal.Add("3");
                    retVal.Add("4");
                    retVal.Add("5");
                    retVal.Add("V55");
                    break;
                case "KLASSE 3-5":
                    retVal.Add("3");
                    retVal.Add("4");
                    retVal.Add("5");
                    break;
                case "ÅPEN":
                    retVal.Add("Å");
                    break;
                case "JEGER":
                    retVal.Add("J");
                    break;
                default:
                    if (klasse.StartsWith("ELDRE JUNIOR"))
                    {
                        retVal.Add("EJ");
                    }
                    else if (klasse.StartsWith("JEG"))
                    {
                        retVal.Add("J");
                    }
                    else if (klasse.StartsWith("EJ"))
                    {
                        retVal.Add("EJ");
                    }
                    else if (klasse.StartsWith("JUNIOR"))
                    {
                        retVal.Add("J");
                    }
                    else if (klasse.StartsWith("J"))
                    {
                        retVal.Add("J");
                    }
                    else if (klasse.StartsWith("ELDRE REKRUTT"))
                    {
                        retVal.Add("ER");
                    }
                    else if (klasse.StartsWith("ER"))
                    {
                        retVal.Add("ER");
                    }
                    else if (klasse.StartsWith("REKRUTT"))
                    {
                        retVal.Add("R");
                    }
                    else if (klasse.StartsWith("R"))
                    {
                        retVal.Add("R");
                    }
                    else if (klasse.StartsWith("NYBEGYNNER UNGDOM"))
                    {
                        retVal.Add("NU");
                    }
                    else if (klasse.StartsWith("NU"))
                    {
                        retVal.Add("NU");
                    }
                    else if (klasse.StartsWith("NYBEGYNNER VOKSEN"))
                    {
                        retVal.Add("NV");
                    }
                    else if (klasse.StartsWith("NV"))
                    {
                        retVal.Add("NV");
                    }
                    else if (klasse.EndsWith("PEN"))
                    {
                        retVal.Add("Å");
                    }
                    else
                    {
                        Log.Warning("Uknown klasse GetKlasse {0}", innerText.Trim().ToUpper());
                    }
                    break;
            }

            return retVal;
        }

        private static void GetReportFilesForFelt(HtmlNode nodeinfo, StartListBane bane)
        {
            HtmlNode nodeinfoH3;

            // if (nodeinfo.InnerText.ToUpper() == "15M" && bane != null)
            // {
            nodeinfoH3 = nodeinfo;
            var nodeSibling = nodeinfoH3.NextSibling;
            while (nodeSibling != null)
            {
                if (nodeSibling.NodeType == HtmlNodeType.Element && nodeSibling.Name == "div")
                {
                    string valclass = nodeSibling.GetAttributeValue("class", "NOVAL");
                    if (string.Compare(valclass, "ACCORDION_BUTTON", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        var allreportNodesBane = nodeSibling.SelectNodes("div");
                        if (allreportNodesBane != null && allreportNodesBane.Count > 0)
                        {
                            foreach (var nodeKlasse in allreportNodesBane)
                            {
                                var val = nodeKlasse.GetAttributeValue("id", "NOVAL");
                                if (!string.IsNullOrEmpty(val) && string.Compare(val, "NOVAL") != 0)
                                {
                                    var overskriftNode = nodeKlasse.SelectSingleNode("a");
                                    if (overskriftNode != null)
                                    {
                                        if (!string.IsNullOrEmpty(overskriftNode.InnerText))
                                        {
                                            if (string.Compare(overskriftNode.InnerText, "UNGDOM", StringComparison.OrdinalIgnoreCase) == 0
                                                || string.Compare(overskriftNode.InnerText, "UNGDOM INDIVIDUELL", StringComparison.OrdinalIgnoreCase)
                                                == 0)
                                            {
                                                Log.Info("Found report Ungdom Lag File {0}", val);
                                                bane.ToppListeLagFilPrefix.Add(val);
                                            }
                                            else if (string.Compare(overskriftNode.InnerText, "VETERAN", StringComparison.OrdinalIgnoreCase) == 0
                                                     || string.Compare(
                                                         overskriftNode.InnerText,
                                                         "VETERAN INDIVIDUELL",
                                                         StringComparison.OrdinalIgnoreCase) == 0)
                                            {
                                                Log.Info("Found report Veteran Lag File {0}", val);
                                                bane.ToppListeLagFilPrefix.Add(val);
                                            }
                                            else if (string.Compare(overskriftNode.InnerText, "SENIOR", StringComparison.OrdinalIgnoreCase) == 0
                                                     || string.Compare(
                                                         overskriftNode.InnerText,
                                                         "SENIOR INDIVIDUELL",
                                                         StringComparison.OrdinalIgnoreCase) == 0)
                                            {
                                                Log.Info("Found report Lag File {0}", val);
                                                bane.ToppListeLagFilPrefix.Add(val);
                                            }
                                            else
                                            {
                                                Log.Info("Found report File {0}", val);
                                                List<string> klasse = GetKlasse(overskriftNode.InnerText);
                                                var element = new TopListElement() { Klasse = klasse, ReportName = val };
                                                bane.ToppListeFilPrefix.Add(element);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
                }

                nodeSibling = nodeSibling.NextSibling;
            }

            // }
        }

        private static void GetReportFilesForMinne(HtmlNode nodeinfo, StartListBane bane)
        {
            HtmlNode nodeinfoH3;

            // if (nodeinfo.InnerText.ToUpper() == "15M" && bane != null)
            // {
            nodeinfoH3 = nodeinfo;
            var nodeSibling = nodeinfoH3.NextSibling;
            while (nodeSibling != null)
            {
                if (nodeSibling.NodeType == HtmlNodeType.Element && nodeSibling.Name == "div")
                {
                    string valclass = nodeSibling.GetAttributeValue("class", "NOVAL");
                    if (string.Compare(valclass, "ACCORDION_BUTTON", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        var allreportNodesBane = nodeSibling.SelectNodes("div");
                        if (allreportNodesBane != null && allreportNodesBane.Count > 0)
                        {
                            foreach (var nodeKlasse in allreportNodesBane)
                            {
                                var val = nodeKlasse.GetAttributeValue("id", "NOVAL");
                                if (!string.IsNullOrEmpty(val) && string.Compare(val, "NOVAL") != 0)
                                {
                                    var overskriftNode = nodeKlasse.SelectSingleNode("a");
                                    if (overskriftNode != null)
                                    {
                                        if (!string.IsNullOrEmpty(overskriftNode.InnerText))
                                        {
                                            if (overskriftNode.InnerText.Contains("Minneskyting kl"))
                                            {
                                                Log.Info("Found report Minne prklasse Lag File {0}", val);
                                                bane.ToppListeLagFilPrefix.Add(val);
                                            }
                                            else
                                            {
                                                Log.Info("Found report File {0}", val);
                                                List<string> klasse = GetKlasse(overskriftNode.InnerText);
                                                var element = new TopListElement() { Klasse = klasse, ReportName = val };
                                                bane.ToppListeFilPrefix.Add(element);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
                }

                nodeSibling = nodeSibling.NextSibling;
            }

            // }
        }

        /// <summary>
        ///     The parse rapport fil.
        /// </summary>
        /// <param name="filNavn">
        ///     The fil navn.
        /// </param>
        private void ParseRapportFil(string filNavn)
        {
            try
            {
                report rapport = null;
                using (StreamReader filestream = new System.IO.StreamReader(filNavn))
                {
                    var xmlSer = new XmlSerializer(typeof(report));
                    XmlReader read = new XmlTextReader(filestream);
                    rapport = xmlSer.Deserialize(read) as report;
                }

                if (rapport != null)
                {
                    StartingListLag lag = null;
                    foreach (var item in rapport.Items)
                    {
                        int lagnr = 0;
                        if (item.GetType() == typeof(reportHeader))
                        {
                            reportHeader header = item as reportHeader;
                            if (!string.IsNullOrEmpty(header.name))
                            {
                                if (header.name.StartsWith("Lag"))
                                {
                                    var splits = header.name.Split(new[] { ' ' });

                                    if (int.TryParse(splits[1], out lagnr))
                                    {
                                        lag = new StartingListLag();
                                        lag.LagNr = lagnr;
                                    }
                                }
                            }
                        }
                        else if (item.GetType() == typeof(reportData))
                        {
                            if (lag == null)
                            {
                                continue;
                            }

                            reportData dataItem = item as reportData;
                            if (lagnr > 0)
                            {
                            }

                            foreach (var part in dataItem.result)
                            {
                                int skive = 0;
                                if (int.TryParse(part.num, out skive))
                                {
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
            }
        }


        public static string RemoveControlCharacters(string inString)
        {
            if (inString == null) return null;
            StringBuilder newString = new StringBuilder();
            char ch;
            for (int i = 0; i < inString.Length; i++)
            {
                ch = inString[i];
                if (!char.IsControl(ch))
                {
                    newString.Append(ch);
                }
            }
            return newString.ToString();
        }

        /// <summary>
        ///     The parse stevne info.
        /// </summary>
        /// <param name="indfile">
        ///     The indfile.
        /// </param>
        /// <returns>
        ///     The <see cref="StartingListStevne" />.
        /// </returns>
        private StartingListStevne ParseStevneInfo(string indfile)
        {
            try
            {
                string inputString;
                Encoding enc = Encoding.GetEncoding("UTF-8");
                
                using (StreamReader read = new StreamReader(indfile, enc))
                {
                    inputString = read.ReadToEnd();
                }
                var teststring=RemoveControlCharacters(inputString);
                if (teststring != inputString)
                {
                    string inputString2;
                    Encoding enc2 = Encoding.GetEncoding("ISO-8859-1");
                    using (StreamReader read = new StreamReader(indfile, enc2))
                    {
                        inputString2 = read.ReadToEnd();
                    }
                    inputString = inputString2;
                }
                var directoryName = Path.GetDirectoryName(indfile);
                List<string> topListeFiler = new List<string>();
                StartingListStevne stevneInfo = null;
                var test = new HtmlAgilityPack.HtmlDocument();
                test.OptionDefaultStreamEncoding = new UTF8Encoding(false);
                test.LoadHtml(inputString);
                var nodehead = test.DocumentNode.SelectSingleNode("/html/body/div[@id='header']/h1");

                var nodebody = test.DocumentNode.SelectSingleNode("/html/body/div[@id='container']/div[@id='navigation']");
                var nodebody2 = test.DocumentNode.SelectNodes("//div[@id='accordion']");

                if (nodebody2 != null)
                {
                    foreach (var node in nodebody2)
                    {
                        var h3 = node.SelectNodes("h3");

                        bool found = false;
                        if (h3 != null)
                        {
                            HtmlNode nodeinfoH3 = null;
                            foreach (var nodeinfo in h3)
                            {
                                if (nodeinfo != null && !string.IsNullOrEmpty(nodeinfo.InnerText))
                                {
                                    if (nodeinfo.InnerText.ToUpper().Contains("LAGVIS"))
                                    {
                                        if (stevneInfo == null)
                                        {
                                            stevneInfo = new StartingListStevne();
                                            stevneInfo.ParseStevneNavn(nodehead.InnerText);
                                        }

                                        BaneType baneType = StartListBane.FindBaneType(nodeinfo.InnerText);
                                        if (baneType == BaneType.Undefined)
                                        {
                                            Log.Error("Uknown Banetype {0} fil ={1}", nodeinfo.InnerText, indfile);
                                            return null;
                                        }

                                        var bane = stevneInfo.FinnBane(baneType);

                                        if (bane == null)
                                        {
                                            bane = stevneInfo.AddNewBane(stevneInfo.StevneNavn, directoryName, baneType);
                                            bane.ReportDirStevneNavn = stevneInfo.ReportDirStevneNavn;
                                        }
                                        else
                                        {
                                            bane.StevneLag.Clear();
                                        }

                                        nodeinfoH3 = nodeinfo;
                                        GetLagvisOpprop(nodeinfoH3, bane);
                                    }
                                    else if (nodeinfo.InnerText.ToUpper().Contains("MINNESKYTING"))
                                    {
                                        if (stevneInfo == null)
                                        {
                                            stevneInfo = new StartingListStevne();
                                            stevneInfo.ParseStevneNavn(nodehead.InnerText);
                                        }
                                        bool breakFound = false;
                                        string klasse = string.Empty;
                                        HtmlNodeCollection KlasseVisMinneNode = null;
                                        HtmlNode sibling = nodeinfo;
                                        do
                                        {
                                            sibling = sibling.NextSibling;
                                            if (sibling == null)
                                            {
                                                breakFound = true;
                                            }
                                            else
                                            {
                                                if (sibling.NodeType == HtmlNodeType.Element && sibling.Name == "div" && sibling.HasAttributes)
                                                {
                                                    if (sibling.GetAttributeValue("class", "") == "accordion_button") ;
                                                    {
                                                        KlasseVisMinneNode = sibling.SelectNodes("div");
                                                        breakFound = true;
                                                    }
                                                }
                                            }

                                        }
                                        while (!breakFound);

                                        if (KlasseVisMinneNode != null)
                                        {
                                            foreach (var minnesk in KlasseVisMinneNode)
                                            {
                                                var typenode = minnesk.SelectSingleNode("a");
                                                var fileid = minnesk.GetAttributeValue("id", "");
                                                if (typenode != null && !string.IsNullOrEmpty(fileid))
                                                {
                                                    var tostr = typenode.InnerText;
                                                    if (!string.IsNullOrEmpty(tostr))
                                                    {
                                                        tostr = tostr.ToUpper();
                                                        var splits = tostr.Split(
                                                            new string[] { "KL." },
                                                            StringSplitOptions.RemoveEmptyEntries);
                                                        if (splits.Length >= 2)
                                                        {
                                                            klasse = splits[1].Trim();
                                                            BaneType baneType = StartListBane.FindMinneBaneTypeForKlasse(klasse);
                                                            if (baneType == BaneType.MinneGrov)
                                                            {
                                                                var Bane = stevneInfo.FinnBane(BaneType.GrovFelt);
                                                                if (Bane != null)
                                                                {
                                                                    TopListElement el = new TopListElement();
                                                                    el.Klasse= new List<string>();
                                                                    el.Klasse.Add(klasse);
                                                                    el.ReportName = fileid;
                                                                    Bane.ToppListeFilPrefix.Add(el);
                                                                }
                                                            }
                                                            else if (baneType == BaneType.MinneFin)
                                                            {
                                                                var Bane = stevneInfo.FinnBane(BaneType.FinFelt);
                                                                if (Bane != null)
                                                                {
                                                                    TopListElement el = new TopListElement();
                                                                    el.Klasse = new List<string>();
                                                                    el.Klasse.Add(klasse);
                                                                    el.ReportName = fileid;
                                                                    Bane.ToppListeFilPrefix.Add(el);
                                                                }  
                                                                
                                                            }
                                                        } 
                                                    }
                                                }
                                            }

                                            //{
                                            //    sibling = sibling.NextSibling;
                                            //    if (sibling != null)
                                            //    {
                                            //        if (sibling.NodeType == HtmlNodeType.Element && sibling.InnerText.ToUpper().Contains("KL."))
                                            //        {
                                            //            var typenode = sibling.SelectSingleNode("div/a");
                                            //            if (typenode != null)
                                            //            {
                                            //                var tostr = typenode.InnerText;
                                            //                if (!string.IsNullOrEmpty(tostr))
                                            //                {
                                            //                    tostr = tostr.ToUpper();
                                            //                    var splits = tostr.Split(
                                            //                        new string[] { "KL." },
                                            //                        StringSplitOptions.RemoveEmptyEntries);
                                            //                    if (splits.Length >= 2)
                                            //                    {
                                            //                        klasse = splits[1].Trim();
                                            //                    }
                                            //                }
                                            //            }

                                            //        }
                                            //    }

                                            //    if (sibling == null)
                                            //    {
                                            //        breakFound = true;
                                            //    }
                                            //}

                                            //BaneType baneType = StartListBane.FindMinneBaneTypeForKlasse(klasse);
                                            //if (baneType == BaneType.Undefined)
                                            //{
                                            //    Log.Error("Uknown Banetype {0} fil ={1}", nodeinfo.InnerText, indfile);
                                            //}
                                            //else
                                            //{


                                            //    var bane = stevneInfo.FinnBane(baneType);

                                            //    if (bane == null)
                                            //    {
                                            //        bane = stevneInfo.AddNewBane(stevneInfo.StevneNavn, directoryName, baneType);
                                            //        bane.ReportDirStevneNavn = stevneInfo.ReportDirStevneNavn;
                                            //    }
                                            //    else
                                            //    {
                                            //        bane.StevneLag.Clear();
                                            //    }

                                            //    nodeinfoH3 = nodeinfo;
                                            //    GetLagvisOpprop(nodeinfoH3, bane);
                                            //}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (nodebody2 != null && stevneInfo != null)
                {
                    foreach (var node in nodebody2)
                    {
                        var h3 = node.SelectNodes("//h3");
                        bool found = false;
                        if (h3 != null)
                        {
                            HtmlNode nodeinfoH3 = null;
                            foreach (var nodeinfo in h3)
                            {
                                if (nodeinfo != null && !string.IsNullOrEmpty(nodeinfo.InnerText))
                                {
                                    var bane = stevneInfo.FinnBane(BaneType.Femtenmeter);
                                    if (nodeinfo.InnerText.ToUpper() == "15M" && bane != null)
                                    {
                                        GetReportFilesForBane(nodeinfo, bane);
                                    }
                                    if (nodeinfo.InnerText.ToUpper() == "RANGERING" && bane != null)
                                    {
                                        GetReportFilesForBane(nodeinfo, bane);
                                    }

                                    bane = stevneInfo.FinnBane(BaneType.Hundremeter);
                                    if (nodeinfo.InnerText.ToUpper() == "100M" && bane != null)
                                    {
                                        GetReportFilesForBane(nodeinfo, bane);
                                    }

                                    bane = stevneInfo.FinnBane(BaneType.Tohundremeter);
                                    if (nodeinfo.InnerText.ToUpper() == "200M" && bane != null)
                                    {
                                        GetReportFilesForBane(nodeinfo, bane);
                                    }

                                    bane = stevneInfo.FinnBane(BaneType.FinFelt);
                                    if (nodeinfo.InnerText.ToUpper() == "FINFELT" && bane != null)
                                    {
                                        GetReportFilesForFelt(nodeinfo, bane);
                                    }

                                    bane = stevneInfo.FinnBane(BaneType.GrovFelt);
                                    if (nodeinfo.InnerText.ToUpper() == "GROVFELT" && bane != null)
                                    {
                                        GetReportFilesForFelt(nodeinfo, bane);
                                    }

                                    bane = stevneInfo.FinnBane(BaneType.MinneFin);
                                    if (nodeinfo.InnerText.ToUpper() == "MINNESKYTING" && bane != null)
                                    {
                                        GetReportFilesForMinne(nodeinfo, bane);
                                    }

                                    bane = stevneInfo.FinnBane(BaneType.MinneGrov);
                                    if (nodeinfo.InnerText.ToUpper() == "MINNESKYTING" && bane != null)
                                    {
                                        GetReportFilesForMinne(nodeinfo, bane);
                                    }
                                }
                            }
                        }
                    }
                }

                GetLagSkytingOpprop(stevneInfo);
               // GetLagOpprop(stevneInfo);

                return stevneInfo;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Format("error parsing {0}", indfile));
            }

            return null;
        }

        //private void GetLagOpprop(StartingListStevne stevneInfo)
        //{
        //    if (stevneInfo == null)
        //    {
        //        return;
        //    }

        //    if (stevneInfo.DynamiskeBaner == null)
        //    {
        //        return;
        //    }

        //    Encoding enc = Encoding.GetEncoding("ISO-8859-1");
        //    foreach (var bane in stevneInfo.DynamiskeBaner)
        //    {
        //        if (this.Info == null)
        //        {
        //            continue;
        //        }

        //        var rapportDir = Path.Combine(this.Info.FullName, RapportDirName);
        //        string prefix = null;
        //        string filenamesorthtml = null;
        //        string prefixhtml = null;
        //        switch (bane.BaneType)
        //        {
        //            case BaneType.MinneFin:
        //                prefix = "Minneskyting-Oppropsliste";
        //                filenamesorthtml = "Minneskyting-Oppropsliste*.html";
        //                prefixhtml = "Minneskyting-Oppropsliste";
        //                break;
        //        }
        //        if (string.IsNullOrEmpty(filenamesorthtml))
        //        {
        //            continue;
        //        }

        //        if (!Directory.Exists(rapportDir))
        //        {
        //            Log.Trace("Dir for lagskyting not found {0}", rapportDir);
        //            continue;
        //        }

        //        var rapportInfo = new DirectoryInfo(rapportDir);
        //        var listopprophtml = rapportInfo.GetFiles(filenamesorthtml);
        //        if (listopprophtml.Length > 0)
        //        {
        //            foreach (var file in listopprophtml)
        //            {
        //                ParseOppropMinneHtml(bane, file.FullName);
        //            }
        //        }
        //    }
        //}

        //public void ParseOppropMinneHtml(StartListBane bane, string indfile)
        //{
        //    try
        //    {
        //        string inputString;
        //        Encoding enc = Encoding.GetEncoding("ISO-8859-1");
        //        using (StreamReader read = new StreamReader(indfile, enc))
        //        {
        //            inputString = read.ReadToEnd();
        //        }

        //        var directoryName = Path.GetDirectoryName(indfile);
        //        List<string> topListeFiler = new List<string>();
        //        StartingListStevne stevneInfo = null;
        //        var test = new HtmlAgilityPack.HtmlDocument();
        //        test.LoadHtml(inputString);
        //        var nodehead = test.DocumentNode.SelectSingleNode("/html/body/table");
        //        if (nodehead == null)
        //        {
        //            return;
        //        }
        //        int lagno = -1;
        //        var nodebodyTable = nodehead.SelectNodes("//td");
        //        foreach (var body in nodebodyTable)
        //        {
        //            if (body.InnerText.ToUpper().Contains("LAG"))
        //            {
        //                string text = body.InnerText.Trim();
        //                var list = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //                if (list.Length > 1)
        //                {
        //                    if (!Int32.TryParse(list[1], out lagno))
        //                    {
        //                        return;
        //                    }
        //                    break;
        //                }
        //            }
        //        }

        //        if (lagno <= 0)
        //        {
        //            return;
        //        }

        //        var nodelistTable = test.DocumentNode.SelectSingleNode("/html/body/table[@border='1']");
        //        if (nodelistTable == null)
        //        {
        //            return;
        //        }
        //        var nodelistSkytter = nodelistTable.SelectNodes("//tr");
        //        if (nodelistSkytter == null)
        //        {
        //            return;
        //        }

        //        int cout = 0;
        //        foreach (var skytter in nodelistSkytter)
        //        {
        //            if (cout <= 1)
        //            {
        //                cout++;
        //                continue;
        //            }
        //            var valnodes = skytter.SelectNodes("//td");
        //            if (valnodes.Count > 4)
        //            {
        //                var name = valnodes[1];
        //            }
        //        }

        //        //if (nodebody2 != null)
        //        //{
        //        //    foreach (var node in nodebody2)
        //        //    {
        //        //        var h3 = node.SelectNodes("//h3");
        //        //        bool found = false;
        //        //        if (h3 != null)
        //        //        {
        //        //            HtmlNode nodeinfoH3 = null;
        //        //            foreach (var nodeinfo in h3)
        //        //            {
        //        //                if (nodeinfo != null && !string.IsNullOrEmpty(nodeinfo.InnerText))
        //        //                {
        //        //                    if (nodeinfo.InnerText.ToUpper().Contains("LAGVIS"))
        //        //                    {
        //        //                        if (stevneInfo == null)
        //        //                        {
        //        //                            stevneInfo = new StartingListStevne();
        //        //                            stevneInfo.ParseStevneNavn(nodehead.InnerText);
        //        //                        }

        //        //                        BaneType baneType = StartListBane.FindBaneType(nodeinfo.InnerText);
        //        //                        if (baneType == BaneType.Undefined)
        //        //                        {
        //        //                            Log.Error("Uknown Banetype {0} fil ={1}", nodeinfo.InnerText, indfile);
        //        //                            return null;
        //        //                        }

        //        //                        var bane = stevneInfo.FinnBane(baneType);

        //        //                        if (bane == null)
        //        //                        {
        //        //                            bane = stevneInfo.AddNewBane(stevneInfo.StevneNavn, directoryName, baneType);
        //        //                            bane.ReportDirStevneNavn = stevneInfo.ReportDirStevneNavn;
        //        //                        }
        //        //                        else
        //        //                        {
        //        //                            bane.StevneLag.Clear();
        //        //                        }

        //        //                        nodeinfoH3 = nodeinfo;
        //        //                        GetLagvisOpprop(nodeinfoH3, bane);
        //        //                    }
        //        //                    else if (nodeinfo.InnerText.ToUpper().Contains("MINNESKYTING"))
        //        //                    {
        //        //                        if (stevneInfo == null)
        //        //                        {
        //        //                            stevneInfo = new StartingListStevne();
        //        //                            stevneInfo.ParseStevneNavn(nodehead.InnerText);
        //        //                        }
        //        //                        bool breakFound = false;
        //        //                        string klasse = string.Empty;
        //        //                        var sibling = nodeinfo.NextSibling;
        //        //                        while (!breakFound)
        //        //                        {
        //        //                            sibling = sibling.NextSibling;
        //        //                            if (sibling.NodeType == HtmlNodeType.Element && sibling.InnerText.ToUpper().Contains("KL."))
        //        //                            {
        //        //                                var typenode = sibling.SelectSingleNode("div/a");
        //        //                                if (typenode != null)
        //        //                                {
        //        //                                    var tostr = typenode.InnerText;
        //        //                                    if (!string.IsNullOrEmpty(tostr))
        //        //                                    {
        //        //                                        tostr = tostr.ToUpper();
        //        //                                        var splits = tostr.Split(new string[] { "KL." }, StringSplitOptions.RemoveEmptyEntries);
        //        //                                        if (splits.Length >= 2)
        //        //                                        {
        //        //                                            klasse = splits[1].Trim();
        //        //                                        }
        //        //                                    }
        //        //                                }
        //        //                                breakFound = true;
        //        //                            }

        //        //                            if (sibling == null)
        //        //                            {
        //        //                                breakFound = true;
        //        //                            }
        //        //                        }

        //        //                        BaneType baneType = StartListBane.FindMinneBaneTypeForKlasse(klasse);
        //        //                        if (baneType == BaneType.Undefined)
        //        //                        {
        //        //                            Log.Error("Uknown Banetype {0} fil ={1}", nodeinfo.InnerText, indfile);
        //        //                            return null;
        //        //                        }

        //        //                        var bane = stevneInfo.FinnBane(baneType);

        //        //                        if (bane == null)
        //        //                        {
        //        //                            bane = stevneInfo.AddNewBane(stevneInfo.StevneNavn, directoryName, baneType);
        //        //                            bane.ReportDirStevneNavn = stevneInfo.ReportDirStevneNavn;
        //        //                        }
        //        //                        else
        //        //                        {
        //        //                            bane.StevneLag.Clear();
        //        //                        }

        //        //                        nodeinfoH3 = nodeinfo;
        //        //                        GetLagvisOpprop(nodeinfoH3, bane);
        //        //                    }
        //        //                }
        //        //            }

        //        //        }
        //        //    }
        //        //}

        //        //if (nodebody2 != null && stevneInfo != null)
        //        //{
        //        //    foreach (var node in nodebody2)
        //        //    {
        //        //        var h3 = node.SelectNodes("//h3");
        //        //        bool found = false;
        //        //        if (h3 != null)
        //        //        {
        //        //            HtmlNode nodeinfoH3 = null;
        //        //            foreach (var nodeinfo in h3)
        //        //            {
        //        //                if (nodeinfo != null && !string.IsNullOrEmpty(nodeinfo.InnerText))
        //        //                {
        //        //                    var bane = stevneInfo.FinnBane(BaneType.Femtenmeter);
        //        //                    if (nodeinfo.InnerText.ToUpper() == "15M" && bane != null)
        //        //                    {
        //        //                        GetReportFilesForBane(nodeinfo, bane);
        //        //                    }
        //        //                    if (nodeinfo.InnerText.ToUpper() == "RANGERING" && bane != null)
        //        //                    {
        //        //                        GetReportFilesForBane(nodeinfo, bane);
        //        //                    }

        //        //                    bane = stevneInfo.FinnBane(BaneType.Hundremeter);
        //        //                    if (nodeinfo.InnerText.ToUpper() == "100M" && bane != null)
        //        //                    {
        //        //                        GetReportFilesForBane(nodeinfo, bane);
        //        //                    }

        //        //                    bane = stevneInfo.FinnBane(BaneType.Tohundremeter);
        //        //                    if (nodeinfo.InnerText.ToUpper() == "200M" && bane != null)
        //        //                    {
        //        //                        GetReportFilesForBane(nodeinfo, bane);
        //        //                    }

        //        //                    bane = stevneInfo.FinnBane(BaneType.FinFelt);
        //        //                    if (nodeinfo.InnerText.ToUpper() == "FINFELT" && bane != null)
        //        //                    {
        //        //                        GetReportFilesForFelt(nodeinfo, bane);
        //        //                    }

        //        //                    bane = stevneInfo.FinnBane(BaneType.GrovFelt);
        //        //                    if (nodeinfo.InnerText.ToUpper() == "GROVFELT" && bane != null)
        //        //                    {
        //        //                        GetReportFilesForFelt(nodeinfo, bane);
        //        //                    }

        //        //                    bane = stevneInfo.FinnBane(BaneType.MinneFin);
        //        //                    if (nodeinfo.InnerText.ToUpper() == "MINNESKYTING" && bane != null)
        //        //                    {
        //        //                        GetReportFilesForMinne(nodeinfo, bane);
        //        //                    }

        //        //                    bane = stevneInfo.FinnBane(BaneType.MinneGrov);
        //        //                    if (nodeinfo.InnerText.ToUpper() == "MINNESKYTING" && bane != null)
        //        //                    {
        //        //                        GetReportFilesForMinne(nodeinfo, bane);
        //        //                    }

        //        //                }
        //        //            }
        //        //        }
        //        //    }
        //        //}

        //        //GetLagSkytingOpprop(stevneInfo);
        //        //GetLagOpprop(stevneInfo);

        //        //return stevneInfo;
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(e, string.Format("error parsing {0}", indfile));
        //    }

        //    return;
        //}

        private void GetLagSkytingOpprop(StartingListStevne stevneInfo)
        {
            if (stevneInfo == null)
            {
                return;
            }

            if (stevneInfo.DynamiskeBaner == null)
            {
                return;
            }
            Encoding enc = Encoding.GetEncoding("ISO-8859-1");
            foreach (var bane in stevneInfo.DynamiskeBaner)
            {
                if (this.Info == null)
                {
                    continue;
                }

                var rapportDir = Path.Combine(this.Info.FullName, RapportDirName);
                string filenamesort = null;
                string prefix = null;
                string filenamesorthtml = null;
                string prefixhtml = null;
                switch (bane.BaneType)
                {
                    case BaneType.Hundremeter:
                        filenamesort = "100m-Lagskyting-opprop-*.csv";
                        prefix = "100m-Lagskyting-opprop-";
                        filenamesorthtml = "100m-Lagskyting-opprop-*.html";
                        prefixhtml = "100m-Lagskyting-opprop-";
                        break;
                    case BaneType.Tohundremeter:
                        filenamesort = "200-300m-Lagskyting-opprop-*.csv";
                        prefix = "200-300m-Lagskyting-opprop-";
                        filenamesorthtml = "200-300m-Lagskyting-opprop-*.html";
                        prefixhtml = "200-300m-Lagskyting-opprop-";
                        break;
                }
                if (string.IsNullOrEmpty(filenamesort) && string.IsNullOrEmpty(filenamesorthtml))
                {
                    continue;
                }

                if (!Directory.Exists(rapportDir))
                {
                    Log.Trace("Dir for lagskyting not found {0}", rapportDir);
                    continue;
                }

                var rapportInfo = new DirectoryInfo(rapportDir);
                var listLagskytinghtml = rapportInfo.GetFiles(filenamesorthtml);
                if (listLagskytinghtml.Length > 0)
                {
                    foreach (var file in listLagskytinghtml)
                    {
                        var filename = Path.GetFileNameWithoutExtension(file.Name);
                        var start = filename.Replace(prefixhtml, string.Empty);
                        int lagNr = -1;
                        if (int.TryParse(start, out lagNr))
                        {
                            ParseLagSkytingHtml(bane, bane.BaneType, lagNr, file.FullName);
                        }
                    }
                }
                else
                {
                    var listLagskyting = rapportInfo.GetFiles(filenamesort);
                    if (listLagskyting.Length > 0)
                    {
                        foreach (var file in listLagskyting)
                        {
                            string text = System.IO.File.ReadAllText(file.FullName, enc);
                            if (!string.IsNullOrEmpty(text))
                            {
                                var filename = Path.GetFileNameWithoutExtension(file.Name);
                                var start = filename.Replace(prefix, string.Empty);
                                int lagNr = -1;
                                if (int.TryParse(start, out lagNr))
                                {
                                    ParseLagSkyting(bane, file.LastWriteTime, bane.BaneType, lagNr, text);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ParseLagSkytingHtml(StartListBane bane, BaneType baneType, int lagNr, string lagfile)
        {
            try
            {
                string inputString;
                Encoding enc = Encoding.GetEncoding("ISO-8859-1");
                using (StreamReader read = new StreamReader(lagfile, enc))
                {
                    inputString = read.ReadToEnd();
                }

                var test = new HtmlAgilityPack.HtmlDocument();
                test.LoadHtml(inputString);
                var nodeopproptid = test.DocumentNode.SelectSingleNode("/html/body/table[@border='0']/tr/td[2]/font/b");
                if (nodeopproptid == null)
                {
                    Log.Error("nodeopproptid null for html lagskyting {0}", lagfile);
                    return;
                }

                DateTime? oppropTimevar = StartingListLag.ParseStartDateLagSkyting(bane.StevneNavn, nodeopproptid.InnerText);
                if (oppropTimevar == null)
                {
                    Log.Error("oppropTime null for html lagskyting {0}", lagfile);
                    return;
                }

                DateTime oppropTime = oppropTimevar.Value;
                var nodebodySKyttere = test.DocumentNode.SelectNodes("/html/body/table[@border='1']/tr");
                var lagskytingLag = new StartingListLag(lagNr + 100) { ProgramType = ProgramType.Lagskyting };

                lagskytingLag.StartTime = new DateTime(oppropTime.Year, oppropTime.Month, oppropTime.Day, oppropTime.Hour, oppropTime.Minute, 0);
                lagskytingLag.StevneNavn = bane.StevneNavn;
                lagskytingLag.BaneType = baneType;

                if (nodebodySKyttere != null)
                {
                    int count = 0;
                    foreach (var node in nodebodySKyttere)
                    {
                        count++;
                        if (count == 1)
                        {
                            continue;
                        }

                        var skytternode = node.SelectNodes("td");
                        if (skytternode.Count >= 4)
                        {
                            if (!string.IsNullOrEmpty(skytternode[1].InnerText))
                            {
                                int skivenr;
                                string skiveStr = skytternode[0].InnerText.Replace('.', ' ').Trim();
                                if (int.TryParse(skiveStr, out skivenr))
                                {
                                    var skive = new StartingListSkive
                                                    {
                                                        LagSkiveNr = skivenr,
                                                        SkytterNavn = skytternode[1].InnerText,
                                                        SkytterLag = skytternode[2].InnerText,
                                                        Klasse = skytternode[3].InnerText
                                                    };
                                    lagskytingLag.Skiver.Add(skive);
                                }
                            }
                        }
                    }
                }

                if (lagskytingLag.Skiver.Count > 0)
                {
                    bane.StevneLag.Add(lagskytingLag);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, string.Format("error parsing {0}", lagfile));
            }

            return;
        }

        private void ParseLagSkyting(StartListBane bane, DateTime oppropTime, BaneType banetype, int lagnr, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var lines = text.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            var lagskytingLag = new StartingListLag(lagnr + 100) { ProgramType = ProgramType.Lagskyting };
            lagskytingLag.StartTime = new DateTime(oppropTime.Year, oppropTime.Month, oppropTime.Day, oppropTime.Hour, oppropTime.Minute, 0);
            lagskytingLag.StevneNavn = bane.StevneNavn;
            lagskytingLag.BaneType = banetype;
            foreach (var line in lines)
            {
                var part = line.Split(new[] { ';' });
                int skivenr = -1;
                if (int.TryParse(part[0], out skivenr))
                {
                    if (part.Length >= 4)
                    {
                        var skive = new StartingListSkive { LagSkiveNr = skivenr, SkytterNavn = part[2], SkytterLag = part[3], Klasse = part[4] };
                        lagskytingLag.Skiver.Add(skive);
                    }
                }
            }

            if (lagskytingLag.Skiver.Count > 0)
            {
                bane.StevneLag.Add(lagskytingLag);
            }
        }

        #endregion
    }
}