using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.KME
{
    using System.IO;
    using System.Xml.Serialization;

    public class StartingListSerie
    {
        public StartingListSerie()
        {
            
        }

        public StartingListSerie(StartingListSerie cpy)
        {
            RawBitmapFile = cpy.RawBitmapFile;
        }


        public int SerieNr { get; set; }


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
                else
                {
                    m_rawBitmapFileName = null;
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

        internal bool UpdatedSerie(StartingListSerie newskiveInfo)
        {
            bool changed = false;
            if (SerieNr != newskiveInfo.SerieNr)
            {
                SerieNr = newskiveInfo.SerieNr;
                changed = true;
            }
            return changed;
        }

        public static bool CheckUpdateSerier(List<StartingListSerie> serier, List<StartingListSerie> startingListSeries)
        {
            if (serier.Count != startingListSeries.Count)
            {
                return true;
            }

            return false;
        }
    }
}
