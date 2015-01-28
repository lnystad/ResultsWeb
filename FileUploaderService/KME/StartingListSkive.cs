using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.KME
{
    using System.IO;
    using System.Xml.Serialization;

    public class StartingListSkive
    {
        public StartingListSkive()
        {
            
        }

        public StartingListSkive(StartingListSkive cpy)
        {
            SkiveNr = cpy.SkiveNr;
            SkytterNavn = cpy.SkytterNavn;
            SkytterLag = cpy.SkytterLag;
            Klasse = cpy.Klasse;
            Extra = cpy.Extra;
            RawBitmapFile = cpy.RawBitmapFile;

        }

        public int LagSkiveNr { get; set; }

        public int SkiveNr { get; set; }
        public string SkytterNavn { get; set; }
        public string SkytterLag { get; set; }
        public string Klasse { get; set; }
        public string Extra { get; set; }


        private FileInfo m_rawBitmapFile { get; set; }

        [XmlIgnore]
        public FileInfo RawBitmapFile
        {
            get
            {
                return m_rawBitmapFile;
            }
            set
            {
                m_rawBitmapFile = value;
                if (m_rawBitmapFile != null)
                {
                    m_rawBitmapFileName = m_rawBitmapFile.Name;
                }
            }
        }

        private string m_rawBitmapFileName;
        public string RawBitmapFileName
        {
            get
            {
                return m_rawBitmapFileName;
            }
            set
            {
                m_rawBitmapFileName = value;
            }
        }

        private FileInfo m_BackUpBitMapFile;
        [XmlIgnore]
        public FileInfo BackUpBitMapFile
        {
            get
            {
                return m_BackUpBitMapFile;
            }
            set
            {
                m_BackUpBitMapFile = value;
                if (m_BackUpBitMapFile != null)
                {
                    m_backUpBitMapFileName = m_BackUpBitMapFile.Name;
                }
            }
        }

        private string m_backUpBitMapFileName;
        public string BackUpBitMapFileName
        {
            get
            {
                return m_backUpBitMapFileName;
            }
            set
            {
                m_backUpBitMapFileName = value;
            }
        }

        public bool Updated { get; set; }

        public bool Checked { get; set; }

        internal bool UpdatedSkive(StartingListSkive newskiveInfo)
        {
            bool changed = false;
            if (SkiveNr != newskiveInfo.SkiveNr)
            {
                SkiveNr = newskiveInfo.SkiveNr;
                changed = true;
            }

            if (SkytterNavn != newskiveInfo.SkytterNavn)
            {
                SkytterNavn = newskiveInfo.SkytterNavn;
                changed = true;
            }

            if (SkytterLag != newskiveInfo.SkytterLag)
            {
                SkytterLag = newskiveInfo.SkytterLag;
                changed = true;
            }

            if (Klasse != newskiveInfo.Klasse)
            {
                Klasse = newskiveInfo.Klasse;
                changed = true;
            }

            return changed;
        }
    }
}
