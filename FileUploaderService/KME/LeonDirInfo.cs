﻿// --------------------------------------------------------------------------------------------------------------------
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
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.Serialization;

    using FileUploaderService.Utils;

    using HtmlAgilityPack;

    using SendingResults.Diagnosis;

    /// <summary>
    ///     The leon dir info.
    /// </summary>
    public class LeonDirInfo : IEquatable<LeonDirInfo>
    {
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

        /// <summary>
        ///     Gets or sets the pdf file name.
        /// </summary>
        public string PdfFileName { get; set; }

        /// <summary>
        /// Gets or sets the presse liste file name.
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
        /// The check bit map.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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
                            case BaneType.FinFelt:
                                bitmaps = bitmapScan[3];
                                bane.BitmapsStoredInBane = bitmapScan[3];
                                bitmaps.BitmapSubDir = Constants.PrefixFinFelt;
                                break;
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

        /// <summary>
        /// The check web files.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool CheckWebFiles()
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
                    var fileelement = list.FirstOrDefault();
                    if (fileelement != null )
                    {
                        DateTime LastWriteTime = new DateTime(fileelement.LastWriteTime.Year,
                            fileelement.LastWriteTime.Month,
                            fileelement.LastWriteTime.Day,
                            fileelement.LastWriteTime.Hour,
                            fileelement.LastWriteTime.Minute,
                            0);
                        if (this.LastWrittenWebFile < LastWriteTime)
                        {
                            
                            if (this.LastWrittenWebFile != DateTime.MinValue)
                            {
                                update = true;
                                Thread.Sleep(5000);
                            }
                            this.LastWrittenWebFile = LastWriteTime;
                            
                            this.ParseIndexHtmlFile(webInfo);
                        }
                    }

                    return update;
                }
            }

            return false;
        }

        private bool ParseIndexHtmlFile(DirectoryInfo webInfo)
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
                            foreach (var filnavn in baneStevne.ToppListeFilPrefix)
                            {
                                if (!string.IsNullOrEmpty(filnavn))
                                {
                                    var filenavn = string.Format("{0}.xml", filnavn);
                                    var funnetToppListeFil = listopprop.FirstOrDefault(x => x.Name == filenavn);
                                    if (funnetToppListeFil != null)
                                    {
                                        RapportXmlClass nyRapport = new RapportXmlClass();
                                        nyRapport.Filnavn = funnetToppListeFil.FullName;
                                        nyRapport.BaneType = baneStevne.BaneType;

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
        /// The get lagvis opprop.
        /// </summary>
        /// <param name="nodeinfoH3">
        /// The nodeinfo h 3.
        /// </param>
        /// <param name="baneInfo">
        /// The bane info.
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
                        var attrValClass = nodeSibling.GetAttributeValue("id","DEF");
                        ProgramType typeOvelse = ProgramType.Innledende;
                        foreach (var noder in nodeSibling.ChildNodes)
                        {
                            if (noder.NodeType == HtmlNodeType.Element && noder.Name == "h3")
                            {
                                typeOvelse = StartingListLag.ParseOvelse(noder.InnerText);
                            }
                            else if (noder.NodeType == HtmlNodeType.Element && noder.Name == "div")
                            {
                                var attrVal = noder.GetAttributeValue("id","DEF");
                                if (attrVal.StartsWith("accordionsub"))
                                {
                                    if (typeOvelse == ProgramType.Finale ||
                                        typeOvelse == ProgramType.Innledende)
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
                                                lagnummer = lagnummer + 200;
                                                break;
                                            case ProgramType.Lagskyting:
                                                lagnummer = lagnummer + 100;
                                                break;
                                            case ProgramType.SamLagskyting:
                                                lagnummer = lagnummer + 100;
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
        /// The get report files for bane.
        /// </summary>
        /// <param name="nodeinfo">
        /// The nodeinfo.
        /// </param>
        /// <param name="bane">
        /// The bane.
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
                                            if (string.Compare(overskriftNode.InnerText, "UNGDOM", StringComparison.OrdinalIgnoreCase)
                                                == 0 ||
                                                string.Compare(overskriftNode.InnerText, "UNGDOM INDIVIDUELL", StringComparison.OrdinalIgnoreCase)
                                                == 0)
                                            {
                                                Log.Info("Found report Ungdom Lag File {0}", val);
                                                bane.ToppListeLagFilPrefix.Add(val);
                                            }
                                            else if (string.Compare(
                                                overskriftNode.InnerText, 
                                                "VETERAN", 
                                                StringComparison.OrdinalIgnoreCase) == 0 ||
                                                string.Compare(
                                                overskriftNode.InnerText,
                                                "VETERAN INDIVIDUELL",
                                                StringComparison.OrdinalIgnoreCase) == 0)
                                            {
                                                Log.Info("Found report Veteran Lag File {0}", val);
                                                bane.ToppListeLagFilPrefix.Add(val);
                                            }
                                            else if (string.Compare(
                                                overskriftNode.InnerText, 
                                                "SENIOR", 
                                                StringComparison.OrdinalIgnoreCase) == 0 ||
                                                string.Compare(
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
                                                bane.ToppListeFilPrefix.Add(val);
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
                                            if (string.Compare(overskriftNode.InnerText, "UNGDOM", StringComparison.OrdinalIgnoreCase)
                                                == 0 ||
                                                string.Compare(overskriftNode.InnerText, "UNGDOM INDIVIDUELL", StringComparison.OrdinalIgnoreCase)
                                                == 0)
                                            {
                                                Log.Info("Found report Ungdom Lag File {0}", val);
                                                bane.ToppListeLagFilPrefix.Add(val);
                                            }
                                            else if (string.Compare(
                                                overskriftNode.InnerText,
                                                "VETERAN",
                                                StringComparison.OrdinalIgnoreCase) == 0 ||
                                                string.Compare(
                                                overskriftNode.InnerText,
                                                "VETERAN INDIVIDUELL",
                                                StringComparison.OrdinalIgnoreCase) == 0)
                                            {
                                                Log.Info("Found report Veteran Lag File {0}", val);
                                                bane.ToppListeLagFilPrefix.Add(val);
                                            }
                                            else if (string.Compare(
                                                overskriftNode.InnerText,
                                                "SENIOR",
                                                StringComparison.OrdinalIgnoreCase) == 0 ||
                                                string.Compare(
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
                                                bane.ToppListeFilPrefix.Add(val);
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
            try
            {
                string inputString;
                Encoding enc = Encoding.GetEncoding("ISO-8859-1");
                using (StreamReader read = new StreamReader(indfile, enc))
                {
                    inputString = read.ReadToEnd();
                }

                var directoryName = Path.GetDirectoryName(indfile);
                List<string> topListeFiler = new List<string>();
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

                                }
                            }
                        }
                    }
                }

                GetLagSkytingOpprop(stevneInfo);


                return stevneInfo;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Format("error parsing {0}", indfile));
            }

            return null;
        }

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
                
                lagskytingLag.StartTime = new DateTime(
                    oppropTime.Year,
                    oppropTime.Month,
                    oppropTime.Day,
                    oppropTime.Hour,
                    oppropTime.Minute,
                    0);
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

            return ;
        }

        private void ParseLagSkyting(StartListBane bane,DateTime oppropTime ,BaneType banetype, int lagnr, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var lines = text.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            var lagskytingLag = new StartingListLag(lagnr + 100) { ProgramType = ProgramType.Lagskyting };
            lagskytingLag.StartTime = new DateTime(
                oppropTime.Year,
                oppropTime.Month,
                oppropTime.Day,
                oppropTime.Hour,
                oppropTime.Minute,
                0);
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