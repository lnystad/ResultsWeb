using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.KME
{
    using System.IO;
    using System.Xml.Serialization;

    public class BitmapDirInfo
    {
        public BitmapDirInfo()
        {
            BitmapFiles = new List<FileInfo>();
            BitmapFileNames = new List<string>();
        }

        [XmlIgnore]
        public List<FileInfo> BitmapFiles { get; set; }


        [XmlArray("Bitmaps")]
        [XmlArrayItem("FileName")]
        public List<string> BitmapFileNames { get; set; }

        //public StevneType StevneType { get; set; }

        public BaneType BaneType { get; set; }

        public string BitmapSubDir { get; set; }

        public bool Updated { get; set; }

        public bool Initial { get; set; }

        public void InitAllNames()
        {
            foreach (var bitmapinfo in this.BitmapFiles)
            {
                if (!this.BitmapFileNames.Contains(bitmapinfo.Name))
                {
                    this.BitmapFileNames.Add(bitmapinfo.Name);
                }
            }
        }
    }
}
