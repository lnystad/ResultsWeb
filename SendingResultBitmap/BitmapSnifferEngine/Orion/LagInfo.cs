using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitmapSnifferEngine.Orion
{
    using BitmapSnifferEngine.Common;

    public class LagInfo
    {
        public HoldType HoldType { get; set; }
        public int LagNr { get; set; }

        public int SkiveNr { get; set; }

        public int? SerieNr { get; set; }
    }
}
