// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LeonDirInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The leon dir info.
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
    using System.Xml.Serialization;
    using System.Xml.Xsl;

    using FileUploaderService.Orion;

    using HtmlAgilityPack;

    using SendingResults.Diagnosis;

    /// <summary>
    /// The leon dir info.
    /// </summary>
    public class LeonDirInfo : IEquatable<LeonDirInfo>
    {
        #region Constants

        /// <summary>
        /// The bit map dir name.
        /// </summary>
        public const string BitMapDirName = "BitMap";

        /// <summary>
        /// The rapport dir name.
        /// </summary>
        public const string RapportDirName = "Rapport";

        /// <summary>
        /// The web dir name.
        /// </summary>
        public const string WebDirName = "Web";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LeonDirInfo"/> class.
        /// </summary>
        public LeonDirInfo()
        {
            this.Command = UploadCommand.none;
            BitmapsStoredInStevne = new List<BitmapDirInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LeonDirInfo"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        public LeonDirInfo(DirectoryInfo info)
        {
            this.Info = info;
            this.TargetName = info.Name;
            this.Command = UploadCommand.none;
            BitmapsStoredInStevne = new List<BitmapDirInfo>();
        }

        #endregion

        #region Public Properties


        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public UploadCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the info.
        /// </summary>
        public DirectoryInfo Info { get; set; }

        /// <summary>
        /// Gets or sets the last create 100 m bitmap file.
        /// </summary>
        public DateTime? LastCreate100mBitmapFile { get; set; }

        /// <summary>
        /// Gets or sets the last create 15 m bitmap file.
        /// </summary>
        public DateTime? LastCreate15mBitmapFile { get; set; }

        /// <summary>
        /// Gets or sets the last create 200 m bitmap file.
        /// </summary>
        public DateTime? LastCreate200mBitmapFile { get; set; }

        /// <summary>
        /// Gets or sets the last written 100 m bitmap file.
        /// </summary>
        public DateTime? LastWritten100mBitmapFile { get; set; }

        /// <summary>
        /// Gets or sets the last written 15 m bitmap file.
        /// </summary>
        public DateTime? LastWritten15mBitmapFile { get; set; }

        /// <summary>
        /// Gets or sets the last written 200 m bitmap file.
        /// </summary>
        public DateTime? LastWritten200mBitmapFile { get; set; }

        /// <summary>
        /// Gets or sets the last written pdf file.
        /// </summary>
        public DateTime? LastWrittenPdfFile { get; set; }

        /// <summary>
        /// Gets or sets the last written starting file.
        /// </summary>
        public DateTime? LastWrittenStartingFile { get; set; }

        /// <summary>
        /// Gets or sets the last written web file.
        /// </summary>
        public DateTime? LastWrittenWebFile { get; set; }

        /// <summary>
        /// Gets or sets the pdf file name.
        /// </summary>
        public string PdfFileName { get; set; }

        /// <summary>
        /// Gets or sets the start list file name.
        /// </summary>
        public string StartListFileName { get; set; }

        /// <summary>
        /// Gets or sets the stevne for alle baner.
        /// </summary>
        public StartingListStevne StevneForAlleBaner { get; set; }

        /// <summary>
        /// Gets or sets the stevne info.
        /// </summary>
        public StartingListStevne StevneInfo { get; set; }


        public List<BitmapDirInfo> BitmapsStoredInStevne { get; set; }

        /// <summary>
        /// Gets or sets the target name.
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// Gets or sets the web name.
        /// </summary>
        public string WebName { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
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

        #region Methods

        /// <summary>
        /// The check bit map list.
        /// </summary>
        /// <param name="OppropslisteMal">
        /// The oppropsliste mal.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool CheckBitMapList(HtmlDocument OppropslisteMal)
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
                BitmapDirInfo[] bitmapScan = new BitmapDirInfo[3];
                bitmapScan[0] = new BitmapDirInfo() { StevneType = StevneType.Femtenmeter };
                bitmapScan[1] = new BitmapDirInfo() { StevneType = StevneType.Hundremeter };
                bitmapScan[2] = new BitmapDirInfo() { StevneType = StevneType.Tohundremeter };

                bitmapScan[0].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix15m);
                bitmapScan[1].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix100m);
                bitmapScan[2].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix200m);

                foreach (var dir2Check in bitmapScan)
                {
                    dir2Check.Updated = false;
                    var sortlist = dir2Check.BitmapFiles.OrderByDescending(x => x.LastWriteTime);
                    if (sortlist.FirstOrDefault() != null)
                    {
                        switch (dir2Check.StevneType)
                        {
                            case StevneType.Femtenmeter:
                                if (this.LastWritten15mBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    this.LastWritten15mBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                            case StevneType.Hundremeter:
                                if (this.LastWritten100mBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    this.LastWritten100mBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                            case StevneType.Tohundremeter:
                                if (this.LastWritten200mBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    this.LastWritten200mBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                        }
                    }

                    var sortlistCreat = dir2Check.BitmapFiles.OrderByDescending(x => x.CreationTime);
                    if (sortlistCreat.FirstOrDefault() != null)
                    {
                        switch (dir2Check.StevneType)
                        {
                            case StevneType.Femtenmeter:
                                if (this.LastWritten15mBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    this.LastWritten15mBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                            case StevneType.Hundremeter:
                                if (this.LastWritten100mBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    this.LastWritten100mBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                            case StevneType.Tohundremeter:
                                if (this.LastWritten200mBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    this.LastWritten200mBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
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

                if (!update)
                {
                    if (this.StevneInfo != null)
                    {
                        if (this.CheckDeletedBitmaps(bitmapScan, bitMapDir))
                        {
                            return true;
                        }
                    }

                    return false;
                }

                foreach (var dirs in bitmapScan)
                {
                    if (!dirs.Updated)
                    {
                        continue;
                    }

                    List<StartingListLag> lagliste;
                    string prefix;
                    switch (dirs.StevneType)
                    {
                        case StevneType.Femtenmeter:
                            lagliste = this.StevneInfo.StevneLag15m;
                            prefix = Constants.Prefix15m;
                            break;
                        case StevneType.Hundremeter:
                            lagliste = this.StevneInfo.StevneLag100m;
                            prefix = Constants.Prefix100m;
                            break;
                        case StevneType.Tohundremeter:
                            lagliste = this.StevneInfo.StevneLag200m;
                            prefix = Constants.Prefix200m;
                            break;
                        default:
                            Log.Error("Wrong stevnetype");
                            return false;
                    }

                    Log.Info("Updated Bitmap detected {0} {1}", this.StevneInfo.StevneNavn, prefix);
                    foreach (var bitmapList in dirs.BitmapFiles)
                    {
                        try
                        {
                            var orionInf = new OrionFileInfo(bitmapList);
                            if (!orionInf.ParseTarget())
                            {
                                continue;
                            }

                            if (this.StevneInfo == null)
                            {
                                this.StevneInfo = new StartingListStevne();
                                this.StevneInfo.StevneNavn = stevneNavn;
                            }

                            var lagInfo = this.FindLag(dirs.StevneType, this.StevneInfo, orionInf.Lag);

                            if (lagInfo == null)
                            {
                                lagInfo = new StartingListLag(orionInf.Lag) { StevneNavn = stevneNavn, StevneType = dirs.StevneType };
                                this.AddLag(this.StevneInfo, lagInfo);
                            }

                            StartingListSkive skive = this.FinnSkive(lagInfo, orionInf.Skive);
                            if (skive == null)
                            {
                                Log.Info(
                                    "Found new bitmapskive for {0} lag={1} skive={2} navn={3}",
                                    prefix,
                                    orionInf.Lag,
                                    orionInf.Skive,
                                    orionInf.FileInfo.FullName);
                                skive = new StartingListSkive() { SkiveNr = orionInf.Skive, SkytterNavn = "-", SkytterLag = "-", Klasse = "-" };
                                skive.BackUpBitMapFile = bitmapList;
                                lagInfo.Skiver.Add(skive);
                                lagInfo.SortSkiver();
                            }
                            else if (skive.BackUpBitMapFile == null)
                            {
                                skive.BackUpBitMapFile = bitmapList;
                            }
                            else if (skive.BackUpBitMapFile.CreationTime < bitmapList.CreationTime)
                            {
                                Log.Info(
                                    "Found updated bitmapskive for {0} lag={1} skive={2} navn={3} {4} {5}",
                                    prefix,
                                    orionInf.Lag,
                                    orionInf.Skive,
                                    orionInf.FileInfo.FullName,
                                    skive.BackUpBitMapFile.CreationTime,
                                    bitmapList.CreationTime);
                            }
                            else if (skive.BackUpBitMapFile.LastWriteTime < bitmapList.LastWriteTime)
                            {
                                Log.Info(
                                    "Found updated bitmapskive for {0} lag={1} skive={2} navn={3} {4} {5}",
                                    prefix,
                                    orionInf.Lag,
                                    orionInf.Skive,
                                    orionInf.FileInfo.FullName,
                                    skive.BackUpBitMapFile.LastWriteTime,
                                    bitmapList.LastWriteTime);
                            }
                            else
                            {
                                continue;
                            }

                            if (lagInfo.Oppropsliste == null)
                            {
                                lagInfo.Oppropsliste = this.GenerateOppropListe(OppropslisteMal, stevneNavn, orionInf.Lag);
                            }

                            if (this.UpdateHrefForBitmap(lagInfo.Oppropsliste, bitmapList.Name, skive, lagInfo.StevneType))
                            {
                                Log.Info("Updated Oppropsliste for lag={0} skive={1}", orionInf.Lag, orionInf.Skive);
                                skive.BackUpBitMapFile = bitmapList;
                                skive.Updated = true;
                                Encoding encOut = Encoding.GetEncoding("ISO-8859-1");
                                if (lagInfo.OppropslisteFileInfo == null)
                                {
                                    var filename = Path.Combine(bitMapDir, string.Format("{0}-Oppropsliste-{1}.html", prefix, orionInf.Lag));
                                    lagInfo.Oppropsliste.Save(filename, encOut);
                                    lagInfo.OppropslisteFileInfo = new FileInfo(filename);
                                    this.LastWrittenStartingFile = lagInfo.OppropslisteFileInfo.LastWriteTime;
                                }
                                else
                                {
                                    lagInfo.Oppropsliste.Save(lagInfo.OppropslisteFileInfo.FullName, encOut);
                                    lagInfo.OppropslisteFileInfo = new FileInfo(lagInfo.OppropslisteFileInfo.FullName);
                                    this.LastWrittenStartingFile = lagInfo.OppropslisteFileInfo.LastWriteTime;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "Error");
                        }
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// The check pdf files.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
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
                            this.LastWrittenPdfFile = list.FirstOrDefault().LastWriteTime;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// The check rapporter.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
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
                BitmapDirInfo[] bitmapScan = new BitmapDirInfo[3];
                bitmapScan[0] = new BitmapDirInfo() { StevneType = StevneType.Femtenmeter };
                bitmapScan[1] = new BitmapDirInfo() { StevneType = StevneType.Hundremeter };
                bitmapScan[2] = new BitmapDirInfo() { StevneType = StevneType.Tohundremeter };

                bitmapScan[0].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix15m);
                bitmapScan[1].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix100m);
                bitmapScan[2].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix200m);

                foreach (var dir2Check in bitmapScan)
                {
                    dir2Check.Updated = false;
                    var sortlist = dir2Check.BitmapFiles.OrderByDescending(x => x.LastWriteTime);
                    if (sortlist.FirstOrDefault() != null)
                    {
                        switch (dir2Check.StevneType)
                        {
                            case StevneType.Femtenmeter:
                                if (this.LastWritten15mBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    this.LastWritten15mBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                            case StevneType.Hundremeter:
                                if (this.LastWritten100mBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    this.LastWritten100mBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                            case StevneType.Tohundremeter:
                                if (this.LastWritten200mBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    this.LastWritten200mBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                        }
                    }

                    var sortlistCreat = dir2Check.BitmapFiles.OrderByDescending(x => x.CreationTime);
                    if (sortlistCreat.FirstOrDefault() != null)
                    {
                        switch (dir2Check.StevneType)
                        {
                            case StevneType.Femtenmeter:
                                if (this.LastWritten15mBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    this.LastWritten15mBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                            case StevneType.Hundremeter:
                                if (this.LastWritten100mBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    this.LastWritten100mBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                    dir2Check.Updated = true;
                                }

                                break;
                            case StevneType.Tohundremeter:
                                if (this.LastWritten200mBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    this.LastWritten200mBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
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

                XmlSerializer serBitInfo = new XmlSerializer(typeof(BitmapDirInfo));
                foreach (var stevne in this.StevneForAlleBaner.DynamiskeStevner)
                {
                    if (stevne.Rapporter.Count > 0)
                    {
                        foreach (var rapport in stevne.Rapporter)
                        {
                            BitmapDirInfo bitmaps = null;
                            switch (rapport.StevneType)
                            {
                                case StevneType.Femtenmeter:
                                    bitmaps = bitmapScan[0];
                                    bitmaps.BitmapSubDir = Constants.Prefix15m;
                                    break;
                                case StevneType.Hundremeter:
                                    bitmaps = bitmapScan[1];
                                    bitmaps.BitmapSubDir = Constants.Prefix100m;
                                    break;
                                case StevneType.Tohundremeter:
                                    bitmaps = bitmapScan[2];
                                    bitmaps.BitmapSubDir = Constants.Prefix200m;
                                    break;
                            }

                            XmlDocument bitMapDoc;
                            if (bitmaps != null)
                            {
                                var memStevne = new MemoryStream();
                                XmlTextWriter write = new XmlTextWriter(memStevne, new UTF8Encoding(false));
                                bitmaps.InitAllNames();
                                serBitInfo.Serialize(write, bitmaps);
                                write.Flush();
                                memStevne.Position = 0;
                                bitMapDoc = new XmlDocument();
                                bitMapDoc.Load(memStevne);
                                rapport.BitMapInfo = bitMapDoc;
                            }
                        }
                    }
                }

                return update;
            }

            return false;
        }

        /// <summary>
        /// The check starting list.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool CheckStartingList()
        {
            bool update = false;
            if (this.Info == null)
            {
                return false;
            }

            if (this.LastWrittenStartingFile == null)
            {
                this.LastWrittenStartingFile = DateTime.MinValue;
            }

            var rapportDir = Path.Combine(this.Info.FullName, RapportDirName);

            if (Directory.Exists(rapportDir))
            {
                var webInfo = new DirectoryInfo(rapportDir);
                var listInfo = webInfo.GetFiles("*Oppropsliste*.html");
                if (listInfo.Length > 0)
                {
                    var list = listInfo.OrderByDescending(x => x.LastWriteTime);
                    if (list.FirstOrDefault() != null)
                    {
                        if (this.LastWrittenStartingFile < list.FirstOrDefault().LastWriteTime)
                        {
                            this.LastWrittenStartingFile = list.FirstOrDefault().LastWriteTime;
                            this.StartListFileName = list.FirstOrDefault().FullName;
                            update = true;
                        }
                    }

                    if (!update)
                    {
                        return false;
                    }

                    Log.Info("Updated Starting list detected {0} {0}", this.StartListFileName, this.LastWrittenStartingFile);

                    // Check to see ifit is a real update;
                    update = false;
                    foreach (var startList in list)
                    {
                        try
                        {
                            string inputString;
                            Encoding enc = Encoding.GetEncoding("ISO-8859-1");
                            using (StreamReader read = new StreamReader(startList.FullName, enc))
                            {
                                inputString = read.ReadToEnd();
                            }

                            if (this.StevneInfo == null || string.IsNullOrEmpty(this.StevneInfo.StevneNavn))
                            {
                                this.StevneInfo = new StartingListStevne();
                            }

                            var stevneType = this.GetStevneType(startList.Name);

                            var test = new HtmlAgilityPack.HtmlDocument();
                            test.LoadHtml(inputString);
                            int lagnr = this.ParseLagnr(startList.Name, stevneType);
                            if (lagnr > 0)
                            {
                                var newlagInfo = this.ParseOutStartInfo(test, lagnr);
                                newlagInfo.Oppropsliste = test;
                                newlagInfo.LastUpdatedOppropslisteFileInfo = startList.LastWriteTime;
                                newlagInfo.StevneType = stevneType;
                                StartingListLag lagInfo;
                                if (string.IsNullOrEmpty(this.StevneInfo.StevneNavn))
                                {
                                    this.StevneInfo.StevneNavn = newlagInfo.StevneNavn;
                                    lagInfo = this.FindLag(stevneType, this.StevneInfo, lagnr);
                                    update = true;
                                }
                                else
                                {
                                    if (string.Compare(this.StevneInfo.StevneNavn, newlagInfo.StevneNavn, StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        lagInfo = this.FindLag(stevneType, this.StevneInfo, lagnr);
                                    }
                                    else
                                    {
                                        Log.Warning(
                                            "Mismatch in stevneNavn {0} {1} {3}",
                                            this.StevneInfo.StevneNavn,
                                            newlagInfo.StevneNavn,
                                            startList.FullName);
                                        continue;
                                    }
                                }

                                if (lagInfo == null)
                                {
                                    Log.Info("New Lag detected ={0}", lagnr);
                                    lagInfo = newlagInfo;
                                    this.AddLag(this.StevneInfo, lagInfo);
                                    lagInfo.Oppropsliste = newlagInfo.Oppropsliste;
                                    lagInfo.UpdatedOppropsliste = true;
                                    update = true;
                                }
                                else
                                {
                                    if (lagInfo.LastUpdatedOppropslisteFileInfo < startList.LastWriteTime)
                                    {
                                        Log.Info("Updated oppropsliste Found");
                                        lagInfo.LastUpdatedOppropslisteFileInfo = startList.LastWriteTime;
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                    newlagInfo.SortSkiver();
                                    lagInfo.SetNotCheckOnAllSkiver();
                                    lagInfo.UpdatedOppropsliste = true;
                                    foreach (var newskiveInfo in newlagInfo.Skiver)
                                    {
                                        var skive = this.FinnSkive(lagInfo, newskiveInfo.SkiveNr);
                                        if (skive.UpdatedSkive(newskiveInfo))
                                        {
                                            this.UpdateHrefForBitmap(lagInfo.Oppropsliste, null, skive, stevneType);
                                            Log.Info("Skive nr ={0} updated", newskiveInfo.SkiveNr);
                                            update = true;
                                            skive.Updated = true;
                                        }

                                        skive.Checked = true;
                                    }

                                    var notset = lagInfo.Skiver.Where(x => x.Checked == false).ToList();
                                    if (notset.Count > 0)
                                    {
                                        Log.Error("Not all skiver exsist in new opprop {0}", startList.FullName);
                                    }
                                }

                                var bitMapDir = Path.Combine(this.Info.FullName, BitMapDirName);
                                if (!Directory.Exists(bitMapDir))
                                {
                                    Directory.CreateDirectory(bitMapDir);
                                }

                                if (update)
                                {
                                    string prefix = string.Empty;
                                    switch (stevneType)
                                    {
                                        case StevneType.Femtenmeter:
                                            prefix = Constants.Prefix15m;
                                            break;
                                        case StevneType.Hundremeter:
                                            prefix = Constants.Prefix100m;
                                            break;
                                        case StevneType.Tohundremeter:
                                            prefix = Constants.Prefix200m;
                                            break;
                                    }

                                    Encoding encOut = Encoding.GetEncoding("ISO-8859-1");
                                    var filename = Path.Combine(bitMapDir, string.Format("{0}-Oppropsliste-{1}.html", prefix, lagnr));
                                    lagInfo.Oppropsliste.Save(filename, encOut);
                                    lagInfo.GravlapplisteFileInfo = new FileInfo(filename);

                                    Log.Info("Updated Oppropsliste for lag={0} stevne={1} {2}", lagnr, this.StevneInfo.StevneNavn, filename);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "Error");
                        }
                    }
                }
            }

            return update;
        }

        /// <summary>
        /// The check updated starting list.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool CheckUpdatedStartingList()
        {
            if (this.StevneInfo != null)
            {
                foreach (var Lag in this.StevneInfo.StevneLag15m)
                {
                    if (Lag.UpdatedOppropsliste)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// The check web files.
        /// </summary>
        /// <param name="opprop15mPrefix">
        /// The opprop 15 m prefix.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool CheckWebFiles(string opprop15mPrefix = null)
        {
            if (this.Info == null)
            {
                return false;
            }

            if (this.LastWrittenWebFile == null)
            {
                this.LastWrittenWebFile = DateTime.MinValue;
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
                    if (list.FirstOrDefault() != null)
                    {
                        if (this.LastWrittenWebFile < list.FirstOrDefault().LastWriteTime)
                        {
                            this.LastWrittenWebFile = list.FirstOrDefault().LastWriteTime;
                            update = true;
                        }
                    }

                    if (update == false)
                    {
                        return false;
                    }

                    Log.Info("Parsing new index File {0}", webInfo.FullName);
                    var indfile = Path.Combine(webInfo.FullName, "index1.html");

                    if (File.Exists(indfile))
                    {
                        StartingListStevne st = this.ParseStevneInfo(indfile);
                        if (st != null)
                        {
                            this.StevneForAlleBaner = st;
                        }
                    }

                    if (this.StevneForAlleBaner != null)
                    {
                        var listopprop = webInfo.GetFiles("*.xml");
                        if (listopprop.Length > 0)
                        {
                            XmlSerializer ser = new XmlSerializer(typeof(StartingListStevne));
                            foreach (var baneStevne in this.StevneForAlleBaner.DynamiskeStevner)
                            {
                                baneStevne.Rapporter.Clear();
                                var memStevne = new MemoryStream();
                                XmlTextWriter write = new XmlTextWriter(memStevne, new UTF8Encoding(false));
                                ser.Serialize(write, baneStevne);
                                write.Flush();
                                memStevne.Position = 0;
                                XmlDocument doc = new XmlDocument();
                                doc.Load(memStevne);

                                foreach (var lagInfo in baneStevne.StevneLag)
                                {
                                    if (lagInfo.XmlOppropsListe.Count > 0)
                                    {
                                        foreach (var rapportFileNavn in lagInfo.XmlOppropsListe)
                                        {
                                            var filenavn = string.Format("{0}.xml", rapportFileNavn);
                                            var funnetRapportFil = listopprop.FirstOrDefault(x => x.Name == filenavn);
                                            if (funnetRapportFil != null)
                                            {
                                                RapportXmlClass nyRapport = new RapportXmlClass();
                                                nyRapport.Filnavn = funnetRapportFil.FullName;
                                                nyRapport.StevneType = baneStevne.StevneType;
                                                nyRapport.Rapport = new XmlDocument();
                                                nyRapport.Rapport.Load(funnetRapportFil.FullName);
                                                baneStevne.Rapporter.Add(nyRapport);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return update;
                }
            }

            return false;
        }

        /// <summary>
        /// The init time stamps.
        /// </summary>
        /// <param name="forcemin">
        /// The forcemin.
        /// </param>
        internal void InitTimeStamps(bool forcemin = true)
        {
            if (forcemin)
            {
                this.LastWrittenWebFile = DateTime.MinValue;
                this.LastWrittenPdfFile = DateTime.MinValue;
                this.LastWrittenStartingFile = DateTime.MinValue;
                this.LastWritten15mBitmapFile = DateTime.MinValue;
                this.LastWritten100mBitmapFile = DateTime.MinValue;
                this.LastWritten200mBitmapFile = DateTime.MinValue;
                this.LastCreate15mBitmapFile = DateTime.MinValue;
                this.LastCreate100mBitmapFile = DateTime.MinValue;
                this.LastCreate200mBitmapFile = DateTime.MinValue;
            }
            else
            {
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
            }
        }

        /// <summary>
        /// The init update info.
        /// </summary>
        internal void InitUpdateInfo()
        {
            if (this.StevneInfo != null)
            {
                foreach (var Lag in this.StevneInfo.StevneLag15m)
                {
                    if (Lag.UpdatedOppropsliste)
                    {
                        Lag.UpdatedOppropsliste = false;
                    }

                    foreach (var skive in Lag.Skiver)
                    {
                        skive.Updated = false;
                    }
                }
            }
        }

        /// <summary>
        /// The get bit map files.
        /// </summary>
        /// <param name="stevnePath">
        /// The stevne path.
        /// </param>
        /// <param name="subDir">
        /// The sub dir.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
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
        /// The add lag.
        /// </summary>
        /// <param name="StevneInfo">
        /// The stevne info.
        /// </param>
        /// <param name="lagInfo">
        /// The lag info.
        /// </param>
        private void AddLag(StartingListStevne StevneInfo, StartingListLag lagInfo)
        {
            switch (lagInfo.StevneType)
            {
                case StevneType.Femtenmeter:
                    StevneInfo.StevneLag15m.Add(lagInfo);
                    break;
                case StevneType.Hundremeter:
                    StevneInfo.StevneLag100m.Add(lagInfo);
                    break;
                case StevneType.Tohundremeter:
                    StevneInfo.StevneLag200m.Add(lagInfo);
                    break;
            }

            StevneInfo.SetStartAndEndDate();
        }

        /// <summary>
        /// The check deleted bitmaps.
        /// </summary>
        /// <param name="bitmapScan">
        /// The bitmap scan.
        /// </param>
        /// <param name="bitMapDir">
        /// The bit map dir.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CheckDeletedBitmaps(BitmapDirInfo[] bitmapScan, string bitMapDir)
        {
            foreach (var dirs in bitmapScan)
            {
                int bitmaps = this.StevneInfo.CountBitmapFiles(dirs.StevneType);
                if (dirs.BitmapFiles.Count < bitmaps)
                {
                    List<StartingListLag> lagliste;
                    string prefix;
                    switch (dirs.StevneType)
                    {
                        case StevneType.Femtenmeter:
                            lagliste = this.StevneInfo.StevneLag15m;
                            prefix = Constants.Prefix15m;
                            break;
                        case StevneType.Hundremeter:
                            lagliste = this.StevneInfo.StevneLag100m;
                            prefix = Constants.Prefix100m;
                            break;
                        case StevneType.Tohundremeter:
                            lagliste = this.StevneInfo.StevneLag200m;
                            prefix = Constants.Prefix200m;
                            break;
                        default:
                            Log.Error("Wrong stevnetype");
                            return false;
                    }

                    foreach (var lag in lagliste)
                    {
                        bool oppropChanged = false;
                        foreach (var skive in lag.Skiver)
                        {
                            if (skive.BackUpBitMapFile != null)
                            {
                                var name = skive.BackUpBitMapFile.Name;
                                var foundFile = dirs.BitmapFiles.FirstOrDefault(x => x.Name == name);
                                if (foundFile == null)
                                {
                                    Log.Info(
                                        "Detected BitMap file Removed {0} stevne {1} StevneType{2}",
                                        name,
                                        this.StevneInfo.StevneNavn,
                                        dirs.StevneType);
                                    if (this.UpdateHrefForBitmap(lag.Oppropsliste, null, skive, dirs.StevneType))
                                    {
                                        oppropChanged = true;
                                    }

                                    skive.Updated = true;
                                    skive.BackUpBitMapFile = null;
                                }
                            }
                        }

                        if (oppropChanged)
                        {
                            Log.Info(
                                "Updated Oppropsliste for lag={0} stevne={1}StevneType{2}",
                                lag.LagNr,
                                this.StevneInfo.StevneNavn,
                                dirs.StevneType);
                            if (lag.OppropslisteFileInfo == null)
                            {
                                var filename = Path.Combine(bitMapDir, string.Format("{0}Oppropsliste-{1}.html", prefix, lag.LagNr));
                                lag.Oppropsliste.Save(filename, new UTF8Encoding(true));
                                lag.OppropslisteFileInfo = new FileInfo(filename);
                                this.LastWrittenStartingFile = lag.OppropslisteFileInfo.LastWriteTime;
                            }
                            else
                            {
                                lag.Oppropsliste.Save(lag.OppropslisteFileInfo.FullName, new UTF8Encoding(true));
                                lag.OppropslisteFileInfo = new FileInfo(lag.OppropslisteFileInfo.FullName);
                                this.LastWrittenStartingFile = lag.OppropslisteFileInfo.LastWriteTime;
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// The find lag.
        /// </summary>
        /// <param name="stevneType">
        /// The stevne type.
        /// </param>
        /// <param name="Stevne">
        /// The stevne.
        /// </param>
        /// <param name="lagnr">
        /// The lagnr.
        /// </param>
        /// <returns>
        /// The <see cref="StartingListLag"/>.
        /// </returns>
        private StartingListLag FindLag(StevneType stevneType, StartingListStevne Stevne, int lagnr)
        {
            if (Stevne == null)
            {
                return null;
            }

            switch (stevneType)
            {
                case StevneType.Femtenmeter:
                    if (Stevne.StevneLag15m == null)
                    {
                        return null;
                    }

                    return Stevne.StevneLag15m.FirstOrDefault(lag => lag.LagNr == lagnr);
                    break;
                case StevneType.Hundremeter:
                    if (Stevne.StevneLag100m == null)
                    {
                        return null;
                    }

                    return Stevne.StevneLag100m.FirstOrDefault(lag => lag.LagNr == lagnr);
                    break;
                case StevneType.Tohundremeter:
                    if (Stevne.StevneLag200m == null)
                    {
                        return null;
                    }

                    return Stevne.StevneLag200m.FirstOrDefault(lag => lag.LagNr == lagnr);
                    break;
            }

            return null;
        }

        /// <summary>
        /// The finn skive.
        /// </summary>
        /// <param name="lagInfo">
        /// The lag info.
        /// </param>
        /// <param name="skivenr">
        /// The skivenr.
        /// </param>
        /// <returns>
        /// The <see cref="StartingListSkive"/>.
        /// </returns>
        private StartingListSkive FinnSkive(StartingListLag lagInfo, int skivenr)
        {
            if (lagInfo == null)
            {
                return null;
            }

            if (skivenr < 1)
            {
                Log.Error("Skivenummer fra Bitmap = {0}", skivenr);
            }

            return lagInfo.Skiver.FirstOrDefault(x => x.SkiveNr == skivenr);
        }

        /// <summary>
        /// The generate opprop liste.
        /// </summary>
        /// <param name="mal">
        /// The mal.
        /// </param>
        /// <param name="stevneNavn">
        /// The stevne navn.
        /// </param>
        /// <param name="lagnr">
        /// The lagnr.
        /// </param>
        /// <returns>
        /// The <see cref="HtmlDocument"/>.
        /// </returns>
        private HtmlDocument GenerateOppropListe(HtmlDocument mal, string stevneNavn, int lagnr)
        {
            var mem = new MemoryStream();
            var enc = new UTF8Encoding(true);
            mal.Save(mem, enc);
            mem.Position = 0;
            HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.Load(mem, enc);

            return document;
        }

        /// <summary>
        /// The get stevne type.
        /// </summary>
        /// <param name="filnavn">
        /// The filnavn.
        /// </param>
        /// <returns>
        /// The <see cref="StevneType"/>.
        /// </returns>
        private StevneType GetStevneType(string filnavn)
        {
            if (string.IsNullOrEmpty(filnavn))
            {
                return StevneType.Undefined;
            }

            if (filnavn.StartsWith(Constants.Prefix15m, StringComparison.OrdinalIgnoreCase))
            {
                return StevneType.Femtenmeter;
            }

            if (filnavn.StartsWith(Constants.Prefix100m, StringComparison.OrdinalIgnoreCase))
            {
                return StevneType.Hundremeter;
            }

            if (filnavn.StartsWith(Constants.Prefix200m, StringComparison.OrdinalIgnoreCase))
            {
                return StevneType.Tohundremeter;
            }

            return StevneType.Undefined;
        }

        /// <summary>
        /// The parse lagnr.
        /// </summary>
        /// <param name="filnavn">
        /// The filnavn.
        /// </param>
        /// <param name="stevneType">
        /// The stevne type.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int ParseLagnr(string filnavn, StevneType stevneType)
        {
            switch (stevneType)
            {
                case StevneType.Femtenmeter:
                    filnavn = filnavn.Remove(0, Constants.Prefix15m.Length);
                    break;
                case StevneType.Hundremeter:
                    filnavn = filnavn.Remove(0, Constants.Prefix100m.Length);
                    break;
                case StevneType.Tohundremeter:
                    filnavn = filnavn.Remove(0, Constants.Prefix200m.Length);
                    break;
            }

            int result = -1;
            var parts = filnavn.Split(new[] { '-', '.' });
            if (parts.Length > 1)
            {
                int.TryParse(parts[parts.Length - 2], out result);
            }

            return result;
        }

        /// <summary>
        /// The parse out start info.
        /// </summary>
        /// <param name="doc">
        /// The doc.
        /// </param>
        /// <param name="Lagnr">
        /// The lagnr.
        /// </param>
        /// <returns>
        /// The <see cref="StartingListLag"/>.
        /// </returns>
        private StartingListLag ParseOutStartInfo(HtmlDocument doc, int Lagnr)
        {
            StartingListLag lag = new StartingListLag(Lagnr);
            var node = doc.DocumentNode.SelectSingleNode("/html/head/title");
            if (node != null)
            {
                lag.StevneNavn = node.InnerText;
                if (!string.IsNullOrEmpty(lag.StevneNavn))
                {
                    lag.StevneNavn = lag.StevneNavn.Replace('/', ' ');
                }
            }

            var nodeLagInfo = doc.DocumentNode.SelectNodes("/html/body/table[@border='0']/tr");
            if (nodeLagInfo != null)
            {
                var info2 = nodeLagInfo[0].SelectNodes("td");
                if (info2.Count > 1)
                {
                    var textNode = info2[1].SelectSingleNode("font/b");
                    if (textNode != null)
                    {
                        lag.ParseLagInfo(textNode.InnerText);
                    }
                }
            }

            var nodeSkiver = doc.DocumentNode.SelectNodes("/html/body/table[@border='1']/tr");
            if (nodeSkiver != null)
            {
                foreach (var SKive in nodeSkiver)
                {
                    var infoNodes = SKive.SelectNodes("td");
                    if (infoNodes.Count > 3)
                    {
                        var element = new StartingListSkive();
                        int skive = -1;
                        var skiveNr = infoNodes[0].InnerText.Trim(new[] { '.' });
                        if (int.TryParse(skiveNr, out skive))
                        {
                            element.SkiveNr = skive;
                        }

                        element.SkytterNavn = infoNodes[1].InnerText;
                        element.SkytterLag = infoNodes[2].InnerText;
                        element.Klasse = infoNodes[3].InnerText;
                        if (element.SkiveNr > 0)
                        {
                            lag.Skiver.Add(element);
                        }
                    }
                }
            }

            return lag;
        }

        /// <summary>
        /// The parse rapport fil.
        /// </summary>
        /// <param name="filNavn">
        /// The fil navn.
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

        /// <summary>
        /// The parse stevne info.
        /// </summary>
        /// <param name="indfile">
        /// The indfile.
        /// </param>
        /// <returns>
        /// The <see cref="StartingListStevne"/>.
        /// </returns>
        private StartingListStevne ParseStevneInfo(string indfile)
        {
            string inputString;
            Encoding enc = Encoding.GetEncoding("ISO-8859-1");
            using (StreamReader read = new StreamReader(indfile, enc))
            {
                inputString = read.ReadToEnd();
            }

            StartingListStevne stevneInfo = null;
            var test = new HtmlAgilityPack.HtmlDocument();
            test.LoadHtml(inputString);
            var nodehead = test.DocumentNode.SelectSingleNode("/html/body/div[@id='header']/h1");

            var nodebody = test.DocumentNode.SelectSingleNode("/html/body/div[@id='container']/div[@id='navigation']");
            var nodebody2 = test.DocumentNode.SelectNodes("//div[@id='accordion']");

            if (nodebody2 != null)
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
                                if (nodeinfo.InnerText.ToUpper().Contains("LAGVIS"))
                                {
                                    stevneInfo = new StartingListStevne();
                                    stevneInfo.ParseStevneNavn(nodehead.InnerText);

                                    nodeinfoH3 = nodeinfo;
                                    break;
                                }
                            }
                        }

                        if (nodeinfoH3 != null && stevneInfo != null)
                        {
                            StevneType lagsType = stevneInfo.LagType(nodeinfoH3.InnerText);

                            HtmlNode Lag = null;
                            HtmlNodeCollection alleLagVise = null;
                            var nodeSibling = nodeinfoH3.NextSibling;
                            while (nodeSibling != null)
                            {
                                if (nodeSibling.NodeType == HtmlNodeType.Element && nodeSibling.Name == "div")
                                {
                                    alleLagVise = nodeSibling.SelectNodes("div");
                                    break;
                                }

                                nodeSibling = nodeSibling.NextSibling;
                            }

                            if (alleLagVise != null)
                            {
                                int dag = 1;
                                foreach (var lagnode in alleLagVise)
                                {
                                    var datonode = lagnode.SelectSingleNode(string.Format("h4[{0}]", dag));
                                    while (datonode != null)
                                    {
                                        var dagnode = lagnode.SelectSingleNode(string.Format("div[{0}]/table", dag));
                                        DateTime? oppropsDato = StartingListLag.ParseStartDate(stevneInfo.StevneNavn, datonode.InnerText);
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
                                                    StartingListLag nyttlag = new StartingListLag(lagnummer);
                                                    nyttlag.StartTime = lagtid;
                                                    nyttlag.StevneType = lagsType;
                                                    nyttlag.StevneNavn = stevneInfo.StevneNavn;
                                                    nyttlag.XmlOppropsListe.Add(xmlfilNavn);
                                                    var stevne = stevneInfo.FinnStevne(nyttlag.StevneType);
                                                    if (stevne == null)
                                                    {
                                                        stevne = stevneInfo.AddNewStevne(
                                                            stevneInfo.StevneNavn,
                                                            stevneInfo.ReportDirStevneNavn,
                                                            nyttlag.StevneType);
                                                    }

                                                    stevneInfo.AddLag(nyttlag);
                                                }
                                            }
                                        }

                                        dag++;
                                        datonode = lagnode.SelectSingleNode(string.Format("h4[{0}]", dag));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return stevneInfo;
        }

        /// <summary>
        /// The update href for bitmap.
        /// </summary>
        /// <param name="htmlDocument">
        /// The html document.
        /// </param>
        /// <param name="bitmapName">
        /// The bitmap name.
        /// </param>
        /// <param name="skiveToCheck">
        /// The skive to check.
        /// </param>
        /// <param name="stevneType">
        /// The stevne type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool UpdateHrefForBitmap(HtmlDocument htmlDocument, string bitmapName, StartingListSkive skiveToCheck, StevneType stevneType)
        {
            string prefix = string.Empty;
            switch (stevneType)
            {
                case StevneType.Femtenmeter:
                    prefix = Constants.Prefix15m;
                    break;
                case StevneType.Hundremeter:
                    prefix = Constants.Prefix100m;
                    break;
                case StevneType.Tohundremeter:
                    prefix = Constants.Prefix200m;
                    break;
            }

            bitmapName = string.Format("{0}/{1}", prefix, bitmapName);

            int inputskiveNr = skiveToCheck.SkiveNr;
            var nodeSkiver = htmlDocument.DocumentNode.SelectNodes("/html/body/table[@border='1']/tr");
            var tabell = htmlDocument.DocumentNode.SelectSingleNode("/html/body/table[@border='1']");
            int maxSkiveFound = -1;
            HtmlNode lastSkive = null;
            if (nodeSkiver != null)
            {
                foreach (var Skive in nodeSkiver)
                {
                    lastSkive = Skive;
                    var infoNodes = Skive.SelectNodes("td");
                    if (infoNodes.Count > 3)
                    {
                        int skive = -1;
                        var skiveNr = infoNodes[0].InnerText.Trim(new[] { '.' });
                        if (int.TryParse(skiveNr, out skive))
                        {
                        }

                        if (skive > maxSkiveFound)
                        {
                            maxSkiveFound = skive;
                        }

                        if (skive == inputskiveNr)
                        {
                            bool updated = false;
                            var node = infoNodes[1];
                            var refnode = node.SelectSingleNode("a");
                            if (!string.IsNullOrEmpty(bitmapName))
                            {
                                if (refnode != null)
                                {
                                    var foundName = refnode.GetAttributeValue("href", string.Empty);
                                    if (foundName != bitmapName)
                                    {
                                        refnode.SetAttributeValue("href", bitmapName);
                                        updated = true;
                                    }
                                }
                                else
                                {
                                    updated = true;
                                    node.RemoveAllChildren();
                                    HtmlNode newNode =
                                        HtmlNode.CreateNode(
                                            string.Format(
                                                "<a href=\"{0}\" style=\"color: #4455AA;text-decoration: none;\" onmouseover=\"style.textDecoration='underline'\" onmouseout=\"style.textDecoration='none'\">{1}</a>",
                                                bitmapName,
                                                skiveToCheck.SkytterNavn));
                                    node.AppendChild(newNode);
                                }
                            }
                            else if (string.IsNullOrEmpty(bitmapName))
                            {
                                if (refnode != null)
                                {
                                    node.ParentNode.ReplaceChild(
                                        HtmlTextNode.CreateNode(string.Format("<td align=\"left\">{0}</td>", skiveToCheck.SkytterNavn)),
                                        node);
                                    updated = true;
                                }
                                else
                                {
                                    if (node.InnerHtml != skiveToCheck.SkytterNavn)
                                    {
                                        node.InnerHtml = skiveToCheck.SkytterNavn;
                                        updated = true;
                                    }
                                }
                            }

                            if (infoNodes[2].InnerHtml != skiveToCheck.SkytterLag)
                            {
                                infoNodes[2].InnerHtml = skiveToCheck.SkytterLag;
                                updated = true;
                            }

                            if (infoNodes[3].InnerHtml != skiveToCheck.Klasse)
                            {
                                infoNodes[3].InnerHtml = skiveToCheck.Klasse;
                                updated = true;
                            }

                            return updated;
                        }
                    }
                }

                if (inputskiveNr > maxSkiveFound)
                {
                    // we add up nr
                    int startnr = nodeSkiver.Count - 1;
                    while (startnr < inputskiveNr - 1)
                    {
                        startnr++;

                        /*<td align="center">1.</td>
                        <td align="left">Anne Grethe Lund</td>
                        <td align="left">Sulitjelma </td>
                        <td align="center">2</td>
                        <td align="center">K</td>*/
                        HtmlNode newNodeTr = HtmlNode.CreateNode("<tr>");
                        tabell.AppendChild(newNodeTr);
                        HtmlNode newNode = HtmlNode.CreateNode("\r\n");
                        newNodeTr.AppendChild(newNode);

                        HtmlNode childNode = HtmlNode.CreateNode(string.Format("<td align=\"center\">{0}.</td>", startnr));
                        newNodeTr.AppendChild(childNode);
                        newNode = HtmlNode.CreateNode("\r\n");
                        newNodeTr.AppendChild(newNode);
                        childNode = HtmlNode.CreateNode("<td align=\"left\">-</td>");
                        newNodeTr.AppendChild(childNode);
                        newNode = HtmlNode.CreateNode("\r\n");
                        newNodeTr.AppendChild(newNode);
                        childNode = HtmlNode.CreateNode("<td align=\"left\">-</td>");
                        newNodeTr.AppendChild(childNode);
                        newNode = HtmlNode.CreateNode("\r\n");
                        newNodeTr.AppendChild(newNode);
                        childNode = HtmlNode.CreateNode("<td align=\"center\">-</td>");
                        newNodeTr.AppendChild(childNode);
                        newNode = HtmlNode.CreateNode("\r\n");
                        newNodeTr.AppendChild(newNode);
                        childNode = HtmlNode.CreateNode("<td align=\"center\">-</td>");
                        newNodeTr.AppendChild(childNode);
                        newNode = HtmlNode.CreateNode("\r\n");
                        newNodeTr.AppendChild(newNode);
                    }

                    HtmlNode newSkiveNode = HtmlNode.CreateNode("<tr>");
                    tabell.AppendChild(newSkiveNode);
                    HtmlNode childSKiveNode = HtmlNode.CreateNode("\r\n");
                    newSkiveNode.AppendChild(childSKiveNode);

                    childSKiveNode = HtmlNode.CreateNode(string.Format("<td align=\"center\">{0}.</td>", inputskiveNr));
                    newSkiveNode.AppendChild(childSKiveNode);
                    childSKiveNode = HtmlNode.CreateNode("\r\n");
                    newSkiveNode.AppendChild(childSKiveNode);

                    if (string.IsNullOrEmpty(bitmapName))
                    {
                        childSKiveNode = HtmlNode.CreateNode(string.Format("<td align=\"left\">{0}</td>", skiveToCheck.SkytterNavn));
                    }
                    else
                    {
                        childSKiveNode =
                            HtmlNode.CreateNode(
                                string.Format(
                                    "<a href=\"{0}\" style=\"color: #4455AA;text-decoration: none;\" onmouseover=\"style.textDecoration='underline'\" onmouseout=\"style.textDecoration='none'\">{1}</a>",
                                    bitmapName,
                                    skiveToCheck.SkytterNavn));
                    }

                    newSkiveNode.AppendChild(childSKiveNode);
                    childSKiveNode = HtmlNode.CreateNode("\r\n");
                    newSkiveNode.AppendChild(childSKiveNode);
                    childSKiveNode = HtmlNode.CreateNode(string.Format("<td align=\"left\">{0}</td>", skiveToCheck.SkytterLag));
                    newSkiveNode.AppendChild(childSKiveNode);
                    childSKiveNode = HtmlNode.CreateNode("\r\n");
                    newSkiveNode.AppendChild(childSKiveNode);
                    childSKiveNode = HtmlNode.CreateNode(string.Format("<td align=\"center\">{0}</td>", skiveToCheck.Klasse));
                    newSkiveNode.AppendChild(childSKiveNode);
                    childSKiveNode = HtmlNode.CreateNode("\r\n");
                    newSkiveNode.AppendChild(childSKiveNode);
                    childSKiveNode = HtmlNode.CreateNode("<td align=\"center\">-</td>");
                    newSkiveNode.AppendChild(childSKiveNode);
                    childSKiveNode = HtmlNode.CreateNode("\r\n");
                    newSkiveNode.AppendChild(childSKiveNode);
                    return true;
                }
            }

            return false;
        }

        #endregion

        internal bool CheckBitMap()
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
                BitmapDirInfo[] bitmapScan = new BitmapDirInfo[3];
                bitmapScan[0] = new BitmapDirInfo() { StevneType = StevneType.Femtenmeter };
                bitmapScan[1] = new BitmapDirInfo() { StevneType = StevneType.Hundremeter };
                bitmapScan[2] = new BitmapDirInfo() { StevneType = StevneType.Tohundremeter };

                bitmapScan[0].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix15m);
                bitmapScan[1].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix100m);
                bitmapScan[2].BitmapFiles = GetBitMapFiles(this.Info.FullName, Constants.Prefix200m);

                foreach (var dir2Check in bitmapScan)
                {
                    dir2Check.Updated = false;
                    var sortlist = dir2Check.BitmapFiles.OrderByDescending(x => x.LastWriteTime);
                    if (sortlist.FirstOrDefault() != null)
                    {
                        switch (dir2Check.StevneType)
                        {
                            case StevneType.Femtenmeter:
                                if (this.LastWritten15mBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    this.LastWritten15mBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                    if (this.LastWritten15mBitmapFile != DateTime.MinValue)
                                    {
                                        dir2Check.Updated = true;
                                    }
                                    
                                }

                                break;
                            case StevneType.Hundremeter:
                                if (this.LastWritten100mBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    this.LastWritten100mBitmapFile = sortlist.FirstOrDefault().LastWriteTime;
                                    if (this.LastWritten100mBitmapFile != DateTime.MinValue)
                                    {
                                        dir2Check.Updated = true;
                                    }
                                    
                                }

                                break;
                            case StevneType.Tohundremeter:
                                if (this.LastWritten200mBitmapFile < sortlist.FirstOrDefault().LastWriteTime)
                                {
                                    this.LastWritten200mBitmapFile = sortlist.FirstOrDefault().LastWriteTime;

                                    if (this.LastWritten200mBitmapFile != DateTime.MinValue)
                                    {
                                        dir2Check.Updated = true;
                                    }
                                }

                                break;
                        }
                    }

                    var sortlistCreat = dir2Check.BitmapFiles.OrderByDescending(x => x.CreationTime);
                    if (sortlistCreat.FirstOrDefault() != null)
                    {
                        switch (dir2Check.StevneType)
                        {
                            case StevneType.Femtenmeter:
                                if (this.LastWritten15mBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    this.LastWritten15mBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                    if (this.LastWritten15mBitmapFile != DateTime.MinValue)
                                    {
                                        dir2Check.Updated = true;
                                    }
                                }

                                break;
                            case StevneType.Hundremeter:
                                if (this.LastWritten100mBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    this.LastWritten100mBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                    if (this.LastWritten100mBitmapFile != DateTime.MinValue)
                                    {
                                        dir2Check.Updated = true;
                                    }
                                }

                                break;
                            case StevneType.Tohundremeter:
                                if (this.LastWritten200mBitmapFile < sortlistCreat.FirstOrDefault().CreationTime)
                                {
                                    this.LastWritten200mBitmapFile = sortlistCreat.FirstOrDefault().CreationTime;
                                    if (this.LastWritten200mBitmapFile != DateTime.MinValue)
                                    {
                                        dir2Check.Updated = true;
                                    }
                                }

                                break;
                        }
                    }
                }

                foreach (var dirs in bitmapScan)
                {
                   
                        BitmapDirInfo newlist=null;
                        foreach (var bitmapDirs in this.BitmapsStoredInStevne)
                        {
                            if (dirs.StevneType == bitmapDirs.StevneType)
                            {
                                newlist = bitmapDirs;
                                break;
                            }
                        }

                        if (newlist != null)
                        {
                            newlist.Updated = dirs.Updated;
                            newlist.BitmapFiles.Clear();
                            newlist.BitmapFiles = dirs.BitmapFiles;
                           
                        }
                        else
                        {
                            newlist = new BitmapDirInfo { Updated = true };
                            newlist.BitmapFiles.Clear();
                            newlist.BitmapFiles = dirs.BitmapFiles;
                            newlist.StevneType = dirs.StevneType;
                            this.BitmapsStoredInStevne.Add(newlist);
                            //update = true;
                        }


                    if (dirs.Updated)
                    {
                        update = dirs.Updated;
                    }
                }
            }

            return update;
        }
    }
}