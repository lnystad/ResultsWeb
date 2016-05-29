using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitmapSnifferEngine.Orion
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Xml.Serialization;

    using BitmapSnifferEngine.Common;
    
    public class HoldTypeConfiguration
    {
        public HoldType HoldType { get; set; }
        public int HoldNr { get; set; }

        public int StartSkive { get; set; }

        public int SluttSkive { get; set; }

        public int VenteBenk { get; set; }

        public bool FixLagnr { get; set; }
    }

    public class SetupConfiguration
    {
        public SetupConfiguration()
        {
            HoldConfig = new Collection<HoldTypeConfiguration>();
        }
        public bool PausedBetweenHold { get; set; }
        public BaneType BaneType { get; set; }

        public string BitMapDir { get; set; }

        public string BitMapErrorDir { get; set; }

        public string BitMapBackupDir { get; set; }

        public int? BitMapFetchTimeOut { get; set; }

        [XmlElement]
        public Collection<HoldTypeConfiguration> HoldConfig { get; set; }
    }
}
